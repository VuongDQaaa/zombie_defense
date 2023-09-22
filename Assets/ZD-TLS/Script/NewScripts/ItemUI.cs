using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    [SerializeField] private Button _btnSelect;
    [SerializeField] private Text _textButton;
    [SerializeField] private Text _value;
    [SerializeField] private string _price;
    [SerializeField] private string _productID;
    [SerializeField] private int _starValue;
    [SerializeField] private CoinPopup _popup;
    [SerializeField] private RectTransform _position;

    public Text TextButton { get => _textButton; set => _textButton = value; }
    public string Price { get => _price; set => _price = value; }
    public Button BtnSelect { get => _btnSelect; set => _btnSelect = value; }
    public string ProductID { get => _productID; set => _productID = value; }
    private void OnEnable()
    {
        AddListenerButton();
    }
    private void OnDisable()
    {
        RemoveListenerButton();
    }
    public void AddListenerButton()
    {
        BtnSelect.onClick.AddListener(() =>
        {
            BuyButton(ProductID, Price);
        });
    }
    private void Start()
    {
        _textButton.text = _price;
        _value.text = _starValue.ToString();
    }
    public void RemoveListenerButton()
    {
        BtnSelect.onClick.RemoveAllListeners();
    }
    public void BuyButton(string productId, string price)
    {
        CoinPopup coinPopup = Instantiate(_popup, _position.transform);
        coinPopup.GetData(productId, price);
        // Debug.Log("Check Product : " + IAPManager.Instance.CheckProduct(_productID));
        // IAPManager.Instance.BuyProduct(_productID, () =>
        // {

        // },
        // () =>
        // {
        //     Debug.LogWarning("Cancel buy product");
        // }
        // );
    }
}
