using UnityEngine;
using System.Collections;
using DG.Tweening;
namespace Fishing
{
    public class FloatEffect : MonoBehaviour
    {
        public float YAmplitude = 0.15f;
        public float YDuration = 2.2f;
        public float RotationAngle = 5.0f;
        public float RotationDuration = 3.2f;
        protected float _fInitY;
        protected Quaternion _initRotation;
        protected Quaternion _leftRotation;
        protected Quaternion _rightRotation;
        public float rotation
        {
            get { return _getRotation(); }
            set
            {
                _setRotation(value);
            }
        }
        protected float _getRotation() { return _fRotation; }
        protected void _setRotation(float value)
        {
            _fRotation = value;
            transform.rotation = Quaternion.AngleAxis(_fRotation, Vector3.forward) * _initRotation;
        }
        protected float _fRotation;
        protected Tweener _positionTweener;
        protected Tweener _rotationTweener;
        // Use this for initialization
        public void Init()
        {
            _fInitY = transform.position.y;
            _initRotation = transform.rotation;
            //StartEffect();
        }
        public void StartEffect()
        {
            if (YAmplitude != 0)
            {
                _positionTweener = transform.DOMoveY(_fInitY - this.YAmplitude, this.YDuration)
                                                           .SetLoops(-1, LoopType.Yoyo)
                                                           .SetAutoKill(false);
            }
            if (RotationAngle != 0)
            {
                _leftRotation = Quaternion.AngleAxis(10, Vector3.forward);
                _rightRotation = Quaternion.AngleAxis(10, Vector3.forward);
                rotation = -5f;
                _rotationTweener = DOTween.To(_getRotation, _setRotation, RotationAngle, RotationDuration)
                                                            .SetLoops(-1, LoopType.Yoyo)
                                                            .SetAutoKill(false);
            }
        }
        public void StopEffect()
        {
            if (_positionTweener != null)
            {
                _positionTweener.Kill();
                _positionTweener = null;
            }
            if (_rotationTweener != null)
            {
                _rotationTweener.Kill();
                _rotationTweener = null;
            }
        }
        void OnDestroy()
        {
            StopEffect();
        }
        void OnDisable()
        {
            StopEffect();
        }
    }
}
