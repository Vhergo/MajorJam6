using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; private set; }

    [Header("Weapons")]
    [SerializeField] private List<Weapon> weaponsList = new List<Weapon>();

    [Header("Inputs")]
    [SerializeField] private KeyCode fireKey;

    [Header("Other Data")]
    [SerializeField] private float weaponSwapDelay;
    private bool canSwap = true;

    public GameObject weaponObject;
    public bool weaponIsEquipped;

    private Weapon currentWeapon;
    private WeaponDataSO currentWeaponData;
    private WeaponLogic currentWeaponLogic;

    public static Action<WeaponLogic> OnWeaponEquipped;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }else {
            Destroy(gameObject);
        }

        weaponObject = GameObject.FindGameObjectWithTag("Weapon");
        Debug.Log(weaponObject.name);
        weaponIsEquipped = true;
    }

    void Start() {
        // Temporary failsafe for Unity Bug
        // This bug registers the Mouse0 keycode as the Print keycode if it is assigned in the inspector
        // If that bug occurs, this manual assignement of Mouse0 will handle it
        // It will probably be or is already fixed in future Unity versions
        if (fireKey == KeyCode.Print) fireKey = KeyCode.Mouse0;

        EquipWeapon(weaponsList[0]);
    }

    void Update() {
        if (!canSwap) return;
        HandleWeaponSwapInput();
        HandleWeaponHolster();
    }

    void HandleWeaponSwapInput() {
        for (int i = 0; i < weaponsList.Count; i++) {
            if (Input.GetKeyDown(weaponsList[i].swapKey)) {
                if (weaponsList[i].swapKey == currentWeapon.swapKey) return;

                SaveCurrentWeaponData();
                EquipWeapon(weaponsList[i]);
                StartCoroutine(WeaponSwapCooldown());
            }
        }
    }

    void HandleWeaponHolster() {
        if (Input.GetKeyDown(KeyCode.X)) {
            if (weaponIsEquipped) {
                // Unequip current weapon
                Debug.Log("Unequip");
                weaponObject.SetActive(false);
                weaponIsEquipped = false;
            } else {
                // Equip current weapon
                Debug.Log("Equip");
                weaponObject.SetActive(true);
                weaponIsEquipped = true;
            }
        }
    }

    void EquipWeapon(Weapon weapon) {
        if (!weaponObject.activeInHierarchy) {
            weaponObject.SetActive(true);
            weaponIsEquipped = true;
        }

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

        HanleOnWeaponEquppedInvoke();
    }

    private void HanleOnWeaponEquppedInvoke() {
        if (currentWeaponData.weaponCategory == WeaponCategory.Melee)
            OnWeaponEquipped.Invoke(currentWeaponLogic as MeleeWeaponLogic);
        else if (currentWeaponData.weaponCategory == WeaponCategory.Range)
            OnWeaponEquipped.Invoke(currentWeaponLogic as RangeWeaponLogic);
    }

    private void RemoveWeaponLogic() {
        WeaponLogic[] currentLogic = GetComponents<WeaponLogic>();
        if (currentLogic != null) {
            foreach (WeaponLogic logicComponent in currentLogic) {
                Destroy(logicComponent);
            }
        }
    }

    // Generic version of RemoveWeaponLogic
    //void RemoveWeaponLogic<T>() where T : WeaponLogic {
    //    T currentLogic = GetComponent<T>();
    //    if (currentLogic != null) Destroy(currentLogic);
    //}

    private void SaveCurrentWeaponData() {
        RotateWithMouse.Instance.ResetRotationSpeed();
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
    public string weaponName;
    public WeaponDataSO weapon;
    public KeyCode swapKey;
    public IAmmoUsage ammoData;
}
