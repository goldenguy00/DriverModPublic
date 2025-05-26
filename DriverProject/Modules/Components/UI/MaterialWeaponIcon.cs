using UnityEngine;
using UnityEngine.UI;
using RoR2;
using RoR2.UI;
using TMPro;

namespace RobDriver.Modules.Components.UI
{
    public class MaterialWeaponIcon : MonoBehaviour
    {
        public DriverController iDrive;
        public HUD targetHUD;

        public Image mask;
        public Image cooldownRing;
        public TextMeshProUGUI ammoText;
        public GameObject ammoBackground;
        private Color defaultColor;

        public void Start()
        {
            if (this.ammoBackground)
                this.ammoBackground.SetActive(false);

            if (this.ammoText)
            {
                this.ammoText?.gameObject.SetActive(true);
                this.ammoText.enabled = true;
            }

            if (this.cooldownRing)
                this.cooldownRing.enabled = true;

            if (this.mask)
                this.mask.enabled = true;

            this.iDrive = this.targetHUD?.targetBodyObject?.GetComponent<DriverController>();

            if (this.iDrive)
            {
                this.defaultColor = this.targetHUD.healthBar.style.trailingOverHealthBarStyle.baseColor;

                if (this.cooldownRing)
                    this.cooldownRing.color = defaultColor;

                if (this.iDrive.passive.isBullets || this.iDrive.passive.isRyan) 
                    this.iDrive.onConsumeAmmo += SetAmmoTypeDisplay;
            }
        }

        public void OnDestroy()
        {
            if (this.iDrive) 
                this.iDrive.onConsumeAmmo -= SetAmmoTypeDisplay;
        }

        private void Update()
        {
            if (!this.iDrive || !this.mask)
                return;

            var fill = Util.Remap(this.iDrive.weaponTimer, 0f, this.iDrive.maxWeaponTimer, 0f, 1f);

            if (this.iDrive.maxWeaponTimer <= 0)
                fill = 0f;

            if (fill > mask.fillAmount)
                this.mask.fillAmount = Mathf.Clamp01(fill);

            this.mask.fillAmount = Mathf.Lerp(this.mask.fillAmount, fill, Time.deltaTime * 8f);
        }

        private void SetAmmoTypeDisplay()
        {
            if (!this.iDrive)
                return;

            // display text and change color
            if (this.iDrive.HasSpecialBullets && this.iDrive.weaponTimer > 0)
            {
                if (this.ammoText)
                {
                    this.ammoText.text = $"<color=#{ColorUtility.ToHtmlStringRGBA(this.iDrive.currentBulletDef.trailColor)}>" + this.iDrive.currentBulletDef.bulletName + Helpers.colorSuffix;
                    this.ammoText.enabled = true;
                    this.ammoText.gameObject.SetActive(true);
                }

                this.ammoBackground?.SetActive(true);

                if (this.cooldownRing)
                    this.cooldownRing.color = this.iDrive.currentBulletDef.trailColor;
            }
            else
            {
                if (this.ammoText)
                    this.ammoText.text = string.Empty;

                this.ammoBackground?.SetActive(false);

                if (this.cooldownRing)
                    this.cooldownRing.color = defaultColor;
            }
        }
    }
}