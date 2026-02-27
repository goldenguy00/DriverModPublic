using RoR2;
using UnityEngine;

namespace RobDriver.Modules
{
    internal static class Skins
    {
        public static SkinDef CreateSkinDef(string skinName, Sprite skinIcon, GameObject root, UnlockableDef unlockableDef,
            CharacterModel.RendererInfo[] rendererInfos, SkinDefParams.MeshReplacement[] meshReplacements, SkinDefParams.GameObjectActivation[] gameObjectActivations)
        {
            return R2API.Skins.CreateNewSkinDef(new R2API.SkinDefParamsInfo
            {
                Name = skinName,
                NameToken = skinName,
                Icon = skinIcon,
                RootObject = root,
                UnlockableDef = unlockableDef,
                RendererInfos = rendererInfos,
                MeshReplacements = meshReplacements,
                GameObjectActivations = gameObjectActivations,
                BaseSkins = [],
                MinionSkinReplacements = [],
                ProjectileGhostReplacements = []
            });
        }
        public static CharacterModel.RendererInfo[] SkinRendererInfos(CharacterModel.RendererInfo[] defaultRenderers, Material[] materials)
        {
            CharacterModel.RendererInfo[] newRendererInfos = new CharacterModel.RendererInfo[defaultRenderers.Length];
            defaultRenderers.CopyTo(newRendererInfos, 0);

            for (int i = 0; i < materials.Length; i++)
                newRendererInfos[i].defaultMaterial = materials[i];

            return newRendererInfos;
        }
    }
}