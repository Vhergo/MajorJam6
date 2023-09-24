using System;
using UnityEngine;

[Serializable]
public class UnlimitedAmmoData : IAmmoUsage
{
    public UnlimitedAmmoConfig config;

    public UnlimitedAmmoData (UnlimitedAmmoConfig config) {
        this.config = config;
    }

    public bool CanShoot() {
        return true;
    }

    public void OnShoot() {

    }

    public void StartReload() {

    }

    public bool CanReload() {
        return false;
    }

    public void OnUpdate() {

    }

    public void OnAmmoRefill(int refillAmount) {

    }

    public float GetAmmoRemainingInClip() {
        return 0;
    }

    public float GetAmmoRemainingInTotal() {
        return 0;
    }
}