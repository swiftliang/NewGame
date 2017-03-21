using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Fishing.Net;
namespace Fishing.UI
{
    public class PlayerInSeatInfoPanel : MonoBehaviour
    {
        private RectTransform _rectTransInfoFrame;
        private Image _imgHeadIcon;
        private Text _txtNickName;
        private Text _txtLevel;
        public void Init()
        {
            var btnPanel = GetComponent<Button>();
            btnPanel.onClick.AddListener(this.OnPanel);
            _rectTransInfoFrame = transform.FindChild("PlayerInfoFrame").GetComponent<RectTransform>();
            _imgHeadIcon = _rectTransInfoFrame.FindChild("ImgHeadIconFrame").FindChild("ImgHeadIcon").GetComponent<Image>();
            _txtNickName = _rectTransInfoFrame.FindChild("NickName").FindChild("ImgNickNameFrame").FindChild("TxtNickNameValue").GetComponent<Text>();
            _txtLevel = _rectTransInfoFrame.FindChild("Level").FindChild("ImgLevelFrame").FindChild("TxtLevelValue").GetComponent<Text>();
            gameObject.SetActive(false);
        }

        void OnPanel()
        {
            gameObject.SetActive(false);
        }
        public void ShowPlayerInfo(PlayerInSeatInfo playerInfo, Vector3 position)
        {
            this._rectTransInfoFrame.position = position;
            _imgHeadIcon.overrideSprite = ResourceMgr.Instance.CreatHeadIconSprite(playerInfo.photoId);
            _txtNickName.text = playerInfo.nickName;
            _txtLevel.text = playerInfo.level + " ¼¶";
            gameObject.SetActive(true);
        }
    }
}

