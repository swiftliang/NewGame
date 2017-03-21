using UnityEngine;
using System.Collections;
namespace Fishing
{
    public class Movement
    {
        private IFish _owner;
        private Vector3 _startPos;
        private Vector3 _endPos;
        private Vector3 _velocity;
        private Vector3 _initPos;
        private bool _bPause = false;
        private float _fmovementPassTime = 0.0f;
        //EscapeMovement need information of endPos
        public Vector3 EndPos
        {
            get { return this._endPos; }
            set { this._endPos = value; }
        }
        public IFish Owner
        {
            get { return this._owner; }
            set { this._owner = value; }
        }
        public bool Pause
        {
            get { return this._bPause; }
            set { this._bPause = value; }
        }
        public Movement(IFish fish, Vector3 startPos, Vector3 endPos)
        {
            Owner = fish;
            this._startPos = startPos;
            this._endPos = endPos;
            Init();
        }
        public void RestPath(Vector3 startPos, Vector3 endPos)
        {
            this._startPos = startPos;
            this._endPos = endPos;
            Init();
        }
        public void Init()
        {
            float dis = Vector3.Distance(this._startPos, this._endPos);
            float speed = dis / Owner.LifeTime;
            this._velocity = Vector3.Normalize(this._endPos - this._startPos) * speed;
            this.Pause = false;
            Owner.SetRotaion(Quaternion.LookRotation(this._velocity));
            this._fmovementPassTime = 0.0f;
            SetPos();

        }
        public void SetPos()
        {
            int passTime = ((ILevelObj)Owner).level.PassTime - Owner.CreateTime;
            this._initPos = this._startPos + this._velocity * passTime;
            Owner.SetPos(this._initPos);
        }
        public void OnUpdate()
        {
            if (this.Pause)
                return;
            this._fmovementPassTime += Time.deltaTime * 1000;
            Owner.SetPos(this._initPos + this._velocity * this._fmovementPassTime);
        }
    }
}
