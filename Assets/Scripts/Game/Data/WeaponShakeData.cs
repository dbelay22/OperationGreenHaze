using UnityEngine;

[CreateAssetMenu(fileName = "WeaponShakeData", menuName = "Custom/WeaponShakeData")]
public class WeaponShakeData : ScriptableObject
{
    [System.Serializable]
    public class ShakeProperties
    {
        public float magnitude;
        public float duration;
        public float maxFOVChange;
    }

    [System.Serializable]
    public class WeaponShakeDataset
    {
        public AmmoType ammoType;
        public ShakeProperties shakeProperties;
    }

    public WeaponShakeDataset[] weaponShakeDataList;
}