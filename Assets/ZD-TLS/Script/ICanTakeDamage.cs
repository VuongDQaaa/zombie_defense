using UnityEngine;
using System.Collections;

public interface ICanTakeDamage {
    void TakeDamage(int damage, Vector3 force, Vector3 hitPoint, GameObject instigator, WeaponEffect weaponEffect = null, WEAPON_EFFECT forceEffect = WEAPON_EFFECT.NONE);
}
