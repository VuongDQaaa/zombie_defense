using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemoveAdsButton : MonoBehaviour
{
    [SerializeField] private string _idAds;
    [SerializeField] private int _dayRemove;
    [SerializeField] private string _price;
    [SerializeField] private Button _btnRemove;

    public string IDAds
    {
        get => _idAds;
    }
    public int DayRemove
    {
        get => _dayRemove;
    }
    public string Price
    {
        get => _price;
    }
    public Button BtnRemove
    {
        get => _btnRemove;
    }

}
