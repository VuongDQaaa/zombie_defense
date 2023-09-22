using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FlashScene : MonoBehaviour {

	public string sceneLoad;
    
    // public GameObject tapToPlayObj; 
	// Use this for initialization
	public void Start () {
		StartCoroutine (LoadSceneCo ());
        // tapToPlayObj.SetActive(false);
    }

    public IEnumerator LoadSceneCo()
    {
        yield return new WaitForSeconds(0);
        StartCoroutine(LoadAsynchronously(sceneLoad));
    }

    [Header("LOADING PROGRESS")]
    public Slider slider;
    public Text progressText;
    IEnumerator LoadAsynchronously(string name)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(name);
        //Don't let the Scene activate until you allow it to
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            //float progress = Mathf.Clamp01(operation.progress / 0.9f);
            //if (slider != null)
            //    slider.value = progress;
            //if (progressText != null)
            //    progressText.text = (int)progress * 100f + "%";
            //yield return null;

            //Output the current progress
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            //m_Text.text = "Loading progress: " + (operation.progress * 100) + "%";
            if (slider != null)
                slider.value = progress;
            if (progressText != null)
                progressText.text = (int)progress * 100f + "%";

            // Check if the load has finished
            if (operation.progress >= 0.9f)
            {
                //Change the Text to show the Scene is ready
                //m_Text.text = "Press the space bar to continue";
                //Wait to you press the space key to activate the Scene
                // tapToPlayObj.SetActive(true);
                slider.gameObject.SetActive(false);
                // if (Input.anyKeyDown)
                    //Activate the Scene
                    operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
