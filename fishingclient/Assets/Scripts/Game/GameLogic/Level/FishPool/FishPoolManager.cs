using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Fishing.Info;
namespace Fishing
{
    public class FishPoolManager : ILevelObj//Singleton<FishPoolManager>
    {
        private Level _level;
        private Dictionary<string, ObjectPool<IFish>> _pools = new Dictionary<string, ObjectPool<IFish>>();
        public FishPoolManager(Level level)
        {
            this.level = level;
        }
        public Level level
        {
            get { return this._level; }
            set { this._level = value; }
        }
        public static IFish Creator(int iType)
        {
            IFish fish = null;
            GameObject objfish = null;
            ResFishInfo info = InfoManager.Instance.GetFishInfo(iType);
            FishType bh_type = (FishType)Enum.Parse(typeof(FishType), info.bh_type);
            switch (bh_type)
            {
                case FishType.NORMAL_FISH:
                    objfish = ResourceMgr.Instance.CreateObj(info.name, SceneMgr.Instance.fishes);
                    if (objfish != null)
                    {
                        fish = objfish.AddComponent<NormalFish>();
                        //Add by Xhj. Initiate fish when it is created.
                        fish.OnCreated(info.size, info.score);
                    }
                    break;
                case FishType.CHEST_FISH:
                    objfish = ResourceMgr.Instance.CreateObj(info.name, SceneMgr.Instance.fishes);
                    if (objfish != null)
                    {
                        fish = objfish.AddComponent<ChestFish>();
                        //Add by Xhj. Initiate fish when it is created.
                        fish.OnCreated(info.size, info.score);
                    }
                    break;
                default:
                    break;
            }
            if (fish != null)
                fish.ObjType = iType;
            return fish;
        }
        public static void Get(IFish fish)
        {
            //fish.Init();
        }
        public static void Recycle(IFish fish)
        {
            fish.Recycle();
        }
        public static void Destroy(IFish fish)
        {
            fish.Destroy();
        }
        public void Init()
        {
            Dictionary<int, ResFishInfo> fishs = InfoManager.Instance.GetFishInfos();
            foreach (var item in fishs)
            {
                if (_pools.ContainsKey(item.Value.name))
                    continue;
                _pools[item.Value.name] = new ObjectPool<IFish>(Creator, item.Value.obj_type, Destroy, Get, Recycle);
            }
        }
        public IFish Spawn(int iType)
        {
            ResFishInfo info = InfoManager.Instance.GetFishInfo(iType);
            if (info == null)
            {
                DebugLogger.LogWarning(eLogType.LT_LOGIC, "no fish type: " + iType.ToString());
                return null;
            }
            if (this._pools.ContainsKey(info.name))
            {
                return this._pools[info.name].Get();
            }
            return null;
        }
        public void Recycle(string strName, IFish fish)
        {
            if (this._pools.ContainsKey(strName))
            {
                this._pools[strName].Release(fish);
            }
        }
        public void Clear()
        {
            foreach (var item in this._pools)
            {
                item.Value.Clear();
            }
        }
    }
}
