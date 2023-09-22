using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoinPopup : MonoBehaviour
{
    private string _productId;
    private string _price;
    [SerializeField] private Button _cancelButton;
    [SerializeField] private Button _buyButton;
    [SerializeField] private TextMeshProUGUI _priceText;

    private void Start()
    {
        //Button
        _cancelButton.onClick.AddListener(() =>
        {
            cancelButton();
        });
        _buyButton.onClick.AddListener(() =>
        {
            BuyButton();
        });
    }

    private void cancelButton()
    {
        Destroy(gameObject);
    }

    public void GetData(string productId, string price)
    {
        _productId = productId;
        _price = price;
        _priceText.text = "$" + _price;
    }

    private void BuyButton()
    {
        Debug.Log("Check Product : " + IAPManager.Instance.CheckProduct(_productId));
        IAPManager.Instance.BuyProduct(_productId, () =>
        {

        },
        () =>
        {
            Debug.LogWarning("Cancel buy product");
        }
        );
        cancelButton();
    }
}
