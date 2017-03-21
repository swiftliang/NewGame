using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
namespace Fishing.UI
{
    public class MessageBox : Window
    {
        public override void Show(Fishing.ArgList args)
        {
            base.Show(args);
            Text messageText = transform.FindChild("message").GetComponent<Text>();
            messageText.text = args.GetAt<string>(0);
            RectTransform rt = transform.GetComponent<RectTransform>();
            rt.localPosition = new Vector3(0, 250, 0);
            messageText.DOFade(0, 2).OnComplete(Close);
        }
    }
}
