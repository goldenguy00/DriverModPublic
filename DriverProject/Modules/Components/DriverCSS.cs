using UnityEngine;
using RoR2;
using RobDriver.Modules.Survivors;

namespace RobDriver.Modules.Components
{
    public class DriverCSS : CharacterSelectSurvivorPreviewDisplayController
    {
        private SkinnedMeshRenderer weaponRenderer;
        private CharacterModel characterModel;
        private ChildLocator childLocator;
        private Animator animator;

        private DriverArsenal driverArsenal;
        private SkillLocator skillLocator;

        private DriverWeaponDef weaponDef;
        private BodyIndex driverBodyIndex;

        private void Awake()
        {
            this.animator = this.GetComponent<Animator>();
            this.childLocator = this.GetComponent<ChildLocator>();
            this.characterModel = this.GetComponent<CharacterModel>();
            this.weaponRenderer = this.childLocator.FindChild("PistolModel").GetComponent<SkinnedMeshRenderer>();
        }

        private new void OnEnable()
        {
            this.driverArsenal = this.bodyPrefab.GetComponent<DriverArsenal>();
            this.skillLocator = this.bodyPrefab.GetComponent<SkillLocator>();

            this.currentLoadout = Loadout.RequestInstance();
            this.driverBodyIndex = Driver.bodyIndex;

            NetworkUser.onLoadoutChangedGlobal += this.OnLoadoutChangedGlobal;
            RoR2Application.onNextUpdate += this.Refresh;
        }

        private new void OnDisable()
        {
            NetworkUser.onLoadoutChangedGlobal -= this.OnLoadoutChangedGlobal;
            this.currentLoadout = Loadout.ReturnInstance(this.currentLoadout);
        }

        private new void Refresh()
        {
            if (this && this.networkUser)
                this.OnLoadoutChangedGlobal(this.networkUser);
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

            SetModelVisuals();
            RoR2Application.onNextUpdate += SetModelVisuals;
        }

        private void SetModelVisuals()
        {
            int skinIndex = (int)this.currentLoadout.bodyLoadoutManager.GetSkinIndex(this.driverBodyIndex);
            var bodySkins = BodyCatalog.GetBodySkins(this.driverBodyIndex);
            var modelSwapInfo = DriverWeaponSkinCatalog.GetModelSwapInfoForWeapon(bodySkins, skinIndex, this.weaponDef)[0];

            this.weaponRenderer.sharedMesh = modelSwapInfo.mesh;
            this.weaponRenderer.sharedMaterial = modelSwapInfo.material;

            for (int i = 0; i < this.characterModel.baseRendererInfos.Length; i++)
            {
                ref var info = ref this.characterModel.baseRendererInfos[i];

                info.renderer.enabled = true;
                if (info.renderer == this.weaponRenderer)
                    info.defaultMaterial = modelSwapInfo.material;
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