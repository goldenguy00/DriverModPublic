using UnityEngine;

[CreateAssetMenu(fileName = "rsd", menuName = "ScriptableObjects/DriverWeaponSkinDef", order = 3)]
public class DriverWeaponSkinDef : ScriptableObject
{
    [Header("General")]
    public string nameToken = "";
    public string mainSkinName = "";
    public ushort weaponDefIndex;
    public DriverWeaponDef.ModelSwapInfo[] modelSwapInfo;

    public Mesh mesh
    {
        get => modelSwapInfo[0].mesh;
        set => modelSwapInfo[0].mesh = value;
    }

    public Material material
    {
        get => modelSwapInfo[0].material;
        set => modelSwapInfo[0].material = value;
    }

    public static DriverWeaponSkinDef CreateWeaponSkinDefFromInfo(DriverWeaponSkinDefInfo skinDefInfo)
    {
        var weaponSkinDef = ScriptableObject.CreateInstance<DriverWeaponSkinDef>();
        weaponSkinDef.nameToken = skinDefInfo.nameToken;
        weaponSkinDef.mainSkinName = skinDefInfo.mainSkinName;
        weaponSkinDef.weaponDefIndex = skinDefInfo.weaponDefIndex;
        weaponSkinDef.modelSwapInfo = skinDefInfo.modelSwapInfo;

        return weaponSkinDef;
    }

    [System.Serializable]
    public struct DriverWeaponSkinDefInfo
    {
        public string nameToken;
        public string mainSkinName;
        public ushort weaponDefIndex;
        public DriverWeaponDef.ModelSwapInfo[] modelSwapInfo;
    }
}