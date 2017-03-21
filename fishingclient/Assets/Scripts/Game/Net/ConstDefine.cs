using UnityEngine;
using System.Collections;
namespace Fishing.Net
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
        public static readonly string REQUEST_SELECTMODE = "gameFrame.tableHandler.reqSelectMode";
        public static readonly string REQUEST_SITDOWN = "gameFrame.tableHandler.reqSitDown";
        public static readonly string REQUEST_QUICKSTART = "gameFrame.tableHandler.reqQuickStart";
        public static readonly string REQUEST_TOUCHFISH = "gameFrame.gameHandler.reqTouchFish";
        public static readonly string REQUEST_FISHRESULT = "gameFrame.gameHandler.reqFishResult";
        public static readonly string REQUEST_SHOOT = "gameFrame.gameHandler.reqShoot";
        public static readonly string REQUEST_CHANGEHOOK = "gameFrame.gameHandler.reqChangeHook";
        public static readonly string REQUEST_LEAVETABLE = "gameFrame.tableHandler.reqLeaveTable";
        public static readonly string REQUEST_GETPAGETABLES = "gameFrame.tableHandler.reqGetPageTables";
        public static readonly string REQUEST_CHAT = "gameFrame.chatHandler.reqChat";
    }
    public class NotifyMsg
    {
        //server to client
        public static readonly string ON_UPDATEMAIL = "onUpdateMail";
        public static readonly string ON_CHAT = "onChat";
        //public static readonly string ON_FISHINFO = "";
        public static readonly string ON_ADDFISH = "onAddFish";
        public static readonly string ON_SYNCTIME = "onSyncTime";
        public static readonly string ON_FISHSTATUS = "onFishStatus";
        public static readonly string ON_NEWENTER = "onNewEnter";
        public static readonly string ON_PLAYERQUIT = "onPlayerQuit";
        public static readonly string ON_SHOOT = "onShoot";
        public static readonly string ON_TOUCHFISH = "onTouchFish";
        public static readonly string ON_FISHRESULT = "onFishResult";
        public static readonly string ON_CHANGEHOOK = "onChangeHook";
        public static readonly string ON_CHANGESCENE = "onChangeScene";
        public static readonly string ON_BACKSCORE = "onBackScore";
        //client to server
        public static readonly string NTF_LOADCOMPLETE = "gameFrame.tableHandler.ntfLoadComplete";
        public static readonly string NTF_BACKSCORE = "gameFrame.gameHandler.ntfBackScore";
    }
}
