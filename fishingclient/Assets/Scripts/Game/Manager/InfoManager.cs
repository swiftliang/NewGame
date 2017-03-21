using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JsonFx.Json;
using Fishing.Info;
using SimpleJson;
using System;
namespace Fishing
{
    using JsonData = System.Collections.Generic.Dictionary<string, object>;
    public class JFJsonObject : Dictionary<string, object>
    {
    }
    public class InfoManager : Singleton<InfoManager>
    {
        private Dictionary<string, string> _paths = new Dictionary<string, string>();
        private bool _bInit = false;
        private Dictionary<int, ResFishInfo> _fishInfo = new Dictionary<int, ResFishInfo>();
        private Dictionary<int, ResSizeInfo> _sizeInfo = new Dictionary<int, ResSizeInfo>();
        private Dictionary<int, string> _tips = new Dictionary<int, string>();
        private List<ResFishPath> _fishPaths = new List<ResFishPath>();
        private Dictionary<int, string> _headIconPaths = new Dictionary<int, string>();
        public List<LevelConfigure> levelConfigs;
        public Dictionary<string, string> chatUsual = new Dictionary<string, string>();
        private Dictionary<int, string> _sceneBgPaths = new Dictionary<int, string>();
        public JsonData wheelInfo = new JsonData();
        public List<ResFishPath> FishPaths
        {
            get { return _fishPaths; }
        }
        public void Init()
        {
            if (this._bInit)
                return;
            LoadPath();
            LoadFishInfo();
            LoadSizeInfo();
            LoadFishPath();
            LoadTips();
            LoadHeadIcon();
            LoadWheel();
            LoadChatUsual();
            LoadSceneBg();
            this._bInit = true;
        }
        public string GetPath(string strName)
        {
            if (_paths.ContainsKey(strName))
            {
                return _paths[strName].ToString();
            }
            return null;
        }
        public string GetHeadIconPath(int nId)
        {
            if (_headIconPaths.ContainsKey(nId))
            {
                return _headIconPaths[nId];
            }
            return null;
        }
        public string GetSceneBg(int nId)
        {
            if (_sceneBgPaths.ContainsKey(nId))
            {
                return _sceneBgPaths[nId];
            }
            return null;
        }
        protected void LoadChatUsual()
        {
            TextAsset ta = ResourceMgr.Instance.LoadInfo("UsualChat");
            if (ta != null)
            {
                List<ChatUsual> info = JsonReader.Deserialize<List<ChatUsual>>(ta.text);
                foreach (var item in info)
                {
                    this.chatUsual.Add(item.id.ToString(), item.text);
                }
            }
        }
        protected void LoadTips()
        {
            TextAsset ta = ResourceMgr.Instance.LoadInfo("Tips");
            if (ta != null)
            {
                List<Tips> tips = JsonReader.Deserialize<List<Tips>>(ta.text);
                foreach (Tips tip in tips)
                {
                    this._tips.Add(tip.id, tip.value);
                }
            }
        }
        protected void LoadPath()
        {
            TextAsset ta = ResourceMgr.Instance.LoadInfo("ResPath");
            if (ta != null)
            {
                List<ResPath> paths = JsonReader.Deserialize<List<ResPath>>(ta.text);
                foreach (ResPath obj in paths)
                {
                    this._paths.Add(obj.name, obj.path);
                }
            }
        }
        protected void LoadWheel()
        {
            /*TextAsset ta = ResourceMgr.Instance.LoadInfo("wheel");
            if (ta != null)
            {
                wheelInfo = JsonReader.Deserialize<JsonData>(ta.text);
            }*/
        }
        public float GetWheelAngle(int iLayer, int iScore, int iIndex)
        {
            JsonData data = wheelInfo[iLayer.ToString()] as JsonData;
            if (data != null && data.ContainsKey(iScore.ToString()))
            {
                JsonData sdata = data[iScore.ToString()] as JsonData;
                if (sdata != null && sdata.ContainsKey(iIndex.ToString()))
                {
                    return float.Parse(sdata[iIndex.ToString()].ToString());
                }
            }
            return 0;
        }
        protected void LoadFishInfo()
        {
            TextAsset ta = ResourceMgr.Instance.LoadInfo("FishInfo");
            if (ta != null)
            {
                List<ResFishInfo> fishs = JsonReader.Deserialize<List<ResFishInfo>>(ta.text);
                foreach (var fish in fishs)
                {
                    this._fishInfo.Add(fish.obj_type, fish);
                }
            }
        }
        protected void LoadSizeInfo()
        {
            TextAsset ta = ResourceMgr.Instance.LoadInfo("SizeInfo");
            if (ta != null)
            {
                List<ResSizeInfo> sizeInfoList = JsonReader.Deserialize<List<ResSizeInfo>>(ta.text);
                foreach (var sizeInfo in sizeInfoList)
                {
                    this._sizeInfo.Add(sizeInfo.size, sizeInfo);
                }
            }
        }
        protected void LoadFishPath()
        {
            TextAsset ta = ResourceMgr.Instance.LoadInfo("fishPath");
            if (ta != null)
            {
                _fishPaths = JsonReader.Deserialize<List<ResFishPath>>(ta.text);
            }
        }
        protected void LoadHeadIcon()
        {
            TextAsset ta = ResourceMgr.Instance.LoadInfo("HeadIconInfo");
            if (ta != null)
            {
                List<ResHeadIconPath> headIconList = JsonReader.Deserialize<List<ResHeadIconPath>>(ta.text);
                foreach (var headIconInfo in headIconList)
                {
                    this._headIconPaths.Add(headIconInfo.id, headIconInfo.path);
                }
            }
        }
        protected void LoadSceneBg()
        {
            TextAsset ta = ResourceMgr.Instance.LoadInfo("SceneBgInfo");
            if (ta != null)
            {
                List<ResSceneBgPath> sceneBgList = JsonReader.Deserialize<List<ResSceneBgPath>>(ta.text);
                //foreach (var sceneBgInfo in sceneBgList)
                //{
                //    this._sceneBgPaths.Add(sceneBgInfo.sceneId, sceneBgInfo.path);
                //}
                int count = sceneBgList.Count;
                for(int i = 0; i < count; i++)
                {
                    ResSceneBgPath sceneBgInfo = sceneBgList[i];
                    this._sceneBgPaths.Add(sceneBgInfo.sceneId, sceneBgInfo.path);
                }
            }
        }
        public ResFishInfo GetFishInfo(int type)
        {
            if (this._fishInfo.ContainsKey(type))
            {
                return this._fishInfo[type];
            }
            return null;
        }
        public ResSizeInfo GetSizeInfo(int size)
        {
            if (this._sizeInfo.ContainsKey(size))
            {
                return this._sizeInfo[size];
            }
            return null;
        }
        public Dictionary<int, ResFishInfo> GetFishInfos()
        {
            return this._fishInfo;
        }
        public string GetTips(int iId)
        {
            if (this._tips.ContainsKey(iId))
                return this._tips[iId];
            else
                return "Î´ÖªÌáÊ¾!";
        }
    }
}
