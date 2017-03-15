using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegisterWindow : MonoBehaviour {

    public InputField inputName;
    public InputField inputPwd;
    public InputField inputPwd2;
    public Button btnRegister;
    public Button btnBack;

    public Transform LoginWindow;
	// Use this for initialization
	void Start () {
        inputPwd.inputType = InputField.InputType.Password;
        inputPwd2.inputType = InputField.InputType.Password;
        btnRegister.onClick.AddListener(this.doRegister);
        btnBack.onClick.AddListener(this.doBack);
	}

    void doRegister()
    {

    }

    void doBack()
    {
        LoginWindow.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
