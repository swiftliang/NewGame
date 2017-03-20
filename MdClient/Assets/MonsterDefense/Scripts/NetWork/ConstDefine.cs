using UnityEngine;
using System.Collections;
namespace NW
{
    public enum Constants
    {
        INVALID = 0,
        ALREADY_IN_LOGINING = -2,
        NETWORK_ERROR = -1,
        SUCCESS = 200,             /// 成功
        INVALID_DATA = 404,     ///      请求数据不正确
        SVR_ERROR = 500,      /// 失败（服务器内部错误）
        SIT_FULL = 1001         //座位已被占用
    }
    public class ConstDefine
    {
    }
    public class RequestMsg
    {
        public static readonly string REQUEST_CONNECTOR = "gate.gateHandler.queryEntry";
        public static readonly string REQUEST_ENTER = "connector.entryHandler.enter";
        public static readonly string REQUEST_UNLOCKLEV = "connector.entryHandler.UpdateStar";
        public static readonly string REQUEST_ADDCOIN = "connector.entryHandler.UpdateCoin";
    }
    public class NotifyMsg
    {
     
    }
}