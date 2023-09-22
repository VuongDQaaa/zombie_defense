using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestAdScene : MonoBehaviour
{
    public Text rewardedVideo;
    public Text timeNextAd;
    public Text totalCoins;
    int totalReward = 0;

    void Start()
    {
        rewardedVideo.text = "+" + AdsManager.Instance.getRewarded;
    }

    void Update()
    {
        if (AdsManager.Instance.TimeWaitingNextWatch() > 0)
            timeNextAd.text = "Wait: " + (int)AdsManager.Instance.TimeWaitingNextWatch()/60 + ":" + (int)AdsManager.Instance.TimeWaitingNextWatch() % 60;
        else
            timeNextAd.text = "";

        totalCoins.text = "Coins: " + totalReward;
    }

    public void ShowBanner(bool show)
    {
        //AdsManager.Instance.ShowAdmobBanner(show);
    }

    public void ShowNormalAd()
    {
        AdsManager.Instance.ShowNormalAd(GameManager.GameState.Success, true);
    }

    public void ShowRewardedAd()
    {
        if (AdsManager.Instance && AdsManager.Instance.isRewardedAdReady() && AdsManager.Instance.TimeWaitingNextWatch() <= 0)
        {
            AdsManager.AdResult += AdsManager_AdResult;
            AdsManager.Instance.ShowRewardedAds();
        }
    }

    private void AdsManager_AdResult(bool isSuccess, int rewarded)
    {
        AdsManager.AdResult -= AdsManager_AdResult;
        if (isSuccess)
            totalReward += rewarded;
    }
}
