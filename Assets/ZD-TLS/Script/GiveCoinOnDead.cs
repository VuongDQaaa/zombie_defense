using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveCoinOnDead : MonoBehaviour
{
    public int coinMin = 30;
    public int coinMax = 40;

    public void GiveCoin()
    {
        var rewarded = Random.Range(coinMin, coinMax);
        GlobalValue.SavedCoins += rewarded;
        FloatingTextManager.Instance.ShowText("+" + rewarded, new Vector2(transform.position.x, transform.position.z + 1f), Vector2.zero, Color.yellow, 50);
    }
}
