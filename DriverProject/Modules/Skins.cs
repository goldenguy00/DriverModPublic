using RoR2;
using UnityEngine;

namespace RobDriver.Modules
{
    public static class Skins
    {
        public static SkinDef CreateSkinDef(string skinName, Sprite skinIcon, CharacterModel.RendererInfo[] rendererInfos, SkinnedMeshRenderer mainRenderer, GameObject root)
        {
            return CreateSkinDef(skinName, skinIcon, rendererInfos, mainRenderer, root, null);
        }

        public static SkinDef CreateSkinDef(string skinName, Sprite skinIcon, CharacterModel.RendererInfo[] rendererInfos, SkinnedMeshRenderer mainRenderer, GameObject root, UnlockableDef unlockableDef)
        {
            R2API.SkinDefInfo skinDefInfo = new R2API.SkinDefInfo
            {
                BaseSkins = [],
                GameObjectActivations = [],
                Icon = skinIcon,
                MeshReplacements = [],
                MinionSkinReplacements = [],
                Name = skinName,
                NameToken = skinName,
                ProjectileGhostReplacements = [],
                RendererInfos = rendererInfos,
                RootObject = root,
                UnlockableDef = unlockableDef
            };

            SkinDef skin = R2API.Skins.CreateNewSkinDef(skinDefInfo);

            return skin;
        }
    }
}