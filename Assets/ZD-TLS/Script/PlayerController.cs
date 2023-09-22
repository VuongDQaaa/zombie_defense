using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IListener
{
    [Header("SET UP")]
    public Transform aimPoint;
    public float aimPointFixedY = 1.5f;
    Camera m_camera;
    [ReadOnly] public Vector2 input;
    public Animator anim;
    [Header("WEAPONS")]
    public GunTypeID gunTypeID;
    public Transform firePoint;
    float lastTimeShooting = -999;
    bool allowShooting = true;
    public bool isFacingRight { get { return transform.rotation.eulerAngles.y == 0; } }

    [Header("---EXTRA---")]
    [ReadOnly] public int DamagePercent = 0;
    [ReadOnly] public int ChanceBounce = 0;
    [ReadOnly] public int ExtraBullet = 0;
    [ReadOnly] public int ChanceCritical = 0;
    [ReadOnly] public int FireRate = 0;
    [ReadOnly] public int BulletSpeed = 0;
    [ReadOnly] public int ChanceExplosion = 0;
    [ReadOnly] public int MoreCrititalDamacePercent = 0;
    [ReadOnly] public int ChanceX3Damage = 0;
    [ReadOnly] public int ThroughEnemyBody = 0;

    bool isRunningAway = false;

    public void UpdateAllUpgrades()
    {
        DamagePercent = gunTypeID.GetUpgradeValue(UPGRADE_TYPE.Damage);
        ChanceBounce = gunTypeID.GetUpgradeValue(UPGRADE_TYPE.ChanceBounce);
        ExtraBullet = gunTypeID.GetUpgradeValue(UPGRADE_TYPE.ExtraBullet);
        ChanceCritical = gunTypeID.GetUpgradeValue(UPGRADE_TYPE.ChanceCritical);
        FireRate = Mathf.Clamp( gunTypeID.GetUpgradeValue(UPGRADE_TYPE.FireRate), 0, 100);
        BulletSpeed = gunTypeID.GetUpgradeValue(UPGRADE_TYPE.BulletSpeed);
        ChanceExplosion = gunTypeID.GetUpgradeValue(UPGRADE_TYPE.ChanceExplosion);
        MoreCrititalDamacePercent = gunTypeID.GetUpgradeValue(UPGRADE_TYPE.MoreCrititalDamacePercent);
        ChanceX3Damage = gunTypeID.GetUpgradeValue(UPGRADE_TYPE.ChanceX3Damage);
        ThroughEnemyBody = gunTypeID.GetUpgradeValue(UPGRADE_TYPE.ThroughEnemyBody);
    }

    void Start()
    {
        UpdateAllUpgrades();
        if (anim == null)
            anim = GetComponent<Animator>();

        GameManager.Instance.Player = this;
        GameManager.Instance.AddListener(this);
        if (gunTypeID == null)
            gunTypeID = GunManager.Instance.getGunID();
        SetGun(gunTypeID);
    }

    private void OnEnable()
    {
        if (GameManager.Instance)
            GameManager.Instance.Player = this;
    }

    void Update()
    {
        if (isRunningAway)
        {
            transform.forward = Vector3.right;
            anim.SetBool("isRunningAway", true);
            transform.Translate(Vector3.right * 3 * Time.deltaTime, Space.World);
            return;
        }

        if (GameManager.Instance.State != GameManager.GameState.Playing)
            return;

            RaycastHit hit;
            var mousePos = Input.mousePosition;
            mousePos.x = Mathf.Clamp(mousePos.x, 0, Screen.width * 0.75f);
            Ray ray = Camera.main.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out hit, 100, 1 << LayerMask.NameToLayer("Ground")))
            {
                var lookAt = hit.point;
                lookAt.y = 1.5f;
                aimPoint.position = lookAt;
                if (Input.GetMouseButton(0))
                    Shoot();
            }
    }

    void GetInput()
    {
        input = new Vector2(ControllerInput.Instance.Horizontak + Input.GetAxis("Horizontal"), ControllerInput.Instance.Vertical + Input.GetAxis("Vertical"));

    }

    public void AnimSetTrigger(string name)
    {
        anim.SetTrigger(name);
    }

    public void AnimSetSpeed(float value)
    {
        if (anim)
            anim.speed = value;
    }

    public void AnimSetFloat(string name, float value)
    {
        anim.SetFloat(name, value);
    }

    public void AnimSetBool(string name, bool value)
    {
        anim.SetBool(name, value);
    }

    public void Shoot()
    {
        if (!allowShooting)
            return;

        if (Time.time < (lastTimeShooting + gunTypeID.rate * ((100- FireRate) * 0.01f)))
            return;

        lastTimeShooting = Time.time;
        AnimSetTrigger("shoot");

        GlobalValue.isPlayerFireBullet = true;

        //for spread bullet
        int _right = 0;
        int _left = 0;

        for (int i = 0; i < (gunTypeID.maxBulletPerShoot + ExtraBullet); i++)
        {
            StartCoroutine(FireCo());

            var projectile = SpawnSystemHelper.GetNextObject(gunTypeID.bulletObj, firePoint.position, false);
            projectile.transform.forward = firePoint.up;

            if (gunTypeID.isSpreadBullet)
            {
                if (i != 0)
                {
                    if (i % 2 == 1)
                    {
                        _right++;
                        projectile.transform.Rotate(Vector3.up, 10 * _right);
                    }
                    else
                    {
                        _left++;
                        projectile.transform.Rotate(Vector3.up, -10 * _left);
                    }
                }
            }
            else
            {
                if (i != 0)
                {
                    if (i % 2 == 1)
                    {
                        _right++;
                        projectile.transform.position += transform.right * 0.2f * _right;
                    }
                    else
                    {
                        _left++;
                        projectile.transform.position -= transform.right * 0.2f * _left;
                    }
                }
            }
            projectile.gameObject.SetActive(true);
            projectile.GetComponent<BulletProjectile>().InitBullet(
                gunTypeID.damage * (100 + DamagePercent) / 100,
                gunTypeID.bulletSpeed * (100 + BulletSpeed) / 100,
                ChanceBounce, ChanceCritical, ChanceExplosion, MoreCrititalDamacePercent, ChanceX3Damage, ThroughEnemyBody > 0);
        }

        SoundManager.PlaySfx(gunTypeID.soundFire, gunTypeID.soundFireVolume);

        CancelInvoke("CheckBulletRemain");
        Invoke("CheckBulletRemain", gunTypeID.rate * ((100- FireRate) * 0.01f));
    }

    public IEnumerator FireCo()
    {
        yield return null;

        if (gunTypeID.muzzleFX)
        {
            var _muzzle = SpawnSystemHelper.GetNextObject(gunTypeID.muzzleFX, firePoint.position, true);
            _muzzle.transform.forward = firePoint.up;
        }
    }

    public void SetGun(GunTypeID gunID)
    {
        gunTypeID = gunID;
        allowShooting = false;
        Invoke("AllowShooting", 0.3f);
    }

    void AllowShooting()
    {
        allowShooting = true;
    }

    public void IPlay()
    {
        UpdateAllUpgrades();
    }

    public void ISuccess()
    {
    }

    public void IPause()
    {
    }

    public void IUnPause()
    {
    }

    public void IGameOver()
    {
        isRunningAway = true;
    }

    public void IOnRespawn()
    {
    }

    public void IOnStopMovingOn()
    {
    }

    public void IOnStopMovingOff()
    {
    }
}