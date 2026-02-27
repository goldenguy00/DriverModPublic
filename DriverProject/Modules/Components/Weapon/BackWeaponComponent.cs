using UnityEngine;

namespace RobDriver.Modules.Components
{
    public class BackWeaponComponent : MonoBehaviour
    {
        public SkinnedMeshRenderer targetRenderer;

        public DriverWeaponDef weaponDef;
        public Mesh mesh;
        public Material material;

        private void Awake()
        {
            this.targetRenderer = this.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>();
            this.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            this.transform.localScale = Vector3.one;
        }

        private void Start()
        {
            this.targetRenderer.sharedMesh = this.mesh;
            this.targetRenderer.sharedMaterial = this.material;
        }
    }
}