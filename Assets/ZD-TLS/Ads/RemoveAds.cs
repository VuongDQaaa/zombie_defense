using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.Purchasing;
using Ensign.Unity;
using Ensign;

public class RemoveAds : Singleton<RemoveAds>
{
    [SerializeField] private Button _btnBack;
    [SerializeField] private List<RemoveAdsButton> _removeAdsButton = new List<RemoveAdsButton>();
    [SerializeField] private GameObject _parent;
    [SerializeField] private AdsPopup _adsPopup;
    private bool _removeAds;
    public bool RemovedAds
    {
        get => _removeAds;
        set => _removeAds = value;
    }
    public static string REMOVE_ADS_DATA = "REMOVE_ADS_DATA";

    DataBuyRemoveAds DataBuyRemoveAds;
    private void OnEnable()
    {
        GetData();
        _removeAdsButton.ForEach(item =>
        {
            item.BtnRemove.onClick.AddListener(() => ShowConfirmPopup(item.DayRemove, item.IDAds, item.Price));
        });
        if (_btnBack != null)
        {
            _btnBack.onClick.AddListener(ClosePopup);
            UpdateStatusButton();
        }
    }

    public void ShowConfirmPopup(int day, string id, string price)
    {
        AdsPopup adsPopup = Instantiate(_adsPopup, _parent.transform);
        adsPopup.SetData(day, id, price);
    }
    private void FixedUpdate()
    {
        CheckTimeEndRemoveAds();
        if (_btnBack != null)
        {
            _btnBack.onClick.AddListener(ClosePopup);
            UpdateStatusButton();
        }
    }
    private void ClosePopup()
    {
        Destroy(gameObject);
    }

    public void RemoveAdForever()
    {
        DataBuyRemoveAds.BuyRemoveAdsForever = true;
        DataBuyRemoveAds.BuyRemoveAdsTrial = true;
        DataBuyRemoveAds.BuyRemoveAdsWithDay = true;
        UpdateData();
        UpdateStatusButton();
        AdmobController.Instance.ShowBanner(false);
    }

    // public void RemoveAdsTrial7Day()
    // {
    //     if (!DataBuyRemoveAds.BuyRemoveAdsTrial)
    //     {
    //         Debug.Log("Buy");
    //         DataBuyRemoveAds.TimeEndTrialAds = DateTime.Now.AddSeconds(15);
    //         DataBuyRemoveAds.BuyRemoveAdsTrial = true;
    //         _btnRemoveAdsTrial7Day.interactable = false;
    //         UpdateData();
    //         UpdateStatusButton();
    //         AdmobController.Instance.ShowBanner(false);
    //     }
    // }

    public void RemoveAdsWithDay(int day, string id)
    {
        IAPManager.Instance.BuyProduct(id, () =>
        {
            if (id == "com.iceberg.zombiedefense.forever")
            {
                RemoveAdForever();
            }
            else
            {
                AddDays(day);
            }
        }
        );
    }

    public void AddDays(int day)
    {
        if (IngameData.Instance.IsShowAds)
        {
            DataBuyRemoveAds.TimeEndTrialAds = DataBuyRemoveAds.TimeEndTrialAds.AddDays(day);
        }
        else
        {
            DataBuyRemoveAds.TimeEndTrialAds = DateTime.Now.AddDays(day);
        }
        DataBuyRemoveAds.BuyRemoveAdsWithDay = true;
        UpdateData();
        UpdateStatusButton();
        if(AdmobController.Instance != null)
        {
            AdmobController.Instance.ShowBanner(false);
        }
    }
    private void UpdateStatusButton()
    {
        _removeAdsButton.ForEach(item =>
            {
                item.BtnRemove.interactable = !DataBuyRemoveAds.BuyRemoveAdsForever;
            });
    }

    private void CheckTimeEndRemoveAds()
    {
        if (TimeEndRemoveAds() && !DataBuyRemoveAds.BuyRemoveAdsForever)
        {
            DataBuyRemoveAds.BuyRemoveAdsWithDay = false;
            if (_btnBack != null)
                UpdateStatusButton();
            GetData();
            UpdateData();
        }
    }
    /// <summary>
    /// Return true when "DateTime.Now" >= "_timeEndTrialRemoveAds"
    /// </summary>
    private bool TimeEndRemoveAds()
    {
        if (DateTime.Now > DataBuyRemoveAds.TimeEndTrialAds)
            return true;
        else
            return false;
    }

    private void UpdateData()
    {
        PlayerPrefs.SetString(REMOVE_ADS_DATA, JsonUtil.Serialize(DataBuyRemoveAds));
        IngameData.Instance.IsShowAds = DataBuyRemoveAds.BuyRemoveAdsForever || ((DataBuyRemoveAds.BuyRemoveAdsTrial || DataBuyRemoveAds.BuyRemoveAdsWithDay) && !TimeEndRemoveAds()) ? true : false;
    }
    private void GetData()
    {
        string jsonData = PlayerPrefs.GetString(REMOVE_ADS_DATA, "");
        if (string.IsNullOrEmpty(jsonData))
        {
            DataBuyRemoveAds = new DataBuyRemoveAds();
            return;
        }
        DataBuyRemoveAds = jsonData.JsonToObject<DataBuyRemoveAds>();

        _removeAds = DataBuyRemoveAds.BuyRemoveAdsForever || ((DataBuyRemoveAds.BuyRemoveAdsTrial || DataBuyRemoveAds.BuyRemoveAdsWithDay) && !TimeEndRemoveAds()) ? true : false;

    }
}

public class DataBuyRemoveAds
{
    public bool BuyRemoveAdsForever;
    public bool BuyRemoveAdsTrial;
    public bool BuyRemoveAdsWithDay;
    public DateTime TimeEndTrialAds;
    public bool IsShowAds;

    public DataBuyRemoveAds()
    {
        BuyRemoveAdsForever = false;
        BuyRemoveAdsTrial = false;
        BuyRemoveAdsWithDay = false;

        TimeEndTrialAds = DateTime.MinValue;
        IsShowAds = false;
    }
}