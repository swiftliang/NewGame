using UnityEngine;
using System.Collections;
using Fishing.Net;

namespace Fishing
{
    public class Player
    {
        private int _uID;
        private int _nGold;
        private int _nScore;
        private int _nLottery;
        private string _strNickName;
        private int _nPhotoId;
        private int _nLastDeltaLottery;
        public int UID
        {
            get { return _uID; }
            set { _uID = value; }
        }
        public int Gold
        {
            get { return _nGold; }
            set { _nGold = value; }
        }
        public int Score
        {
            get { return _nScore; }
            set { _nScore = value; }
        }
        public int Lottery
        {
            get { return _nLottery; }
            set { _nLottery = value; }
        }
        public string NickName
        {
            get { return _strNickName; }
            set { _strNickName = value; }
        }
        public int LastDeltaLottery
        {
            get { return _nLastDeltaLottery; }
            set { _nLastDeltaLottery = value; }
        }
        public int PhotoId
        {
            get { return _nPhotoId; }
            set { _nPhotoId = value; }
        }
        public Player()
        {

        }
        public Player(PlayerData playerData)
        {
            this._uID = playerData.uid;
            this._nGold = playerData.gold;
            this._nScore = playerData.score;
            this._nLottery = playerData.lottery;
            this._strNickName = playerData.nickName;
            this._nPhotoId = playerData.photoId;
        }
        public void Update(PlayerData playerData)
        {
            this._uID = playerData.uid;
            this._nGold = playerData.gold;
            this._nScore = playerData.score;
            this._nLottery = playerData.lottery;
            this._strNickName = playerData.nickName;
            this._nPhotoId = playerData.photoId;
        }
    }
}
