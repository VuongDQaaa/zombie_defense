using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    public GameObject impactParticle;
    public LayerMask layerToHit;
    public float colliderRadius = 1f;
    [Range(0f, 1f)]
    public float collideOffset = 0.15f;
    Rigidbody rig;
    [ReadOnly] public int damage = 10;
    [ReadOnly] public float speed = 10;
    [Header("---EXTRA---")]
    [ReadOnly] public int ChanceBounce = 0;
    [ReadOnly] public int ChanceCritical = 0;
    [ReadOnly] public int ChanceExplosion = 0;
    [ReadOnly] public int MoreCrititalDamacePercent = 0;
    [ReadOnly] public int ChanceX3Damage = 0;
    [ReadOnly] public bool ThroughEnemyBody;

    [Header("---Explosion---")]
    public int explosionDamage = 100;
    public float radius = 3;
    public LayerMask targetLayer;
    public AudioClip explosionSound;
    public GameObject explosionBlowFX;

    int bounceTime = 2;
    int bounceCounter = 0;

    [ReadOnly] public List<GameObject> zombieHits; 

    // Damage, ChanceBounce, ExtraBullet, ChanceCritical, FireRate, BulletSpeed, ChanceExplosion, MoreCrititalDamacePercent, ChanceX3Damage, ThroughEnemyBody
    private void Start()
    {
        rig = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        zombieHits = new List<GameObject>();
    }

    public void InitBullet(int dmg, float _speed, int chanceBounce, int changeCrit, int chanceExplosion, int moreCritDamage, int chanceX3Dmg, bool throughBody)
    {
        speed = _speed;
        ChanceBounce = chanceBounce;
        ChanceCritical = changeCrit;
        ChanceExplosion = chanceExplosion;
        MoreCrititalDamacePercent = moreCritDamage;
        ChanceX3Damage = chanceX3Dmg;
        ThroughEnemyBody = throughBody;

        damage = dmg;

        if (rig == null)
            rig = GetComponent<Rigidbody>();

        rig.velocity = Vector3.zero;
        rig.AddForce(transform.forward * speed);
    }

    void FixedUpdate()
    {
        RaycastHit hit;

        float radius;
        radius = colliderRadius;

        Vector3 direction = rig.velocity; 
        direction = direction.normalized;

        float detectionDistance = rig.velocity.magnitude * Time.fixedDeltaTime; 

        if (Physics.SphereCast(transform.position - transform.forward, radius, direction, out hit, detectionDistance, layerToHit)) 
        {
            if (!zombieHits.Contains(hit.collider.gameObject))
            {
                var takeDamage = (ICanTakeDamage)hit.collider.gameObject.GetComponent(typeof(ICanTakeDamage));
                if (takeDamage != null)
                {
                    bool isCritical = Random.Range(0, 100) < ChanceCritical;
                    bool x3Damage = Random.Range(0, 100) < ChanceX3Damage;
                    float finalDamage = damage * (x3Damage ? 3 : 1) * (isCritical ? (2f * (1f + (MoreCrititalDamacePercent / 100f))) : 1);
                    takeDamage.TakeDamage((int)finalDamage, Vector3.zero, hit.point, gameObject);

                    zombieHits.Add(hit.collider.gameObject);

                    //check Explosion
                    if (Random.Range(0, 100) < ChanceExplosion)
                    {
                        DoExplosion();
                    }
                }


                transform.position = hit.point + (hit.normal * collideOffset);
                SpawnSystemHelper.GetNextObject(impactParticle, transform.position, true);
            }

            // Check bounce
            if (bounceCounter < bounceTime && ( Random.Range(0, 100) < ChanceBounce))
            {
                bounceCounter++;
                   //check enemy in range
                   var hits = Physics.OverlapSphere(transform.position, 6, 1 << (LayerMask.NameToLayer("Enemy")));
                if (hits.Length > 0)
                {
                    var nextPos = hits[Random.Range(0, hits.Length)].transform.position;
                    nextPos.y = transform.position.y;

                    transform.forward = nextPos - transform.position;
                    rig.velocity = Vector3.zero;
                    rig.AddForce(transform.forward * speed);
                }
                else if(!ThroughEnemyBody)
                    DisableBullet();
            }
            else if (!ThroughEnemyBody)
                DisableBullet();
            else
            {
                if (Vector3.Distance(transform.position, GameManager.Instance.Player.transform.position) > 20)
                    DisableBullet();
            }
        }
    }

    void DoExplosion()
    {
        var hits = Physics.OverlapSphere(transform.position, radius, targetLayer);
        foreach (var hit in hits)
        {
            hit.gameObject.GetComponent<ICanTakeDamage>().TakeDamage(damage, Vector2.zero, hit.transform.position, gameObject);
        }

        SoundManager.PlaySfx(explosionSound);
        SpawnSystemHelper.GetNextObject(explosionBlowFX, true).transform.position = transform.position;
    }

    void DisableBullet()
    {
        bounceCounter = 0;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, colliderRadius);
    }
}
