using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NW;
using UnityEngine.SceneManagement;

public class LoginWindow : MonoBehaviour {

    public InputField inputName;
    public InputField inputPwd;
    public Button btnLogin;
    public Button btnRegister;
    public Text txtTip;

    public Transform RegisterWindow;
	// Use this for initialization
	void Start () {
        inputPwd.inputType = InputField.InputType.Password;
        btnLogin.onClick.AddListener(this.doLogin);
        btnRegister.onClick.AddListener(this.doRegister);
        txtTip.gameObject.SetActive(false);
	}

    void doLogin()
    {
        if (inputName.text.Length <= 0 || inputPwd.text.Length <= 0)
            return;
        NetWorkMgr.Instance.Login(inputName.text, inputPwd.text, (result) => {
            if(result.code == Constants.SUCCESS)
            {
                GameData.Instance.SetGameData(result);
                SceneManager.LoadScene(GameSetting.MAIN_SCENE);
            }
            else
            {

            }
        });
    }

    void doRegister()
    {
        RegisterWindow.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
