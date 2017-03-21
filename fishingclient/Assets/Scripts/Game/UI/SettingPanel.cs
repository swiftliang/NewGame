using UnityEngine;
using System.Collections;
using UnityEngine.UI;
namespace Fishing.UI
{
    public class SettingPanel : MonoBehaviour
    {
        private Toggle _togMusic;
        private Toggle _togVibrate;
        private Toggle _togChat;
        private Toggle _togScreenAlwsOn;
        //Record the values of 4 toggles
        private bool[] _bFlags;
        public void Init()
        {
            _bFlags = new bool[4] { true, true, false, true };
            Transform transSettingFrame = transform.FindChild("ImgSetting");
            _togMusic = transSettingFrame.FindChild("TogMusic").GetComponent<Toggle>();
            _togVibrate = transSettingFrame.FindChild("TogVibrate").GetComponent<Toggle>();
            _togChat = transSettingFrame.FindChild("TogChat").GetComponent<Toggle>();
            _togScreenAlwsOn = transSettingFrame.FindChild("TogScreenAlwsOn").GetComponent<Toggle>();
            var btnCancel = transform.GetComponent<Button>();
            btnCancel.onClick.AddListener(this.OnCancel);
            var btnMakeSure = transSettingFrame.FindChild("BtnMakeSure").GetComponent<Button>();
            btnMakeSure.onClick.AddListener(this.OnMakeSure);
        }
        public void ShowPanel()
        {
            //Show as the recorded value
            _togMusic.isOn = _bFlags[0];
            _togVibrate.isOn = _bFlags[1];
            _togChat.isOn = _bFlags[2];
            _togScreenAlwsOn.isOn = _bFlags[3];
            gameObject.SetActive(true);
        }
        public void OnCancel()
        {
            gameObject.SetActive(false);
        }
        public void OnMakeSure()
        {
            //Change the values stored
            _bFlags[0] = _togMusic.isOn;
            _bFlags[1] = _togVibrate.isOn;
            _bFlags[2] = _togChat.isOn;
            _bFlags[3] = _togScreenAlwsOn.isOn;
            //Make the changes happen
            SoundMgr.Instance.SetActive(_togMusic.isOn);
            Game.Instance.level.bVibration = _togVibrate.isOn;
            Game.Instance.level.bForbidChat = _togChat.isOn;
            if (_togScreenAlwsOn.isOn)
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
            else
                Screen.sleepTimeout = SleepTimeout.SystemSetting;
            gameObject.SetActive(false);
        }
    }
}

