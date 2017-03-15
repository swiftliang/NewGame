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
	// Use this for initialization
	void Start () {
        inputPwd.inputType = InputField.InputType.Password;
        btnLogin.onClick.AddListener(this.doLogin);
        btnRegister.onClick.AddListener(this.doRegister);
	}

    void doLogin()
    {

    }

    void doRegister()
    {

    }
}
