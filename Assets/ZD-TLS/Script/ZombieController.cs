using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ENEMYSTATE
{
    SPAWNING,
    IDLE,
    ATTACK,
    WALK,
    HIT,
    DEATH
}

public class ZombieController : MonoBehaviour, ICanTakeDamage
{
    [ReadOnly] public ENEMYSTATE enemyState = ENEMYSTATE.IDLE;
    public float moveSpeed = 5;
    Rigidbody rig;

    [Header("HEALTH")]
    [Range(0, 5000)]
    public int health = 100;
    [ReadOnly] public int currentHealth;
    public Vector3 healthBarOffset = new Vector3(0, 0, 2);
    public GameObject hitFX;
    protected HealthBarEnemyNew healthBar;

    Animator anim;
    [ReadOnly] public Vector3 finalVelocity;

    public LayerMask targetLayer;
    CapsuleCollider capsuleCollider;

    [Header("---ATACKING---")]
    public float atkTimeMin = 2;
    public float atkTimeMax = 4;

    [Header("---THROW---")]
    public GameObject dummyGrenade;
    public ThrowGrenade grenade;
    public float throwRadius = 8;

    [Header("---BLINK EFFECT---")]
    public Color blinkColor = Color.white;
    [Range(0, 10)]
    public float blinkingSpeed = 1f;
    public Renderer renderer;


    // Start is called before the first frame update
    void Start()
    {
        LevelEnemyManager.Instance.totalZombie++;
        anim = GetComponent<Animator>();
        SetEnemyState(ENEMYSTATE.WALK);
        rig = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        currentHealth = health;
        var healthBarObj = (HealthBarEnemyNew)Resources.Load("HealthBar", typeof(HealthBarEnemyNew));
        healthBar = (HealthBarEnemyNew)Instantiate(healthBarObj, healthBarOffset, Quaternion.identity);
        healthBar.Init(transform, (Vector3)healthBarOffset);
    }

    private void Update()
    {
        healthBar.transform.localScale = new Vector2(transform.localScale.x > 0 ? Mathf.Abs(healthBar.transform.localScale.x) : -Mathf.Abs(healthBar.transform.localScale.x), healthBar.transform.localScale.y);

        anim.SetFloat("speed", enemyState == ENEMYSTATE.WALK? Vector3.Magnitude(finalVelocity) : 0);

        if (enemyState == ENEMYSTATE.WALK)
        {
            RaycastHit hit;
            Vector3 p1 = transform.position + capsuleCollider.center + Vector3.up * -capsuleCollider.height * 0.5F;
            Vector3 p2 = p1 + Vector3.up * capsuleCollider.height;

            // Cast character controller shape 10 meters forward to see if it is about to hit anything.
            if (Physics.CapsuleCast(p1, p2, capsuleCollider.radius, transform.forward, out hit, 10, targetLayer))
            {
                if (hit.distance < 0.5f)
                {
                    StartCoroutine(AttackingCo());
                }
            }

            if(!isThrow && grenade && Vector3.Distance(transform.position, GameManager.Instance.Player.transform.position) < throwRadius)
            {
                StartCoroutine(ThrowCo());
            }
        }
    }

    IEnumerator ThrowCo()
    {
        isThrow = true;
        SetEnemyState(ENEMYSTATE.ATTACK);
        yield return new WaitForSeconds(0.5f);
        anim.SetTrigger("throw");
        yield return new WaitForSeconds(0.65f);
        dummyGrenade.SetActive(false);
        var _gre = Instantiate(grenade.gameObject, dummyGrenade.transform.position, dummyGrenade.transform.rotation).GetComponent<ThrowGrenade>();
        _gre.Throw();
        yield return new WaitForSeconds(1);
        SetEnemyState(ENEMYSTATE.WALK);
    }

    bool isThrow = false;
    IEnumerator AttackingCo()
    {
        SetEnemyState(ENEMYSTATE.ATTACK);
        while (true)
        {
            anim.SetTrigger("melee");
            yield return new WaitForSeconds(Random.Range(atkTimeMin, atkTimeMax));
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameManager.Instance.State != GameManager.GameState.Playing)
            return;

        transform.LookAt(GameManager.Instance.Player.transform);

        finalVelocity = transform.forward * moveSpeed;
        if (enemyState != ENEMYSTATE.WALK)
            finalVelocity = Vector3.zero;

        if (enemyState == ENEMYSTATE.WALK)
            anim.speed = moveSpeed;
        else
            anim.speed = 1;
        //rig.velocity = finalVelocity;
    }

    public void TakeDamage(int damage, Vector3 force, Vector3 hitPoint, GameObject instigator, WeaponEffect weaponEffect = null, WEAPON_EFFECT forceEffect = WEAPON_EFFECT.NONE)
    {
        if (enemyState == ENEMYSTATE.DEATH)
            return;

        currentHealth -= damage;

        if (healthBar)
            healthBar.UpdateValue(currentHealth / (float)health);

        if (hitFX)
            SpawnSystemHelper.GetNextObject(hitFX, hitPoint, true).transform.RotateAround(transform.position, Vector3.up, Random.Range(0, 360));

        if (currentHealth <= 0)
        {
            LevelEnemyManager.Instance.totalZombie--;
            SoundManager.PlaySfx(SoundManager.Instance.zombieDie);
            StopAllCoroutines();
            SetEnemyState(ENEMYSTATE.DEATH);
            anim.SetInteger("die", Random.Range(1, 4));
            GetComponent<Collider>().enabled = false;
            GetComponent<GiveCoinOnDead>().GiveCoin();

            StartCoroutine(DeathCo());
        }
        else
        {
            SoundManager.PlaySfx(SoundManager.Instance.zombieHurt);
            //StartCoroutine(BlinkingCo());
        }
    }

    IEnumerator DeathCo()
    {
        yield return new WaitForSeconds(4);
        var pos = transform.position;
        float value = 0;
        while (value < 1)
        {
            value += 0.25f * Time.deltaTime;
            transform.position = Vector3.Lerp(pos, pos + Vector3.down * 2, value);
            yield return null;
        }

        Destroy(gameObject);
    }

    bool isBlinking = false;
    IEnumerator BlinkingCo()
    {
        if (isBlinking)
            yield break;

        isBlinking = true;
        Color originalColor = renderer.material.color;
        float value = 0;
        while (value < 1)
        {
            value += blinkingSpeed * Time.deltaTime;
            value = Mathf.Clamp01(value);
            renderer.material.color = Color.Lerp(blinkColor, originalColor, value);
            yield return null;
        }
        isBlinking = false;
    }

    public void SetEnemyState(ENEMYSTATE state)
    {
        enemyState = state;
    }

    public void AnimHitBarricade()
    {
        Barricade.Instance.TakeDamage();
    }
}