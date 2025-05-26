using UnityEngine;
using RoR2;
using RoR2.UI;
using UnityEngine.UI;

namespace RobDriver.Modules.Components.UI
{
    public class CrosshairChargeRing : MonoBehaviour
    {
        public CrosshairController crosshairController { get; set; }

        private SkillLocator target;
        private Image fillBar;

        private void Awake()
        {
            this.fillBar = this.GetComponent<Image>();
            this.fillBar.fillAmount = 0f;
        }

        private void FixedUpdate()
        {
            if (this.target)
                if (this.fillBar) this.fillBar.fillAmount = this.target.secondary.rechargeStopwatch / this.target.secondary.finalRechargeInterval;

            if (this.crosshairController)
            {
                if (this.crosshairController.hudElement.targetCharacterBody)
                    this.target = this.crosshairController.hudElement.targetCharacterBody.skillLocator;
            }
            else
            {
                this.crosshairController = this.GetComponentInParent<CrosshairController>();
            }
        }
    }
}