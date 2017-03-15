using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJson;
using Pomelo.DotNetClient;
using System;
using UnityEngine.SceneManagement;

namespace NW
{
    public enum LoginState
    {
        Offline,

        Validating,
        Validated,

        GettingSvrInfo,
        SvrInfoGot,

        Logining,
        Logined,
    }

    public class NetWorkMgr : SingletonBehaviour<NetWorkMgr>
    {

        private PomeloClient _GameSvrConnection;
        private PomeloClient _GateClientConnection;
        protected LoginInfo _loginInfo;
        protected string _gameSvrIP;
        protected int _gameSvrPort;

        public string strLoginUrl;

        public LoginState loginState { get; protected set; }

        private Coroutine __loginCoroutine;

        private void Start()
        {
            SceneManager.LoadSceneAsync("Login");
        }

        public void Login(string strName, string strPwd, Action<Constants> cb)
        {
            if (__loginCoroutine != null)
            {
                if (cb != null) cb(Constants.ALREADY_IN_LOGINING);
                return;
            }

            __loginCoroutine = StartCoroutine(_doLogin(strName, strPwd, cb));
        }

        public void Register(string strName, string strPwd, Action<Constants> cb)
        {
            StartCoroutine(_doRegister(strName, strPwd, cb));
        }

        protected void GetLoginInfo(string strName, string strPwd, Action<Constants> cb)
        {
            loginState = LoginState.Validating;
            StartCoroutine(_doGetLoginInfo(strName, strPwd, cb));
        }

        private IEnumerator _doLogin(string strName, string strPwd, Action<Constants> cb)
        {
            //get logininfo
            Constants result = Constants.INVALID;
            GetLoginInfo(strName, strPwd, code =>
            {
                result = code;
            });

            yield return new WaitUntil(() => result != Constants.INVALID);
            if (result != Constants.SUCCESS)
            {
                if (cb != null)
                {
                    cb(result);
                    __loginCoroutine = null;
                    loginState = LoginState.Offline;
                    yield break;
                }
            }

            result = Constants.INVALID;
        }

        private IEnumerator _doRegister(string strName, string strPwd, Action<Constants> cb)
        {
            WWWForm form = new WWWForm();
            form.AddField("userName", strName);
            form.AddField("password", strPwd);
            form.AddField("deviceID", "deviceID");
            WWW www = new WWW(strLoginUrl + "/register", form);
            yield return www;

            if (www.error == null)
            {
                var ret = JsonUtility.FromJson<MessageBase>(www.text);
                if (cb != null) cb(ret.code);
            }
            else
            {
                if (cb != null) cb(Constants.NETWORK_ERROR);
                Debug.logger.Log(www.error);
            }
            yield break;
        }

        private void GetGameSvrInfo(Action<Constants> cb)
        {
            Debug.Assert(loginState == LoginState.Validated);

            loginState = LoginState.GettingSvrInfo;

            _GameSvrConnection.disconnect();

            _gameSvrIP = null;
            _gameSvrPort = -1;

            /*_GameSvrConnection.on("Disconnect", msg =>
            {
            Debug.logger.Log("Get gameSvr info from gate failed! Reason: ", msg["reason"]);
            if (cb != null) cb(Constants.NETWORK_ERROR);
            });*/
        }

        private IEnumerator _doGetLoginInfo(string strName, string strPwd, Action<Constants> cb)
        {
            WWWForm wf = new WWWForm();
            wf.AddField("userName", strName);
            wf.AddField("password", strPwd);
            wf.AddField("rid", 1);
            WWW www = new WWW(strLoginUrl + "/login", wf);
            yield return www;
            if (www.error == null)
            {
                _loginInfo = JsonUtility.FromJson<LoginInfo>(www.text);
                if (_loginInfo.code == Constants.SUCCESS)
                {
                    loginState = LoginState.Validated;
                    Debug.Log(_loginInfo.token);
                }
                else loginState = LoginState.Offline;

                if (cb != null) cb(_loginInfo.code);
            }
            else
            {
                Debug.Log(www.error);
                if (cb != null) cb(Constants.NETWORK_ERROR);
                loginState = LoginState.Offline;
            }
            yield break;
        }

        private void OnApplicationQuit()
        {
            if (_GameSvrConnection != null)
            {
                _GameSvrConnection.disconnect();
            }
        }
    }
}