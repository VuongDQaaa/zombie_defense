using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public GameObject[] shopPanels;
    public Sprite buttonActiveImage, buttonInActiveImage;
    public Image upgradeBut, upgradeWallBtn, buyCoinBut;
    void Start()
    {
        DisableObj();
        ActivePanel(shopPanels[0]);
        SetActiveBut(0);
    }

    void DisableObj()
    {
        foreach (var obj in shopPanels)
        {
            obj.SetActive(false);
        }
    }

    void ActivePanel(GameObject obj)
    {

        obj.SetActive(true);
    }

    public void SwichPanel(GameObject obj)
    {
        for (int i = 0; i < shopPanels.Length; i++)
        {
            if (obj == shopPanels[i])
            {
                DisableObj();
                ActivePanel(shopPanels[i]);
                SetActiveBut(i);

                break;
            }
        }
        SoundManager.Click();
    }

    void SetActiveBut(int i)
    {
        upgradeBut.sprite = buttonInActiveImage;
        upgradeWallBtn.sprite = buttonInActiveImage;
        buyCoinBut.sprite = buttonInActiveImage;

        switch (i)
        {
            case 0:
                upgradeBut.sprite = buttonActiveImage;
                break;
            case 1:
                upgradeWallBtn.sprite = buttonActiveImage;
                break;
            case 2:
                buyCoinBut.sprite = buttonActiveImage;
                break;
            default:

                break;
        }
    }
}