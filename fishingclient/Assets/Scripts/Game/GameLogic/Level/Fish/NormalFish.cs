using UnityEngine;
using System.Collections;
using Fishing.UI;
namespace Fishing
{
    public class NormalFish : MonoBehaviour, IFish, ILevelObj
    {
        //private float _fSpeed;
        private float _fSCore;
        private int _iLifeTime = 20;
        private int _iCreateTime;
        private int _objType;
        private int _nLottery;
        private FishType _type;
        private Level _level;
        //Add by Xhj. Add two private fields, "_collider", "_transMouth" and "_nFishID"
        private Collider2D _collider;
        private Transform _transMouth;
        private int _nFishID;
        protected Movement _move;
        protected EscapeMovement _escapeMove;
        private float _fEscapeTime;
        private SingleHook _singleHook;
        private Animator _animator;
        private LotteryNum _ln;
        //private int _iSize;
        //public float Speed
        //{
        //    get { return this._fSpeed; }
        //    set { this._fSpeed = value; }
        //}
        public int ObjType
        {
            get { return this._objType; }
            set { this._objType = value; }
        }
        public int CreateTime
        {
            get { return this._iCreateTime; }
            set { this._iCreateTime = value; }
        }
        public float Score
        {
            get { return this._fSCore; }
            set { this._fSCore = value; }
        }
        public int LifeTime
        {
            get { return this._iLifeTime; }
            set { this._iLifeTime = value; }
        }
        public FishType Type
        {
            get { return this._type; }
            set { this._type = value; }
        }
        public Level level
        {
            get { return this._level; }
            set { this._level = value; }
        }
        //Add by Xhj. Add property, "RelativeMouthPosition" and "FishID"
        public Transform Mouth
        {
            get { return this._transMouth; }
            set { this._transMouth = value; }
        }
        public int FishID
        {
            get { return this._nFishID; }
            set { this._nFishID = value; }
        }
        public float EscapeTime
        {
            get { return this._fEscapeTime; }
            set { this._fEscapeTime = value; }
        }
        public SingleHook SHook
        {
            get { return this._singleHook; }
            set { this._singleHook = value; }
        }
        public Animator FishAnimator
        {
            get { return this._animator; }
            set { this._animator = value; }
        }
        public int Lottery
        {
            get { return this._nLottery; }
            set { this._nLottery = value; }
        }
        public void OnCreated(int iSize, int iScore)
        {
            //_collider = transform.FindChild("Collider").GetComponent<Collider>();
            GameObject collider = ResourceMgr.Instance.CreateObj("FishCollider_" + iSize);
            if (collider != null)
            {
                collider.transform.SetParent(transform, false);
                _collider = collider.GetComponent<Collider2D>();
            }
            //_iSize = iSize;
            _animator = GetComponent<Animator>();
            _transMouth = transform.FindChild("Mouth");
            this._ln = UILotteryMgr.Creator(0);
            this._ln.SetSize(iSize);
            this._ln.SetNum(iScore);
        }
        public bool TimeOver()
        {
            if ((level.PassTime - CreateTime) >= LifeTime)
            {
                return true;
            }
            return false;
        }
        public bool EscapeTimeOver()
        {
            return _escapeMove.TimeOver();
        }
        public void SetPos(Vector3 pos)
        {
            transform.SetPosition(pos);
        }
        public void SetRotaion(Quaternion rotation)
        {
            transform.rotation = rotation;
        }
        public void SetPath(Vector3 startPos, Vector3 endPos)
        {
            if (this._move == null)
                this._move = new Movement(this, startPos, endPos);
            else
                this._move.RestPath(startPos, endPos);
        }
        public void SetEscapePath()
        {
            if (this._escapeMove == null)
                this._escapeMove = new EscapeMovement(this, transform.position, _move.EndPos);
            else
                this._escapeMove.RestPath(transform.position, _move.EndPos);
            if (_collider != null)
                _collider.SetActive(false);
        }
        public virtual void Init()
        {
            //Add by Xhj. Set collider true when init.
            if (_collider != null)
                _collider.SetActive(true);
            this.gameObject.SetActive(true);
            //_ln = UILotteryMgr.Instance.CreatLn(10);
            //_ln.SetSize(_iSize);
            if (this._ln != null)
            {
                this._ln.gameObject.SetActive(true);
            }
        }
        public virtual void RemoveLottery()
        {
            //if (_ln != null && _ln.gameObject.activeSelf)
            //    UILotteryMgr.Instance.Recycle(_ln);
            if (_ln.gameObject.activeSelf)
            {
                this._ln.SetPos(new Vector3(1000f, 0f, 0f));
                this._ln.gameObject.SetActive(false);
            }
        }
        public void OnEscape()
        {
            RemoveLottery();
            SetEscapePath();
        }
        public void OnGotFish()
        {
            RemoveLottery();
        }
        public virtual void Recycle()
        {
            this.gameObject.SetActive(false);
            //UILotteryMgr.Instance.Recycle(_ln);
            RemoveLottery();
        }
        public virtual void Destroy()
        {
            DestroyObject(this.gameObject);
            if (this._ln != null)
            {
                UILotteryMgr.Instance.Remove(this._ln);
            }
        }
        public virtual void OnUpDate()
        {
            if (this._move != null)
                this._move.OnUpdate();
            if (_ln != null)
            {
                _ln.SetPos(transform.position);
                /* if (_transMouth != null)
                 {
                     _ln.SetPos(_transMouth.position);
                 }
                 else
                 {
                     _ln.SetPos(transform.position);
                 }*/
            }
        }
        public virtual void OnEscapeUpDate()
        {
            if (this._escapeMove != null)
                this._escapeMove.OnUpdate();
        }
    }
}
