using UnityEngine;
using RoR2;
using UnityEngine.AddressableAssets;

namespace RobDriver.Modules.Components
{
    public class DiscardedWeaponComponent : MonoBehaviour
    {
        private const float upForce = 9f;
        private const float backForce = 2.4f;
        private const float lifetime = 60f;
        private const float rotateSpeedX = 0f;
        private const float rotateSpeedZ = -1200f;

        private SkinnedMeshRenderer targetRenderer;
        private Rigidbody rb;
        private Transform targetTransform;
        private GameObject effectInstance;
        private DriverWeaponDef weaponDef;

        private float stopwatch;
        private bool spinning;

        private void Awake()
        {
            this.targetRenderer = this.GetComponentInChildren<SkinnedMeshRenderer>();
            this.rb = this.GetComponent<Rigidbody>();
            this.targetTransform = this.transform.GetChild(1);

            Destroy(this.gameObject, lifetime);
        }

        private void FixedUpdate()
        {
            if (this.targetTransform && this.spinning)
            {
                this.stopwatch += Time.fixedDeltaTime;

                this.targetTransform.RotateAround(this.transform.position, this.transform.forward, rotateSpeedX * Time.fixedDeltaTime);
                this.targetTransform.RotateAround(this.transform.position, this.transform.right, rotateSpeedZ * Time.fixedDeltaTime);
                //this.targetTransform.Rotate(new Vector3(Time.fixedDeltaTime * this.rotateSpeed), this.targetTransform.localRotation.eulerAngles.y + (Time.fixedDeltaTime * this.rotateSpeedY), this.targetTransform.localRotation.eulerAngles.z + (Time.fixedDeltaTime * this.rotateSpeedZ)));
                //this.targetTransform.localRotation = Quaternion.Euler(;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (this.spinning && this.stopwatch >= 0.25f)
            {
                this.spinning = false;

                if (this.effectInstance)
                    Destroy(this.effectInstance);

                if (this.rb)
                    this.rb.collisionDetectionMode = CollisionDetectionMode.Discrete; // optimization

                Util.PlaySound("sfx_driver_gun_drop", this.gameObject);
            }
        }

        private void StartSpin()
        {
            this.spinning = true;

            this.effectInstance = GameObject.Instantiate(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/CommandoReloadFX.prefab").WaitForCompletion());
            this.effectInstance.transform.parent = this.transform;
            if (this.weaponDef && this.weaponDef.animationSet == DriverWeaponDef.AnimationSet.TwoHanded)
            {
                this.effectInstance.transform.localPosition = new Vector3(-0.3f, 0f, 0f);
                this.effectInstance.transform.localScale = new Vector3(3.5f, 2.5f, -32f);
            }
            else
            {
                this.effectInstance.transform.localRotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
                this.effectInstance.transform.localPosition = Vector3.zero;
            }

            Util.PlaySound("sfx_driver_gun_throw", this.gameObject);
        }

        public void Init(DriverWeaponDef weaponDef, Vector3 forward, Vector3 velocity)
        {
            if (this.targetRenderer)
            {
                this.targetRenderer.sharedMesh = weaponDef.mesh;
                this.targetRenderer.material = weaponDef.material;
            }

            if (this.rb) 
                this.rb.velocity = (forward * -backForce) + (Vector3.up * upForce) + velocity;

            this.weaponDef = weaponDef;
            this.StartSpin();
        }
    }
}