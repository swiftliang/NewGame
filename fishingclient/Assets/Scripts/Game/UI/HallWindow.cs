using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using Fishing.GameState;
using Fishing.Net;
using System.Collections.Generic;
using DG.Tweening;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Fishing.UI
{

    public class HallWindow : Window
    {
        public event Action<bool> quickStartResultEvent;

        public readonly int NUMBER_OF_TABLE_PER_PAGE = 3;
        public readonly float PERIOD_TO_UPDATE_PAGE = 10f;

        private int _nPageNumber;
        private List<Page> _pageList = new List<Page>();
        private int _nIndexOfWhichPageShowing = 1;
        private List<TableInfo> _tableInfoList;
        //number of all pages
        private int _nAllPages = 1;
        //Width of Screen
        private int _nWidthOfScreen;
        //if is flipping
        private bool _bIsFlipping = false;
        private float _fFlipPeriod = 0.4f;
        private Player _player;
        //Timer. When player stay on one page for certain period, update the page.
        private float _fUnUpdatePeriod = 0;

        private Transform _transQuitPanel;
        private Transform _transTableOption;
        private Transform _transTopPanel;
        private Transform _transPlayerInSeatInfo;
        //Component manage the panel to show info of player in certain seat.
        private PlayerInSeatInfoPanel _playerInSeatInfoPanel;
        private Slider _sliderPageNum;
        private int _nSliderSpacing = 28;

        public override void Show(ArgList args)
        {
            base.Show(args);

            _nWidthOfScreen = FishingCamera.Instance.WIDTH;

            _transQuitPanel = transform.FindChild("QuitPanel");
            _transTableOption = transform.FindChild("TableOption");
            _transTopPanel = transform.FindChild("TopPanel");
            _transPlayerInSeatInfo = transform.FindChild("PlayerInfoPanel");

            _playerInSeatInfoPanel = _transPlayerInSeatInfo.gameObject.AddComponent<PlayerInSeatInfoPanel>();
            _playerInSeatInfoPanel.Init();

            Text txtNickName = _transTopPanel.FindChild("TxtNickName").GetComponent<Text>();
            Text txtGold = _transTopPanel.FindChild("Gold").FindChild("ImgFrame").FindChild("TxtValue").GetComponent<Text>();
            Text txtLottery = _transTopPanel.FindChild("Lottery").FindChild("ImgFrame").FindChild("TxtValue").GetComponent<Text>();
            Image imgHeadIcon = _transTopPanel.FindChild("ImgHeadIconFrame").FindChild("ImgHeadIcon").GetComponent<Image>();
            _player = Game.Instance.player;
            txtNickName.text = _player.NickName;
            txtGold.text = _player.Gold.ToString();
            txtLottery.text = _player.Lottery.ToString();
            imgHeadIcon.overrideSprite = ResourceMgr.Instance.CreatHeadIconSprite(_player.PhotoId);

            _transQuitPanel.SetActive(false);
            var btnGoBack = transform.FindChild("BtnGoBack").GetComponent<Button>();
            btnGoBack.onClick.AddListener(this.OnGoBack);
            var btnQuitMakeSure = _transQuitPanel.FindChild("QuitFrame").FindChild("BtnMakeSure").GetComponent<Button>();
            btnQuitMakeSure.onClick.AddListener(this.OnQuitMakeSure);
            var btnQuitCancel = _transQuitPanel.FindChild("QuitFrame").FindChild("BtnCancel").GetComponent<Button>();
            btnQuitCancel.onClick.AddListener(this.OnQuitCancel);
            var btnQuickStart = _transTableOption.FindChild("BtnQuickStart").GetComponent<Button>();
            btnQuickStart.onClick.AddListener(this.OnQuickStart);
            var btnLastPage = _transTableOption.FindChild("BtnLastPage").GetComponent<Button>();
            btnLastPage.onClick.AddListener(this.OnLastPage);
            var btnNextPage = _transTableOption.FindChild("BtnNextPage").GetComponent<Button>();
            btnNextPage.onClick.AddListener(this.OnNextPage);
            _sliderPageNum = transform.FindChild("SliderPageNum").GetComponent<Slider>();

            Transform transTablePages = transform.FindChild("TablePages");
            int i;
            for (i = 0; i < transTablePages.childCount; i++)
            {
                Page page = transTablePages.GetChild(i).gameObject.AddComponent<Page>();
                page.init(this);
                _pageList.Add(page);
            }

            //If the page number passed in args is 0, it means that requesting table info of page is needed.
            if (args.GetAt<int>(0) == 0)
            {
                NetWorkManager.Instance.reqGetPageTables(0, result =>
                {
                    if (result.code == Constants.SUCCESS)
                    {
                        _nPageNumber = result.page;
                        _tableInfoList = result.pageData;
                        _nAllPages = result.tableNum / NUMBER_OF_TABLE_PER_PAGE + 1;
                        _pageNumSliderInit();
                        if (_nAllPages <= 10)
                            _sliderPageNum.value = _nPageNumber - 1;
                        else
                            _sliderPageNum.value = (_nPageNumber + 1) / 2 - 1;
                        _fillPage(_nIndexOfWhichPageShowing);
                    }
                });
            }
            else
            {
                _nPageNumber = args.GetAt<int>(0);
                _tableInfoList = args.GetAt<List<TableInfo>>(1);
                _nAllPages = args.GetAt<int>(2) / NUMBER_OF_TABLE_PER_PAGE + 1;
                _pageNumSliderInit();
                if (_nAllPages <= 10)
                    _sliderPageNum.value = _nPageNumber - 1;
                else
                    _sliderPageNum.value = (_nPageNumber + 1) / 2 - 1;
                _fillPage(_nIndexOfWhichPageShowing);
            }

        }

        public void UpDate()
        {
            _fUnUpdatePeriod += Time.deltaTime;
            if (_fUnUpdatePeriod > PERIOD_TO_UPDATE_PAGE)
            {
                NetWorkManager.Instance.reqGetPageTables(_nPageNumber, result =>
                {
                    if (result.code == Constants.SUCCESS)
                    {
                        _tableInfoList = result.pageData;
                        _fillPage(_nIndexOfWhichPageShowing);
                    }
                });
                _fUnUpdatePeriod = 0;
            }
        }

        public void OnSeat(int nTableId, string strTableName, int nTableRate, string pos)
        {
            NetWorkManager.Instance.reqSitDown("free", nTableId, pos, result =>
            {
                if (result.code == Constants.SUCCESS)
                {
                    Game.Instance.GameFsm.ChangeState<Loading>(ArgList.Create("GamePlay", nTableId, strTableName, nTableRate, result.tableInfo.sceneId), false);
                    Game.Instance.player.Gold = result.updatePlayer.gold;
                    Game.Instance.player.Score = result.updatePlayer.score;
                }
                else
                {
                    Utils.ShowMessageBox((int)result.code);
                    _fUnUpdatePeriod = 0;

                    if (result.code == Constants.SIT_FULL)
                        _tableInfoList = result.pageData;
                    _fillPage(_nIndexOfWhichPageShowing);
                }
            });
        }

        public void OnHeadIcon(PlayerInSeatInfo playerInfo, Vector3 position)
        {
            _playerInSeatInfoPanel.ShowPlayerInfo(playerInfo, position);
        }

        void OnGoBack()
        {
            _transQuitPanel.SetActive(true);
        }

        void OnQuitMakeSure()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        void OnQuitCancel()
        {
            _transQuitPanel.SetActive(false);
        }

        void OnQuickStart()
        {
            NetWorkManager.Instance.reqQuickStart(result =>
            {
                if (result.code == Constants.SUCCESS)
                {
                    Game.Instance.GameFsm.ChangeState<Loading>(ArgList.Create("GamePlay", result.tableInfo.tableId, result.tableInfo.tableName,
                        result.tableInfo.rate, result.tableInfo.sceneId), false);
                    _player.Gold = result.updatePlayer.gold;
                    _player.Score = result.updatePlayer.score;

                    _raiseQuickStartEvent(true);
                }
                else
                {
                    Utils.ShowMessageBox((int)result.code);

                    _raiseQuickStartEvent(false);
                }
            });
        }

        void OnLastPage()
        {
            if (!_bIsFlipping)
            {
                if (_nAllPages > 1)
                {
                    _fUnUpdatePeriod = 0;

                    _bIsFlipping = true;
                    int nextPageNumber = (_nPageNumber - 1 - 1 + _nAllPages) % _nAllPages + 1;
                    NetWorkManager.Instance.reqGetPageTables(nextPageNumber, result =>
                    {
                        if (result.code == Constants.SUCCESS)
                        {
                            _nPageNumber = result.page;
                            _tableInfoList = result.pageData;
                            int nIndexOfWhichPageOnceShowed = _nIndexOfWhichPageShowing;
                            _nIndexOfWhichPageShowing = (_nIndexOfWhichPageShowing - 1 + _pageList.Count) % _pageList.Count;

                            _pageList[_nIndexOfWhichPageShowing].rectTrans.anchoredPosition = new Vector2(-_nWidthOfScreen, 0);

                            _fillPage(_nIndexOfWhichPageShowing);

                            _pageList[nIndexOfWhichPageOnceShowed].rectTrans.DOMoveX(_nWidthOfScreen, _fFlipPeriod).SetRelative();
                            Tween tweener = _pageList[_nIndexOfWhichPageShowing].transform.DOMoveX(_nWidthOfScreen, _fFlipPeriod).SetRelative();
                            tweener.OnComplete(() =>
                            {
                                _bIsFlipping = false;
                                if (_nAllPages <= 10)
                                    _sliderPageNum.value = _nPageNumber - 1;
                                else
                                    _sliderPageNum.value = (_nPageNumber + 1) / 2 - 1;
                            });
                        }
                        else
                        {
                            _bIsFlipping = false;
                        }
                    });
                }
            }

        }

        void OnNextPage()
        {
            if (!_bIsFlipping)
            {
                if (_nAllPages > 1)
                {
                    _fUnUpdatePeriod = 0;

                    _bIsFlipping = true;
                    int nextPageNumber = _nPageNumber % _nAllPages + 1;
                    NetWorkManager.Instance.reqGetPageTables(nextPageNumber, result =>
                    {
                        if (result.code == Constants.SUCCESS)
                        {
                            _nPageNumber = result.page;
                            _tableInfoList = result.pageData;
                            int nIndexOfWhichPageOnceShowed = _nIndexOfWhichPageShowing;
                            _nIndexOfWhichPageShowing = (_nIndexOfWhichPageShowing + 1) % _pageList.Count;

                            _pageList[_nIndexOfWhichPageShowing].rectTrans.anchoredPosition = new Vector2(_nWidthOfScreen, 0);
                            _fillPage(_nIndexOfWhichPageShowing);

                            _pageList[nIndexOfWhichPageOnceShowed].rectTrans.DOMoveX(-_nWidthOfScreen, _fFlipPeriod).SetRelative();
                            Tween tweener = _pageList[_nIndexOfWhichPageShowing].transform.DOMoveX(-_nWidthOfScreen, _fFlipPeriod).SetRelative();
                            tweener.OnComplete(() =>
                            {
                                _bIsFlipping = false;
                                if (_nAllPages <= 10)
                                    _sliderPageNum.value = _nPageNumber - 1;
                                else
                                    _sliderPageNum.value = (_nPageNumber + 1) / 2 - 1;
                            });
                        }
                        else
                        {
                            _bIsFlipping = false;
                        }
                    });
                }
            }
        }

        private void _fillPage(int index)
        {
            _pageList[index].FillTables(_tableInfoList);
        }

        private void _pageNumSliderInit()
        {
            Debug.Assert(_nAllPages <= 20, "more than 20 pages! Do not know how to deal..lol");
            int nPoints;
            if (_nAllPages > 10)
                nPoints = (_nAllPages + 1) / 2;
            else
                nPoints = _nAllPages;
            _sliderPageNum.GetComponent<RectTransform>().SetWidth(nPoints * _nSliderSpacing);
            _sliderPageNum.maxValue = nPoints - 1;
            GameObject gmObjPoint = _sliderPageNum.transform.FindChild("BackPoints").FindChild("Point").gameObject;
            int i;
            int nLeftStartPosX;
            if (_nAllPages % 2 != 0)
                nLeftStartPosX = -(_nAllPages - 1) / 2 * _nSliderSpacing;
            else
                nLeftStartPosX = -(_nAllPages / 2 - 1) * _nSliderSpacing - _nSliderSpacing / 2;

            for (i = 0; i < _nAllPages; i++, nLeftStartPosX += _nSliderSpacing)
            {
                Vector3 vec3NewPos = new Vector3(nLeftStartPosX, gmObjPoint.transform.position.y, gmObjPoint.transform.position.z);
                GameObject gmObjNewPoint = (GameObject)Instantiate(gmObjPoint, vec3NewPos, gmObjPoint.transform.rotation);
                gmObjNewPoint.transform.SetParent(_sliderPageNum.transform.FindChild("BackPoints"), true);
                gmObjNewPoint.SetActive(true);
            }
        }

        private void _raiseQuickStartEvent(bool bFlag)
        {
            if (quickStartResultEvent != null)
                quickStartResultEvent(bFlag);
        }
    }
}
