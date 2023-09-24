using UnityEngine;

public abstract class WeaponDataSO : ScriptableObject
{
    public WeaponCategory weaponCategory;

    public string weaponName;
    public DamageRange damage = new DamageRange();
    public float knockback;

    public GameObject weaponPrefab;
    public RuntimeAnimatorController weaponStance;

    public abstract WeaponLogic AddWeaponLogicComponent(GameObject weaponHolder);
    public abstract void InitializeWeaponData(WeaponLogic weaponLogic, KeyCode fireInput);
}

[System.Serializable]
public class DamageRange
{
    public float minDamage;
    public float maxDamage;
}

public enum WeaponCategory
{
    Melee,
    Range
}
