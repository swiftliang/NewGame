using UnityEngine;
using System.Collections;
using Fishing.UI;
using System;
using DG.Tweening;
namespace Fishing
{
    public class ChestFish : MonoBehaviour, IFish, ILevelObj
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
        //objects of Chest
        private Transform _transChest;
        private Transform _transChestOpen;
        private Transform _transCHestClose;
        private Renderer _rendererFish;

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
        public Renderer FishRenderer
        {
            get { return this._rendererFish; }
            set { this._rendererFish = value; }
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
            _transChest = transform.FindChild("B1").FindChild("BaoXiang");
            _transChestOpen = _transChest.FindChild("BaoXiang_open");
            _transCHestClose = _transChest.FindChild("BaoXiang_close");
            _rendererFish = transform.GetComponentInChildren<SkinnedMeshRenderer>();
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
            _setSpriteOnMove(endPos);
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
            _rendererFish.enabled = true;
            _transChest.localScale = Vector3.one;
            _transCHestClose.SetActive(true);
            _transChestOpen.SetActive(false);
        }
        public void OnEscape()
        {
            SetEscapePath();
            _setSpriteOnMove(_move.EndPos);
        }
        public void OnGotFish()
        {
            _setSpriteOnGotFish();
        }
        public virtual void Recycle()
        {
            this.gameObject.SetActive(false);
            //UILotteryMgr.Instance.Recycle(_ln);
        }
        public virtual void Destroy()
        {
            DestroyObject(this.gameObject);
        }
        public virtual void OnUpDate()
        {
            if (this._move != null)
                this._move.OnUpdate();
        }
        public virtual void OnEscapeUpDate()
        {
            if (this._escapeMove != null)
                this._escapeMove.OnUpdate();
        }
        public void OpenChest(Vector3 endPos, Action Callback)
        {
            Tween tweening = _transChest.DORotate(Vector3.zero, 0.5f);
            _transChest.DOMove(endPos, 0.5f);
            tweening.OnComplete(() =>
            {
                _transCHestClose.SetActive(false);
                _transChestOpen.SetActive(true);
                Callback();
            });

        }
        public Vector3 GetChestOpenPositon()
        {
            return _transChestOpen.transform.position;
        }
        public void ChestDisappear(Action Callback)
        {
            Sequence s = DOTween.Sequence();
            s.Insert(1f, _transChest.DOScale(Vector3.zero, 0.4f));
            s.OnComplete(() => { Callback(); });
        }
        private void _setSpriteOnGotFish()
        {
            //Set sprite
            _transChest.localPosition = new Vector3(0, 0.14f, 0);
            _transChest.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
            _transChest.localScale = Vector3.one;
        }
        private void _setSpriteOnMove(Vector3 endPos)
        {
            //set sprite
            if (endPos.x > 0)
            {
                _transChest.localPosition = new Vector3(0.55f, 0.22f, 0);
                _transChest.localRotation = Quaternion.Euler(new Vector3(0, 270, 0));
                _transChest.localScale = Vector3.one;
            }
            else
            {
                _transChest.localPosition = new Vector3(-0.6f, 0.25f, 0);
                _transChest.localRotation = Quaternion.Euler(new Vector3(0, 270, 0));
                _transChest.localScale = new Vector3(-1, 1, 1);
            }
        }
    }
}
