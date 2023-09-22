using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockGunUI : MonoBehaviour
{
    public GunTypeID gunTypeID;
    public GameObject lockedObj;
    public Text unlockAtLv;

    void Start()
    {
        lockedObj.SetActive(GlobalValue.LevelReached < gunTypeID.unlockAtLevel);
        GetComponent<Button>().interactable = GlobalValue.LevelReached >= gunTypeID.unlockAtLevel;
        unlockAtLv.text = "DAY " + gunTypeID.unlockAtLevel + "";
    }
}