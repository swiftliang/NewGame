using UnityEngine;
using System.Collections;
using UnityEngine.UI;
namespace Fishing.UI
{
    public class LotteryNum : Window
    {
        private Text _txtNum;
        private Vector3 _OriginScale;
        public override void Show(ArgList args)
        {
            base.Show(args);
            _txtNum = transform.FindChild("Num").GetComponent<Text>();
            _OriginScale = transform.localScale;
        }
        public void SetNum(int iNum)
        {
            _txtNum.text = iNum.ToString();
        }
        public void SetSize(int iSize)
        {
            transform.localScale = _OriginScale * iSize * 0.5f;
        }
        public void SetPos(Vector3 pos)
        {
            transform.SetPosition(new Vector3(pos.x/*-0.2f*/, pos.y - 0.3f, pos.z - 5));
        }
    }
}
