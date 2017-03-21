using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Fishing.Net;
using Fishing.GameState;
namespace Fishing.UI
{
    public class Table : MonoBehaviour
    {
        private TableInfo _tableInfo;
        private Text _txtTableName;
        private Text _txtRateValue;
        private Text _txtLeastGold;
        private Button _btnHead1;
        private Image _imgHeadIcon1;
        private Button _btnHead2;
        private Image _imgHeadIcon2;
        private HallWindow _hallWindow;
        public void Init(HallWindow hallWindow)
        {
            _hallWindow = hallWindow;
            var btnSeat1 = transform.FindChild("BtnSeat1").GetComponent<Button>();
            btnSeat1.onClick.AddListener(this.OnSeat1);
            var btnSeat2 = transform.FindChild("BtnSeat2").GetComponent<Button>();
            btnSeat2.onClick.AddListener(this.OnSeat2);
            Transform transHeadIconFrame1 = transform.FindChild("ImgHeadIconFrame1");
            _btnHead1 = transHeadIconFrame1.FindChild("BtnHeadIcon").GetComponent<Button>();
            _btnHead1.onClick.AddListener(this.OnHead1);
            _imgHeadIcon1 = _btnHead1.GetComponent<Image>();
            Transform transHeadIconFrame2 = transform.FindChild("ImgHeadIconFrame2");
            _btnHead2 = transHeadIconFrame2.FindChild("BtnHeadIcon").GetComponent<Button>();
            _btnHead2.onClick.AddListener(this.OnHead2);
            _imgHeadIcon2 = _btnHead2.GetComponent<Image>();
            _txtTableName = transform.FindChild("TxtTableName").GetComponent<Text>();
            _txtRateValue = transform.FindChild("TxtRateLabel").FindChild("TxtRateValue").GetComponent<Text>();
            _txtLeastGold = transform.FindChild("TxtLeastGoldLabel").FindChild("TxtLeastGoldValue").GetComponent<Text>();
            gameObject.SetActive(false);
        }
        public void OnHead1()
        {
            _hallWindow.OnHeadIcon(_tableInfo.left, _btnHead1.transform.position);
        }
        public void OnHead2()
        {
            _hallWindow.OnHeadIcon(_tableInfo.right, _btnHead2.transform.position);
        }
        public void OnSeat1()
        {
            _hallWindow.OnSeat(_tableInfo.tableId, _tableInfo.tableName, _tableInfo.rate, "left");
        }
        public void OnSeat2()
        {
            _hallWindow.OnSeat(_tableInfo.tableId, _tableInfo.tableName, _tableInfo.rate, "right");
        }
        public void FillTable(TableInfo tableInfo)
        {
            _tableInfo = tableInfo;
            _txtTableName.text = _tableInfo.tableName;
            _txtRateValue.text = _tableInfo.rate.ToString();
            _txtLeastGold.text = _tableInfo.minGold.ToString();
            if (tableInfo.left == null)
            {
                _imgHeadIcon1.overrideSprite = null;
                _btnHead1.gameObject.SetActive(false);
            }
            else
            {
                _imgHeadIcon1.overrideSprite = ResourceMgr.Instance.CreatHeadIconSprite(tableInfo.left.photoId);
                _btnHead1.gameObject.SetActive(true);
            }
            if (tableInfo.right == null)
            {
                _imgHeadIcon2.overrideSprite = null;
                _btnHead2.gameObject.SetActive(false);
            }
            else
            {
                _imgHeadIcon2.overrideSprite = ResourceMgr.Instance.CreatHeadIconSprite(tableInfo.right.photoId);
                _btnHead2.gameObject.SetActive(true);
            }
            gameObject.SetActive(true);
        }
    }
}
