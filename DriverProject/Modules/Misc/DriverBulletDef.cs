using R2API;
using RobDriver;
using RoR2;
using UnityEngine;

[CreateAssetMenu(fileName = "blt", menuName = "ScriptableObjects/BulletDef", order = 2)]
public class DriverBulletDef : ScriptableObject
{
    [Header("General")]
    public string nameToken = "";
    public DamageType damageType = DamageType.Generic;
    public DamageTypeExtended damageTypeExtended = DamageTypeExtended.Generic;
    public DamageAPI.ModdedDamageType moddedDamageType = DriverDamageTypes.Generic;
    public DriverWeaponTier tier = DriverWeaponTier.Common;

    [Header("Visuals")]
    public Color trailColor = Color.black;

    [HideInInspector]
    public ushort index; // assigned at runtime

    public DamageTypeCombo bulletType
    {
        get
        {
            var damage = new DamageTypeCombo
            {
                damageType = this.damageType,
                damageTypeExtended = this.damageTypeExtended,
                damageSource = DamageSource.Primary
            };
            damage.AddModdedDamageType(moddedDamageType);
            return damage;
        }
    }

    public static DriverBulletDef CreateBulletDefFromInfo(DriverBulletDefInfo bulletDefInfo)
    {
        DriverBulletDef bulletDef = ScriptableObject.CreateInstance<DriverBulletDef>();
        bulletDef.name = bulletDefInfo.nameToken;
        bulletDef.nameToken = bulletDefInfo.nameToken;
        bulletDef.damageType = bulletDefInfo.damageType ?? DamageType.Generic;
        bulletDef.damageTypeExtended = bulletDefInfo.damageTypeExtended ?? DamageTypeExtended.Generic;
        bulletDef.moddedDamageType = bulletDefInfo.moddedDamageType ?? DriverDamageTypes.Generic;
        bulletDef.tier = bulletDefInfo.tier;
        bulletDef.trailColor = bulletDefInfo.trailColor;

        return bulletDef;
    }
}

[System.Serializable]
public struct DriverBulletDefInfo
{
    public string nameToken;

    public DamageType? damageType;
    public DamageTypeExtended? damageTypeExtended;
    public DamageAPI.ModdedDamageType? moddedDamageType;

    public DriverWeaponTier tier;
    public Color trailColor;
}
