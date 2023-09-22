using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SPAWN_POS { Random, Top, Left, Bottom}
public class LevelEnemyManager : MonoBehaviour, IListener
{
    [ReadOnly] public int totalZombie = 0;
    public static LevelEnemyManager Instance;
    //public Transform[] spawnPositions;
    public EnemyWave[] EnemyWaves;

    List<GameObject> listEnemySpawned = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    int totalEnemy, currentSpawn;

    // Start is called before the first frame update
    void Start()
    {
        if (GameLevelSetup.Instance)
        {
            EnemyWaves = GameLevelSetup.Instance.GetLevelWave();
        }

        //calculate number of enemies
        totalEnemy = 0;
        for (int i = 0; i < EnemyWaves.Length; i++)
        {

            for (int j = 0; j < EnemyWaves[i].enemySpawns.Length; j++)
            {
                var enemySpawn = EnemyWaves[i].enemySpawns[j];
                for (int k = 0; k < enemySpawn.numberEnemy; k++)
                {
                    totalEnemy++;
                }
            }
        }

        currentSpawn = 0;
    }

    IEnumerator PlayZombieSoundRandom()
    {
        while (true) {
            if (isEnemyAlive() && GameManager.Instance.State == GameManager.GameState.Playing)
            {
                SoundManager.PlaySfx(SoundManager.Instance.zombieSound);
            }

            yield return new WaitForSeconds(Random.Range(0.5f, 2f));
        }
    }
    IEnumerator SpawnEnemyCo()
    {
        for (int i = 0; i < EnemyWaves.Length; i++)
        {
            yield return new WaitForSeconds(EnemyWaves[i].wait);

            var enemySpawnList = EnemyWaves[i].shuffleList ? Shuffle(EnemyWaves[i].enemySpawns) : new List<EnemySpawn>(EnemyWaves[i].enemySpawns);

            for (int j = 0; j < enemySpawnList.Count; j++)
            {
                var enemySpawn = enemySpawnList[j];
                yield return new WaitForSeconds(enemySpawn.wait);
                for (int k = 0; k < enemySpawn.numberEnemy; k++)
                {
                    //GameObject _temp = Instantiate(enemySpawn.enemy, (Vector2) spawnPositions[Random.Range(0,spawnPositions.Length)].position, Quaternion.identity) as GameObject;
                    GameObject _temp = null;

                    if (enemySpawn.enemy != null)
                    {
                        _temp = Instantiate(enemySpawn.enemy, spawnPos(EnemyWaves[i].spawnPosition), Quaternion.identity) as GameObject;
                    }
                    else
                    {
                        switch (enemySpawn.enemyGroup)
                        {
                            case EnemyGroup.Normal:
                                _temp = Instantiate(GameLevelSetup.Instance.zombieNormal[Random.Range(0, GameLevelSetup.Instance.zombieNormal.Length)], spawnPos(EnemyWaves[i].spawnPosition), Quaternion.identity) as GameObject;
                                break;
                            case EnemyGroup.Run:
                                _temp = Instantiate(GameLevelSetup.Instance.zombieRun[Random.Range(0, GameLevelSetup.Instance.zombieRun.Length)], spawnPos(EnemyWaves[i].spawnPosition), Quaternion.identity) as GameObject;
                                break;
                            case EnemyGroup.Shield:
                                _temp = Instantiate(GameLevelSetup.Instance.zombieShield[Random.Range(0, GameLevelSetup.Instance.zombieShield.Length)], spawnPos(EnemyWaves[i].spawnPosition), Quaternion.identity) as GameObject;
                                break;
                            case EnemyGroup.Throw:
                                _temp = Instantiate(GameLevelSetup.Instance.zombieThrow[Random.Range(0, GameLevelSetup.Instance.zombieThrow.Length)], spawnPos(EnemyWaves[i].spawnPosition), Quaternion.identity) as GameObject;
                                break;
                            case EnemyGroup.Boss:
                                _temp = Instantiate(GameLevelSetup.Instance.zombieBoss[Random.Range(0, GameLevelSetup.Instance.zombieBoss.Length)], spawnPos(EnemyWaves[i].spawnPosition), Quaternion.identity) as GameObject;
                                break;
                        }
                    }

                    listEnemySpawned.Add(_temp);

                    currentSpawn++;

                    yield return new WaitForSeconds(enemySpawn.rate);
                }
            }
        }

        //check all enemy killed
        while (isEnemyAlive()) {
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.5f);
        GameManager.Instance.Victory();
    }

    List<EnemySpawn> Shuffle(EnemySpawn[] _list)
    {
        List<EnemySpawn> temp = new List<EnemySpawn>( _list);
        List<EnemySpawn> randomList = new List<EnemySpawn>();
        for(int i = 0; i < _list.Length; i++)
        {
            int rand = Random.Range(0, temp.Count);
            randomList.Add(temp[rand]);
            temp.RemoveAt(rand);
        }

        return randomList;
    }

    bool isEnemyAlive()
    {
        //for (int i = 0; i < listEnemySpawned.Count; i++)
        //{
        //    if (listEnemySpawned[i].gameObject != null && listEnemySpawned[i].activeInHierarchy)
        //        return true;
        //}

        return totalZombie > 0;

        //return false;
    }

    public void IGameOver()
    {
        //throw new System.NotImplementedException();
    }

    public void IOnRespawn()
    {
        //throw new System.NotImplementedException();
    }

    public void IOnStopMovingOff()
    {
        //throw new System.NotImplementedException();
    }

    public void IOnStopMovingOn()
    {
        //throw new System.NotImplementedException();
    }

    public void IPause()
    {
        //throw new System.NotImplementedException();
    }

    public void IPlay()
    {
        StartCoroutine(SpawnEnemyCo());
        StartCoroutine(PlayZombieSoundRandom());
    }

    public void ISuccess()
    {
        StopAllCoroutines();
        //throw new System.NotImplementedException();
    }

    public void IUnPause()
    {
        //throw new System.NotImplementedException();
    }

    public Vector3 spawnPos(SPAWN_POS spawnPos)
    {
        int _index = 0;
        switch (spawnPos)
        {
            case SPAWN_POS.Top:
                _index = 1;
                break;
            case SPAWN_POS.Left:
                _index = 2;
                break;
            case SPAWN_POS.Bottom:
                _index = 3;
                break;
            default:
                _index = Random.Range(1, 4);
                break;
        }
        RaycastHit hit; Ray ray;
        switch (_index)
        {
            case 1:
                
                 ray = Camera.main.ViewportPointToRay(new Vector2(Random.Range(0f,0.4f),1));

                if (Physics.Raycast(ray, out hit, 100, 1 << (LayerMask.NameToLayer("Ground"))))
                {
                    return hit.point + Vector3.forward;
                }
                break;
            case 2:
                 ray = Camera.main.ViewportPointToRay(new Vector2(0, Random.Range(0f, 1f)));

                if (Physics.Raycast(ray, out hit, 100, 1 << (LayerMask.NameToLayer("Ground"))))
                {
                    return hit.point + Vector3.left;
                }
                break;
            case 3:
                 ray = Camera.main.ViewportPointToRay(new Vector2(Random.Range(0f, 0.4f), 0));

                if (Physics.Raycast(ray, out hit, 100, 1 << (LayerMask.NameToLayer("Ground"))))
                {
                    return hit.point + Vector3.forward * -1;
                }
                break;
            default:
                return Vector3.zero;
        }

        return Vector3.zero;
    }
}

[System.Serializable]
public class EnemyWave
{
    public bool shuffleList = true;
    public SPAWN_POS spawnPosition;
    public float wait = 3;
    public EnemySpawn[] enemySpawns;
}


