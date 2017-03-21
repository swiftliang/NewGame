using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fishing.Info;
using System.Linq;
namespace Fishing
{
    public class FishManager : ILevelObj
    {
        private Level _level;
        private Dictionary<int, IFish> _fishes = new Dictionary<int, IFish>();
        List<int> _timeOver = new List<int>();
        //It is a container for fishes that escape from the hooks
        private List<IFish> _fishesEscaped = new List<IFish>();
        List<IFish> _escapeTimeOver = new List<IFish>();
        public FishManager(Level level)
        {
            this.level = level;
        }
        public Level level
        {
            get { return this._level; }
            set { this._level = value; }
        }
        //Code by Xhj. Add this property of the map "_fishes" to let hook remove fish from it.
        public Dictionary<int, IFish> fishes { get { return _fishes; } }
        public void UpDate()
        {
            this._timeOver.Clear();
            foreach (var item in _fishes)
            {
                item.Value.OnUpDate();
                if (item.Value.TimeOver())
                {
                    this._timeOver.Add(item.Key);
                }
            }
            foreach (var key in this._timeOver)
            {
                IFish fish = this._fishes[key];
                ResFishInfo info = InfoManager.Instance.GetFishInfo(fish.ObjType);
                level.fishPoolMgr.Recycle(info.name, fish);
                this._fishes.Remove(key);
            }
            _escapeTimeOver.Clear();
            foreach (var fish in _fishesEscaped)
            {
                fish.OnEscapeUpDate();
                if (fish.EscapeTimeOver())
                {
                    this._escapeTimeOver.Add(fish);
                }
            }
            foreach (var fish in this._escapeTimeOver)
            {
                ResFishInfo info = InfoManager.Instance.GetFishInfo(fish.ObjType);
                level.fishPoolMgr.Recycle(info.name, fish);
                this._fishesEscaped.Remove(fish);
            }
        }
        public void Add(int id, IFish fish)
        {
            _fishes.Add(id, fish);
        }
        public void AddEscaped(IFish fish)
        {
            _fishesEscaped.Add(fish);
        }
        public void Remove(int id)
        {
            IFish fish = this._fishes[id];
            ResFishInfo info = InfoManager.Instance.GetFishInfo(fish.ObjType);
            level.fishPoolMgr.Recycle(info.name, fish);
            this._fishes.Remove(id);
        }

        //Used when scene is changed, moving fishes out of screen
        public void AllFishesEscape()
        {
            foreach (var item in _fishes)
            {
                item.Value.OnEscape();
                //item.Value.RemoveLottery();
                //item.Value.SetEscapePath();
                AddEscaped(item.Value);
            }
            _fishes.Clear();
        }
        public void ClearAllActiveFish()
        {
            foreach (var item in _fishes)
            {
                IFish fish = item.Value;
                ResFishInfo info = InfoManager.Instance.GetFishInfo(fish.ObjType);
                level.fishPoolMgr.Recycle(info.name, fish);
            }
            _fishes.Clear();
        }
        public void Clear()
        {
            _fishes.Clear();
            _fishesEscaped.Clear();
        }
    }
}
