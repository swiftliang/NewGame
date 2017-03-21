using UnityEngine;
using System.Collections.Generic;
namespace Fishing.Net
{
    public class MessageBase
    {
        public Constants code;
    }
    //-----------------------------------------------
    public class PlayerInSeatInfo
    {
        public int uid;
        public int photoId;
        public string nickName;
        public int level;
    }
    public class TableInfo
    {
        public int tableId;
        public PlayerInSeatInfo left;
        public PlayerInSeatInfo right;
        public int rate;
        public string tableName;
        public int minGold;
        public int sceneId;
    }
    //public class SelectModeReturn : MessageBase
    //{
    //    public List<TableInfo> tables;
    //}
    public class MessageGetPageTables : MessageBase
    {
        public int page;
        public List<TableInfo> pageData;
        public int tableNum;
    }
    //-----------------------------------------------
    public class FishInfo
    {
        public int fishId;
        public int pathId;
        public int fishType;
        public int expireTime;
        public int createTime;
        public int escapePlace;
        public int lottery;
    }
    public class MessageFishInfo : MessageBase
    {
        public List<FishInfo> fishList;
    }
    public class MessageFishStatus : MessageBase
    {
        public int currentTime;
        public List<FishInfo> fishList;
    }
    //-----------------------------------------------
    public class MessageSycTime
    {
        public int currentTime;
    }
    //-----------------------------------------------
    public class HookInfo
    {
        public int uid;
        public int enterTime;
        public string chairId;
        public int score;
        public int lottery;
        public int hook;
    }
    public class MessageSycHook
    {
        public List<HookInfo> playerList;
    }
    //-----------------------------------------------
    public class MessagePlayerInfo
    {
        public int uid;
        public string chairId;
    }
    //-----------------------------------------------
    public class MessageShootInfo
    {
        public int uid;
        public int angle;
        public int score;
    }
    //-----------------------------------------------
    public class MessageTouchInfo
    {
        public int uid;
        public int fishId;
    }
    //-----------------------------------------------
    public class MessageFishResult
    {
        public int uid;
        public int fishId;
        public bool isCatch;
        public int lottery;
    }
    //-----------------------------------------------
    public class MessageChangeHook
    {
        public int hook;
    }
    //-----------------------------------------------
    public class MessageEnterGame : MessageBase
    {
        public PlayerData playerData;
    }
    public class PlayerData
    {
        public int uid;
        public string nickName;
        public int photoId;
        public int level;
        public int gold;
        public int score;
        public int lottery;
    }
    //-----------------------------------------------
    public class MessageAboutSeat : MessageBase
    {
        public TableInfo tableInfo;
        public UpdatePlayerData updatePlayer;
        public List<TableInfo> pageData;
    }
    public class UpdatePlayerData
    {
        public int gold;
        public int score;
    }
    //-----------------------------------------------
    public class MessageLeaveTable : MessageBase
    {
        public UpdatePlayerData updatePlayer;
        public int page;
        public List<TableInfo> pageData;
        public int tableNum;
    }
    //-----------------------------------------------
    public class MessageBackScore
    {
        public int uid;
        public int score;
    }
    //-----------------------------------------------
    public class MessageChat
    {
        public int uid;
        public string nickName;
        public int type;
        public string message;
    }
    //-----------------------------------------------
    public class MessageChangeScene
    {
        public int sceneId;
    }
}
