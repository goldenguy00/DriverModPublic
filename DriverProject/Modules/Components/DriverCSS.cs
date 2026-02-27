using UnityEngine;
using RoR2;
using RobDriver.Modules.Survivors;

namespace RobDriver.Modules.Components
{
    public class DriverCSS : CharacterSelectSurvivorPreviewDisplayController
    {
        private CharacterModel characterModel;
        private ChildLocator childLocator;
        private Animator animator;

        private DriverArsenal driverArsenal;
        private SkillLocator skillLocator;

        private DriverWeaponDef weaponDef;
        private BodyIndex driverBodyIndex;
        private DriverWeaponDef.ModelSwapInfo modelSwapInfo;

        private void Awake()
        {
            this.animator = this.GetComponent<Animator>();
            this.childLocator = this.GetComponent<ChildLocator>();
            this.characterModel = this.GetComponent<CharacterModel>();

            this.driverArsenal = Driver.characterPrefab.GetComponent<DriverArsenal>();
            this.skillLocator = Driver.characterPrefab.GetComponent<SkillLocator>();

            this.driverBodyIndex = Driver.bodyIndex;
            this.currentLoadout = Loadout.RequestInstance();
        }

        private new void OnEnable()
        {
            NetworkUser.onLoadoutChangedGlobal += this.OnLoadoutChangedGlobal;
        }

        private new void OnDisable()
        {
            NetworkUser.onLoadoutChangedGlobal -= this.OnLoadoutChangedGlobal;
        }

        private void OnDestroy()
        {
            this.currentLoadout = Loadout.ReturnInstance(this.currentLoadout);
        }

        private new void OnLoadoutChangedGlobal(NetworkUser changedNetworkUser)
        {
            if (changedNetworkUser != this.networkUser || this.driverBodyIndex == BodyIndex.None)
                return;

            changedNetworkUser.networkLoadout.CopyLoadout(this.currentLoadout);

            var newWeaponDef = DriverWeaponCatalog.Pistol;
            var skillFamily = this.driverArsenal.weaponSkillSlot?.skillFamily;
            if (skillFamily)
            {
                var index = FindSkillSlotIndex(this.driverBodyIndex, skillFamily);
                var bodyLoadout = this.currentLoadout.bodyLoadoutManager.GetReadOnlyBodyLoadout(this.driverBodyIndex);
                if (bodyLoadout?.IsSkillVariantValid(index) == true)
                {
                    var newSkillDef = skillFamily.variants[bodyLoadout.skillPreferences[index]].skillDef;
                    if (newSkillDef)
                        newWeaponDef = DriverWeaponCatalog.GetWeaponFromIndex(DriverArsenal.passiveSkills.IndexOf(newSkillDef));
                }
            }

            if (this.weaponDef != newWeaponDef)
            {
                this.weaponDef = newWeaponDef;

                // animator layer
                this.animator.SetLayerWeight((int)DriverWeaponDef.AnimationSet.TwoHanded, 0f);
                this.animator.SetLayerWeight((int)DriverWeaponDef.AnimationSet.BigMelee, 0f);

                if (newWeaponDef.animationSet != DriverWeaponDef.AnimationSet.Default)
                    this.animator.SetLayerWeight((int)newWeaponDef.animationSet, 1f);

                this.animator.Play("CSSIdleIn", (int)DriverWeaponDef.AnimationSet.Default);
            }
        }

        private void Update()
        {
            SetModelVisuals();
        }

        private void SetModelVisuals()
        {
            int skinIndex = (int)this.currentLoadout.bodyLoadoutManager.GetSkinIndex(this.driverBodyIndex);
            var bodySkins = SkinCatalog.GetBodySkinDefs(this.driverBodyIndex);
            var modelSwapInfos = DriverWeaponSkinCatalog.GetModelSwapInfoForWeapon(bodySkins, skinIndex, this.weaponDef);

            for (int i = 0; i < modelSwapInfos.Length; i++)
            {
                SetMeshRenderer(modelSwapInfos[i].childName, modelSwapInfos[i].material, modelSwapInfos[i].mesh);
            }

            this.childLocator.FindChildGameObject("PistolModel").SetActive(true);

            var active = HasSkillVariantEnabled(this.currentLoadout, this.driverBodyIndex, this.skillLocator.utility.skillFamily, Skills.skateboardSkillDef);
            this.childLocator.FindChildGameObject("SkateboardBackModel").SetActive(active);

            /*if (Config.enableRevengence.Value)
            {
                active = HasSkillVariantEnabled(this.currentLoadout, this.driverBodyIndex, this.skillLocator.special.skillFamily, Skills.knifeSkillDef);
                this.childLocator.FindChildGameObject("BackpackModel").SetActive(active);
            }*/
            //this.childLocator.FindChildGameObject("BackpackModel").SetActive(true);
        }

        private void SetMeshRenderer(string childName, Material material, Mesh mesh)
        {
            var childTransform = this.childLocator.FindChild(childName);
            if (!childTransform)
            {
                Log.Error("No child transform with name " + childName);
                return;
            }

            for (int i = 0; i < this.characterModel.baseRendererInfos.Length; i++)
            {
                ref var info = ref this.characterModel.baseRendererInfos[i];

                if (info.renderer?.transform != childTransform)
                    continue;

                info.defaultMaterial = material;
                info.renderer.sharedMaterial = material;

                if (info.renderer is SkinnedMeshRenderer skinRenderer)
                    skinRenderer.sharedMesh = mesh;
                else if (info.renderer.TryGetComponent<MeshFilter>(out var filter))
                    filter.sharedMesh = mesh;
                else
                    Log.Error("no skinned mesh renderer or mesh filter found for " + childTransform.name);
            }
        }

        public void ThrowGun()
        {
            if (this.weaponDef?.animationSet != DriverWeaponDef.AnimationSet.BigMelee)
                Util.PlaySound("sfx_driver_gun_throw", this.gameObject);
        }

        public void CatchGun()
        {
            if (this.weaponDef?.animationSet != DriverWeaponDef.AnimationSet.BigMelee)
                Util.PlaySound("sfx_driver_gun_catch", this.gameObject);
        }

        public void FailCatchGun()
        {

        }

        public void GunDrop()
        {
            if (this.weaponDef?.animationSet != DriverWeaponDef.AnimationSet.BigMelee)
                Util.PlaySound("sfx_driver_gun_drop", this.gameObject);
        }
    }
}