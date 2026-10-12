using UnityEngine;
using UnityEngine.UI;
using RoR2;
using RoR2.UI;

namespace RobDriver.Modules.Components.UI
{
    public class AmmoDisplay : MonoBehaviour
    {
        public HUD targetHUD;
        public DriverController iDrive;

        public HGTextMeshProUGUI targetText;
        public GameObject textParent;
        public GameObject durationDisplay;
        public Image durationBar;
        public Image durationBarRed;

        private void Start()
        {
            this.iDrive = this.targetHUD?.targetBodyObject?.GetComponent<DriverController>();

            if (this.iDrive) 
                this.iDrive.onConsumeAmmo += SetDisplay;

            if (this.targetText)
            {
                this.targetText.text = string.Empty;
                this.textParent = this.targetText.transform.parent.gameObject;
                this.textParent.SetActive(false);
            }

            this.durationDisplay?.SetActive(false);
        }

        private void OnDestroy()
        {
            if (this.iDrive)
                this.iDrive.onConsumeAmmo -= SetDisplay;
        }

        private void Update()
        {
            if (this.iDrive && this.iDrive.maxWeaponTimer > 0f)
            {
                var fill = Util.Remap(this.iDrive.weaponTimer, 0f, this.iDrive.maxWeaponTimer, 0f, 1f);

                if (this.durationBarRed)
                {
                    if (fill >= 1f) this.durationBarRed.fillAmount = 1f;
                    this.durationBarRed.fillAmount = Mathf.Lerp(this.durationBarRed.fillAmount, fill, Time.deltaTime * 2f);
                }

                if (this.durationBar)
                    this.durationBar.fillAmount = fill;
            }
        }

        private void SetDisplay()
        {
            if (this.iDrive.maxWeaponTimer <= 0f)
            {
                if (this.targetText)
                {
                    this.targetText.text = string.Empty;
                    this.textParent.SetActive(false);
                }
                this.durationDisplay?.SetActive(false);
                return;
            }

            // pistol only, text and no bar
            if (this.iDrive.passive.isPistolOnly)
            {
                if (this.iDrive.weaponTimer <= 0f)
                {
                    const string txtFormat = "<color=#C80000>0 / {0}</color>";
                    SetText(string.Format(txtFormat, Mathf.CeilToInt(this.iDrive.maxWeaponTimer)));
                }
                else
                {
                    const string txtFormat = "{0} / {1}";
                    SetText(string.Format(txtFormat, Mathf.CeilToInt(this.iDrive.weaponTimer), Mathf.CeilToInt(this.iDrive.maxWeaponTimer)));
                }
                return;
            }

            // ammo passives, bar and text
            if (this.iDrive.HasSpecialBullets)
            {
                // disable text first, let the empty bar linger for a bit
                if (this.iDrive.weaponTimer <= 0)
                {
                    SetText(string.Empty);
                    return;
                }

                this.durationDisplay?.SetActive(true);

                if (this.durationBar)
                    this.durationBar.color = this.iDrive.AmmoPercent < 0.2f ? Helpers.badColor : this.iDrive.currentBulletDef.trailColor;

                const string txtFormat = "<color=#{0}>{1}</color>";
                SetText(string.Format(txtFormat, ColorUtility.ToHtmlStringRGBA(this.iDrive.currentBulletDef.trailColor), this.iDrive.currentBulletDef.bulletName));
            }

            // infinite ammo
            else if (this.iDrive.weaponTimer == this.iDrive.maxWeaponTimer)
            {
                this.durationDisplay?.SetActive(false);
                SetText(string.Empty);
            }

            // default weapon
            else
            {
                this.durationDisplay?.SetActive(false);
                SetText(string.Empty);
            }
        }

        private void SetText(string text)
        {
            if (!this.targetText)
                return;

            this.targetText.text = text;
            this.textParent.SetActive(!string.IsNullOrEmpty(text));
        }
    }
}