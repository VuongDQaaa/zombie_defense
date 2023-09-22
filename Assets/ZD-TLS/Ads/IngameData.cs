using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ensign.Unity;
public class IngameData : Singleton<IngameData>
{
    private bool _isShowAds;
    public bool IsShowAds
    {
        get => _isShowAds;
        set => _isShowAds = value;
    }
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
