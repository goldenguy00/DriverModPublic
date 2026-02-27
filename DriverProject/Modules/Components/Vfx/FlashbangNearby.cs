using HunkMod.Modules;
using RoR2;
using UnityEngine;

namespace RobDriver.Modules.Components
{
    public class FlashbangNearby : MonoBehaviour
    {
        private void OnEnable()
        {
            if (HunkMod.Modules.Config.disableFlashbang.Value)
                return;

            PlayerCharacterMasterController localPlayer = null;
            foreach (var instance in PlayerCharacterMasterController.instances)
            {
                if (instance.hasAuthority)
                {
                    localPlayer = instance;
                    break;
                }
            }

            if (!localPlayer)
            {
                return;
            }

            NetworkUser networkUser = localPlayer.networkUser;
            if (!networkUser || !networkUser.cameraRigController || !networkUser.cameraRigController.hud)
            {
                return;
            }

            if (localPlayer.body != null)
            {
                if (Vector3.Distance(base.transform.position, localPlayer.body.aimOrigin) < 50f &&
                    !Physics.Linecast(base.transform.position, localPlayer.body.aimOrigin, LayerIndex.world.mask))
                {
                    GameObject overlay = Object.Instantiate(HunkAssets.mainAssetBundle.LoadAsset<GameObject>("FlashbangOverlay"));
                    overlay.transform.parent = networkUser.cameraRigController.hud.mainContainer.transform;
                    overlay.gameObject.SetActive(value: true);

                    RectTransform rect = overlay.GetComponent<RectTransform>();
                    rect.sizeDelta = Vector2.one;
                    rect.localPosition = Vector3.zero;

                    Object.Destroy(overlay, 5f);
                }
            }
        }
    }
}
