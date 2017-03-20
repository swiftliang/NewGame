using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ItemSkill : MonoBehaviour {

    public string nameSkill;
    public string keySave;
    public int cost;
    public Sprite icon;

    public Image iconSkill;
    public GameObject btBuy;
    public Text costText;
    public Text nameSkillText;

    private Map _map;
    void Start()
    {
        _map = FindObjectOfType<Map>();
    }

	void OnEnable () {
        setupItem();
	}
	
	void setupItem () {
        nameSkillText.text = nameSkill;
        costText.text = cost + " coin";
        iconSkill.sprite = icon;
        if (PlayerPrefs.GetInt(keySave) == GameSetting.TRUE_RESULT)
        {
            btBuy.SetActive(false);
        }
        else
        {
            btBuy.SetActive(true);
        }
    }

    public void buy()
    {
        int money = GameData.Instance.Coin;//PlayerPrefs.GetInt(GameSetting.MONEY_KEY);
        if (money > cost)
        {
            SoundManager.instance.playSoundBuy();
            money -= cost;
            PlayerPrefs.SetInt(GameSetting.MONEY_KEY, money);
            PlayerPrefs.SetInt(keySave, GameSetting.TRUE_RESULT);
            setupItem();
            _map.updateMoney();
        }
    }
}
