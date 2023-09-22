using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeItemUI : MonoBehaviour
{
    public GunTypeID gunID;
    public UPGRADE_TYPE itemType;
    public string description = "item power";
    public string itemUnit = "%";
    public Text price;
    public Text statusTxt;
    public Text upgrade_currentValueTxt, upgrade_nextValueTxt, upgrade_descriptionTxt;
    public GameObject upgradeBtn;

    bool isMax = false;

    void Start()
    {
        isMax = gunID.GetCurrentUpgrade(itemType) == gunID.GetMaxUpgradeAvailable(itemType);
        UpdateParameter();
    }

    void UpdateParameter()
    {
        statusTxt.text = gunID.GetCurrentUpgrade(itemType) + "/" + gunID.GetNextUpgradeInspector(itemType).Length;
        upgrade_currentValueTxt.text = gunID.GetUpgradeValue(itemType) + itemUnit;
        if (!isMax)
            upgrade_nextValueTxt.text = "(+" + gunID.GetNextUpgradeInspector(itemType)[gunID.GetCurrentUpgrade(itemType)].value + itemUnit + ")";
        else
            upgrade_nextValueTxt.text = "";

        upgrade_descriptionTxt.text = description;

        if (isMax)
        {
            price.enabled = false;
        }

        else
        {
            price.text = gunID.GetNextUpgradeInspector(itemType)[gunID.GetCurrentUpgrade(itemType)].price + "";
        }

        upgradeBtn.SetActive(!isMax);
    }

    public void Upgrade()
    {
        if (isMax)
            return;

        var _price = gunID.GetNextUpgradeInspector(itemType)[gunID.GetCurrentUpgrade(itemType) ].price;
        if (GlobalValue.SavedCoins >= _price)
        {
            GlobalValue.SavedCoins -= _price;
            SoundManager.PlaySfx(SoundManager.Instance.soundUpgrade);

            gunID.AddCurrentUpgrade(itemType);

            isMax = gunID.GetCurrentUpgrade(itemType) == gunID.GetMaxUpgradeAvailable(itemType);

            UpdateParameter();
        }
        else
            SoundManager.PlaySfx(SoundManager.Instance.soundNotEnoughCoin);
    }

    private void OnDrawGizmos()
    {
        gameObject.name = itemType.ToString();
    }
}