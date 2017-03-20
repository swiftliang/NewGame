using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJson;
using Pomelo.DotNetClient;
using System;
using UnityEngine.SceneManagement;
//using JsonFx.Json;
using Pathfinding.Serialization.JsonFx;

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

        private PomeloClient _GameSvrConnection = new PomeloClient();
        private PomeloClient _GateClientConnection = new PomeloClient();
        protected LoginInfo _loginInfo;
        //protected GameSvrInfo _gsInfo;
        protected string _gameSvrIP;
        protected int _gameSvrPort;

        public string strLoginUrl;

        public LoginState loginState { get; protected set; }

        private Coroutine __loginCoroutine;

        private void Start()
        {
            SceneManager.LoadSceneAsync("Login");
        }

        public void Login(string strName, string strPwd, Action<GameInfo> cb)
        {
            if (__loginCoroutine != null)
            {
                //if (cb != null) cb(Constants.ALREADY_IN_LOGINING);
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

        private IEnumerator _doLogin(string strName, string strPwd, Action<GameInfo> cb)
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
                    //cb(result);
                    GameInfo ginfo = JsonReader.Deserialize<GameInfo>(result.ToString());
                    cb(ginfo);
                    __loginCoroutine = null;
                    loginState = LoginState.Offline;
                    yield break;
                }
            }

            result = Constants.INVALID;
            GetGameSvrInfo(code =>
            {
                result = code;
            });
            yield return new WaitUntil(() => result != Constants.INVALID);
            if (result != Constants.SUCCESS)
            {
                if (cb != null)
                {
                    //cb(result);
                    //GameInfo ginfo = JsonUtility.FromJson<GameInfo>(result.ToString());
                    GameInfo ginfo = JsonReader.Deserialize<GameInfo>(result.ToString());
                    cb(ginfo);
                    __loginCoroutine = null;
                    loginState = LoginState.Offline;
                    yield break;
                }
            }

            GameInfo gif;
            LoginGameServer(info =>
            {
                gif = info;
                if(gif.code != Constants.SUCCESS)
                {
                    __loginCoroutine = null;
                    loginState = LoginState.Offline;
                }
                else
                {
                    __loginCoroutine = null;
                    loginState = LoginState.Logined;
                }
                if(cb != null)
                {
                    cb(gif);
                }
            });
        }


        public void LoginGameServer(Action<GameInfo> cb)
        {
            _GameSvrConnection.disconnect();
            _GameSvrConnection.initClient(_gameSvrIP, _gameSvrPort, () =>
            {
                _GameSvrConnection.connect(null, data =>
                {
                    Debug.Log("connect connector data: " + data.ToString());
                    JsonObject msg = new JsonObject();
                    msg["uid"] = _loginInfo.uid;
                    msg["token"] = _loginInfo.token;
                    msg["game"] = "MDGame";
                    _GameSvrConnection.request(RequestMsg.REQUEST_ENTER, msg, result =>
                    {
                        if (cb != null)
                        {
                            Message retMsg = (Message)result;
                            GameInfo ginfo = JsonReader.Deserialize<GameInfo>(retMsg.rawString);
                            cb(ginfo);
                        }
                    });
                });
            });
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

            StartCoroutine(_doGetGsInfo(cb));
            /*_GameSvrConnection.on("Disconnect", msg =>
            {
            Debug.logger.Log("Get gameSvr info from gate failed! Reason: ", msg["reason"]);
            if (cb != null) cb(Constants.NETWORK_ERROR);
            });*/
        }

        private IEnumerator _doGetGsInfo(Action<Constants> cb)
        {
            _GateClientConnection.disconnect();

            _gameSvrIP = null;
            _gameSvrPort = -1;

            Constants code = Constants.INVALID;
            _GateClientConnection.initClient(_loginInfo.gateHost, _loginInfo.gatePort, () =>
            {
                _GateClientConnection.connect(null, (data) =>
                {
                    Debug.Log("connect gate data: " + data);
                    SimpleJson.JsonObject msg = new SimpleJson.JsonObject();
                    msg["uid"] = _loginInfo.uid;
                    //msg["token"] = _loginInfo.token;
                    _GateClientConnection.request(RequestMsg.REQUEST_CONNECTOR, msg, (result) =>
                    {
                       code = (Constants)Convert.ToInt32(result.data["code"]);
                        _gameSvrIP = (string)result.data["host"];
                        _gameSvrPort = Convert.ToInt32(result.data["port"]);
                    });
                });
            });

            yield return new WaitUntil(() => code != Constants.INVALID);
            if (cb != null)
                cb(code);
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

        public void Update()
        {
            _GateClientConnection.Update();
            _GameSvrConnection.Update();
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