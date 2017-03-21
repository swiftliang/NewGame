using SimpleJson;
using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

namespace Pomelo.DotNetClient
{
    /// <summary>
    /// network state enum
    /// </summary>
    public enum NetWorkState
    {
        [Description("connecting server")]
        CONNECTING,

        [Description("server connected")]
        CONNECTED,

        [Description("disconnected with server")]
        DISCONNECTED,
    }

    public class Connection
    {
        static public string DisconnectEvent = "Disconnect";
        static public string ErrorEvent = "Error";

        static public uint SYS_MSG_CONNECTED = 1;
        static public uint SYS_MSG_CONNECT_FAILED = 2;

        public event Action<uint> OnRPCRequest;
        public event Action<uint> OnRPCReturn;

        private Queue<Message> __receiveMsgQueue;

        //current network state
        public NetWorkState netWorkState { get; protected set; }

        private EventManager _eventManager;
        private Socket _socket;
        private Protocol _protocol;
        private uint _reqId = 100;
        private JsonObject __emptyMsg = new JsonObject();

        public Connection()
        {
            netWorkState = NetWorkState.DISCONNECTED;
            _eventManager = new EventManager();

            __receiveMsgQueue = new Queue<Message>();
        }

        /// 初始化连接
        public void Connect(string host, int port, Action<Message> callback)
        {
            Assert(netWorkState == NetWorkState.DISCONNECTED);

            UnityEngine.Debug.Log("Connect to " + host + " with port " + port);

            netWorkState = NetWorkState.CONNECTING;

            IPAddress ipAddress = null;

            try
            {
                if (!IPAddress.TryParse(host, out ipAddress))
                {
                    IPAddress[] addresses = Dns.GetHostEntry(host).AddressList;
                    foreach (var item in addresses)
                    {
                        if (item.AddressFamily == AddressFamily.InterNetwork)
                        {
                            ipAddress = item;
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _onDisconnect(e.Message);
                return;
            }

            if (ipAddress == null) throw new Exception("Cannot parse host : " + host);

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ie = new IPEndPoint(ipAddress, port);

            _eventManager.RemoveCallback(SYS_MSG_CONNECTED);
            _eventManager.RemoveCallback(SYS_MSG_CONNECT_FAILED);

            _eventManager.AddCallback(SYS_MSG_CONNECTED, callback);
            _eventManager.AddCallback(SYS_MSG_CONNECT_FAILED, callback);
            
            _socket.BeginConnect(ie, _onConnectCallback, this._socket);
        }

        public void Handshake()
        {
            Handshake(null, null);
        }

        public void Handshake(JsonObject user)
        {
            Handshake(user, null);
        }

        public void Handshake(Action<JsonObject> handshakeCallback)
        {
            Handshake(null, handshakeCallback);
        }

        public bool Handshake(JsonObject user, Action<JsonObject> handshakeCallback)
        {
            try
            {
                _protocol.start(user, handshakeCallback);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }

        public void Request(string route, Action<Message> action)
        {
            if (netWorkState != NetWorkState.CONNECTED)
            {
                _onError("Network is down, cannot send request now!");
                return;
            }

            this.Request(route, __emptyMsg, action);
        }

        public void Request(string route, JsonObject msg, Action<Message> action)
        {
            if(netWorkState != NetWorkState.CONNECTED)
            {
                _onError("Network is down, cannot send request now!");
                return;
            }

            UnityEngine.Debug.Log("<color=magenta>>>> Send: </color>" + route + " data: " + msg.ToString());

            this._eventManager.AddCallback(_reqId, action);
            _protocol.send(route, _reqId, msg);

            if (OnRPCRequest != null) OnRPCRequest(_reqId);

            _reqId++;
            if(_reqId >= uint.MaxValue) _reqId = 100;
        }

        public void Notify(string route, JsonObject msg)
        {
            if (netWorkState != NetWorkState.CONNECTED)
            {
                _onError("Network is down, cannot send request now!");
                return;
            }

            _protocol.send(route, msg);
        }

        public void ListenTo(string eventName, Action<Message> action)
        {
            _eventManager.AddOnEvent(eventName, action);
        }

        public void RemoveListener(string eventName, Action<Message> action)
        {
            _eventManager.RemoveOnEvent(eventName, action);
        }

        public void RemoveEventListeners(string eventName)
        {
            _eventManager.RemoveOnEvent(eventName);
        }

        public void RemoveAllEventListeners()
        {
            _eventManager.ClearEventMap();
        }

        public void Disconnect()
        {
            if (netWorkState == NetWorkState.DISCONNECTED) return;

            /// Force update to make sure all received messages been dispatched.
            Update();

            // free managed resources
            if (_protocol != null) _protocol.close();

            try
            {
                this._socket.Shutdown(SocketShutdown.Both);
                this._socket.Close();
                this._socket = null;
            }
            catch (Exception e)
            {
                _onError(e.Message);
            }

            netWorkState = NetWorkState.DISCONNECTED;

            _eventManager.ClearCallBackMap();
            _eventManager.ClearCallBackMap();

            _reqId = 100;
        }

        public void Update()
        {
            while(__receiveMsgQueue.Count != 0)
            {
                var msg = __receiveMsgQueue.Dequeue();

                if (msg.type == MessageType.MSG_RESPONSE)
                {
                    UnityEngine.Debug.Log("<color=green><<< Receive: </color>" + msg.route + " data: " + msg.rawString);

                    ///UnityEngine.Debug.Assert(_eventManager._GetCallbackCount() != 0);

                    if (OnRPCReturn != null) OnRPCReturn(msg.id);

                    _eventManager.InvokeCallBack(msg.id, msg);
                    _eventManager.RemoveCallback(msg.id);
                }
                else if (msg.type == MessageType.MSG_PUSH)
                {
                    UnityEngine.Debug.Log("<color=blue><<< Receive event:</color> " + msg.route + " data: " + msg.rawString);
                    _eventManager.InvokeOnEvent(msg.route, msg);
                }
                else if(msg.type == MessageType.MSG_SYS)
                {
                    if (msg.id != 0)
                    {
                        _eventManager.InvokeCallBack(msg.id, msg);
                        _eventManager.RemoveCallback(msg.id);
                    }
                    else
                    {
                        _eventManager.InvokeOnEvent(msg.route, msg);
                    }
                }
            }
        }
        
        ///--------------------
        protected void _onConnectCallback(IAsyncResult result)
        {
            try
            {
                this._socket.EndConnect(result);
                netWorkState = NetWorkState.CONNECTED;

                this._protocol = new Protocol(this, this._socket);

                Message msg = new Message(MessageType.MSG_SYS, SYS_MSG_CONNECTED);
                __receiveMsgQueue.Enqueue(msg);
            }
            catch (SocketException e)
            {
                Message msg = new Message(MessageType.MSG_SYS, SYS_MSG_CONNECT_FAILED);
                msg.jsonObj = new JsonObject();
                msg.jsonObj.Add("reason", e.Message);

                __receiveMsgQueue.Enqueue(msg);

                netWorkState = NetWorkState.DISCONNECTED;

                _socket.Close();
                _socket = null;
            }
        }

        protected void Assert(bool bOperation, string msg = "")
        {
            if (!bOperation)
            {
                throw new Exception(msg);
            }
        }

        internal void _onError(string reason)
        {
            JsonObject jsonObj = new JsonObject();
            jsonObj.Add("reason", reason);
            Message msg = new Message(MessageType.MSG_SYS, ErrorEvent, jsonObj);
            __receiveMsgQueue.Enqueue(msg);
        }

        internal void _onDisconnect(string reason)
        {
            netWorkState = NetWorkState.DISCONNECTED;

            JsonObject jsonObj = new JsonObject();
            jsonObj.Add("reason", reason);
            Message msg = new Message(MessageType.MSG_SYS, DisconnectEvent, jsonObj);
            __receiveMsgQueue.Enqueue(msg);

            _socket.Close();
            _socket = null;
        }

        internal void _processMessage(Message msg)
        {
            __receiveMsgQueue.Enqueue(msg);
        }

    }
}