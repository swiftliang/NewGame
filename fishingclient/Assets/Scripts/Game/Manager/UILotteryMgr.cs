using UnityEngine;
using System.Collections.Generic;
namespace Fishing.UI
{
    public class UILotteryMgr : SingletonBehaviour<UILotteryMgr>
    {
        private ObjectPool<LotteryNum> _uiPool;
        private List<LotteryNum> _lns;
        protected override void Init()
        {
            base.Init();
            if (_uiPool == null)
                _uiPool = new ObjectPool<LotteryNum>(Creator, 0, Destroy, null, Release);
            if (_lns == null)
                _lns = new List<LotteryNum>();
        }
        public void Add(LotteryNum ln)
        {
            this._lns.Add(ln);
        }
        public static LotteryNum Creator(int iType)
        {
            GameObject prefab = Resources.Load("UI/LotteryNum", typeof(GameObject)) as GameObject;
            var go = Instantiate(prefab) as GameObject;
            go.transform.SetParent(UILotteryMgr.Instance.transform);
            //var rt = go.transform as RectTransform;
            //Helper.Normalize(rt);

            LotteryNum ln = go.AddComponent<LotteryNum>();
            ln.Show(null);
            UILotteryMgr.Instance.Add(ln);
            return ln;
        }
        public static void Release(LotteryNum ln)
        {
            ln.gameObject.SetActive(false);
        }
        public static void Destroy(LotteryNum ln)
        {
            DestroyObject(ln.gameObject);
        }
        public void Remove(LotteryNum ln)
        {
            this._lns.Remove(ln);
            DestroyObject(ln.gameObject);
        }
        public LotteryNum CreatLn(int iNum)
        {
            if (_uiPool == null)
                return null;
            LotteryNum ln = _uiPool.Get();
            ln.SetNum(iNum);
            ln.gameObject.SetActive(true);
            return ln;
        }
        public void Clear()
        {
            foreach (var ln in this._lns)
            {
                Destroy(ln);
            }
            this._lns.Clear();
        }
        public void Recycle(LotteryNum ln)
        {
            ln.SetPos(new Vector3(1000f, 0f, 0f));
            if (_uiPool != null)
                _uiPool.Release(ln);
        }
    }
}
