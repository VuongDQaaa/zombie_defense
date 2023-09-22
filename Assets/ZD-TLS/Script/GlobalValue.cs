using UnityEngine;
using System.Collections;

public class GlobalValue : MonoBehaviour {
    public static bool isPlayerFireBullet = false;

    public static int PickPlayerID {
        get { return PlayerPrefs.GetInt("PickPlayerID", 0); }
        set { PlayerPrefs.SetInt("PickPlayerID", value); }
    }

	public static bool isSound = true;
	public static bool isMusic = true;

    public static bool isNewGame
    {
        get { return PlayerPrefs.GetInt("isNewGame", 0) == 0; }
        set { PlayerPrefs.SetInt("isNewGame", value ? 0 : 1); }
    }

	public static int SavedCoins
    {
        get { return PlayerPrefs.GetInt("Coins", 0); }
        set
        {
            PlayerPrefs.SetInt("Coins", value);
        }
    }
    
    public static int LevelReached
    { 
		get { return PlayerPrefs.GetInt ("LevelReached", 1); } 
		set { PlayerPrefs.SetInt ("LevelReached", value); } 
	}

	public static bool RemoveAds { 
		get { return PlayerPrefs.GetInt ("RemoveAds", 0) == 1 ? true : false; } 
		set { PlayerPrefs.SetInt ("RemoveAds", value ? 1 : 0); } 
	}
}