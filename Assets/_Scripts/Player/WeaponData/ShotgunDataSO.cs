using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/Shotgun")]
public class ShotgunDataSO : WeaponDataSO
{
    public float arcBulletSpread;
    public float arcBulletCount;
}
