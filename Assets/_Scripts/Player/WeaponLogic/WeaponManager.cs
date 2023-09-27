using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapons")]
    [SerializeField] private List<Weapon> weaponsList = new List<Weapon>();

    [Header("Inputs")]
    [SerializeField] private KeyCode fireKey;

    [Header("Other Data")]
    [SerializeField] private float weaponSwapDelay;
    private bool canSwap = true;

    private Weapon currentWeapon;
    private WeaponDataSO currentWeaponData;
    private WeaponLogic currentWeaponLogic;

    public static Action<WeaponLogic> OnWeaponEquipped;

    private void Start() {
        // Temporary failsafe for Unity Bug
        // This bug registers the Mouse0 keycode as the Print keycode if it is assigned in the inspector
        // If that bug occurs, this manual assignement of Mouse0 will handle it
        // It will probably be or is already fixed in future Unity versions
        if (fireKey == KeyCode.Print) fireKey = KeyCode.Mouse0;

        EquipWeapon(weaponsList[0]);
    }

    void Update() {
        HandleWeaponSwapInput();
    }

    void HandleWeaponSwapInput() {
        if (!canSwap) return;

        for (int i = 0; i < weaponsList.Count; i++) {
            if (Input.GetKeyDown(weaponsList[i].swapKey)) {
                // We don't need to do anything if we are attempting to swap TO the currently equiped weapon
                if (weaponsList[i].swapKey == currentWeapon.swapKey) return;

                SaveCurrentWeaponData();
                EquipWeapon(weaponsList[i]);
                StartCoroutine(WeaponSwapCooldown());
            }
        }
    }

    void EquipWeapon(Weapon weapon) {
        if (weapon == null) {
            print("No Weapon Available");
            return;
        }
        
        RemoveWeaponLogic();
        //RemoveWeaponLogic<WeaponLogic>();

        currentWeapon = weapon;

        currentWeaponData = weapon.weapon;

        currentWeaponLogic = currentWeaponData.AddWeaponLogicComponent(gameObject);
        currentWeaponData.InitializeWeaponData(currentWeaponLogic, fireKey);
        if (weapon.ammoData != null)
            currentWeaponLogic.InitializeSavedData(weapon.ammoData);

        OnWeaponEquipped.Invoke(currentWeaponLogic);
    }

    void RemoveWeaponLogic() {
        WeaponLogic[] currentLogic = GetComponents<WeaponLogic>();
        if (currentLogic != null) {
            foreach (WeaponLogic logicComponent in currentLogic) {
                Destroy(logicComponent);
            }
        }
    }

    //void RemoveWeaponLogic<T>() where T : WeaponLogic {
    //    T currentLogic = GetComponent<T>();
    //    if (currentLogic != null) Destroy(currentLogic);
    //}

    private void SaveCurrentWeaponData() {
        if (currentWeapon != null && currentWeaponLogic != null) {
            currentWeapon.ammoData = currentWeaponLogic.GetMutableData();
        }
    }

    private IEnumerator WeaponSwapCooldown() {
        canSwap = false;
        yield return new WaitForSeconds(weaponSwapDelay);
        canSwap = true;
    }
}

[Serializable]
public class Weapon
{
    public WeaponDataSO weapon;
    public KeyCode swapKey;
    public IAmmoUsage ammoData;
}
