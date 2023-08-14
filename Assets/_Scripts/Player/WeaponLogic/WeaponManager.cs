using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private KeyCode fireKey;
    [SerializeField] private KeyCode swapKey;

    [Header("Weapons")]
    [SerializeField] private WeaponDataSO primaryWeapon;
    [SerializeField] private WeaponDataSO secondaryWeapon;

    [Header("Other")]
    [SerializeField] Transform gunBarrel;
    [SerializeField] GameObject muzzleFlash;

    private WeaponDataSO currentWeaponData;
    private WeaponLogic currentWeaponLogic;

    private void Start() {
        if (fireKey == KeyCode.Print) fireKey = KeyCode.Mouse0;
        EquipWeapon(primaryWeapon);
    }

    void Update() {
        ChangeCurrentWeapon();
    }

    void GetWeaponLogicComponent(WeaponDataSO weapon) {
        switch (weapon.weaponType) {
            case WeaponDataSO.WeaponType.Pistol:
                SetWeaponLogic<PistolLogic>();
                print("PISTOL");
                break;
            case WeaponDataSO.WeaponType.Shotgun:
                SetWeaponLogic<ShotgunLogic>();
                print("SHOTGUN");
                break;
            case WeaponDataSO.WeaponType.SMG:
                print("GET SMG");
                break;
            case WeaponDataSO.WeaponType.Rifle:
                print("GET RIFLE");
                break;
            case WeaponDataSO.WeaponType.Sniper:
                print("GET SNIPER");
                break;
            case WeaponDataSO.WeaponType.Railgun:
                print("GET RAILGUN");
                break;
        }
    }

    void SetWeaponLogic<T>() where T : WeaponLogic {
        var logicComponent = gameObject.AddComponent<T>();
        currentWeaponLogic = logicComponent;
    }

    void EquipWeapon(WeaponDataSO weapon) {
        RemoveWeaponLogic<WeaponLogic>();
        currentWeaponData = weapon;
        GetWeaponLogicComponent(weapon);
        currentWeaponLogic.InitializeWeaponData(weapon, gunBarrel, muzzleFlash, fireKey);
        muzzleFlash.GetComponent<Animator>().runtimeAnimatorController = currentWeaponData.muzzleFlash;
    }

    void RemoveWeaponLogic<T>() where T : WeaponLogic {
        T currentLogic = GetComponent<T>();
        if (currentLogic != null) Destroy(currentLogic);
    }

    void ChangeCurrentWeapon() {
        if (Input.GetKeyDown(swapKey)) {
            print("SWAP WEAPON");
            SwapWeapon();
        }
    }

    private void SwapWeapon() {
        if (currentWeaponData == primaryWeapon) {
            print("SWAPPING TO SHOTGUN");
            EquipWeapon(secondaryWeapon);
        } else if (currentWeaponData == secondaryWeapon) {
            print("SWAPPING TO PISTOL");
            EquipWeapon(primaryWeapon);
        }
    }
}
