using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AdsPopup : MonoBehaviour
{
    private string _idAds;
    private int _dayRemove;
    private string _price;
    [SerializeField] private Button _cancelButton;
    [SerializeField] private Button _BuyButton;
    [SerializeField] private GameObject _popupPanel;
    [SerializeField] private TextMeshProUGUI _productPrice;

    private void Start()
    {
        _cancelButton.onClick.AddListener(() => CancelButton());
        _BuyButton.onClick.AddListener(() => BuyButton());
    }

    public void SetData(int dayRemove, string idAds, string price)
    {
        _idAds = idAds;
        _dayRemove = dayRemove;
        _price = price;
        _productPrice.text = "$ " + _price;
    }

    public void CancelButton()
    {
        Destroy(_popupPanel);
    }

    public void BuyButton()
    {
        RemoveAds.Instance.RemoveAdsWithDay(_dayRemove, _idAds);
        Destroy(_popupPanel);
    }
}
