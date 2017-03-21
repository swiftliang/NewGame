using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Fishing.GameState;
using Fishing.Net;
using DG.Tweening;
using System;

namespace Fishing.UI
{
    public class PlayWindow : Window
    {
        private Text _txtLeftScore;
        private Text _txtLeftLottery;
        private Text _txtRightScore;
        private Text _txtRightLottery;
        private Text _txtHookPower;
        private Text _txtTablePowerValue;
        private Text _txtTableName;
        private Transform _transLeftPlayer;
        private Transform _transRightPlayer;

        private Text _txtOptTip;
        private RectTransform _rectTransPosTip;
        private RectTransform _rectTransDrawingLottery;
        private Text _txtApostrophe;
        private Vector2 _leftAnchoredPos = new Vector2(-320, 130);
        private Vector2 _rightAnchoredPos = new Vector2(320, 130);
        private RectTransform _rectTransLaunchTip;

        private RectTransform _rectTransPanelButtons;
        private Transform _transImgFold;
        private float _fPanelButtonsLeftX = -691.4f;
        private float _fPanelButtonsRightX = -585.9f;
        private bool _bFolded = true;
        private bool _bFolding = false;
        private float _fFoldPeriod = 0.3f;
        private SettingPanel _settingPanel;
        private ChatPanel _chatPanel;
        private Text _youxiguize;

        private Level _level;
        private bool _bIsOKToOper = false;
        private bool _bIsOKToToggle = true;

        private UILottery _lUIlottery;
        private UILottery _RUIlottery;

        public override void Show(ArgList args)
        {
            _level = Game.Instance.level;

            var btnLaunch = transform.FindChild("LaunchHook").FindChild("BtnLaunch").GetComponent<Button>();
            btnLaunch.onClick.AddListener(this.OnLaunch);
            var btnToggleHooks = transform.FindChild("ToggleHooks").FindChild("BtnToggleHooks").GetComponent<Button>();
            btnToggleHooks.onClick.AddListener(this.OnToggleHooks);
            var btnTouch = transform.FindChild("BtnTouch").GetComponent<Button>();
            btnTouch.onClick.AddListener(this.OnTouch);
            _transLeftPlayer = transform.FindChild("LeftPlayer");
            _txtLeftScore = _transLeftPlayer.FindChild("ImgScoreBubble").FindChild("TxtScoreValue").GetComponent<Text>();
            _txtLeftLottery = _transLeftPlayer.FindChild("ImgLotteryBubble").FindChild("TxtLotteryValue").GetComponent<Text>();
            _transRightPlayer = transform.FindChild("RightPlayer");
            _txtRightScore = _transRightPlayer.FindChild("ImgScoreBubble").FindChild("TxtScoreValue").GetComponent<Text>();
            _txtRightLottery = _transRightPlayer.FindChild("ImgLotteryBubble").FindChild("TxtLotteryValue").GetComponent<Text>();
            _txtHookPower = transform.FindChild("ToggleHooks").FindChild("HookPower").FindChild("TxtHookPowerValue").GetComponent<Text>();
            _txtTableName = transform.FindChild("TxtTableName").GetComponent<Text>();
            _txtTableName.text = _level.tableName;
            _txtTablePowerValue = transform.FindChild("TxtTablePowerValue").GetComponent<Text>();
            _txtTablePowerValue.text = _level.tablePower + "," + _level.tablePower * 2 + "," + _level.tablePower * 3;
            _transLeftPlayer.SetActive(false);
            _transRightPlayer.SetActive(false);

            Transform transTips = transform.FindChild("Tips");
            _txtOptTip = transTips.FindChild("TxtOptTip").GetComponent<Text>();
            _rectTransPosTip = transTips.FindChild("TxtPosTip").GetComponent<RectTransform>();
            _rectTransLaunchTip = transTips.FindChild("TxtLaunchTip").GetComponent<RectTransform>();
            _rectTransDrawingLottery = transTips.FindChild("TipDrawingLottery").GetComponent<RectTransform>();
            _txtApostrophe = _rectTransDrawingLottery.FindChild("TxtApostrophe").GetComponent<Text>();

            _rectTransPanelButtons = transform.FindChild("PanelButtons").GetComponent<RectTransform>();
            var btnQuit = _rectTransPanelButtons.FindChild("BtnQuit").GetComponent<Button>();
            btnQuit.onClick.AddListener(this.OnQuit);
            var btnFold = _rectTransPanelButtons.FindChild("BtnFold").GetComponent<Button>();
            btnFold.onClick.AddListener(this.OnFold);
            _transImgFold = btnFold.transform.FindChild("ImgFold");
            var btnSetting = _rectTransPanelButtons.FindChild("BtnSetting").GetComponent<Button>();
            btnSetting.onClick.AddListener(this.OnSetting);
            _settingPanel = transform.FindChild("PanelSetting").gameObject.AddComponent<SettingPanel>();
            _settingPanel.Init();

            var btnChat = _rectTransPanelButtons.FindChild("BtnChat").GetComponent<Button>();
            btnChat.onClick.AddListener(this.OnChat);
            _chatPanel = transform.FindChild("ChatPanel").gameObject.AddComponent<ChatPanel>();
            _chatPanel.Init();

            var btnRule = _rectTransPanelButtons.FindChild("BtnRule").GetComponent<Button>();
            btnRule.onClick.AddListener(delegate { this.OnRule(); });
            _youxiguize = transform.FindChild("GuiZePanel").FindChild("Text").GetComponent<Text>();
            _youxiguize.text = InfoManager.Instance.GetTips(2001);
            Text roule2 = transform.FindChild("GuiZePanel").FindChild("Text2").GetComponent<Text>();
            roule2.text = InfoManager.Instance.GetTips(2003);
            Text roule3 = transform.FindChild("GuiZePanel").FindChild("Text3").GetComponent<Text>();
            roule3.text = InfoManager.Instance.GetTips(2002);

            _level.hookStruggleEvent += OnStruggle;
            _level.scoreChangeEvent += OnScoreChange;
            _level.lotteryChangeEvent += OnLotteryChange;
            _level.hookPowerEvent += OnHookPowerChange;
            _level.playerChangeEvent += OnPlayerChange;
            _level.playerInitCompleteEvent += OnPlayerInitComplete;
            _level.firstLaunchEvent += OnFirstLaunch;
            _level.drawingLotteryEvent += OnDrawingLottery;

            _lUIlottery = UIManager.Instance.ShowWindow<UILottery>("UILottery", null, false);
            _RUIlottery = UIManager.Instance.ShowWindow<UILottery>("UILottery", null, false);
        }

        void OnLaunch()
        {
            _level.hook.ApplyLaunch();
        }

        void OnToggleHooks()
        {
            if (_bIsOKToToggle)
            {
                _level.hook.ToggleHooks();
                _bIsOKToToggle = false;
                Invoke("_beOKToToggleAgain", 1);
            }
        }

        //Temporarily set CD for toggling hooks
        private void _beOKToToggleAgain()
        {
            _bIsOKToToggle = true;
        }

        void OnQuit()
        {
            NetWorkManager.Instance.reqLeaveTable(Game.Instance.level.tableID, result =>
            {

                if (result.code == Constants.SUCCESS)
                {
                    //Remember to cancel subscription of event!
                    _level.hookStruggleEvent -= OnStruggle;
                    _level.scoreChangeEvent -= OnScoreChange;
                    _level.lotteryChangeEvent -= OnLotteryChange;
                    _level.hookPowerEvent -= OnHookPowerChange;
                    _level.playerChangeEvent -= OnPlayerChange;
                    _level.playerInitCompleteEvent -= OnPlayerInitComplete;
                    _level.firstLaunchEvent -= OnFirstLaunch;
                    _level.drawingLotteryEvent -= OnDrawingLottery;

                    Game.Instance.player.Gold = result.updatePlayer.gold;
                    Game.Instance.player.Score = result.updatePlayer.score;
                    Game.Instance.LevelClear();
                    UnityEngine.Object.Destroy(SceneMgr.Instance);
                    Game.Instance.GameFsm.ChangeState<Hall>(ArgList.Create(result.page, result.pageData, result.tableNum), false);
                }
            });

        }

        void OnTouch()
        {
            if (_bIsOKToOper)
                _level.hook.TouchScreen();

            this._chatPanel.OnChat(true);
            this.OnRule(true);
            _settingPanel.OnCancel();
        }

        void OnStruggle(bool bFlag)
        {
            _txtOptTip.SetActive(bFlag);
            _bIsOKToOper = bFlag;
        }

        void OnScoreChange(HookPos hookPos, int nScore)
        {
            switch (hookPos)
            {
                case HookPos.left:
                    _txtLeftScore.text = nScore.ToString();
                    break;
                case HookPos.right:
                    _txtRightScore.text = nScore.ToString();
                    break;
            }
        }

        void OnLotteryChange(HookPos hookPos, int nLottery, LotteryChange change)
        {
            switch (hookPos)
            {
                case HookPos.left:
                    if (change != null)
                    {
                        Vector2 uiPos = UIManager.Instance.WorldPos2UI(change.stratPos);
                        _lUIlottery.RealShow(uiPos, _txtLeftLottery.transform.position, change.nDelta, () => {
                            _txtLeftLottery.text = nLottery.ToString();
                        });
                    }
                    else
                    {
                        _txtLeftLottery.text = nLottery.ToString();
                    }
                    break;
                case HookPos.right:
                    if (change != null)
                    {
                        Vector2 uiPos = UIManager.Instance.WorldPos2UI(change.stratPos);
                        _RUIlottery.RealShow(uiPos, _txtRightLottery.transform.position, change.nDelta, () => {
                            _txtRightLottery.text = nLottery.ToString();
                        });
                    }
                    else
                    {
                        _txtRightLottery.text = nLottery.ToString();
                    }
                    break;
            }

        }

        void OnHookPowerChange(int nPower)
        {
            _txtHookPower.text = nPower.ToString();
        }

        void OnPlayerChange(HookPos hookPos, bool bFlag)
        {
            switch (hookPos)
            {
                case HookPos.left:
                    _transLeftPlayer.SetActive(bFlag);
                    break;
                case HookPos.right:
                    _transRightPlayer.SetActive(bFlag);
                    break;
            }
        }

        void OnPlayerInitComplete(HookPos hookPos)
        {
            _rectTransPosTip.SetActive(true);
            switch (hookPos)
            {
                case HookPos.left:
                    _rectTransPosTip.anchoredPosition = _leftAnchoredPos;
                    break;
                case HookPos.right:
                    _rectTransPosTip.anchoredPosition = _rightAnchoredPos;
                    break;
            }
            _rectTransLaunchTip.SetActive(true);
        }

        void OnFirstLaunch()
        {
            _rectTransPosTip.GetComponent<Text>().DOFade(0, 2);
            _rectTransLaunchTip.GetComponent<Text>().DOFade(0, 2);
        }

        void OnFold()
        {
            if (!_bFolding)
            {
                _bFolding = true;
                Tween tweening;
                if (_bFolded)
                {
                    tweening = _rectTransPanelButtons.DOMoveX(_fPanelButtonsRightX, _fFoldPeriod);
                }
                else
                {
                    tweening = _rectTransPanelButtons.DOMoveX(_fPanelButtonsLeftX, _fFoldPeriod);
                }
                tweening.OnComplete(() =>
                {
                    _bFolding = false;
                    if (_bFolded)
                    {
                        _transImgFold.rotation = Quaternion.Euler(0, 0, 0);
                    }
                    else
                    {
                        _transImgFold.rotation = Quaternion.Euler(0, 0, 180);
                    }
                    _bFolded = !_bFolded;
                });
            }
        }

        void OnSetting()
        {
            _settingPanel.ShowPanel();
            _chatPanel.OnChat(true);
            OnRule(true);
        }

        void OnChat()
        {
            _chatPanel.OnChat();
            OnRule(true);
            _settingPanel.OnCancel();
        }

        void OnRule(bool bForceClose = false)
        {
            if (!bForceClose)
            {
                bool bShow = !this._youxiguize.transform.parent.gameObject.activeSelf;
                this._youxiguize.transform.parent.SetActive(bShow);
                if (bShow)
                {
                    _chatPanel.OnChat(true);
                    _settingPanel.OnCancel();
                }
            }
            else
            {
                this._youxiguize.transform.parent.SetActive(false);
            }
        }

        void OnDrawingLottery(HookPos hookPos, float fPeriod, Action<Vector3> callBack)
        {
            _rectTransDrawingLottery.SetActive(true);
            switch (hookPos)
            {
                case HookPos.left:
                    _rectTransDrawingLottery.anchoredPosition = _leftAnchoredPos;
                    break;
                case HookPos.right:
                    _rectTransDrawingLottery.anchoredPosition = _rightAnchoredPos;
                    break;
            }
            StartCoroutine(_apostropheChange());
            StartCoroutine(_countApostropheChangeTime(fPeriod, callBack));
        }

        private IEnumerator _apostropheChange()
        {
            _txtApostrophe.text = "。";
            while (true)
            {
                yield return new WaitForSeconds(0.5f);
                _apostropheChange(_txtApostrophe.text.Length);
            }
        }

        private IEnumerator _countApostropheChangeTime(float fPeriod, Action<Vector3> callBack)
        {
            yield return new WaitForSeconds(fPeriod);
            StopCoroutine(_apostropheChange());
            _rectTransDrawingLottery.SetActive(false);
            callBack(UIManager.Instance.UI2WorldPos(_rectTransDrawingLottery.anchoredPosition));
        }

        private void _apostropheChange(int nLength)
        {
            if (nLength == 1)
                _txtApostrophe.text = "。。";
            else if (nLength == 2)
                _txtApostrophe.text = "。。。";
            else if (nLength == 3)
                _txtApostrophe.text = "。";
        }
    }
}

