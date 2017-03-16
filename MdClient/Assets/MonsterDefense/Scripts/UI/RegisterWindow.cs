using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NW;

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
        if(inputName.text.Length <= 0 || inputPwd.text.Length <= 0 || inputPwd2.text.Length <=0)
        {
            return;
        }
        if (inputPwd.text != inputPwd2.text)
        {
            return;
        }
        NetWorkMgr.Instance.Register(inputName.text, inputPwd.text, code =>
        {
            Debug.Log("register result code: " + code);
            if(code == Constants.SUCCESS)
            {
                btnRegister.interactable = false;
            }
        });
    }

    void doBack()
    {
        LoginWindow.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
