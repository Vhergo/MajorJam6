public interface IAmmoUsage
{
    bool CanShoot();
    void OnShoot();
    void StartReload();
    bool CanReload();
    void OnAmmoRefill(int refillAmount);
    void OnUpdate();

    float GetAmmoRemainingInClip();
    float GetAmmoRemainingInTotal();
}