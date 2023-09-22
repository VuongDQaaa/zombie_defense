using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldController : MonoBehaviour, ICanTakeDamage
{
    public int health = 80;
    [ReadOnly] public int currentHealth = 0;

    void Start()
    {
        currentHealth = health;
    }


    public void TakeDamage(int damage, Vector3 force, Vector3 hitPoint, GameObject instigator, WeaponEffect weaponEffect = null, WEAPON_EFFECT forceEffect = WEAPON_EFFECT.NONE)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            transform.parent = null;
            gameObject.AddComponent<Rigidbody>();
            gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * -200);
            GetComponent<Collider>().enabled = false;
            Destroy(gameObject, 3);
        }
    }
}
