using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum UPGRADE_TYPE { Damage, ChanceBounce, ExtraBullet, ChanceCritical, FireRate, BulletSpeed, ChanceExplosion, MoreCrititalDamacePercent, ChanceX3Damage, ThroughEnemyBody }

public class GunTypeID : MonoBehaviour
{
    public int unlockAtLevel = 3;
    public GameObject bulletObj;
    public int damage = 10;
    public float bulletSpeed = 1000;
    public string gunID = "gun ID";
    [Header("WEAPONS")]
    public float rate = 0.2f;
    public AudioClip soundFire;
    [Range(0, 1)]
    public float soundFireVolume = 0.5f;
    public int maxBulletPerShoot = 1;
    public bool isSpreadBullet = false;
    public GameObject muzzleFX;

    [Header("------UPGRADE---------")]
    [Space]
    public UpgradeStep[] upgradeDamagePercent;
    public UpgradeStep[] upgradeChanceBounce;
    public UpgradeStep[] upgradeExtraBullet;
    public UpgradeStep[] upgradeChanceCritical;
    public UpgradeStep[] upgradeFireRate;
    public UpgradeStep[] upgradeBulletSpeed;
    public UpgradeStep[] upgradeChanceExplosion;
    public UpgradeStep[] upgradeMoreCrititalDamacePercent;
    public UpgradeStep[] upgradeChanceX3Damage;
    public UpgradeStep[] upgradeThroughEnemyBody;

    public int GetCurrentUpgrade (UPGRADE_TYPE upgradeType)
    {
        return PlayerPrefs.GetInt(gunID + upgradeType + "upgrade" + "Current", 0);
    }

    public void AddCurrentUpgrade(UPGRADE_TYPE upgradeType)
    {
        PlayerPrefs.SetInt(gunID + upgradeType + "upgrade" + "Current", GetCurrentUpgrade(upgradeType) + 1);
    }

    public int GetUpgradeValue(UPGRADE_TYPE upgradeType)
    {
        int _upgradeLevel = GetCurrentUpgrade(upgradeType);
        if (_upgradeLevel == 0)
            return 0;

        switch (upgradeType)
        {
            case UPGRADE_TYPE.Damage:
                _upgradeLevel = Mathf.Min(_upgradeLevel, upgradeDamagePercent.Length);
                return upgradeDamagePercent[_upgradeLevel - 1].value;
            case UPGRADE_TYPE.BulletSpeed:
                _upgradeLevel = Mathf.Min(_upgradeLevel, upgradeBulletSpeed.Length);
                return upgradeBulletSpeed[_upgradeLevel - 1].value;
            case UPGRADE_TYPE.ChanceBounce:
                _upgradeLevel = Mathf.Min(_upgradeLevel, upgradeChanceBounce.Length);
                return upgradeChanceBounce[_upgradeLevel - 1].value;
            case UPGRADE_TYPE.ChanceCritical:
                _upgradeLevel = Mathf.Min(_upgradeLevel, upgradeChanceCritical.Length);
                return upgradeChanceCritical[_upgradeLevel - 1].value;
            case UPGRADE_TYPE.ChanceExplosion:
                _upgradeLevel = Mathf.Min(_upgradeLevel, upgradeChanceExplosion.Length);
                return upgradeChanceExplosion[_upgradeLevel - 1].value;
            case UPGRADE_TYPE.ChanceX3Damage:
                _upgradeLevel = Mathf.Min(_upgradeLevel, upgradeChanceX3Damage.Length);
                return upgradeChanceX3Damage[_upgradeLevel - 1].value;
            case UPGRADE_TYPE.ExtraBullet:
                _upgradeLevel = Mathf.Min(_upgradeLevel, upgradeExtraBullet.Length);
                return upgradeExtraBullet[_upgradeLevel - 1].value;
            case UPGRADE_TYPE.FireRate:
                _upgradeLevel = Mathf.Min(_upgradeLevel, upgradeFireRate.Length);
                return upgradeFireRate[_upgradeLevel - 1].value;
            case UPGRADE_TYPE.MoreCrititalDamacePercent:
                _upgradeLevel = Mathf.Min(_upgradeLevel, upgradeMoreCrititalDamacePercent.Length);
                return upgradeMoreCrititalDamacePercent[_upgradeLevel - 1].value;
            case UPGRADE_TYPE.ThroughEnemyBody:
                _upgradeLevel = Mathf.Min(_upgradeLevel, upgradeThroughEnemyBody.Length);
                return upgradeThroughEnemyBody[_upgradeLevel - 1].value;
            default:
                return 0;
        }
    }

    public int GetMaxUpgradeAvailable(UPGRADE_TYPE upgradeType)
    {
        int maxUpgrade = 0;
        switch (upgradeType)
        {
            case UPGRADE_TYPE.Damage:
                maxUpgrade = upgradeDamagePercent.Length;
                break;
            case UPGRADE_TYPE.BulletSpeed:
                maxUpgrade = upgradeBulletSpeed.Length;
                break;
            case UPGRADE_TYPE.ChanceBounce:
                maxUpgrade = upgradeChanceBounce.Length;
                break;
            case UPGRADE_TYPE.ChanceCritical:
                maxUpgrade = upgradeChanceCritical.Length;
                break;
            case UPGRADE_TYPE.ChanceExplosion:
                maxUpgrade = upgradeChanceExplosion.Length;
                break;
            case UPGRADE_TYPE.ChanceX3Damage:
                maxUpgrade = upgradeChanceX3Damage.Length;
                break;
            case UPGRADE_TYPE.ExtraBullet:
                maxUpgrade = upgradeExtraBullet.Length;
                break;
            case UPGRADE_TYPE.FireRate:
                maxUpgrade = upgradeFireRate.Length;
                break;
            case UPGRADE_TYPE.MoreCrititalDamacePercent:
                maxUpgrade = upgradeMoreCrititalDamacePercent.Length;
                break;
            case UPGRADE_TYPE.ThroughEnemyBody:
                maxUpgrade = upgradeThroughEnemyBody.Length;
                break;
        }

        return maxUpgrade;
    }

    public UpgradeStep[] GetNextUpgradeInspector(UPGRADE_TYPE upgradeType)
    {
        switch (upgradeType)
        {
            case UPGRADE_TYPE.Damage:
                return upgradeDamagePercent;
            case UPGRADE_TYPE.BulletSpeed:
                return upgradeBulletSpeed;
            case UPGRADE_TYPE.ChanceBounce:
                return upgradeChanceBounce;
            case UPGRADE_TYPE.ChanceCritical:
                return upgradeChanceCritical;
            case UPGRADE_TYPE.ChanceExplosion:
                return upgradeChanceExplosion;
            case UPGRADE_TYPE.ChanceX3Damage:
                return upgradeChanceX3Damage;
            case UPGRADE_TYPE.ExtraBullet:
                return upgradeExtraBullet;
            case UPGRADE_TYPE.FireRate:
                return upgradeFireRate;
            case UPGRADE_TYPE.MoreCrititalDamacePercent:
                return upgradeMoreCrititalDamacePercent;
            case UPGRADE_TYPE.ThroughEnemyBody:
                return upgradeThroughEnemyBody;
            default:
                return null;
        }
    }
}

[System.Serializable]
public class UpgradeStep
{
    public int price;
    public int value;
}