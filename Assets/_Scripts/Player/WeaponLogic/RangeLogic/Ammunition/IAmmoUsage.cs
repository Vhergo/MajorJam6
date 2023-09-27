public interface IAmmoUsage
{
    bool CanShoot();
    void OnShoot();
    void StartReload();
    bool CanReload();
    void OnAmmoRefill(int refillAmount);
    void OnUpdate();

    void SaveTimeAtWeaponSwap();
    void ApplyTimeAtWeaponSwap();

    float GetAmmoRemainingInClip();
    float GetAmmoRemainingInTotal();
}