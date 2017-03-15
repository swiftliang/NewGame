using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NW;

public class LoginWindow : MonoBehaviour {

    public InputField inputName;
    public InputField inputPwd;
    public Button btnLogin;
    public Button btnRegister;

    public Transform RegisterWindow;
	// Use this for initialization
	void Start () {
        inputPwd.inputType = InputField.InputType.Password;
        btnLogin.onClick.AddListener(this.doLogin);
        btnRegister.onClick.AddListener(this.doRegister);
	}

    void doLogin()
    {
        if (inputName.text.Length <= 0 || inputPwd.text.Length <= 0)
            return;
        NetWorkMgr.Instance.Login(inputName.text, inputPwd.text, (result) => {
            if(result == Constants.SUCCESS)
            {

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
