using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour, IListener
{
    public GameObject[] Players;

    public void IGameOver()
    {
    }

    public void IOnRespawn()
    {
    }

    public void IOnStopMovingOff()
    {
    }

    public void IOnStopMovingOn()
    {
    }

    public void IPause()
    {
    }

    public void IPlay()
    {
        Players[GlobalValue.PickPlayerID].SetActive(true);
    }

    public void ISuccess()
    {
    }

    public void IUnPause()
    {
    }
}
