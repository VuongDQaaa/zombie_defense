using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowGrenade : MonoBehaviour,ICanTakeDamage
{
    public LayerMask targetLayer;
    Rigidbody rig;
    public float speed = 500;
    public GameObject blowFX;
    RaycastHit hit;
    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (rig == null)
            rig = GetComponent<Rigidbody>();
        if (Physics.SphereCast(transform.position, 0.2f, rig.velocity.normalized, out hit, targetLayer))
        {
            if (hit.distance < 0.2f)
            {
                Barricade.Instance.TakeDamage();
                Destroy();
            }
        }
    }

    public void Throw()
    {
        if (rig == null)
            rig = GetComponent<Rigidbody>();

        rig.velocity = Vector3.zero;
        var destination = GameManager.Instance.Player.transform.position;
        destination.y = 1.5f;

        rig.AddForce((destination - transform.position).normalized * speed);
    }

    public void TakeDamage(int damage, Vector3 force, Vector3 hitPoint, GameObject instigator, WeaponEffect weaponEffect = null, WEAPON_EFFECT forceEffect = WEAPON_EFFECT.NONE)
    {
        Destroy();
    }

    void Destroy()
    {
        if (blowFX)
            SpawnSystemHelper.GetNextObject(blowFX, transform.position, true);
        Destroy(gameObject);
    }
}
