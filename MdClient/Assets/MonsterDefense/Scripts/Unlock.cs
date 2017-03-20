using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Unlock : MonoBehaviour {
	
	public int numberlevel;
	private int checkLock;

	public GameObject unLock;
	public GameObject levelLock;
	public GameObject Star;

    public Sprite backGround;
    public List<Enemy> listEnemy;

    // Use this for initialization
    void Start () {

        checkLock = GameData.Instance.GetLevelStar(numberlevel);//PlayerPrefs.GetInt (GameSetting.LOCK_KEY+numberlevel);

		if (checkLock >= GameSetting.TRUE_RESULT || numberlevel == 0) {
			levelLock.SetActive (false);
			unLock.SetActive (true);
			Star.SetActive(true);
		} else {
			levelLock.SetActive (true);
			unLock.SetActive (false);
			Star.SetActive(false);
		}

	}

	public void playLevel(){
        if (checkLock >= 1 || numberlevel == 0)
        {
            Data.instance.level = numberlevel;
            Data.instance.setData(listEnemy,backGround);
			//Application.LoadLevel(GameSetting.GAME_SCENE);
            SceneManager.LoadScene(GameSetting.GAME_SCENE);
		}
	}
}
