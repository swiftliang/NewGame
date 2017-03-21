using UnityEngine;
using System.Collections;
namespace Fishing
{
    public class EscapeMovement
    {
        private IFish _owner;
        private Vector3 _endPos;
        private Vector3 _startPos;
        private float _fSpeed = 0.006f;
        private Vector3 _velocity;
        private float _fmovementPassTime = 0.0f;
        private float _fEndTime;
        public EscapeMovement(IFish owner, Vector3 startPos, Vector3 endPos)
        {
            this._owner = owner;
            this._startPos = new Vector3(startPos.x, startPos.y, endPos.z);
            this._endPos = new Vector3(endPos.x, startPos.y, endPos.z);
            Init();
        }
        public void RestPath(Vector3 startPos, Vector3 endPos)
        {
            this._startPos = new Vector3(startPos.x, startPos.y, endPos.z);
            this._endPos = new Vector3(endPos.x, startPos.y, endPos.z);
            Init();
        }
        public void Init()
        {
            _velocity = Vector3.Normalize(_endPos - _startPos) * _fSpeed;
            _fEndTime = Mathf.Abs(_endPos.x - _startPos.x) / _fSpeed;
            _owner.SetRotaion(Quaternion.LookRotation(this._velocity));
            this._fmovementPassTime = 0.0f;
        }
        public bool TimeOver()
        {
            if (_fmovementPassTime > _fEndTime)
            {
                return true;
            }
            return false;
        }
        public void OnUpdate()
        {
            this._fmovementPassTime += Time.deltaTime * 1000;
            _owner.SetPos(this._startPos + this._velocity * this._fmovementPassTime);
        }
    }
}
