using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Fishing;
using Fishing.GameState;
using Fishing.Net;
using System;
namespace Fishing.UI
{
    public class LoginWindow : Window
    {
        public event Action<bool> loginResultEvent;
        //private Button _btnQuickLogin;
        //private Button _btnDeleteAccount;
        private InputField _inputAccount;
        public override void Show(Fishing.ArgList args)
        {
            base.Show(args);
            var btnQuickLogin = transform.FindChild("btnQuickLogin").GetComponent<Button>();
            btnQuickLogin.onClick.AddListener(this.OnQuickLogin);
            var btnDeleteAccount = transform.FindChild("btnDeleteAccount").GetComponent<Button>();
            btnDeleteAccount.onClick.AddListener(this.OnDeleteAccount);
            _inputAccount = transform.FindChild("InputAccount").GetComponent<InputField>();
        }
        void OnQuickLogin()
        {
            if (_inputAccount.text != null && _inputAccount.text.Length > 0)
                NetWorkManager.Instance.Login(_inputAccount.text, "123456", 0, LoginResult);
            else
                Utils.ShowMessageBox(2004);
        }
        void LoginResult(Constants result, PlayerData playerData)
        {
            if (result == Constants.SUCCESS)
            {
                Game.Instance.player = new Player(playerData);
                Game.Instance.GameFsm.ChangeState<Hall>(ArgList.Create(0), false);
                //Trigger the event.
                _raiseLoginResultEvent(true);
            }
            else
            {
                //DebugLogger.LogError("login failed!!!!!!");
                //Trigger the event.
                _raiseLoginResultEvent(false);
            }
        }
        void OnDeleteAccount()
        {
        }
        private void _raiseLoginResultEvent(bool bFlag)
        {
            if (loginResultEvent != null)
                loginResultEvent(bFlag);
        }
    }
}
