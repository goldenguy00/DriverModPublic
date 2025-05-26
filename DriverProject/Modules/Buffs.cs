using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RobDriver.Modules
{
    public static class Buffs
    {
        internal static List<BuffDef> buffDefs = new List<BuffDef>();

        internal static BuffDef dazedDebuff;
        internal static BuffDef woundDebuff;
        internal static BuffDef gougeDebuff;
        internal static BuffDef syringeDamageBuff;
        internal static BuffDef syringeAttackSpeedBuff;
        internal static BuffDef syringeCritBuff;
        internal static BuffDef syringeScepterBuff;
        internal static BuffDef syringeNewBuff;

        static Buffs()
        {
            dazedDebuff = AddNewBuff("RobDriverDazedDebuff", Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/Common/bdCloak.asset").WaitForCompletion().iconSprite, Color.grey, canStack: false, isDebuff: true);
            woundDebuff = AddNewBuff("RobDriverWoundDebuff", Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/Bandit2/bdBanditSkull.asset").WaitForCompletion().iconSprite, Color.red, canStack: false, isDebuff: true);
            gougeDebuff = AddNewBuff("RobDriverGougeDebuff", Assets.mainAssetBundle.LoadAsset<Sprite>("texIconBuffGouge"), new Color(0.67058825f, 0.15686275f, 0.16862746f), canStack: true, isDebuff: true, isDot: true);

            syringeDamageBuff = AddNewBuff("RobDriverSyringeDamageBuff", Assets.mainAssetBundle.LoadAsset<Sprite>("texBuffSyringe"), new Color(1f, 70f / 255f, 75f / 255f));
            syringeAttackSpeedBuff = AddNewBuff("RobDriverSyringeAttackSpeedBuff", Assets.mainAssetBundle.LoadAsset<Sprite>("texBuffSyringe"), new Color(1f, 170f / 255f, 45f / 255f));
            syringeCritBuff = AddNewBuff("RobDriverSyringeCritBuff", Assets.mainAssetBundle.LoadAsset<Sprite>("texBuffSyringe"), new Color(1f, 80f / 255f, 17f / 255f));
            syringeNewBuff = AddNewBuff("RobDriverSyringeNewBuff", Assets.mainAssetBundle.LoadAsset<Sprite>("texBuffSyringe"), new Color(1f, 70f / 255f, 75f / 255f));
            syringeScepterBuff = AddNewBuff("RobDriverSyringeScepterBuff", Assets.mainAssetBundle.LoadAsset<Sprite>("texBuffSyringe"), Survivors.Driver.characterColor);
        }

        // simple helper method
        internal static BuffDef AddNewBuff(string buffName, Sprite buffIcon, Color buffColor, bool canStack = false, bool isDebuff = false, bool isDot = false)
        {
            BuffDef buffDef = ScriptableObject.CreateInstance<BuffDef>();
            buffDef.name = buffName;
            buffDef.buffColor = buffColor;
            buffDef.canStack = canStack;
            buffDef.isDebuff = isDebuff;
            buffDef.eliteDef = null;
            buffDef.iconSprite = buffIcon;
            buffDef.ignoreGrowthNectar = isDebuff | isDot;
            buffDef.isDOT = isDot;
            buffDef.flags = isDot ? BuffDef.Flags.NONE : BuffDef.Flags.ExcludeFromNoxiousThorns;

            buffDefs.Add(buffDef);

            return buffDef;
        }
    }
}