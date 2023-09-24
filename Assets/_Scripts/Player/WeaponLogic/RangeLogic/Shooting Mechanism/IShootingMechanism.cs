using System.Collections;
using UnityEngine;

public interface IShootingMechanism
{
    void ShootBullet(RangeWeaponLogic weaponLogic);
    IEnumerator ShootBeam(IAmmoUsage ammoUsage, KeyCode fireKey);
}