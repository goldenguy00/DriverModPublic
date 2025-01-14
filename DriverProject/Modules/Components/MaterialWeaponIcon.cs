using UnityEngine;
using UnityEngine.UI;
using RoR2;
using RoR2.UI;
using TMPro;

namespace RobDriver.Modules.Components
{
    public class MaterialWeaponIcon : MonoBehaviour
	{
		public DriverController iDrive;
        public HUD targetHUD;
		public WeaponIcon icon;

		public Image mask;
        public Image cooldownRing;
		public TextMeshProUGUI ammoText;
        public GameObject ammoBackground;

        private bool registerEvent;
        private Color defaultColor;

		public void Start()
        {
            this.ammoBackground.SetActive(false);
            this.ammoText.enabled = true;
            this.ammoText.gameObject.SetActive(true);
            this.cooldownRing.enabled = true;
            this.mask.enabled = true;
            this.iDrive = this.targetHUD?.targetBodyObject?.GetComponent<DriverController>();

            if (this.iDrive)
			{
                this.defaultColor = this.targetHUD.healthBar.style.trailingOverHealthBarStyle.baseColor;
                this.cooldownRing.color = defaultColor;

                this.registerEvent = this.iDrive.passive.isBullets || this.iDrive.passive.isRyan;
                if (this.registerEvent) this.iDrive.onConsumeAmmo += SetAmmoTypeDisplay;
            }
		}

		public void OnDestroy()
		{
            if (this.iDrive && this.registerEvent) this.iDrive.onConsumeAmmo -= SetAmmoTypeDisplay;
		}

        private void Update()
        {
            if (this.iDrive)
            {
                float fill = Util.Remap(this.iDrive.weaponTimer, 0f, this.iDrive.maxWeaponTimer, 0f, 1f);
                if (this.iDrive.maxWeaponTimer <= 0) fill = 0f;

                if (fill > mask.fillAmount) this.mask.fillAmount = Mathf.Clamp01(fill);
                this.mask.fillAmount = Mathf.Lerp(this.mask.fillAmount, fill, Time.deltaTime * 8f);
            }
        }

        private void SetAmmoTypeDisplay()
        {
            if (!this.iDrive) return;

            // display text and change color
            if (this.iDrive.HasSpecialBullets && this.iDrive.weaponTimer > 0)
            {
                this.ammoText.text = $"<color=#{ColorUtility.ToHtmlStringRGBA(this.iDrive.currentBulletDef.trailColor)}>" + this.iDrive.currentBulletDef.nameToken + Helpers.colorSuffix;
                this.ammoText.enabled = true;
                this.ammoText.gameObject.SetActive(true);
                this.ammoBackground.SetActive(true);
                this.cooldownRing.color = this.iDrive.currentBulletDef.trailColor;
            }
            else
            {
                this.ammoText.text = string.Empty;
                this.ammoBackground.SetActive(false);
                this.cooldownRing.color = defaultColor;
            }
        }
	}
}