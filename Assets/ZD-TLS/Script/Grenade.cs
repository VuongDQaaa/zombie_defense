using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public int damage = 100;
    public float radius = 3;
    public LayerMask targetLayer;
    public AudioClip sound;
    public GameObject blowFX;
    Rigidbody2D rig;

    private void Start()
    {
        var hits = Physics.OverlapSphere(transform.position, radius, targetLayer);
        foreach (var hit in hits)
        {
            hit.gameObject.GetComponent<ICanTakeDamage>().TakeDamage(damage, Vector2.zero, hit.transform.position, gameObject);
        }

        SoundManager.PlaySfx(sound);
        SpawnSystemHelper.GetNextObject(blowFX, true).transform.position = transform.position;
        gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
