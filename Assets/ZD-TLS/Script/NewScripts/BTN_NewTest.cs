using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class BTN_NewTest : MonoBehaviour
{
    [SerializeField] private GameObject _loadingScreen;
    [SerializeField] private Slider _loadingBar;
    [SerializeField] private TMP_Text _loadingText;
    [SerializeField] private Button _playGameBtn;
    [SerializeField] private Button _storeBtn;
    [SerializeField] private Button _adsBtn;
    [SerializeField] private RectTransform _loadScene;
    [SerializeField] private GameObject _store;
    [SerializeField] private GameObject _ads;
    private float processValue = 0;
    private float nextPhase = 0;
    private bool showLoadingScene = false;
    [SerializeField] private int _loadingTime;

    private void Start()
    {
        _playGameBtn.onClick.AddListener(()
        =>
        {
            Debug.Log("PlayGame");
            showLoadingScene = true;
        });
        _storeBtn.onClick.AddListener(()
        =>
        {
            GameObject go = Instantiate(_store, _loadScene.transform);
            Debug.Log("Store");
        });
        _adsBtn.onClick.AddListener(()
        =>
        {
            GameObject go = Instantiate(_ads, _loadScene.transform);
            Debug.Log("Ads");
        });
    }

    private void Update()
    {
        if (showLoadingScene == true)
        {
            LoadingProcess("Playing");
            //use input to test
            if(Input.GetKeyDown(KeyCode.Space))
            {
                nextPhase += 10;
            }
            IncreaseValue(nextPhase);
        }
    }

    private void LoadingProcess(string sceneName)
    {
        _loadingScreen.SetActive(true);
        _loadingBar.value = processValue;
        _loadingText.text = "Loading..." + ((short)processValue) + "%";
        if (processValue >= 100)
        {
            SceneManager.LoadScene(sceneName);
        }
    }


    private void IncreaseValue(float nextPhase)
    {
        if(processValue < nextPhase)
        {
            processValue += 100 / _loadingTime;
        }
    }
}

