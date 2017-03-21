using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using System;
namespace Fishing.UI
{
    public class WheelWindow : Window
    {
        private RectTransform _tfOutSide;
        private RectTransform _tfgInside;
        private RectTransform _tfMiddle;
        private RectTransform _tfJiantou;
        private RectTransform _jiantou2;
        private RectTransform _jiantou3;
        private Button _btnClick;
        private RectTransform _curRTF;
        private Ease easeType = Ease.OutBack;
        private float duration = 3.0f;
        private int _layer1 = 0;
        private int _layer2 = 0;
        private int _layer3 = 0;
        private Vector3 _startPos;
        //private Vector3 _endPos;
        //bool playing = false;
        private Action _callBack;
        public override void Show(Fishing.ArgList args)
        {
            base.Show(args);
            this._tfgInside = transform.FindChild("imginside").GetComponent<RectTransform>();
            this._tfOutSide = transform.FindChild("imgoutside").GetComponent<RectTransform>();
            this._tfMiddle = transform.FindChild("imgmiddle").GetComponent<RectTransform>();
            this._tfJiantou = transform.FindChild("jiantou").GetComponent<RectTransform>();
            this._jiantou2 = transform.FindChild("jiantou2").GetComponent<RectTransform>();
            this._jiantou3 = transform.FindChild("jiantou3").GetComponent<RectTransform>();
            this._btnClick = transform.FindChild("btnClick").GetComponent<Button>();
            //this._btnClick.onClick.AddListener(this.OnClick);
            this._startPos = args.GetAt<Vector3>(0);
            //this._endPos = args.GetAt<Vector3>(1);
            this._layer1 = args.GetAt<int>(1);
            this._layer2 = args.GetAt<int>(2);
            this._layer3 = args.GetAt<int>(3);
            this._callBack = args.GetAt<Action>(4);
            if (this._layer1 >= 60 && this._layer1 <= 90)
            {
                this._layer2 = this._layer1;
                this._layer1 = 0;
            }
            else if (this._layer1 >= 100 && this._layer1 <= 150)
            {
                this._layer3 = this._layer1;
                this._layer1 = 0;
            }
            this._btnClick.SetActive(false);
            this.Init();
        }
        public void Init()
        {
            //transform.SetPosition(UIManager.Instance.WorldPos2UI(this._startPos));
            RectTransform rct = transform as RectTransform;
            rct.localScale = new Vector3(0.05f, 0.05f, 1.0f);
            rct.anchoredPosition = UIManager.Instance.WorldPos2UI(this._startPos);
            //transform.DOMove(Vector3.zero, 1).OnComplete(MoveComplete);
            float deltaX = -rct.anchoredPosition.x;
            float deltaY = -rct.anchoredPosition.y;
            Vector3[] path = new Vector3[2] { new Vector3((rct.anchoredPosition.x + deltaX * 0.5f), rct.anchoredPosition.y - deltaY, 100f), new Vector3(0f, 0f, 100f) };
            //rct.DOAnchorPos(new Vector2(0f, 0f), 1f).OnComplete(MoveComplete);
            rct.DOPath(path, 1f, PathType.CatmullRom, PathMode.TopDown2D).OnComplete(MoveComplete);
        }
        void MoveComplete()
        {
            transform.DOScale(1, 1f).SetEase(Ease.OutBounce).OnComplete(ScaleComplete);
        }
        void ScaleComplete()
        {
            //this._btnClick.SetActive(true);
            OnClick();
        }
        public void OnClick()
        {
            _curRTF = this._tfOutSide;
            float angle = InfoManager.Instance.GetWheelAngle(1, this._layer1, UnityEngine.Random.Range(0, 2));
            Tweener tween = DOTween.To(GetRotateZ, SetRotate, angle + 720.0f, duration).OnComplete(tojiantou2);
            tween.SetEase(easeType);
            this._btnClick.SetActive(false);
        }
        void SetRotate(float rotatez)
        {
            //_curRTF.Rotate(new Vector3(0,0, rotatez));
            _curRTF.localRotation = Quaternion.Euler(new Vector3(0, 0, rotatez));
            //DebugLogger.Log(eLogType.LT_LOGIC, "SetRotate: " + rotatez.ToString());
        }
        float GetRotateZ()
        {
            return _curRTF.rotation.z;
        }
        void tojiantou2()
        {
            if (this._layer1 > 0)
            {
                complete();
            }
            else
            {
                this._tfJiantou.DOMoveY(this._jiantou2.position.y, 1).OnComplete(RotateLayer2);
            }
        }
        void tojiantou3()
        {
            if (this._layer2 > 0)
            {
                complete();
            }
            else
            {
                this._tfJiantou.DOMoveY(this._jiantou3.position.y, 1).OnComplete(RotateLayer3);
            }
        }
        void RotateLayer2()
        {
            DebugLogger.Log(eLogType.LT_LOGIC, "RotateLayer2");
            _curRTF = this._tfMiddle;
            float angle = InfoManager.Instance.GetWheelAngle(2, this._layer2, UnityEngine.Random.Range(0, 1));
            Tweener tween = DOTween.To(GetRotateZ, SetRotate, angle + 720f, duration).OnComplete(tojiantou3);
            tween.SetEase(easeType);
        }
        void RotateLayer3()
        {
            _curRTF = this._tfgInside;
            float angle = InfoManager.Instance.GetWheelAngle(3, this._layer3, 0);
            Tweener tween = DOTween.To(GetRotateZ, SetRotate, angle + 720f, duration).OnComplete(complete);
            tween.SetEase(easeType);
        }
        void complete()
        {
            _callBack();
            StartCoroutine(DelayClose(2f));
        }
        IEnumerator DelayClose(float delay)
        {
            yield return new WaitForSeconds(delay);
            Close();
        }
    }
}
