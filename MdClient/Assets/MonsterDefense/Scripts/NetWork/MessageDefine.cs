using UnityEngine;
using System.Collections.Generic;
namespace NW
{
    public class MessageBase
    {
        public Constants code;
    }
    public class LoginInfo : MessageBase
    {
        public int uid;
        public string token;
        public string gateHost;
        public int gatePort;
    }

    public class GInfoDetail
    {
        public int coin;
        public string levels;
        public string skills;
    }
    public class GameInfo : MessageBase
    {
        public GInfoDetail gameInfo;
    }
}