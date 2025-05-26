using System;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace RobDriver.SkillStates.BaseStates
{
    public abstract class BaseDriverAimThrowable : BaseDriverSkillState
    {
        private struct CalculateArcPointsJob : IJobParallelFor, IDisposable
        {
            [ReadOnly]
            private Vector3 origin;

            [ReadOnly]
            private Vector3 velocity;

            [ReadOnly]
            private float indexMultiplier;

            [ReadOnly]
            private float gravity;

            [WriteOnly]
            public NativeArray<Vector3> outputPositions;

            public void SetParameters(Vector3 origin, Vector3 velocity, float totalTravelTime, int positionCount, float gravity)
            {
                this.origin = origin;
                this.velocity = velocity;

                if (this.outputPositions.Length != positionCount)
                {
                    if (this.outputPositions.IsCreated)
                        this.outputPositions.Dispose();

                    this.outputPositions = new NativeArray<Vector3>(positionCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                }

                this.indexMultiplier = totalTravelTime / (float)(positionCount - 1);
                this.gravity = gravity;
            }

            public void Dispose()
            {
                if (this.outputPositions.IsCreated)
                {
                    this.outputPositions.Dispose();
                }
            }

            public void Execute(int index)
            {
                float t = (float)index * this.indexMultiplier;
                this.outputPositions[index] = Trajectory.CalculatePositionAtTime(this.origin, this.velocity, t, this.gravity);
            }
        }

        protected struct TrajectoryInfo
        {
            public Ray finalRay;

            public Vector3 hitPoint;

            public Vector3 hitNormal;

            public float travelTime;

            public float speedOverride;
        }

        public float maxDistance;
        public float rayRadius;
        public float endpointVisualizerRadiusScale;
        public float damageCoefficient;
        public float baseMinimumDuration;
        public bool setFuse;

        public GameObject arcVisualizerPrefab;
        public GameObject projectilePrefab;
        public GameObject endpointVisualizerPrefab;

        protected LineRenderer arcVisualizerLineRenderer;
        protected Transform endpointVisualizerTransform;

        protected float projectileBaseSpeed;
        protected float detonationRadius;
        protected float minimumDuration;
        protected bool useGravity;

        protected TrajectoryInfo currentTrajectoryInfo;
        private CalculateArcPointsJob calculateArcPointsJob;
        private JobHandle calculateArcPointsJobHandle;
        private Vector3[] pointsBuffer = [];
        private Action completeArcPointsVisualizerJobMethod;

        public override void OnEnter()
        {
            base.OnEnter();

            this.minimumDuration = this.baseMinimumDuration / base.attackSpeedStat;

            if (this.arcVisualizerPrefab)
            {
                this.calculateArcPointsJob = default;
                this.arcVisualizerLineRenderer = UnityEngine.Object.Instantiate(this.arcVisualizerPrefab, base.transform.position, Quaternion.identity).GetComponent<LineRenderer>();
                this.completeArcPointsVisualizerJobMethod = CompleteArcVisualizerJob;
                RoR2Application.onLateUpdate += this.completeArcPointsVisualizerJobMethod;
            }

            if (this.endpointVisualizerPrefab)
                this.endpointVisualizerTransform = UnityEngine.Object.Instantiate(this.endpointVisualizerPrefab, base.transform.position, Quaternion.identity).transform;

            if (base.characterBody)
                base.characterBody.hideCrosshair = true;

            if (this.projectilePrefab.TryGetComponent<ProjectileSimple>(out var projectileSimple))
                this.projectileBaseSpeed = projectileSimple.desiredForwardSpeed;

            if (this.projectilePrefab.TryGetComponent<Rigidbody>(out var prefabRigidBody))
                this.useGravity = prefabRigidBody.useGravity;

            if (this.projectilePrefab.TryGetComponent<ProjectileImpactExplosion>(out var projectileImpactExplosion))
            {
                this.detonationRadius = projectileImpactExplosion.blastRadius;
                if (this.endpointVisualizerTransform)
                    this.endpointVisualizerTransform.localScale = Vector3.one * this.detonationRadius;
            }

            this.UpdateVisualizers(this.currentTrajectoryInfo);

            SceneCamera.onSceneCameraPreRender += OnPreRenderSceneCam;
        }

        public override void OnExit()
        {
            SceneCamera.onSceneCameraPreRender -= OnPreRenderSceneCam;

            if (!base.outer.destroying)
            {
                if (base.isAuthority)
                {
                    this.FireProjectile();
                }

                this.OnProjectileFiredLocal();
            }

            if (base.characterBody)
                base.characterBody.hideCrosshair = false;

            this.calculateArcPointsJobHandle.Complete();
            if (this.arcVisualizerLineRenderer)
            {
                EntityState.Destroy(this.arcVisualizerLineRenderer.gameObject);
                this.arcVisualizerLineRenderer = null;
            }

            if (this.completeArcPointsVisualizerJobMethod != null)
            {
                RoR2Application.onLateUpdate -= this.completeArcPointsVisualizerJobMethod;
                this.completeArcPointsVisualizerJobMethod = null;
            }

            this.calculateArcPointsJob.Dispose();
            this.pointsBuffer = [];

            if (this.endpointVisualizerTransform)
            {
                EntityState.Destroy(this.endpointVisualizerTransform.gameObject);
                this.endpointVisualizerTransform = null;
            }

            base.OnExit();
        }

        protected virtual bool KeyIsDown()
        {
            return base.IsKeyDownAuthority();
        }

        protected virtual void OnProjectileFiredLocal()
        {
        }

        protected virtual void FireProjectile()
        {
            var fireProjectileInfo = new FireProjectileInfo
            {
                crit = base.RollCrit(),
                owner = base.gameObject,
                position = this.currentTrajectoryInfo.finalRay.origin,
                projectilePrefab = this.projectilePrefab,
                rotation = Util.QuaternionSafeLookRotation(this.currentTrajectoryInfo.finalRay.direction, Vector3.up),
                speedOverride = this.currentTrajectoryInfo.speedOverride,
                damage = this.damageCoefficient * base.damageStat,
                fuseOverride = this.setFuse ? this.currentTrajectoryInfo.travelTime : -1f
            };

            this.ModifyProjectile(ref fireProjectileInfo);
            ProjectileManager.instance.FireProjectile(fireProjectileInfo);
        }

        protected virtual void ModifyProjectile(ref FireProjectileInfo fireProjectileInfo)
        {
            fireProjectileInfo.damageTypeOverride = this.iDrive.DamageType;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority && !this.KeyIsDown() && base.fixedAge >= this.minimumDuration)
            {
                this.UpdateTrajectoryInfo(out this.currentTrajectoryInfo);

                EntityState entityState = this.PickNextState();
                if (entityState != null)
                    base.outer.SetNextState(entityState);
                else
                    base.outer.SetNextStateToMain();
            }
        }

        protected virtual EntityState PickNextState()
        {
            return null;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        public override void Update()
        {
            base.Update();
            this.UpdateTrajectoryInfo(out this.currentTrajectoryInfo);
            this.UpdateVisualizers(this.currentTrajectoryInfo);
        }

        protected virtual void UpdateTrajectoryInfo(out TrajectoryInfo dest)
        {
            dest = default;
            RaycastHit hitInfo = default;

            var aimRay = base.GetAimRay();

            bool hitEnemy = this.rayRadius > 0f && Util.CharacterSpherecast(base.gameObject, aimRay, this.rayRadius, out hitInfo, this.maxDistance,
                            LayerIndex.CommonMasks.bullet, QueryTriggerInteraction.UseGlobal) && hitInfo.collider.GetComponent<HurtBox>();

            if (!hitEnemy)
                hitEnemy = Util.CharacterRaycast(base.gameObject, aimRay, out hitInfo, this.maxDistance, LayerIndex.CommonMasks.bullet, QueryTriggerInteraction.UseGlobal);


            if (hitEnemy)
            {
                dest.hitPoint = hitInfo.point;
                dest.hitNormal = hitInfo.normal;
            }
            else
            {
                dest.hitPoint = aimRay.GetPoint(this.maxDistance);
                dest.hitNormal = -aimRay.direction;
            }

            Vector3 relativeVector = dest.hitPoint - aimRay.origin;
            if (this.useGravity)
            {
                var horizontalVector = new Vector2(relativeVector.x, relativeVector.z);
                float horizontalDistance = horizontalVector.magnitude;
                float speed = this.projectileBaseSpeed;

                var vector3 = new Vector3(y: Trajectory.CalculateInitialYSpeed(horizontalDistance / speed, relativeVector.y), 
                    x: horizontalVector.x / horizontalDistance * speed, 
                    z: horizontalVector.y / horizontalDistance * speed);
                dest.speedOverride = vector3.magnitude;
                dest.finalRay = new Ray(aimRay.origin, vector3 / dest.speedOverride);
                dest.travelTime = Trajectory.CalculateGroundTravelTime(speed, horizontalDistance);
            }
            else
            {
                dest.speedOverride = this.projectileBaseSpeed;
                dest.finalRay = aimRay;
                dest.travelTime = this.projectileBaseSpeed / relativeVector.magnitude;
            }
        }

        private void CompleteArcVisualizerJob()
        {
            this.calculateArcPointsJobHandle.Complete();

            if (this.arcVisualizerLineRenderer)
            {
                Array.Resize(ref this.pointsBuffer, this.calculateArcPointsJob.outputPositions.Length);
                this.calculateArcPointsJob.outputPositions.CopyTo(this.pointsBuffer);
                this.arcVisualizerLineRenderer.SetPositions(this.pointsBuffer);
            }
        }

        private void UpdateVisualizers(TrajectoryInfo trajectoryInfo)
        {
            if (this.arcVisualizerLineRenderer && this.calculateArcPointsJobHandle.IsCompleted)
            {
                this.calculateArcPointsJob.SetParameters(trajectoryInfo.finalRay.origin, trajectoryInfo.finalRay.direction * trajectoryInfo.speedOverride,
                    trajectoryInfo.travelTime, this.arcVisualizerLineRenderer.positionCount, this.useGravity ? Physics.gravity.y : 0f);

                this.calculateArcPointsJobHandle = this.calculateArcPointsJob.Schedule(this.calculateArcPointsJob.outputPositions.Length, 32);
            }
            if (this.endpointVisualizerTransform)
            {
                this.endpointVisualizerTransform.SetPositionAndRotation(trajectoryInfo.hitPoint, Util.QuaternionSafeLookRotation(trajectoryInfo.hitNormal));

                if (!this.endpointVisualizerRadiusScale.Equals(0f))
                    this.endpointVisualizerTransform.localScale = Vector3.one * this.endpointVisualizerRadiusScale;
            }
        }

        private void OnPreRenderSceneCam(SceneCamera sceneCam)
        {
            if (this.arcVisualizerLineRenderer)
                this.arcVisualizerLineRenderer.renderingLayerMask = (sceneCam.cameraRigController.target == base.gameObject) ? 1u : 0u;

            if (this.endpointVisualizerTransform)
                this.endpointVisualizerTransform.gameObject.layer = (sceneCam.cameraRigController.target == base.gameObject) ? LayerIndex.defaultLayer.intVal : LayerIndex.noDraw.intVal;
        }
    }
}
