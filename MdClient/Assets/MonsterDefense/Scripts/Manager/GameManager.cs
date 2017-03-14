using UnityEngine;
using System.Collections;

public enum GameState { none,play,pause,end};
public enum SkillState { none,ice,lighting,boom}

public class GameManager : MonoBehaviour {

    [HideInInspector]
    public GameState gameState = GameState.none;
    [HideInInspector]
    public SkillState skillState = SkillState.none;
    [HideInInspector]
    public UIManager _uiManager;
    [HideInInspector]
    public GenManager _genManager;
    [HideInInspector]
    public Tower _tower;
    public int money;
    [HideInInspector]
    public int countEnemyDie;
    public GameObject[] listPlayer;

    void Start () {
		Application.targetFrameRate = 60;
        setGameState(GameState.play);
        _uiManager = GameSetting._uiManager;
        _tower = GameSetting._tower;
        _genManager = GameSetting._genManager;
        //get money
        updateMoney(GameSetting.instance.getMoneyPre());
        listPlayer = GameSetting.instance.listPlayer;
	}

    public void checkWin()
    {
        countEnemyDie++;
        if (countEnemyDie >= LevelManager.countAllEnemy)
            gameWin();
    }

    public void gameOver()
    {
        GameSetting.instance.setMoneyPre(money);
        setGameState(GameState.end);
        _uiManager.gameOver();
    }

    public void gameWin()
    {
        GameSetting.instance.setMoneyPre(money);
        setGameState(GameState.end);
        _uiManager.gameWin();
    }

    public void updateMoney(int bonus)
    {
        money += bonus;
        _uiManager.updateMoney(money);
    }

    public void setGameState(GameState gameState)
    {
        this.gameState = gameState;
    }

    public bool addPlayer(int number)
    {
        if (money >= GameSetting.instance.costBuyPlayer)
        {
            updateMoney(-GameSetting.instance.costBuyPlayer);
            listPlayer[number].SetActive(true);
            return true;
        }
        return false;
    }

    public void upgradeArrow()
    {
        if (_genManager.levelArrow < _genManager.listArrowUpgrade.Length - 1)
            if (money >= _genManager.listArrowUpgrade[_genManager.levelArrow + 1].cost)
            {
                updateMoney(-_genManager.listArrowUpgrade[_genManager.levelArrow + 1].cost);
                _genManager.upgradeArrow();
            }
    }

    public void setSkillState(SkillState skill)
    {
        if (skillState == SkillState.none)
            skillState = skill;
    }
}
