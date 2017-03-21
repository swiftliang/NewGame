using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using System;
namespace Fishing.UI
{
    public class UILottery : Window
    {
        private Text _numTxt;
        public override void Show(ArgList args)
        {
            base.Show(args);
            this._numTxt = transform.GetComponent<Text>();
            gameObject.SetActive(false);
        }
        public void RealShow(Vector2 startPos, Vector2 endPos, int nDelta, Action callback)
        {
            gameObject.SetActive(true);
            this._numTxt.text = "+" + nDelta.ToString();
            //transform.SetPosition(startPos);
            transform.position = startPos;
            float deltaX = endPos.x - startPos.x;
            float deltaY = endPos.y - startPos.y;
            Vector3[] path = new Vector3[2] { new Vector3((startPos.x + deltaX * 0.8f), startPos.y + deltaY * 1.25f, 100f), endPos };
            transform.DOPath(path, 1f, PathType.CatmullRom, PathMode.TopDown2D).OnComplete(delegate {
                Hide();
                if (callback != null)
                {
                    callback();
                }
            });
            //RectTransform rct = transform.GetComponent<RectTransform>();
        }
        void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
