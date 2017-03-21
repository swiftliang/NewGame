using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
namespace Fishing.UI
{
    public class ChatPaopao : Window
    {
        private Text _txtMessage;
        RectTransform _rctPaopao;
        RectTransform _rctImg;
        float _imgScale;
        public override void Show(ArgList args)
        {
            base.Show(args);
            this._txtMessage = transform.FindChild("message").GetComponent<Text>();
            string strMessage = args.GetAt<string>(0);
            this._txtMessage.text = strMessage;
            Vector3 pos = args.GetAt<Vector3>(1);
            float angel = args.GetAt<float>(2);
            Init(pos, angel);
        }
        void Init(Vector3 pos, float angle)
        {
            RectTransform rct = transform.GetComponent<RectTransform>();
            //rct.SetPosition(pos);
            rct.anchoredPosition3D = pos;
            _rctPaopao = this._txtMessage.transform.GetComponent<RectTransform>();
            _rctImg = transform.FindChild("bg").GetComponent<RectTransform>();
            _imgScale = _rctImg.sizeDelta.y / _rctImg.sizeDelta.x;
            _rctImg.rotation = Quaternion.AngleAxis(angle, Vector3.up);
            //_rctPaopao.rotation = Quaternion.AngleAxis(angle, Vector3.up);
            CanvasGroup cg = transform.GetComponent<CanvasGroup>();
            //cg.alpha = 1.0f;
            cg.DOFade(0, 5).OnComplete(Close);
        }
        private void Update()
        {
            _rctImg.sizeDelta = new Vector2(_rctPaopao.sizeDelta.x + 50, (_rctPaopao.sizeDelta.x + 50) * this._imgScale);
        }
    }
}
