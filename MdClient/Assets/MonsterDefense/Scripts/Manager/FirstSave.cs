using UnityEngine;
using System.Collections;

public class FirstSave : MonoBehaviour {

	// Use this for initialization
	void Awake () {
        //PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt(GameSetting.FIRST_GAME_CHECK, GameSetting.FALSE_RESULT);
        if (PlayerPrefs.GetInt(GameSetting.FIRST_GAME_CHECK) != GameSetting.TRUE_RESULT)
        {
            setUp_firstGame();
            PlayerPrefs.SetInt(GameSetting.FIRST_GAME_CHECK, GameSetting.TRUE_RESULT);
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void setUp_firstGame()
    {
        PlayerPrefs.SetInt(GameSetting.LOCK_KEY+0,1);
        PlayerPrefs.SetFloat(GameSetting.SFX_KEY, 1);
    }
}

