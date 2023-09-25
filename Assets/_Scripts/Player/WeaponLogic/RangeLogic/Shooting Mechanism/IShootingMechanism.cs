using System.Collections;
using UnityEngine;

public interface IShootingMechanism
{
    void Shoot(IAmmoUsage ammoUsage, KeyCode fireKey);
    void StopShoot();
}