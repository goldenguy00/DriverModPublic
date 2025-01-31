using UnityEngine;

namespace RobDriver.Modules.Components
{
    public class BackWeaponComponent : MonoBehaviour
    {
        public SkinnedMeshRenderer targetRenderer { get; set; }

        private void Awake()
        {
            if (!this.targetRenderer) this.targetRenderer = this.GetComponentInChildren<SkinnedMeshRenderer>();
        }

        public void Init(DriverWeaponDef weaponDef)
        {
            if (this.targetRenderer)
            {
                if (weaponDef.equipAnimationString != "BufferEmpty")
                    this.transform.localPosition = new Vector3(-0.35f, 0f, 1.5f);
                else
                    this.transform.localPosition = Vector3.zero;

                this.targetRenderer.sharedMesh = weaponDef.mesh;
                this.targetRenderer.material = weaponDef.material;
            }
        }
    }
}