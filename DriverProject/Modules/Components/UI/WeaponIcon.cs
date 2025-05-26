using UnityEngine;
using UnityEngine.UI;
using RoR2;
using RoR2.UI;

namespace RobDriver.Modules.Components.UI
{
    public class WeaponIcon : MonoBehaviour
    {
        public HUD targetHUD;
        public DriverController iDrive;

        public GameObject displayRoot;
        public RawImage iconImage;

        public GameObject flashPanelObject;
        public GameObject reminderFlashPanelObject;
        public GameObject isReadyPanelObject;
        public TooltipProvider tooltipProvider;

        public GameObject durationDisplay;
        public Image durationBar;
        public Image durationBarRed;

        public float maxFill = 1f;

        public void Start()
        {
            this.iDrive = this.targetHUD?.targetBodyObject?.GetComponent<DriverController>();
            if (this.iDrive)
            {
                this.iDrive.onWeaponUpdate += SetDisplay;
            }

            if (this.durationDisplay) 
                this.durationDisplay.SetActive(false);

            SetDisplay();
        }

        public void OnDestroy()
        {
            if (this.iDrive)
                this.iDrive.onWeaponUpdate -= SetDisplay;
        }

        private void Update()
        {
            if (!this.iDrive || this.iDrive.passive.isPistolOnly || !this.durationDisplay)
                return;

            if (this.iDrive.maxWeaponTimer <= 0f)
            {
                this.durationDisplay.SetActive(false);
                return;
            }

            this.durationDisplay.SetActive(true);

            var fill = Util.Remap(this.iDrive.weaponTimer, 0f, this.iDrive.maxWeaponTimer, 0f, this.maxFill);

            if (this.durationBarRed)
            {
                if (fill >= 1f)
                    this.durationBarRed.fillAmount = 1f;

                this.durationBarRed.fillAmount = Mathf.Lerp(this.durationBarRed.fillAmount, fill, Time.deltaTime * 2f);
            }

            this.durationBar.fillAmount = fill;
            this.durationBar.color = this.iDrive.AmmoPercent < 0.2f ? Helpers.badColor : this.iDrive.currentBulletDef.trailColor;
        }

        private void SetDisplay()
        {
            if (!this.iDrive) 
                return;

            if (this.reminderFlashPanelObject)
            {
                var animateUI = this.reminderFlashPanelObject.GetComponent<AnimateUIAlpha>();
                if (animateUI)
                    animateUI.time = 0f;

                this.reminderFlashPanelObject.SetActive(true);
            }

            if (this.flashPanelObject)
            {
                var animateUI = this.flashPanelObject.GetComponent<AnimateUIAlpha>();
                if (animateUI)
                    animateUI.time = 0f;

                this.flashPanelObject.SetActive(true);
            }

            if (this.displayRoot)
            {
                this.displayRoot.SetActive(true);
            }

            if (this.isReadyPanelObject)
            {
                this.isReadyPanelObject.SetActive(true);
            }

            if (this.iconImage)
            {
                this.iconImage.texture = this.iDrive.weaponDef.icon.texture;
                this.iconImage.color = Color.white;
                this.iconImage.enabled = true;
            }

            if (this.tooltipProvider)
            {
                this.tooltipProvider.titleToken = this.iDrive.weaponDef.nameToken;
                this.tooltipProvider.bodyToken = this.iDrive.weaponDef.descriptionToken;
                this.tooltipProvider.titleColor = Survivors.Driver.characterColor;
                this.tooltipProvider.bodyColor = Color.gray;
            }
        }
    }
}