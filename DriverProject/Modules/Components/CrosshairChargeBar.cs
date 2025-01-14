using UnityEngine;
using RoR2.UI;
using UnityEngine.UI;

namespace RobDriver.Modules.Components
{
    public class CrosshairChargeBar : MonoBehaviour
    {
        public CrosshairController crosshairController { get; set; }

        private DriverController iDrive;
        private Image fillBar;

        private void Awake()
        {
            this.fillBar = this.GetComponent<Image>();
            this.fillBar.fillAmount = 0f;
        }

        private void FixedUpdate()
        {
            if (this.iDrive)
            {
                if (this.fillBar) this.fillBar.fillAmount = this.iDrive.chargeValue;

                return;
            }

            if (this.crosshairController)
            {
                if (this.crosshairController.hudElement.targetCharacterBody)
                {
                    this.iDrive = this.crosshairController.hudElement.targetCharacterBody.gameObject.GetComponent<DriverController>();
                }
            }
            else
            {
                this.crosshairController = this.GetComponentInParent<CrosshairController>();
            }
        }
    }
}