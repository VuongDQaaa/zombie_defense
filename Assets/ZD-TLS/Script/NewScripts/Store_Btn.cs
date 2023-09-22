using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Store_Btn : MonoBehaviour
{
    [SerializeField] private Button _closeButton;
    private void Start()
    {
        _closeButton.onClick.AddListener(() =>
        {
            RemoveObject();
        });
    }

    private void RemoveObject()
    {
        Destroy(gameObject);
    }
}
