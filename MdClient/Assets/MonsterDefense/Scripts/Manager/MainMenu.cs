using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	// Use this for initialization
	public void loadLevelMap () {
        SoundManager.instance.playSoundClick();
        //Application.LoadLevel(GameSetting.MAP_SCENE);
        SceneManager.LoadScene(GameSetting.MAP_SCENE);
	}
	
	// Update is called once per frame
	public void quitGame () {
        //Application.Quit();
        SceneManager.LoadScene(GameSetting.LOGIN_SCENE);
	}
	public void moreGame () {
		AdsControl.Instance.showAds ();
	}
}
