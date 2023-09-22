using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour, IListener
{
    public static MenuManager Instance;
    public GameObject WeaponSystemUI;
    public GameObject UI;
    public GameObject VictotyUI;
    public GameObject FailUI;
    public GameObject PauseUI;
    public GameObject LoadingUI;
    [Header("Sound and Music")]
    public Image soundImage;
    public Image musicImage;
    public Sprite soundImageOn, soundImageOff, musicImageOn, musicImageOff;
    public Text levelTxt;
    public Text[] coinTxt;
    [Header("---HEARTS---")]
    public Image heart;
    [ReadOnly] public List<Image> listDot;
    Barricade barricade;
    public GameObject tutorial;

    private void Awake()
    {
        Instance = this;
        WeaponSystemUI.SetActive(false);
        VictotyUI.SetActive(false);
        FailUI.SetActive(false);
        PauseUI.SetActive(false);
        LoadingUI.SetActive(false);
        levelTxt.text = "DAY " + GlobalValue.LevelReached;
    }



    public void BeginGame()
    {
        GameManager.Instance.StartGame();
    }

    void Start()
    {
        soundImage.sprite = GlobalValue.isSound ? soundImageOn : soundImageOff;
        musicImage.sprite = GlobalValue.isMusic ? musicImageOn : musicImageOff;
        if (!GlobalValue.isSound)
            SoundManager.SoundVolume = 0;
        if (!GlobalValue.isMusic)
            SoundManager.MusicVolume = 0;

        //init hearts
        barricade = FindObjectOfType<Barricade>();
        listDot = new List<Image>();
        listDot.Add(heart);
        if (barricade)
        {
            for (int i = 1; i < barricade.hearts; i++)
            {
                listDot.Add(Instantiate(heart, heart.transform.parent));
            }
        }
        else
            Debug.LogError("There are no BARRICADE in the scene, please check!");

        WeaponSystemUI.SetActive(true);

        //InvokeRepeating("UpdateRewardedAd", 0, 0.1f);
    }

    private void Update()
    {
        UpdateHearts();

        tutorial.SetActive(!GlobalValue.isPlayerFireBullet && GlobalValue.LevelReached == 1);
    }

    void UpdateHearts()
    {
        for(int i = 0; i < listDot.Count; i++)
        {
            listDot[(listDot.Count  -1) - i].color = i < barricade.currentHearts ? Color.white : Color.black;
        }

        foreach(var txt in coinTxt)
        {
            txt.text = "COIN: " + GlobalValue.SavedCoins.ToString();
        }
    }

    float currentTimeScale;
    public void Pause()
    {
        SoundManager.PlaySfx(SoundManager.Instance.soundPause);
        if (Time.timeScale != 0)
        {
            GameManager.Instance.Gamepause();
            currentTimeScale = Time.timeScale;
            Time.timeScale = 0;
            UI.SetActive(false);
            PauseUI.SetActive(true);
            SoundManager.Instance.PauseMusic(true);
        }
        else
        {
            GameManager.Instance.UnPause();
            Time.timeScale = currentTimeScale;
            UI.SetActive(true);
            PauseUI.SetActive(false);
            SoundManager.Instance.PauseMusic(false);
        }
    }

    public void IPlay()
    {
        WeaponSystemUI.SetActive(false);
        UI.SetActive(true);
    }

    public void ISuccess()
    {
        StartCoroutine(VictoryCo());
    }

    IEnumerator VictoryCo()
    {
        UI.SetActive(false);
        yield return new WaitForSeconds(1.5f);
        VictotyUI.SetActive(true);
        SoundManager.PlaySfx(SoundManager.Instance.soundVictoryPanel);
    }


    public void IPause()
    {
      
    }

    public void IUnPause()
    {
        
    }

    public void IGameOver()
    {
        StartCoroutine(GameOverCo());
    }

    IEnumerator GameOverCo()
    {
        UI.SetActive(false);

        yield return new WaitForSeconds(1.5f);
        FailUI.SetActive(true);
    }

    public void IOnRespawn()
    {
        
    }

    public void IOnStopMovingOn()
    {
        
    }

    public void IOnStopMovingOff()
    {
       
    }

    
    #region Music and Sound
    public void TurnSound()
    {
        GlobalValue.isSound = !GlobalValue.isSound;
        soundImage.sprite = GlobalValue.isSound ? soundImageOn : soundImageOff;

        SoundManager.SoundVolume = GlobalValue.isSound ? 1 : 0;
    }

    public void TurnMusic()
    {
        GlobalValue.isMusic = !GlobalValue.isMusic;
        musicImage.sprite = GlobalValue.isMusic ? musicImageOn : musicImageOff;

        SoundManager.MusicVolume = GlobalValue.isMusic ? SoundManager.Instance.musicsGameVolume : 0;
    }
    #endregion

    #region Load Scene
    public void LoadHomeMenuScene()
    {
        SoundManager.Click();
        StartCoroutine(LoadAsynchronously("Menu"));
    }

    public void RestarLevel()
    {
        SoundManager.Click();
        StartCoroutine(LoadAsynchronously(SceneManager.GetActiveScene().name));
    }

    public void LoadNextLevel()
    {
        SoundManager.Click();
        StartCoroutine(LoadAsynchronously(SceneManager.GetActiveScene().name));
    }

    [Header("Load scene")]
    public Slider slider;
    public Text progressText;
    IEnumerator LoadAsynchronously(string name)
    {
        LoadingUI.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(name);
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            slider.value = progress;
            progressText.text = (int)progress * 100f + "%";
            yield return null;
        }
    }
    #endregion

    private void OnDisable()
    {
        Time.timeScale = 1;
    }

    //public Text rewardedCoin;
    //public Text rewardedAdWait;

    //void UpdateRewardedAd()
    //{
    //    rewardedCoin.text = "+" + AdsManager.Instance.getRewarded;

    //    if (AdsManager.Instance.TimeWaitingNextWatch() > 0)
    //        rewardedAdWait.text = "Wait: " + (int)AdsManager.Instance.TimeWaitingNextWatch() / 60 + ":" + (int)AdsManager.Instance.TimeWaitingNextWatch() % 60;
    //    else
    //        rewardedAdWait.text = "";
    //}

    //public void WatchRewardedAd()
    //{
    //    if (AdsManager.Instance && AdsManager.Instance.isRewardedAdReady() && AdsManager.Instance.TimeWaitingNextWatch() <= 0)
    //    {
    //        AdsManager.AdResult += AdsManager_AdResult;
    //        AdsManager.Instance.ShowRewardedAds();
    //    }
    //}

    //private void AdsManager_AdResult(bool isSuccess, int rewarded)
    //{
    //    AdsManager.AdResult -= AdsManager_AdResult;
    //    if (isSuccess)
    //        GlobalValue.SavedCoins += rewarded;
    //}
}
