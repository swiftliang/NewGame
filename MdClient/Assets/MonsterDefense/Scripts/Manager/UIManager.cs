using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using NW;

public class UIManager : MonoBehaviour {

    private GameManager _gameManager;
    private GenManager _genManager;
    private int numberPlayer;

    public Image hp;
    public Image imgStar;
    public Image btUpgrade;
    public Sprite[] iconArrowUpgrade; 
    public GameObject tableGameOver;
    public GameObject tableWin;
    public GameObject tablePause;
    public Sprite[] star;
    public Text money;
    public Text enemyKiller;

    public Text costUpgradeArrowTxt;
    public Text costBuyPlayerTxt;

    public GameObject lockSkillice;
    public GameObject lockSkillBoom;
    public GameObject lockSkillLighting;

    public Slider sound;
    public Image button_on_off_sond;
    public Sprite onSound;
    public Sprite offSound;

	// Use this for initialization
	void Start () {
        _gameManager = GameSetting._gameManger;
        _genManager = GameSetting._genManager;
        updateCostUpgrade(_genManager.listArrowUpgrade[_genManager.levelArrow + 1].cost);
        iconArrowUpgrade = GameSetting.instance.iconArrowUpgrade;
        // update costbuyplayer on scene
        costBuyPlayerTxt.text = GameSetting.instance.costBuyPlayer + "$";

        // check skill
        checkSkill("icemagic", lockSkillice);
        checkSkill("skillboom", lockSkillBoom);
        checkSkill("lighting", lockSkillLighting);

        updateSetting();
    }
	
    void checkSkill(string keySave,GameObject lockSKill)
    {
        if (PlayerPrefs.GetInt(keySave) == GameSetting.TRUE_RESULT)
            lockSKill.SetActive(false);
        else
            lockSKill.SetActive(true);
    }

    public void gameWin()
    {
        SoundManager.instance.playSoundWin();
        Time.timeScale = 0;
        tableWin.SetActive(true);
        enemyKiller.text = LevelManager.countAllEnemy + "";
        // unlock next level
        PlayerPrefs.SetInt(GameSetting.LOCK_KEY + (Data.instance.level + 1), GameSetting.TRUE_RESULT);
        // rate
        int nStar = 0;
        if (_gameManager._tower.hpPre < _gameManager._tower.hpMax / 3) {
            //PlayerPrefs.SetInt(GameSetting.STAR_KEY + Data.instance.level, GameSetting.TRUE_RESULT);
            nStar = 1;
            imgStar.GetComponent<Image>().sprite = star[0];
        }
        if (_gameManager._tower.hpPre > _gameManager._tower.hpMax / 3 && _gameManager._tower.hpPre < _gameManager._tower.hpMax * 2 / 3)
        {
            //PlayerPrefs.SetInt(GameSetting.STAR_KEY + Data.instance.level, 2);
            nStar = 2;
            imgStar.GetComponent<Image>().sprite = star[1];
        }
        if (_gameManager._tower.hpPre > _gameManager._tower.hpMax * 2 / 3)
        {
            //PlayerPrefs.SetInt(GameSetting.STAR_KEY + Data.instance.level, 3);
            nStar = 3;
            imgStar.GetComponent<Image>().sprite = star[2];
        }
        NetWorkMgr.Instance.UnlockLevel(Data.instance.level, nStar, code =>
        {
            if(code == Constants.SUCCESS)
            {
                Debug.Log("unlock level: " + Data.instance.level + 1);
            }
            else
            {
                Debug.Log("unlock level error");
            }
        });
		AdsControl.Instance.showAds ();
    }

    public void btPause()
    {
        SoundManager.instance.playSoundClick();
        Time.timeScale = 0;
		AdsControl.Instance.showAds ();
        tablePause.SetActive(true);
    }

    public void btResume()
    {
        SoundManager.instance.playSoundClick();
        Time.timeScale = 1;
        tablePause.SetActive(false);
    }

    public void gameOver()
    {
        SoundManager.instance.playSoundOver();
        Time.timeScale = 0;
        tableGameOver.SetActive(true);
		AdsControl.Instance.showAds ();
    }

    public void replay()
    {
        SoundManager.instance.playSoundClick();
        Time.timeScale = 1;
        //Application.LoadLevel(GameSetting.GAME_SCENE);
        SceneManager.LoadScene(GameSetting.GAME_SCENE);
    }

    public void mainMenu()
    {
        SoundManager.instance.playSoundClick();
        Time.timeScale = 1;
        //Application.LoadLevel(GameSetting.MAP_SCENE);
        SceneManager.LoadScene(GameSetting.MAP_SCENE);
    }

    public void updateHp(float hpPre,float hpMax)
    {
        hp.fillAmount = hpPre / hpMax;
    }

    public void updateMoney(int money)
    {
        this.money.text = money+"";
    }

    public void updateCostUpgrade(int cost)
    {
        costUpgradeArrowTxt.text = cost + "$";
    }

    public void activeLighting(GameObject cooldown)
    {
        _gameManager.setSkillState(SkillState.lighting);
        if (_gameManager.skillState == SkillState.lighting)
        {
            cooldown.SetActive(true);
        }
    }

    public void activeBoom(GameObject cooldown)
    {
        _gameManager.setSkillState(SkillState.boom);
        if (_gameManager.skillState == SkillState.boom)
        {
            cooldown.SetActive(true);
        }
    }

    public void activeIce(GameObject cooldown)
    {
        _gameManager.setSkillState(SkillState.ice);
        if (_gameManager.skillState == SkillState.ice)
        {
            cooldown.SetActive(true);
        }
    }

    public void buyPlayer(GameObject obj)
    {
        if (numberPlayer < _gameManager.listPlayer.Length)
        {
            if (_gameManager.addPlayer(numberPlayer))
            {
                numberPlayer++;
                SoundManager.instance.playSoundBuy();
            }
            if (numberPlayer >= _gameManager.listPlayer.Length)
                obj.SetActive(false);
        }
            
    }

    public void upgradeArrow(GameObject obj)
    {
        SoundManager.instance.playSoundBuy();
        _gameManager.upgradeArrow();
        if (_genManager.levelArrow < _genManager.listArrowUpgrade.Length - 1)
        {
            updateCostUpgrade(_genManager.listArrowUpgrade[_genManager.levelArrow + 1].cost);
        }
        if (_genManager.levelArrow >= _genManager.listArrowUpgrade.Length - 1)
            obj.SetActive(false);
        btUpgrade.sprite = iconArrowUpgrade[_genManager.levelArrow];
    }

    public void activeTable(GameObject table)
    {
        SoundManager.instance.playSoundClick();
        table.SetActive(true);
    }

    public void hideTable(GameObject table)
    {
        SoundManager.instance.playSoundClick();
        table.SetActive(false);
    }

    public void changeSound()
    {
        PlayerPrefs.SetFloat(GameSetting.SFX_KEY, sound.value);
        SoundManager.instance.updateVol();
    }

    public void on_off_Sound()
    {
        if (PlayerPrefs.GetFloat(GameSetting.SFX_KEY) > 0)
        {
            PlayerPrefs.SetFloat(GameSetting.SFX_KEY, 0);
            button_on_off_sond.sprite = offSound;
        }
        else {
            PlayerPrefs.SetFloat(GameSetting.SFX_KEY, 1);
            button_on_off_sond.sprite = onSound;
        }
        updateSetting();
    }

    void updateSetting()
    {
        sound.value = PlayerPrefs.GetFloat(GameSetting.SFX_KEY);
        if (sound.value > 0)
        {
            button_on_off_sond.sprite = onSound;
        }
        else {
            button_on_off_sond.sprite = offSound;
        }
    }
}
