using System;
using UnityEngine;

[Serializable]
public class StandardAmmoData : IAmmoUsage
{
    private StandardAmmoConfig config;

    public int remainingAmmo;
    public int currentAmmoInClip;

    public float reloadTimer;
    public bool isReloading;

    public float saveTimeAtSwap;

    public StandardAmmoData (StandardAmmoConfig config) {
        this.config = config;
        currentAmmoInClip = config.maxClipCapacity;
        remainingAmmo = config.maxAmmoCapacity;
    }

    public bool CanShoot() {
        return !isReloading && HasAmmoInClip();
    }

    public void OnShoot() {
        if (CanShoot()) {
            currentAmmoInClip -= config.ammoConsumption;
            if (currentAmmoInClip <= 0 && config.autoReload) {
                StartReload();
            }
        }
    }

    public void StartReload() {
        reloadTimer = config.reloadTime;
        isReloading = true;
        Debug.Log("Start Reloading");
    }

    public bool CanReload() {
        return !isReloading && (currentAmmoInClip < config.maxClipCapacity) && HasAmmo();
    }

    public void OnUpdate() {
        if (isReloading) {
            reloadTimer -= Time.deltaTime;
            if (reloadTimer <= 0) {
                Reload();
            }
        }
    }

    public void OnAmmoRefill(int refillAmount) {
        remainingAmmo = Mathf.Min(remainingAmmo + refillAmount, config.maxAmmoCapacity);
    }

    public void Reload() {
        if (config.infiniteAmmo) {
            currentAmmoInClip = config.maxClipCapacity;
            isReloading = false;
            return;
        }

        if (remainingAmmo <= 0) {
            Debug.Log("No More Ammunition");
        }else {
            int ammoToReload = Mathf.Min(remainingAmmo, config.maxClipCapacity - currentAmmoInClip);
            remainingAmmo -= ammoToReload;
            currentAmmoInClip += ammoToReload;
            isReloading = false;
            reloadTimer = 0;
            Debug.Log("Stop Reloading");
        }
    }

    public void SaveTimeAtWeaponSwap() {
        saveTimeAtSwap = Time.time;
    }

    public void ApplyTimeAtWeaponSwap() {
        float timePassed = Time.time - saveTimeAtSwap;
        reloadTimer -= timePassed;
    }

    public bool HasAmmoInClip() {
        return currentAmmoInClip > 0;
    }

    public bool HasAmmo() {
        return remainingAmmo > 0;
    }

    public float GetAmmoRemainingInClip() {
        return currentAmmoInClip;
    }

    public float GetAmmoRemainingInTotal() {
        return remainingAmmo;
    }
}