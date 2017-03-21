using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

namespace Fishing.UI
{
    public delegate void OnLoadProgress(float fPorgress);
    public delegate void CompleteCallBack();
    public class LoadingWindow : Window
    {
        private Slider _slider;
        private float _fLastProgress = 0.0f;
        private Tweener __tweener;
        public CompleteCallBack CCB;
        public override void Show(Fishing.ArgList args)
        {
            base.Show(args);
            _slider = transform.FindChild("Slider").GetComponent<Slider>();
            SetSlider(0.0f);
        }
        public override void Close()
        {
            if (__tweener != null)
                __tweener.Kill();
            base.Close();
        }
        public void OnProgress(float fProgress)
        {
            if (_fLastProgress == fProgress)
            {
                return;
            }
            if (__tweener != null)
                __tweener.Kill();
            _fLastProgress = fProgress;
            __tweener = DOTween.To(GetSliderValue, SetSlider, _fLastProgress >= 0.9f ? 1.0f : _fLastProgress, 0.5f)
                .OnKill(() =>
                {
                    __tweener = null;
                })
                .OnComplete(OnComplete);
        }
        private float GetSliderValue()
        {
            return _slider.value;
        }
        private void SetSlider(float fProgress)
        {
            _slider.value = fProgress;
        }
        private void OnComplete()
        {
            if (_fLastProgress >= 0.9f)
            {
                this.CCB();
            }
        }
    }
}
