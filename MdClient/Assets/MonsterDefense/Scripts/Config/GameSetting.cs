﻿using UnityEngine;
using System.Collections;

public class GameSetting : MonoBehaviour {

    //tips
    public const string PWD_ERROR = "用户名或密码错误";
    public const string ACCOUNT_ERROR = "用户名和密码不能为空";
    public const string REGISTER_OK = "注册成功请返回登录界面登录";
    public const string REGISTER_PWDERROR = "输入密码不匹配";

    public const string GAME_SCENE = "GamePlay";
    public const string MAP_SCENE = "Map";
    public const string MAIN_SCENE = "MainMenu";
    public const string LOGIN_SCENE = "Login";

    public const string LOCK_KEY = "key";
    public const string STAR_KEY = "star";
    public const string FIRST_GAME_CHECK = "checkFirst";
    public const string MONEY_KEY = "coin";
    public const string SOUND_KEY = "sound";
    public const string SFX_KEY = "sfx";
    public const int FALSE_RESULT = 0;
    public const int TRUE_RESULT = 1;

    public static GameSetting instance;

    public static Vector2 sizeCam;
    public static Vector2 positionCam;

    public static GameManager _gameManger;
    public static UIManager _uiManager;
    public static Tower _tower;
    public static GenManager _genManager;

    public ArrowUpgrade[] listArrowUpgrade;
    public Sprite[] iconArrowUpgrade;
    public GameObject[] listPlayer;
    public int costBuyPlayer;

    void Awake()
    {
        instance = this;
        sizeCam = new Vector2(2f * Camera.main.aspect * Camera.main.orthographicSize, 2f * Camera.main.orthographicSize);
        positionCam = Camera.main.transform.position;
        _gameManger = FindObjectOfType<GameManager>();
        _uiManager = FindObjectOfType<UIManager>();
        _tower = FindObjectOfType<Tower>();
        _genManager = FindObjectOfType<GenManager>();
    }

    public void setMoneyPre (int coin) {
        PlayerPrefs.SetInt(MONEY_KEY, coin);
	}

    public int getMoneyPre()
    {
        return PlayerPrefs.GetInt(MONEY_KEY);
    }

   
}
