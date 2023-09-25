using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WeaponDataSO))]
public class WeaponDataSOEditor : Editor
{
    // ADD TOOLTIPS HERE
    #region SERIALIZED PROPERTIES
    SerializedProperty weaponCategory;
    SerializedProperty weaponType;
    SerializedProperty firingType;
    SerializedProperty ammoUsageType;

    SerializedProperty weaponName;
    SerializedProperty damageMax;
    SerializedProperty damageMin;
    SerializedProperty knockback;

    SerializedProperty bulletSpeed;
    SerializedProperty fireRate;
    SerializedProperty bulletSpread;
    SerializedProperty bulletsPerShot;
    SerializedProperty symmetricalSpread;

    SerializedProperty burstEnabled;
    SerializedProperty burstCount;
    SerializedProperty burstRate;

    SerializedProperty chargeTime;
    SerializedProperty chargeAutoFire;
    SerializedProperty chargeAutoFireDelay;

    SerializedProperty beamRange;
    SerializedProperty activationDuration;
    SerializedProperty deactivationDuration;

    SerializedProperty standardAmmoConfig;
    SerializedProperty overheatAmmoConfig;
    SerializedProperty unlimitedAmmoConfig;

    SerializedProperty selfKnockback;
    SerializedProperty healAmount;

    SerializedProperty weaponPrefab;
    SerializedProperty weaponStance;

    SerializedProperty bulletPrefab;
    SerializedProperty firePosition;
    SerializedProperty handlePosition;
    SerializedProperty muzzleFlashPrefab;
    SerializedProperty muzzleFlashPosition;
    SerializedProperty muzzleFlashAnimation;
    #endregion

    #region EDITOR PROPERTIES
    [SerializeField] bool showReferenceVariables;
    #endregion

    void OnEnable() {
        GetCommonWeaponFields();
        GetRangeWeaponFields();
    }

    public override void OnInspectorGUI() {
        // Update the serialized object
        serializedObject.Update();

        EditorGUILayout.PropertyField(weaponCategory, new GUIContent("Weapon Category", "Define the weapon category"));

        if (weaponCategory.enumValueIndex == (int)WeaponCategory.Range) {
            DisplayRangeWeaponFields();
        } else if (weaponCategory.enumValueIndex == (int)WeaponCategory.Melee) {
            DisplayMeleeWeaponFields();
        }

        // Apply changes to the serialized object
        serializedObject.ApplyModifiedProperties();
    }

    void DisplayRangeWeaponFields() {
        EditorGUILayout.PropertyField(weaponType, new GUIContent("Weapon Type", "Define the weapon type"));
        EditorGUILayout.PropertyField(firingType, new GUIContent("Firing Type", "Define the firing type"));
        EditorGUILayout.PropertyField(ammoUsageType, new GUIContent("Ammo Usage Type", "Define how the weapon ammunition is used"));
        EditorGUILayout.Separator();

        DisplayCommonFields();
        EditorGUILayout.Separator();

        DisplayAmmoConfigurationFields();
        EditorGUILayout.Separator();

        EditorGUILayout.PropertyField(bulletSpeed, new GUIContent("Bullet Speed", "The speed of the fired bullet(s)"));
        EditorGUILayout.PropertyField(fireRate, new GUIContent("Fire Rate", "The weapon's fire rate"));
        EditorGUILayout.PropertyField(bulletSpread, new GUIContent("Bullet Spread", "The spread of the fired bullets"));
        EditorGUILayout.PropertyField(bulletsPerShot, new GUIContent("Bullets Per Shot", "The number of bullets fired per shot"));
        if (bulletsPerShot.intValue > 1)
            EditorGUILayout.PropertyField(symmetricalSpread, new GUIContent("Symmetrical Spread", "If true, the bullet spread will be symmetrical"));
        EditorGUILayout.Separator();

        EditorGUILayout.PropertyField(burstEnabled, new GUIContent("Enable Burst Fire", "If true, burst fire is enabled for this weapon"));
        if (burstEnabled.boolValue) {
            EditorGUILayout.PropertyField(burstCount, new GUIContent("Burst Count", "The number of bullets in each burst. Set the 'Ammo Consumption' to this value if burst is enabled"));
            EditorGUILayout.PropertyField(burstRate, new GUIContent("Burst Rate", "The rate at which the bullets in the burst are fired"));
        }
        EditorGUILayout.Separator();

        if (firingType.enumValueIndex == (int)RangeFiringType.Charge) {
            EditorGUILayout.PropertyField(chargeTime, new GUIContent("Charge Time", "The maximum charge time for this weapon"));
            EditorGUILayout.PropertyField(chargeAutoFire, new GUIContent("Charge Auto Fire", "If true, this weapon fires automatically once the max charge time is reached"));
            if (chargeAutoFire.boolValue)
                EditorGUILayout.PropertyField(chargeAutoFireDelay, new GUIContent("Charged Auto Fire Delay", "The delay before this weapon is Auto Fired"));
            EditorGUILayout.Separator();
        }

        if (firingType.enumValueIndex == (int)RangeFiringType.Beam) {
            EditorGUILayout.PropertyField(beamRange, new GUIContent("Beam Range", "The maximum range of the beam"));
            EditorGUILayout.PropertyField(activationDuration, new GUIContent("Activation Duration", "The duration of all beam activation logic and animations"));
            EditorGUILayout.PropertyField(deactivationDuration, new GUIContent("Deactivation Duration", "The duration of all beam deactivation logic and animations"));
        }

        EditorGUILayout.Separator();

        EditorGUILayout.PropertyField(selfKnockback, new GUIContent("Self Knockback", "The knockback force applied to self"));
        EditorGUILayout.PropertyField(healAmount, new GUIContent("Heal Amount", "The amount each fired bullet will heal for"));
        EditorGUILayout.Separator();

        // Display the 'showReferenceVariables' variable HERE
        showReferenceVariables = EditorGUILayout.Toggle("Show References", showReferenceVariables);
        if (showReferenceVariables) DisplayRangeReferenceFields();
    }

    void DisplayCommonFields() {
        EditorGUILayout.PropertyField(weaponName, new GUIContent("Weapon Name", "The name of this weapon"));
        EditorGUILayout.PropertyField(damageMax, new GUIContent("Max Damage", "The maximum damage per bullet fired"));
        if (firingType.enumValueIndex == (int)RangeFiringType.Charge)
            EditorGUILayout.PropertyField(damageMin, new GUIContent("Min Damage", "The minimum damage per bullet fired"));
        EditorGUILayout.PropertyField(knockback, new GUIContent("Knockback", "The knockback force applied to the imapcted target"));
    }

    void DisplayMeleeWeaponFields() {

    }

    void DisplayAmmoConfigurationFields() {
        switch (ammoUsageType.enumValueIndex) {
            case 0:
                EditorGUILayout.PropertyField(standardAmmoConfig, new GUIContent("Standard Ammo Configuration", "The values for the 'Standard' ammo configuration"));
                break;
            case 1:
                EditorGUILayout.PropertyField(overheatAmmoConfig, new GUIContent("Overheat Ammo Configuration", "The values for the 'Overheat' ammo configuration"));
                break;
            case 2:
                EditorGUILayout.PropertyField(unlimitedAmmoConfig, new GUIContent("Unlimited Ammo Configuration", "The values for the 'Unlimited' ammo configuration (Nothing)"));
                break;
        }
    }

    void DisplayRangeReferenceFields() {
        EditorGUILayout.PropertyField(weaponPrefab);
        EditorGUILayout.PropertyField(weaponStance);
        EditorGUILayout.Separator();

        EditorGUILayout.PropertyField(bulletPrefab);
        EditorGUILayout.PropertyField(firePosition);
        EditorGUILayout.PropertyField(handlePosition);
        EditorGUILayout.PropertyField(muzzleFlashPrefab);
        EditorGUILayout.PropertyField(muzzleFlashPosition);
        EditorGUILayout.PropertyField(muzzleFlashAnimation);
    }

    void GetCommonWeaponFields() {
        weaponCategory = serializedObject.FindProperty("weaponCategory");
        weaponPrefab = serializedObject.FindProperty("weaponPrefab");
        weaponStance = serializedObject.FindProperty("weaponStance");
    }

    void GetRangeWeaponFields() {
        if (target is RangeWeaponDataSO) {
            RangeWeaponDataSO rangeWEaponData = (RangeWeaponDataSO)target;

            weaponType = serializedObject.FindProperty("rangeWeaponType");
            firingType = serializedObject.FindProperty("firingType");
            ammoUsageType = serializedObject.FindProperty("ammoUsageType");

            standardAmmoConfig = serializedObject.FindProperty("standardAmmoConfig");
            overheatAmmoConfig = serializedObject.FindProperty("overheatAmmoConfig");
            unlimitedAmmoConfig = serializedObject.FindProperty("unlimitedAmmoConfig");

            weaponName = serializedObject.FindProperty("weaponName");
            damageMin = serializedObject.FindProperty("damage.minDamage");
            damageMax = serializedObject.FindProperty("damage.maxDamage");
            knockback = serializedObject.FindProperty("knockback");

            bulletSpeed = serializedObject.FindProperty("bulletSpeed");
            fireRate = serializedObject.FindProperty("fireRate");
            bulletSpread = serializedObject.FindProperty("bulletSpread");
            bulletsPerShot = serializedObject.FindProperty("bulletsPerShot");
            symmetricalSpread = serializedObject.FindProperty("symmetricalSpread");

            burstEnabled = serializedObject.FindProperty("burstEnabled");
            burstCount = serializedObject.FindProperty("burstCount");
            burstRate = serializedObject.FindProperty("burstRate");

            chargeTime = serializedObject.FindProperty("chargeTime");
            chargeAutoFire = serializedObject.FindProperty("chargeAutoFire");
            chargeAutoFireDelay = serializedObject.FindProperty("chargeAutoFireDelay");

            beamRange = serializedObject.FindProperty("beamRange");
            activationDuration = serializedObject.FindProperty("activationDuration");
            deactivationDuration = serializedObject.FindProperty("deactivationDuration");

            selfKnockback = serializedObject.FindProperty("selfKnockback");
            healAmount = serializedObject.FindProperty("healAmount");

            bulletPrefab = serializedObject.FindProperty("bulletPrefab");
            firePosition = serializedObject.FindProperty("firePosition");
            handlePosition = serializedObject.FindProperty("handlePosition");
            muzzleFlashPrefab = serializedObject.FindProperty("muzzleFlashPrefab");
            muzzleFlashPosition = serializedObject.FindProperty("muzzleFlashPosition");
            muzzleFlashAnimation = serializedObject.FindProperty("muzzleFlashAnimation");
        }
    }
}
