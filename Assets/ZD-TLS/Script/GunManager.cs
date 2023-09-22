using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunManager : MonoBehaviour
{
    public static GunManager Instance;
    public List<GunTypeID> listGun;
   [ReadOnly] public List<GunTypeID> listGunPicked;

    int currentPos = 0;

    private void Awake()
    {
        if (GunManager.Instance != null)
            Destroy(gameObject);
        else
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);
        }
    }

    public GunTypeID getGunID()
    {
        return listGunPicked[currentPos];
    }
}
