using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barricade : MonoBehaviour
{
    public static Barricade Instance;
    public AudioClip hitSound;
    public int hearts = 4;
    [ReadOnly] public int currentHearts = 0;
    [Header("HIT EFFECT")]
    public bool playEarthQuake = true;
    public float _eqTime = 0.1f;
    public float _eqSpeed = 60;
    public float _eqSize = 1;
    void Start()
    {
        currentHearts = hearts;
        Instance = this;
    }


    public void TakeDamage()
    {
        if (GameManager.Instance.State != GameManager.GameState.Playing)
            return;

        currentHearts--;
        if (playEarthQuake)
            CameraPlay.EarthQuakeShake(_eqTime, _eqSpeed, _eqSize);
        SoundManager.PlaySfx(hitSound);

        if (currentHearts == 0)
        {
            GameManager.Instance.GameOver();
        }
    }
}
