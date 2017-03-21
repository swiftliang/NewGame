using UnityEngine;
using System.Collections;
using Pomelo.DotNetClient;
using System;
using Fishing.Net;
using SimpleJson;
using JsonFx.Json;

namespace Fishing
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

    public class RoleInfo
    {
        public int roleID = 1;
        public int faceID = 0;
        public string nickNmae = "jijicheng";
        public string sex = "male";
        public string birthday = "20880808";
        public string address = "火星";
    }

    public class NetWorkManager : SingletonBehaviour<NetWorkManager>
    {
        public const int MAX_RECONNECT_TIMES = 5;

        /// 客户端连接异常断开，或者被服务器主动断开
        public event Action<string> OnDisconnected;

        public event Action OnReconnectStart;
        public event Action<bool> OnReconnectResult;

        private Connection _connection;
        private bool _bPause = false;

        [Header("SERVER INFO")]
        public string ServerIP;
        public int Port;

        private string _strUserName;
        private string _strPwd;
        private string _Language;

        public LoginState loginState { get; protected set; }

        private Coroutine __loginCoroutine;
        private Coroutine __reconnectCoroutine;

        public bool isSvrInfoGot { get { return _gameSvrIP != null && _gameSvrPort != 0; } }

        protected string _gameSvrIP;
        protected int _gameSvrPort;

        protected override void Init()
        {
            base.Init();
            loginState = LoginState.Offline;
            //Hide by Xhj temporarily
            //GetUserinfoFromHall();
            _InitPomeloClient();
        }

        private void GetUserinfoFromHall()
        {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        this._strUserName = StartWithFile._instence.game_user;
        this._strPwd = StartWithFile._instence.game_pwd;
        this._gameSvrIP = StartWithFile._instence.game_ip;
        this._Language = StartWithFile._instence.game_lang;
        this._gameSvrPort = int.Parse(StartWithFile._instence.game_port);
#endif
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        this._strUserName = jo.Get<string>("mUserId");
        this._strPwd = jo.Get<string>("mPwd");
        this._gameSvrIP = jo.Get<string>("mIp");
        if(jo.Get<int>("mLanguage") == 1)
            this._Language = "en";
        else
            this._Language = "zh";
        //this._gameSvrPort = 10015;
#endif
#if UNITY_IOS && !UNITY_EDITOR
        this._strUserName = IOSGameStart.GetSingleton().GetPassedUserName();
        this._strPwd =     IOSGameStart.GetSingleton().GetPassedPwd();
        this._gameSvrIP = IOSGameStart.GetSingleton().GetPassedIp();
        //Constants.LANG = IOSGameStart.GetSingleton().GetPassedLan();
        string lang = IOSGameStart.GetSingleton().GetPassedLan();
        if (lang == "1")
            this._Language = "en";
        else
            this._Language = "zh";
        //this._gameSvrPort = 10015;
#endif
        }

        public void AddGameEvent(string strEvent, Action<Message> handler)
        {
            if (loginState == LoginState.Logined)
            {
                _connection.ListenTo(strEvent, handler);
            }
        }

        public void RemoveEvent(string strEvent)
        {
            _connection.RemoveEventListeners(strEvent);
        }

        #region Others
        public bool Pause
        {
            get
            {
                return this._bPause;
            }
            set
            {
                this._bPause = value;
            }
        }

        public void Login(string strName, string pwd, int regionId, Action<Constants, PlayerData> callback)
        {
            if (__loginCoroutine != null)
            {
                if (callback != null) callback(Constants.ALREADY_IN_LOGINING, null);
                return;
            }

            __loginCoroutine = StartCoroutine(_doLogin(strName, pwd, regionId, null, callback));
        }

        /// 获取游戏服务器IP和Port
        public void GetGameSvrInfo(Action<Constants> callback)
        {
            //Debug.Assert(loginState == LoginState.Offline);

            loginState = LoginState.GettingSvrInfo;

            _connection.Disconnect();

            _gameSvrIP = null;
            _gameSvrPort = -1;

            _connection.ListenTo(Connection.DisconnectEvent, msg =>
            {
                Debug.logger.Log("Get gameSvr info from gate failed! Reason: ", msg.jsonObj["reason"]);
                if (callback != null) callback(Constants.NETWORK_ERROR);
            });

            StartCoroutine(_doGetGameSvrInfo(callback));
        }

        protected IEnumerator _doLogin(string strName, string pwd, int regionID
                                                    , RoleInfo createRoleInfo, Action<Constants, PlayerData> callback)
        {
            Constants lastErrCode = Constants.INVALID;
            PlayerData playerData = null;
            GetGameSvrInfo(retCode => { lastErrCode = retCode; });

            yield return new WaitUntil(() => lastErrCode != Constants.INVALID);
            if (lastErrCode != Constants.SUCCESS)
            {
                if (callback != null) callback(lastErrCode, playerData);
                __loginCoroutine = null;
                loginState = LoginState.Offline;
                yield break;
            }

            ///------
            lastErrCode = Constants.INVALID;
            LoginToGameSvr(strName, pwd, (retCode, pData) => { lastErrCode = retCode; playerData = pData; });

            yield return new WaitUntil(() => lastErrCode != Constants.INVALID);
            if (lastErrCode != Constants.SUCCESS)
            {
                if (callback != null) callback(lastErrCode, playerData);
                __loginCoroutine = null;
                loginState = LoginState.Offline;
                yield break;
            }

            yield return new WaitUntil(() => lastErrCode != Constants.INVALID);
            if (callback != null) callback(lastErrCode, playerData);

            __loginCoroutine = null;
        }

        public void LoginToGameSvr(string strName, string pwd, Action<Constants, PlayerData> callback)
        {
            Debug.Assert(isSvrInfoGot);

            loginState = LoginState.Logining;

            _connection.Disconnect();
            _connection.RemoveEventListeners(Connection.DisconnectEvent);

            StartCoroutine(_doLoginToGameSvr(strName, pwd, callback));
        }

        protected IEnumerator _doLoginToGameSvr(string strName, string pwd, Action<Constants, PlayerData> callback)
        {
            bool bIsDone = false;
            Message retMsg = null;
            _connection.Connect(_gameSvrIP, _gameSvrPort, retMsg_ =>
            {
                bIsDone = true;
                retMsg = retMsg_;
            });

            yield return new WaitUntil(() => bIsDone);
            if (retMsg.id != Connection.SYS_MSG_CONNECTED)
            {
                var reason = (string)retMsg.jsonObj["reason"];
                Debug.Log(reason);
                if (callback != null) callback(Constants.NETWORK_ERROR, null);
                yield break;
            }

            bIsDone = false;
            _connection.Handshake(new JsonObject(), data => { bIsDone = true; });

            yield return new WaitUntil(() => bIsDone);
            bIsDone = false;

            JsonObject msg = new JsonObject();
            msg["userName"] = strName;
            msg["password"] = pwd;
            Message message = null;
            //"MessageEnterGame
            _connection.Request(RequestMsg.REQUEST_ENTER, msg, retMsg_ =>
            {
                bIsDone = true;
                message = retMsg_;

            });

            yield return new WaitUntil(() => bIsDone);
            bIsDone = false;

            MessageEnterGame result = JsonReader.Deserialize<MessageEnterGame>(message.rawString);

            //Constants code = (Constants)Enum.Parse(typeof(Constants), message.jsonObj["code"].ToString());

            if (result.code == Constants.SUCCESS)
            {
                loginState = LoginState.Logined;

                _connection.RemoveEventListeners(Connection.DisconnectEvent);
                _connection.ListenTo(Connection.DisconnectEvent, _onDisconnect);
                this._strUserName = strName;
                this._strPwd = pwd;
            }

            if (callback != null) callback(result.code, result.playerData);
        }

        ///处理网路断开事件
        private void _onDisconnect(Message retMsg)
        {
            if (__reconnectCoroutine != null)
            {
                /// 在重连过程中，又发生了断开连接（因为整个登录过程包含多个步骤，这是完全有可能的）
                StopCoroutine(__reconnectCoroutine);
                __reconnectCoroutine = null;
            }

            Disconnect();

            var reason = (string)retMsg.jsonObj["reason"];
            DebugLogger.Log(eLogType.LT_NET, "<color=red>Connection down, reason: </color>" + reason);

            if (OnDisconnected != null) OnDisconnected(reason);

            __reconnectCoroutine = StartCoroutine(_doReconnect());
        }

        private IEnumerator _doReconnect()
        {
            if (OnReconnectStart != null) OnReconnectStart();
            Debug.Log("<color=yellow>Reconnecting to server...</color>");

            loginState = LoginState.Logining;
            Constants lastErrCode = Constants.INVALID;
            PlayerData pd = null;

            for (int i = 0; i < MAX_RECONNECT_TIMES; ++i)
            {
                LoginToGameSvr(this._strUserName, this._strPwd, (retCode, playerData) => {
                    lastErrCode = retCode;
                    pd = playerData;
                });

                yield return new WaitUntil(() => lastErrCode != Constants.INVALID);
                if (lastErrCode != Constants.SUCCESS)
                {
                    loginState = LoginState.Offline;
                    Debug.Log("<color=red>Reconnect failed, times: </color>" + (i + 1));
                }
                else
                {
                    loginState = LoginState.Logined;
                    bool bIsDone = false;
                    Game.Instance.player.Update(pd);
                    //levelData.UpdateLevelDatas(retCode => { bIsDone = true; });

                    yield return new WaitUntil(() => bIsDone);

                    Debug.Log("<color=green>Reconnect successful!</color>");
                    break;
                }

                if (i != MAX_RECONNECT_TIMES - 1) yield return new WaitForSeconds(i * 2);
            }

            if (loginState != LoginState.Logined) Debug.Log("<color=red>Reconnect failed with 5 times!</color>");
            if (OnReconnectResult != null) OnReconnectResult(lastErrCode == Constants.SUCCESS ? true : false);

            __reconnectCoroutine = null;
        }
        protected IEnumerator _doGetGameSvrInfo(Action<Constants> callback)
        {
            bool bIsDone = false;
            Message retMsg = null;
            _connection.Connect(ServerIP, Port, retMsg_ =>
            {
                bIsDone = true;
                retMsg = retMsg_;
            });

            yield return new WaitUntil(() => bIsDone);
            if (retMsg.id != Connection.SYS_MSG_CONNECTED)
            {
                var reason = (string)retMsg.jsonObj["reason"];
                DebugLogger.Log(eLogType.LT_NET, reason);
                if (callback != null) callback(Constants.NETWORK_ERROR);
            }

            bIsDone = false;
            _connection.Handshake(new JsonObject(), data => { bIsDone = true; });

            yield return new WaitUntil(() => bIsDone);
            bIsDone = false;

            bIsDone = false;
            JsonObject msg = new JsonObject();
            msg["uid"] = 1;
            Message message = null;
            _connection.Request(RequestMsg.REQUEST_CONNECTOR, msg, retMsg_ =>
            {
                bIsDone = true;
                message = retMsg_;
                _connection.RemoveAllEventListeners();
            });

            yield return new WaitUntil(() => bIsDone);
            bIsDone = false;

            Constants code = (Constants)Enum.Parse(typeof(Constants), message.jsonObj["code"].ToString());

            if (code == Constants.SUCCESS)
            {
                _gameSvrIP = message.jsonObj["host"].ToString();
                _gameSvrPort = int.Parse(message.jsonObj["port"].ToString());

                DebugLogger.Log(eLogType.LT_NET, "Game server IP: " + _gameSvrIP + ", Port: " + _gameSvrPort);

                loginState = LoginState.SvrInfoGot;
            }
            else loginState = LoginState.Validated;

            if (callback != null) callback(code);
            _connection.Disconnect();
        }

        #endregion Others

        #region Requests
        //public void reqSelectMode(string strMode, Action<SelectModeReturn> callback)
        //{
        //    JsonObject msg = new JsonObject();
        //    msg["mode"] = strMode;
        //    _connection.Request(RequestMsg.REQUEST_SELECTMODE, msg, retMsg_ =>
        //    {
        //        SelectModeReturn retMsg = JsonReader.Deserialize<SelectModeReturn>(retMsg_.rawString);
        //        if(callback != null)
        //        {
        //            callback(retMsg);
        //        }
        //    });
        //}

        public void reqGetPageTables(int nPage, Action<MessageGetPageTables> callback)
        {
            JsonObject msg = new JsonObject();
            msg["page"] = nPage;
            _connection.Request(RequestMsg.REQUEST_GETPAGETABLES, msg, retMsg_ =>
            {
                MessageGetPageTables retMsg = JsonReader.Deserialize<MessageGetPageTables>(retMsg_.rawString);
                if (callback != null)
                {
                    callback(retMsg);
                }
            });
        }

        public void reqSitDown(string strMode, int tableId, string strSit, Action<MessageAboutSeat> callback)
        {
            JsonObject msg = new JsonObject();
            msg["mode"] = strMode;
            msg["tableId"] = tableId;
            msg["chairId"] = strSit;
            _connection.Request(RequestMsg.REQUEST_SITDOWN, msg, retMsg_ =>
            {
                MessageAboutSeat result = JsonReader.Deserialize<MessageAboutSeat>(retMsg_.rawString);
                if (callback != null)
                {
                    callback(result);
                }
            });
        }

        public void reqQuickStart(Action<MessageAboutSeat> callback)
        {
            JsonObject msg = new JsonObject();
            _connection.Request(RequestMsg.REQUEST_QUICKSTART, msg, retMsg_ =>
            {
                MessageAboutSeat result = JsonReader.Deserialize<MessageAboutSeat>(retMsg_.rawString);
                if (callback != null)
                {
                    callback(result);
                }
            });
        }

        public void reqChat(string strMessage, Action<Constants> callback)
        {
            JsonObject msg = new JsonObject();
            msg["type"] = 2;
            msg["message"] = strMessage;
            _connection.Request(RequestMsg.REQUEST_CHAT, msg, retMsg_ =>
            {
                MessageBase result = JsonReader.Deserialize<MessageBase>(retMsg_.rawString);
                if (callback != null)
                {
                    callback(result.code);
                }
            });
        }

        public void reqTouchFish(int nFishId, Action<Constants> callback)
        {
            JsonObject msg = new JsonObject();
            msg["fishId"] = nFishId;
            _connection.Request(RequestMsg.REQUEST_TOUCHFISH, msg, retMsg_ =>
            {
                MessageBase result = JsonReader.Deserialize<MessageBase>(retMsg_.rawString);
                if (callback != null)
                {
                    callback(result.code);
                }
            });
        }

        public void reqFishResult(int nFishId, bool bIsCatch, Action<Constants> callback)
        {
            JsonObject msg = new JsonObject();
            msg["fishId"] = nFishId;
            msg["isCatch"] = bIsCatch;
            _connection.Request(RequestMsg.REQUEST_FISHRESULT, msg, retMsg_ =>
            {
                MessageBase result = JsonReader.Deserialize<MessageBase>(retMsg_.rawString);
                if (callback != null)
                {
                    callback(result.code);
                }
            });
        }

        public void reqShoot(int nAngle, Action<Constants> callback)
        {
            JsonObject msg = new JsonObject();
            msg["angle"] = nAngle;
            _connection.Request(RequestMsg.REQUEST_SHOOT, msg, retMsg_ =>
            {
                MessageBase result = JsonReader.Deserialize<MessageBase>(retMsg_.rawString);
                if (callback != null)
                {
                    callback(result.code);
                }
            });
        }

        public void reqChangeHook(int nHook, Action<Constants> callback)
        {
            JsonObject msg = new JsonObject();
            msg["hook"] = nHook;
            _connection.Request(RequestMsg.REQUEST_CHANGEHOOK, msg, retMsg_ =>
            {
                MessageBase result = JsonReader.Deserialize<MessageBase>(retMsg_.rawString);
                if (callback != null)
                {
                    callback(result.code);
                }
            });
        }

        public void reqLeaveTable(int tableId, Action<MessageLeaveTable> callback)
        {
            JsonObject msg = new JsonObject();
            msg["tableId"] = tableId;
            _connection.Request(RequestMsg.REQUEST_LEAVETABLE, msg, retMsg_ =>
            {
                MessageLeaveTable result = JsonReader.Deserialize<MessageLeaveTable>(retMsg_.rawString);
                if (callback != null)
                {
                    callback(result);
                }
            });
        }

        #endregion Requests

        #region PomeloClient
        void _InitPomeloClient()
        {
            //init connetion
            _connection = new Connection();
            //init notify
            _connection.ListenTo(NotifyMsg.ON_UPDATEMAIL, _onUpdateEmail);
        }

        public void Connect(Action<Message> callback)
        {
            _connection.Connect(ServerIP, Port, callback);
        }

        public void Disconnect()
        {
            _connection.Disconnect();
        }

        public void LateUpdate()
        {
            if (!this.Pause)
                _connection.Update();
        }

        public void OnApplicationQuit()
        {
            _connection.Disconnect();
        }

        #endregion PomeloClient

        #region Notifys
        private void _onUpdateEmail(Message msg)
        {

        }

        public void ntfLoadComplete()
        {
            //_initGameNotifyListeners();
            _connection.Notify(NotifyMsg.NTF_LOADCOMPLETE, new JsonObject());
        }

        public void ntfBackScore()
        {
            _connection.Notify(NotifyMsg.NTF_BACKSCORE, new JsonObject());
        }

        #endregion Notifys
    }
}
