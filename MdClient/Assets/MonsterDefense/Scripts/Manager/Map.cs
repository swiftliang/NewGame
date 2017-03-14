using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Map : MonoBehaviour {

    public Text moneyText;
    
    void Start()
    {
        updateMoney();
    }

    public void updateMoney()
    {
        moneyText.text = PlayerPrefs.GetInt(GameSetting.MONEY_KEY) + "";
    }

    public void activeTable (GameObject table) {
        SoundManager.instance.playSoundClick();
        table.SetActive(true);
	}
	public void ShowAds()
	{
		SoundManager.instance.playSoundClick();
		AdsControl.Instance.ShowRewardVideo ();
	}
    public void hideTable(GameObject table)
    {
        SoundManager.instance.playSoundClick();
        table.SetActive(false);
    }

    public void mainMenu()
    {
        SoundManager.instance.playSoundClick();
        //Application.LoadLevel(GameSetting.MAIN_SCENE);
        SceneManager.LoadScene(GameSetting.MAIN_SCENE);
    }
}
