using RoR2;
using UnityEngine;
using EntityStates;
using RobDriver.SkillStates.BaseStates;

namespace RobDriver.SkillStates.Driver.GolemGun
{
    public class ChargeLaser : BaseDriverSkillState
	{
		public static float baseDuration = 0.3f;
		public static float laserMaxWidth = 0.2f;
        public static float maxDistance = 1000f;

		private float duration;
		private uint chargePlayID;

		private GameObject chargeEffect;
		private GameObject laserEffect;
		private LineRenderer laserLineComponent;
        private EffectManagerHelper _efh_Charge;

		private float flashTimer;
		private bool laserOn;

        private GameObject effectPrefab => EntityStates.GolemMonster.ChargeLaser.effectPrefab;
        private GameObject laserPrefab => EntityStates.GolemMonster.ChargeLaser.laserPrefab;

        public override void OnEnter()
		{
			base.OnEnter();

			this.duration = ChargeLaser.baseDuration / this.attackSpeedStat;
			this.chargePlayID = Util.PlayAttackSpeedSound(EntityStates.GolemMonster.ChargeLaser.attackSoundString, this.gameObject, 10f + this.attackSpeedStat);

            var parent = this.childLocator.FindChild("ShotgunMuzzle");
            if (this.effectPrefab)
            {
                if (!EffectManager.ShouldUsePooledEffect(effectPrefab))
                {
                    this.chargeEffect = Object.Instantiate(effectPrefab, parent.position, parent.rotation);
                }
                else
                {
                    this._efh_Charge = EffectManager.GetAndActivatePooledEffect(effectPrefab, parent.position, parent.rotation);
                    this.chargeEffect = this._efh_Charge.gameObject;
                }

                this.chargeEffect.transform.parent = parent;

                var particleDuration = chargeEffect.GetComponent<ScaleParticleSystemDuration>();
                if (particleDuration)
                    particleDuration.newDuration = this.duration;
            }

            if (this.laserPrefab)
            {
                this.laserEffect = GameObject.Instantiate(this.laserPrefab, parent.position, parent.rotation);
                this.laserEffect.transform.parent = parent;
                this.laserEffect.SetActive(value: true);

                this.laserLineComponent = this.laserEffect.GetComponent<LineRenderer>();
            }

            base.characterBody.SetAimTimer(this.duration);

			this.flashTimer = 0f;
			this.laserOn = true;

			base.PlayCrossfade("Gesture, Override", "AimTwohand", this.duration * 0.5f);
			base.PlayAnimation("AimPitch", "ShotgunAimPitch");
		}

		public override void Update()
		{
			base.Update();

			if (this.laserEffect && this.laserLineComponent)
			{
				Ray aimRay = base.GetAimRay();
				Vector3 position = this.laserEffect.transform.parent.position;
				Vector3 point = aimRay.GetPoint(maxDistance);

				if (Physics.Raycast(aimRay, out var raycastHit, maxDistance, LayerIndex.world.mask | LayerIndex.entityPrecise.mask))
				{
					point = raycastHit.point;
				}
				this.laserLineComponent.SetPosition(0, position);
				this.laserLineComponent.SetPosition(1, point);

				float num2;
				if (this.duration - base.age > 0.5f)
				{
					num2 = base.age / this.duration;
				}
				else
				{
					this.flashTimer -= Time.deltaTime;
					if (this.flashTimer <= 0f)
					{
						this.laserOn = !this.laserOn;
						this.flashTimer = 0.033333335f;
					}
					num2 = (this.laserOn ? 1f : 0f);
				}

				num2 *= ChargeLaser.laserMaxWidth;

				this.laserLineComponent.startWidth = num2;
				this.laserLineComponent.endWidth = num2;
			}
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();

			this.characterBody.outOfCombatStopwatch = 0f;
			base.characterBody.SetAimTimer(0.2f);

			if (base.fixedAge >= this.duration && base.isAuthority)
			{
                this.outer.SetNextState(new FireLaser());
			}
        }

        public override void OnExit()
        {
            base.OnExit();
            
            AkSoundEngine.StopPlayingID(this.chargePlayID);

            if (chargeEffect)
            {
                if (!EffectManager.UsePools)
                {
                    EntityState.Destroy(chargeEffect);
                }
                else if (_efh_Charge != null && _efh_Charge.OwningPool != null)
                {
                    if (!_efh_Charge.OwningPool.IsObjectInPool(_efh_Charge))
                    {
                        _efh_Charge.OwningPool.ReturnObject(_efh_Charge);
                    }
                }
                else
                {
                    if (_efh_Charge != null)
                    {
                        Debug.LogFormat("ChargeLaser has no owning pool {0} {1}", base.gameObject.name, base.gameObject.GetInstanceID());
                    }

                    EntityState.Destroy(chargeEffect);
                }
            }

            if (laserEffect && laserEffect.activeInHierarchy)
                laserEffect.SetActive(value: false);

            if (this.outer.destroying)
            {
                base.PlayAnimation("Gesture, Override", "BufferEmpty");
                base.PlayAnimation("AimPitch", "AimPitch");
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority() => InterruptPriority.Skill;
    }
}