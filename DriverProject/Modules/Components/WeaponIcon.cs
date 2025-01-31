using UnityEngine;
using UnityEngine.UI;
using RoR2;
using RoR2.UI;

namespace RobDriver.Modules.Components
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

		public void Start()
		{
            this.iDrive = this.targetHUD?.targetBodyObject?.GetComponent<DriverController>();
			if (this.durationDisplay) this.durationDisplay.SetActive(false);
			if (this.iDrive) this.iDrive.onWeaponUpdate += SetDisplay;
        }

        public void OnDestroy()
        {
            if (this.iDrive) this.iDrive.onWeaponUpdate -= SetDisplay;
        }

        private void Update()
        {
			if (!this.iDrive || this.iDrive.passive.isPistolOnly || !this.durationDisplay) return;

            if (this.iDrive.maxWeaponTimer > 0f)
            {
                this.durationDisplay.SetActive(true);

                float fill = Util.Remap(this.iDrive.weaponTimer, 0f, this.iDrive.maxWeaponTimer, 0f, 1f);

                if (this.durationBarRed)
                {
                    if (fill >= 1f) this.durationBarRed.fillAmount = 1f;
                    this.durationBarRed.fillAmount = Mathf.Lerp(this.durationBarRed.fillAmount, fill, Time.deltaTime * 2f);
                }

                this.durationBar.fillAmount = fill;
				this.durationBar.color = this.iDrive.currentBulletDef.trailColor;
            }
            else
            {
                this.durationDisplay.SetActive(false);
            }
        }

        private void SetDisplay()
		{
			if (!this.iDrive) return;

			this.DoStockFlash();

            if (this.displayRoot) this.displayRoot.SetActive(true); 
            if (this.isReadyPanelObject) this.isReadyPanelObject.SetActive(true);

            if (this.iconImage)
			{
				this.iconImage.texture = this.iDrive.weaponDef.icon;
				this.iconImage.color = Color.white;
                this.iconImage.enabled = true;
			}
            if (this.tooltipProvider)
			{
				this.tooltipProvider.titleToken = this.iDrive.weaponDef.nameToken;
				this.tooltipProvider.bodyToken = this.iDrive.weaponDef.descriptionToken;
				this.tooltipProvider.titleColor = Modules.Survivors.Driver.characterColor;
				this.tooltipProvider.bodyColor = Color.gray;
			}
        }

        private void DoReminderFlash()
		{
			if (this.reminderFlashPanelObject)
			{
				AnimateUIAlpha animateUI = this.reminderFlashPanelObject.GetComponent<AnimateUIAlpha>();
				if (animateUI) animateUI.time = 0f;
				this.reminderFlashPanelObject.SetActive(true);
			}
		}

		private void DoStockFlash()
		{
			this.DoReminderFlash();
			if (this.flashPanelObject)
			{
				AnimateUIAlpha animateUI = this.flashPanelObject.GetComponent<AnimateUIAlpha>();
				if (animateUI) animateUI.time = 0f;
				this.flashPanelObject.SetActive(true);
			}
		}
	}
}