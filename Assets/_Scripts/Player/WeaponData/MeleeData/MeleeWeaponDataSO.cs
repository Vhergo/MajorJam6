using UnityEngine;

[CreateAssetMenu(fileName = "New Melee Weapon", menuName = "Weapons/Melee Weapon")]
public class MeleeWeaponDataSO : WeaponDataSO
{
    // Different damage types(like piercing, slashing, etc.)
    public MeleeWeaponDataSO() {
        weaponCategory = WeaponCategory.Melee;
    }

    public MeleeWeaponType meleeWeaponType;

    public float attackSpeed;
    public float attackRange;

    public Transform handlePosition;

    public override WeaponLogic AddWeaponLogicComponent(GameObject weapon) {
        return weapon.AddComponent<MeleeWeaponLogic>();
    }

    public override void InitializeWeaponData(WeaponLogic weaponLogic, KeyCode fireInput) {
        MeleeWeaponLogic rangeWeaponLogic = (MeleeWeaponLogic)weaponLogic;
        // melee data assignement
    }
}

public enum MeleeWeaponType
{
    Sword,
    Dagger,
    Axe,
    Spear,
    Whip,
    Hammer
}