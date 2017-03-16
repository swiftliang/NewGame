﻿using UnityEngine;
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

    public class GameInfo : MessageBase
    {
        public int coin;
        public string stars;
        public string skills;
    }
}