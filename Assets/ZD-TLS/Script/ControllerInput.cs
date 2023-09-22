using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerInput : MonoBehaviour
{
    public static ControllerInput Instance;

    public float Vertical, Horizontak;
    bool shooting;

    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        if (shooting)
        {
            GameManager.Instance.Player.Shoot();
        }
    }
    public void Shoot(bool hold)
    {
        shooting = hold;
    }

    public void Melee()
    {
        //GameManager.Instance.Player.MeleeAttack();
    }
}
