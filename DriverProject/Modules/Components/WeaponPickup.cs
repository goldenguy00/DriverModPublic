using UnityEngine;
using RoR2;
using UnityEngine.Networking;

namespace RobDriver.Modules.Components
{
    public class WeaponPickup : MonoBehaviour
    {
        [Tooltip("The base object to destroy when this pickup is consumed.")]
        public GameObject baseObject;
        [Tooltip("The team filter object which determines who can pick up this pack.")]
        public TeamFilter teamFilter;

        public DriverWeaponDef weaponDef;
        private DriverBulletDef bulletDef;
        private bool cutAmmo;
        private bool isNewAmmoType;

        public GameObject pickupEffect;
        public GameObject ammoPickup;

        public bool alive { get; private set; } = false;

        public void UpdateWeaponPickup(DriverBulletDef bulletDef, bool cutAmmo, bool isNewAmmoType)
        {
            // make sure this is called before handling the collider logic
            alive = true;
            this.bulletDef = bulletDef;
            this.cutAmmo = cutAmmo;
            this.isNewAmmoType = isNewAmmoType;

            UpdateVisuals();
        }

        private void Awake()
        {
            // default text
            SetTextComponent(this.transform.parent.GetComponentInChildren<RoR2.UI.LanguageTextMeshController>(), this.weaponDef.nameToken, this.weaponDef.tier);

            // disable visuals for non driver
            if (!Modules.Config.sharedPickupVisuals.Value)
            {
                BeginRapidlyActivatingAndDeactivating blinker = this.transform.parent.GetComponentInChildren<BeginRapidlyActivatingAndDeactivating>();
                if (blinker)
                {
                    bool isDriver = false;

                    var localPlayers = LocalUserManager.readOnlyLocalUsersList;
                    foreach (LocalUser i in localPlayers)
                    {
                        if (i.cachedBody.baseNameToken == Modules.Survivors.Driver.bodyNameToken) isDriver = true;
                    }

                    if (!isDriver)
                    {
                        blinker.blinkingRootObject.SetActive(false);
                        Destroy(blinker);
                    }
                    else
                    {
                        SetTextComponent(this.transform.parent.GetComponentInChildren<RoR2.UI.LanguageTextMeshController>(), this.weaponDef.nameToken, this.weaponDef.tier);
                    }
                }
            }

            // uh will this work?
            /*if (Run.instance)
			{
				float rng = Run.instance.stageRng.nextNormalizedFloat;

				if (rng > 0.5f) this.SetWeapon(DriverWeapon.MachineGun);
				else this.SetWeapon(DriverWeapon.Shotgun);
			}*/
            // no it doesn't, clients don't have the rng

            // i'm a dirty hack
            // lock me up and throw away the key
            this.Invoke("Fuck", 59.5f);
        }

        private void OnTriggerStay(Collider collider)
        {
            // can this run on every client? i don't know but let's find out
            if (NetworkServer.active && this.alive/* && TeamComponent.GetObjectTeam(collider.gameObject) == this.teamFilter.teamIndex*/)
            {
                // well it can but it's not a fix.
                DriverController iDrive = collider.GetComponent<DriverController>();
                if (iDrive)
                {
                    this.alive = false;

                    Modules.Achievements.DriverPistolPassiveAchievement.weaponPickedUp = true;
                    Modules.Achievements.DriverGodslingPassiveAchievement.weaponPickedUpHard = true;

                    iDrive.ServerPickUpWeapon(iDrive, this.weaponDef, this.bulletDef, this.cutAmmo, this.isNewAmmoType);
                    EffectManager.SimpleEffect(this.pickupEffect, this.transform.position, Quaternion.identity, true);
                    UnityEngine.Object.Destroy(this.baseObject);
                }
            }
        }

        private void UpdateVisuals()
        {
            // non - driver player, ignore all
            var blinker = this.transform.parent.GetComponentInChildren<BeginRapidlyActivatingAndDeactivating>();
            if (!blinker) return;

            // swap to ammo visuals
            foreach (LocalUser i in LocalUserManager.readOnlyLocalUsersList)
            {
                if (i?.cachedBody && i.cachedBody.hasEffectiveAuthority)
                {
                    if (i.cachedBody.baseNameToken == Modules.Survivors.Driver.bodyNameToken && i.cachedBody.TryGetComponent<DriverController>(out var iDrive) && iDrive)
                    {
                        if (iDrive.passive.isPistolOnly || iDrive.passive.isBullets || (iDrive.passive.isRyan && this.isNewAmmoType))
                        {
                            if (!ammoPickup)
                            {
                                foreach (MeshRenderer h in blinker.blinkingRootObject.GetComponentsInChildren<MeshRenderer>())
                                {
                                    h.enabled = false;
                                }

                                ammoPickup = GameObject.Instantiate(Modules.Assets.ammoPickupModel, blinker.blinkingRootObject.transform);
                                ammoPickup.transform.localPosition = Vector3.zero;
                                ammoPickup.transform.localRotation = Quaternion.identity;
                            }

                            var ammoText = ammoPickup.transform.GetComponentInChildren<RoR2.UI.LanguageTextMeshController>();
                            if (this.bulletDef)
                            {
                                SetTextComponent(ammoText, this.bulletDef.nameToken, this.bulletDef.tier);
                                ammoText.textMeshPro.outlineColor = this.bulletDef.trailColor;
                                ammoText.textMeshPro.outlineWidth *= 0.75f;
                            }
                            else
                            {
                                // this should never happen, but juuuust in case
                                SetTextComponent(ammoText, "Bullets", DriverWeaponTier.Uncommon);
                            }
                        }
                    }
                    break;
                }
            }
        }

        private void SetTextComponent(RoR2.UI.LanguageTextMeshController textComponent, string nameToken, DriverWeaponTier tier)
        {
            if (textComponent)
            {
                textComponent.token = nameToken;

                if (this.cutAmmo)
                {
                    textComponent.textMeshPro.color = Modules.Helpers.badColor;
                    return;
                }

                switch (tier)
                {
                    case DriverWeaponTier.Common:
                        textComponent.textMeshPro.color = Modules.Helpers.whiteItemColor;
                        break;
                    case DriverWeaponTier.Uncommon:
                        textComponent.textMeshPro.color = Modules.Helpers.greenItemColor;
                        break;
                    case DriverWeaponTier.Legendary:
                        textComponent.textMeshPro.color = Modules.Helpers.redItemColor;
                        break;
                    case DriverWeaponTier.Unique:
                        textComponent.textMeshPro.color = Modules.Helpers.yellowItemColor;
                        break;
                    case DriverWeaponTier.Lunar:
                        textComponent.textMeshPro.color = Modules.Helpers.lunarItemColor;
                        break;
                    case DriverWeaponTier.Void:
                        textComponent.textMeshPro.color = Modules.Helpers.voidItemColor;
                        break;
                }
            }
        }

        private void Fuck()
        {
            if (this.alive)
            {
                Modules.Achievements.SupplyDropAchievement.weaponHasDespawned = true;
            }
        }
    }
}