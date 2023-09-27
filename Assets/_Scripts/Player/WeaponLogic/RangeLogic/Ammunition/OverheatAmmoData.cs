using System;
using UnityEngine;

[Serializable]
public class OverheatAmmoData : IAmmoUsage
{
    private OverheatAmmoConfig config;

    public float currentEnergy;
    public float currentHeat;

    public float lockoutTimer;
    public bool lockedOut;

    float saveTimeAtSwap;

    [HideInInspector] public float lastShootTime;

    public OverheatAmmoData (OverheatAmmoConfig config) {
        this.config = config;
        currentEnergy = config.maxEnergy;
        currentHeat = 0;
    }

    public bool CanShoot() {
        return !lockedOut && HasEnergyRemaining();
    }

    public void OnShoot() {
        if (CanShoot()) {
            currentHeat += config.heatRate;
            currentEnergy -= config.infiniteAmmo ? 0 : config.heatRate;
            lastShootTime = Time.time;
            if (currentHeat >= config.heatLimit) {
                StartReload();
            }
        }
    }

    // Used for Triggering Lockout
    public void StartReload() {
        lockoutTimer = config.lockoutTime;
        lockedOut = true;
        Debug.Log("Enter Lockout");
    }

    public bool CanReload() {
        return false;
    }

    public void OnUpdate() {
        if (lockedOut) {
            lockoutTimer -= Time.deltaTime;
            if (lockoutTimer <= 0) {
                lockedOut = false;
                Debug.Log("Exit Lockout");
                currentHeat = 0;
            }
        }else if (CanCooldown()) {
            Cooldown();
        }

        if (CanCooldown()) Cooldown();
    }

    public void OnAmmoRefill(int refillAmount) {
        currentEnergy = Mathf.Min(currentEnergy + refillAmount, config.maxEnergy);
    }

    // Helper Function
    private void Cooldown() {
        if (currentHeat > 0 && !lockedOut) {
            currentHeat -= config.cooldownRate * Time.deltaTime;
            if (currentHeat < 0) currentHeat = 0;
        }
    }

    public void SaveTimeAtWeaponSwap() {
        saveTimeAtSwap = Time.time;
    }

    public void ApplyTimeAtWeaponSwap() {
        float timePassed = Time.time - saveTimeAtSwap;
        lockoutTimer -= timePassed;
    }

    private bool CanCooldown() {
        return Time.time - lastShootTime > config.cooldownDelay;
    }

    private bool HasEnergyRemaining() {
        return currentEnergy > 0;
    }

    public float GetAmmoRemainingInClip() {
        return currentEnergy;
    }

    public float GetAmmoRemainingInTotal() {
        return currentHeat;
    }
}