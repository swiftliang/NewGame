using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Fishing.Net;
using Pomelo.DotNetClient;
using JsonFx.Json;
namespace Fishing.UI
{
    public class ChatPanel : MonoBehaviour
    {
        private Text _txtRecord;
        private InputField _ipfMessage;
        private Button _btnSend;
        private Toggle _togRecord;
        //private Toggle _togUsual;
        private GridLayoutGroup _glg;
        public void Init()
        {
            this._txtRecord = transform.FindChild("ScrollRecord").GetComponentInChildren<Text>();
            this._glg = transform.FindChild("ScrollUsual").GetComponentInChildren<GridLayoutGroup>();
            this._ipfMessage = transform.FindChild("InputField").GetComponent<InputField>();
            this._btnSend = transform.FindChild("btnSend").GetComponent<Button>();
            this._togRecord = transform.FindChild("togRecord").GetComponent<Toggle>();
            this._btnSend.onClick.AddListener(SendMessage);
            this._togRecord.onValueChanged.AddListener(OnValueChanged);
            this._togRecord.isOn = false;
            this._glg.transform.parent.parent.SetActive(true);
            AddUsualText();
            NetWorkManager.Instance.AddGameEvent(NotifyMsg.ON_CHAT, OnMessage);
        }
        void AddUsualText()
        {
            foreach (var text in InfoManager.Instance.chatUsual)
            {
                Button item = Utils.Instantiate<Button>("UI/UsualText", this._glg.transform);
                if (item != null)
                {
                    Text message = item.transform.GetComponentInChildren<Text>();
                    message.text = text.Value.ToString();
                    item.onClick.AddListener(delegate { SelectUsual(item); });
                }
            }
        }
        void SendMessage()
        {
            if (this._ipfMessage.text.Length > 0)
            {
                //send
                //string strMessage = "<color=#00ff00ff>Œ“: </color>" + this._ipfMessage.text;
                //AddMessage(strMessage);
                NetWorkManager.Instance.reqChat(this._ipfMessage.text, (Constants result) => {
                    if (result == Constants.SUCCESS)
                    {
                        //AddMessage(strMessage);
                    }
                });
                //UIManager.Instance.ShowWindow<ChatPaopao>("ChatPaopao", ArgList.Create(this._ipfMessage.text, new Vector3(200f, 300f, 0f), 180f), false);
                this._ipfMessage.text = "";
            }
        }
        void OnMessage(Message msg)
        {
            MessageChat msgChat = JsonReader.Deserialize<MessageChat>(msg.rawString);
            string strName = string.Format("<color=#ff00ffff>{0}: </color>", msgChat.nickName);
            //if (Game.Instance.player.UID != msgChat.uid)
            {

                bool bShow = Game.Instance.level.bForbidChat;
                if (Game.Instance.level.uIDHookMap.ContainsKey(msgChat.uid) && !bShow)
                {
                    BaseHook bh = Game.Instance.level.uIDHookMap[msgChat.uid];
                    if (bh != null)
                    {
                        if (bh.hookPos == HookPos.right)
                        {
                            UIManager.Instance.ShowWindow<ChatPaopao>("ChatPaopao", ArgList.Create(msgChat.message, new Vector3(200f, 300f, 0f), 180f), false);
                        }
                        else
                        {
                            UIManager.Instance.ShowWindow<ChatPaopao>("ChatPaopao", ArgList.Create(msgChat.message, new Vector3(-200f, 300f, 0f), 0f), false);
                        }
                    }
                }
            }
            if (Game.Instance.player.UID == msgChat.uid)
            {
                string strMessage = "<color=#00ff00ff>Œ“: </color>" + msgChat.message;
                AddMessage(strMessage);
            }
            else
            {
                AddMessage(strName + msgChat.message);
            }
        }
        void AddMessage(string strMessage)
        {
            this._txtRecord.text += strMessage;
            this._txtRecord.text += "\n";
        }
        void OnValueChanged(bool value)
        {
            this._glg.transform.parent.parent.SetActive(!value);
            this._txtRecord.transform.parent.parent.SetActive(value);
        }
        void SelectUsual(Button btn)
        {
            //if (InfoManager.Instance.chatUsual.ContainsKey(key))
            {
                string message = btn.transform.GetComponentInChildren<Text>().text;//InfoManager.Instance.chatUsual[key];
                this._ipfMessage.text = message;
            }
        }
        public void OnChat(bool bforceClose = false)
        {
            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
            else if (!bforceClose)
            {
                gameObject.SetActive(true);
            }
        }
        private void OnDestroy()
        {
            NetWorkManager.Instance.RemoveEvent(NotifyMsg.ON_CHAT);
        }
    }
}
