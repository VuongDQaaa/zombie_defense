using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyGroup {Normal, Run, Shield, Throw, Boss}

public class GameLevelSetup : MonoBehaviour
{
    public static GameLevelSetup Instance;
    public LevelWave testLevel;
    [ReadOnly] public List<LevelWave> levelWaves = new List<LevelWave>();

    public GameObject[] zombieBoss;
    public GameObject[] zombieThrow;
    public GameObject[] zombieNormal;
    public GameObject[] zombieRun;
    public GameObject[] zombieShield;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        var waves = transform.GetComponentsInChildren<LevelWave>();
        levelWaves = new List<LevelWave>(waves);

        for (int i = 0; i < levelWaves.Count; i++)
        {
            levelWaves[i].level = i + 1;
            levelWaves[i].gameObject.name = "Level " + levelWaves[i].level;
        }
    }

    public EnemyWave[] GetLevelWave()
    {
        if(testLevel != null)
        {
            Debug.LogError("TEST LEVEL = " + testLevel);
            return testLevel.Waves;
        }

        foreach(var obj in levelWaves)
        {
            int takeLevel = Mathf.Clamp(GlobalValue.LevelReached, 0, getTotalLevels());
            if (obj.level == takeLevel)
                return obj.Waves;
        }

        return null;
    }

    public int getTotalLevels()
    {
        return levelWaves.Count;
    }

    public bool isFinalLevel()
    {
        return GlobalValue.LevelReached == levelWaves.Count;
    }

    private void OnDrawGizmos()
    {
        if (levelWaves.Count != transform.childCount)
        {
            var waves = transform.GetComponentsInChildren<LevelWave>();
            levelWaves = new List<LevelWave>(waves);

            for(int i = 0; i<levelWaves.Count;i++)
            {
                levelWaves[i].level = i + 1;
                levelWaves[i].gameObject.name = "Level " + levelWaves[i].level; 
            }
        }
    }
}
