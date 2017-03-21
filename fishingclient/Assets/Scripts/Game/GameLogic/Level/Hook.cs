using UnityEngine;
using System.Collections;
using Fishing.FSM;
using Fishing.BaseHookState;
using Fishing.Info;
using System.Collections.Generic;
using Fishing.Net;
using Fishing.UI;
using System;

namespace Fishing
{
    //Hook of player
    public class Hook : BaseHook
    {
        public event Action launchHookSuccessEvent;
        public event Action enterStruggleEvent;
        public event Action idleAgainEvent;

        private RaycastHit2D[] __hits;
        private bool _bApplyedLaunch = false;
        //It is a counter to record how many requests for touching fish that have not been replied.
        private int _nCountOfApplyedTouchFishes = 0;
        protected Dictionary<int, IFish> _waitForResultFishs = new Dictionary<int, IFish>();
        private float _fEndStruggleTime;
        //For preventing fish to escape, Player needs to tap screen randomTimes;
        private int _nFloorTapTimes = 3;
        private int _nCeilTapTimes = 5;
        private int _nTapTimes;
        private bool _bApplyedResult = false;
        private bool _bApplyedToggleHooks = false;
        //Chest. Only one chest because designer assure that only one chest can be hooked at one time.
        private ChestFish _chest;
        private bool _bChest = false;

        public void ApplyLaunch()
        {
            //Verification on client end
            if (player.Score > _nCountOfHooks * _level.tablePower)
            {
                if (!_level.BInSceneChanging && !_bChest)
                {
                    if (HookFsm.CurrentState.GetType().IsAssignableFrom(typeof(Idle)))
                    {
                        if (!_bApplyedLaunch)
                        {
                            _bApplyedLaunch = true;
                            NetWorkManager.Instance.reqShoot(Mathf.RoundToInt(transform.rotation.eulerAngles.z * 1000), result =>
                            {

                                _bApplyedLaunch = false;
                                if (result != Constants.SUCCESS)
                                    Utils.ShowMessageBox((int)result);
                            });
                        }
                    }
                }
            }
            else
            {
                Utils.ShowMessageBox(1002);
            }
        }

        public override void UpDate()
        {
            foreach (var item in _waitForResultFishs)
            {
                item.Value.OnUpDate();
            }
            base.UpDate();
        }

        public override void LaunchHook(int nAngle)
        {
            base.LaunchHook(nAngle);
            _bApplyedLaunch = false;

            _raiseLaunchHookSuccessEvent();
        }

        public override void IdleSwing()
        {
            if (!_bApplyedLaunch)
                base.IdleSwing();
        }

        public override void MoveForward()
        {
            base.MoveForward();
            _detectFish();
        }

        public override void GotFish(int fishId)
        {
            _nCountOfApplyedTouchFishes--;

            IFish fish = _waitForResultFishs[fishId];
            _waitForResultFishs.Remove(fishId);
            _hookedFishes.Add(fishId, fish);
            fish.OnGotFish();
            //fish.RemoveLottery();
            //Couple hook and fish
            SingleHook hook = _activatedHooks[_hookedFishes.Count - 1];
            hook.fish = fish;
            fish.SHook = hook;
            Vector3 vec3DirectionToTop = Vector3.Normalize(hook.transHookTopEnd.position - hook.transHookBottomEnd.position);
            fish.SetRotaion(Quaternion.LookRotation(vec3DirectionToTop, Vector3.back));
            hook.mouthLocalPositionInWorld = fish.Mouth.position - (fish as MonoBehaviour).transform.position;
        }

        public override void CalculateBackPeriod()
        {
            float fRealBackPeriod = _fEmptyBackPeriod;
            foreach (var item in _hookedFishes)
            {
                int size = InfoManager.Instance.GetFishInfo(item.Value.ObjType).size;
                ResSizeInfo sizeInfo = InfoManager.Instance.GetSizeInfo(size);
                float backTime = sizeInfo.backTime;
                if (fRealBackPeriod < backTime)
                {
                    fRealBackPeriod = backTime;
                }

                if (sizeInfo.struggleTime > 0)
                {
                    if (_toEscapeFishes.Count == 0)
                    {
                        _toEscapeFishes.Add(item.Value);
                    }
                    else
                    {
                        int i;
                        for (i = 0; i < _toEscapeFishes.Count; i++)
                        {
                            if (item.Value.EscapeTime < _toEscapeFishes[i].EscapeTime)
                            {
                                _toEscapeFishes.Insert(i, item.Value);
                                break;
                            }
                        }
                        if (i == _toEscapeFishes.Count)
                        {
                            _toEscapeFishes.Add(item.Value);
                        }
                    }
                }
                else
                {
                    NetWorkManager.Instance.reqFishResult(item.Value.FishID, true, result => { });
                }
            }
            _fBackf = 1 / fRealBackPeriod;
            _fBackI = 0;
            _vec3StartBackLocalPos = _transHook.localPosition;
            _nCountOfApplyedTouchFishes = 0;
            foreach (var item in _waitForResultFishs)
            {
                _level.fishMgr.Add(item.Key, item.Value);
            }
            _waitForResultFishs.Clear();
        }

        public override void OnEnterStruggle()
        {
            base.OnEnterStruggle();
            _bApplyedResult = false;
            _level.raiseHookStruggleEvent(true);
            //Calculate the time limit
            int size = InfoManager.Instance.GetFishInfo(_toEscapeFishes[0].ObjType).size;
            ResSizeInfo sizeInfo = InfoManager.Instance.GetSizeInfo(size);
            _fEndStruggleTime = _fHookPassTime + sizeInfo.struggleTime * 1000;
            //Randomly generate how many times does player need to tap.
            _nTapTimes = UnityEngine.Random.Range(_nFloorTapTimes, _nCeilTapTimes + 1);

            //Vibration!!
#if !UNITY_STANDALONE_WIN && !UNITY_EDITOR
            if (_level.bVibration)
                Handheld.Vibrate();
#endif

            _raiseEnterStruggleEvent();
        }

        public override void Struggle()
        {
            if (!_bApplyedResult)
            {
                if (_fHookPassTime > _fEndStruggleTime && _nTapTimes > 0)
                {
                    NetWorkManager.Instance.reqFishResult(_toEscapeFishes[0].FishID, false, result => { });
                    _bApplyedResult = true;
                }
                else if (_fHookPassTime <= _fEndStruggleTime && _nTapTimes <= 0)
                {
                    NetWorkManager.Instance.reqFishResult(_toEscapeFishes[0].FishID, true, result => { });
                    _bApplyedResult = true;
                }
            }
        }

        public override void OnQuitStruggle()
        {
            base.OnQuitStruggle();
            _level.raiseHookStruggleEvent(false);
        }

        public void TouchScreen()
        {
            _nTapTimes--;
        }

        public override void NailDownFish(int fishId, bool bIsCatch, int nLottery)
        {
            if (_hookedFishes.ContainsKey(fishId))
            {
                int objType = _hookedFishes[fishId].ObjType;
                int size = InfoManager.Instance.GetFishInfo(objType).size;
                ResSizeInfo sizeInfo = InfoManager.Instance.GetSizeInfo(size);
                float struggleTime = sizeInfo.struggleTime;

                if (bIsCatch)
                {
                    //Ensure that certain fish has been caught.
                    // 26 or 27 means it is a chest
                    if (objType != 26 && objType != 27)
                    {
                        player.LastDeltaLottery += nLottery - player.Lottery;
                    }
                    else
                    {
                        _nChestLottery = nLottery - player.Lottery;
                    }
                    player.Lottery = nLottery;

                    if (struggleTime > 0)
                    {
                        //Recover normal speed of swimming
                        _toEscapeFishes[0].FishAnimator.speed = 1;
                        _toEscapeFishes.RemoveAt(0);
                    }
                }
                else
                {
                    if (struggleTime > 0)
                    {
                        //Recover normal speed of swimming
                        _toEscapeFishes[0].FishAnimator.speed = 1;

                        _hookedFishes.Remove(_toEscapeFishes[0].FishID);
                        //Decouple fish and hook
                        _toEscapeFishes[0].SHook.fish = null;
                        //_toEscapeFishes [0].SetEscapePath ();
                        _toEscapeFishes[0].OnEscape();
                        _level.fishMgr.AddEscaped(_toEscapeFishes[0]);

                        //Sound
                        Utils.PlaySound(AudioNames.FishEscape);
                        if (UnityEngine.Random.Range(0, 1f) > 0.5f)
                        {
                            Utils.PlaySound(AudioNames.FishSayBye);
                        }
                        _toEscapeFishes.RemoveAt(0);
                    }
                }

                HookFsm.ChangeState<Back>();
            }
        }

        public void ToggleHooks()
        {
            if (HookFsm.CurrentState.GetType().IsAssignableFrom(typeof(Idle)))
            {
                if (!_bApplyedToggleHooks)
                {
                    _bApplyedToggleHooks = true;
                    //It must be in order to toggle numbers of Hooks (1->2->3->1).
                    NetWorkManager.Instance.reqChangeHook(_nCountOfHooks % 3 + 1, result =>
                    {
                        if (result == Constants.SUCCESS)
                        {
                            _nCountOfHooks = _nCountOfHooks % 3 + 1;
                            _changeHooks(_nCountOfHooks);
                            _level.raiseHookPowerEvent(_nCountOfHooks);
                        }
                        _bApplyedToggleHooks = false;
                    });
                }
            }
        }

        public override void ClearLastHookThenIdle()
        {
            base.ClearLastHookThenIdle();
            foreach (var item in _hookedFishes)
            {
                if (item.Value.ObjType == 26 || item.Value.ObjType == 27)
                {
                    _chest = (ChestFish)item.Value;
                    _bChest = true;
                }
                else
                {
                    ResFishInfo info = InfoManager.Instance.GetFishInfo(item.Value.ObjType);
                    _level.fishPoolMgr.Recycle(info.name, item.Value);
                }

            }
            _hookedFishes.Clear();

            if (_bChest)
            {
                _chest.FishRenderer.enabled = false;
                _chest.OpenChest(_vec3ChestPos, _openChestCallBack);
            }

            _raiseIdleAgainEvent();
        }

        private void _openChestCallBack()
        {
            if (_chest.ObjType == 26)
            {
                if (player != null)
                {
                    LotteryChange change = new LotteryChange();
                    change.stratPos = _chest.GetChestOpenPositon();
                    change.nDelta = _nChestLottery;
                    _level.raiseLotteryChangeEvent(hookPos, player.Lottery, change);
                }

                Utils.PlaySound(AudioNames.GotReward);

                _chest.ChestDisappear(() =>
                {
                    _bChest = false;
                    ResFishInfo info = InfoManager.Instance.GetFishInfo(_chest.ObjType);
                    _level.fishPoolMgr.Recycle(info.name, _chest);
                });
            }
            else if (_chest.ObjType == 27)
            {
                Action callBack = _wheelCallBack;
                UIManager.Instance.ShowWindow<WheelWindow>("WheelWindow", ArgList.Create(_vec3ChestPos, _nChestLottery, 0, 0, callBack), false);
            }
        }

        public void _wheelCallBack()
        {
            if (player != null)
            {
                LotteryChange change = new LotteryChange();
                change.stratPos = Vector3.back;
                change.nDelta = _nChestLottery;
                _level.raiseLotteryChangeEvent(hookPos, player.Lottery, change);
            }

            Utils.PlaySound(AudioNames.GotReward);

            _bChest = false;
            ResFishInfo info = InfoManager.Instance.GetFishInfo(_chest.ObjType);
            _level.fishPoolMgr.Recycle(info.name, _chest);
        }

        private void _detectFish()
        {
            if (_hookedFishes.Count + _nCountOfApplyedTouchFishes < _nCountOfHooks)
            {
                //_hits = Physics.SphereCastAll(_transHook.position + Vector3.back, _fHookRadius, Vector3.forward);
                __hits = Physics2D.CircleCastAll(new Vector2(_transHook.position.x, _transHook.position.y), _fHookRadius, Vector2.zero);
                if (__hits.Length > 0)
                {
                    foreach (RaycastHit2D hit in __hits)
                    {
                        IFish fish = hit.collider.transform.GetComponentInParent<IFish>();
                        if (fish == null)
                        {
                            DebugLogger.LogWarning(eLogType.LT_LOGIC, " Hook something not fish!");
                            continue;
                        }

                        hit.collider.SetActive(false);//Deactivate collider of hooked fish
                        _waitForResultFishs.Add(fish.FishID, fish);
                        _level.fishMgr.fishes.Remove(fish.FishID);
                        //fish.RemoveLottery();//身上显示的彩票数取消
                        _nCountOfApplyedTouchFishes++;
                        NetWorkManager.Instance.reqTouchFish(fish.FishID, result =>
                        {
                            if (result != Constants.SUCCESS)
                            {
                                _nCountOfApplyedTouchFishes--;
                            }
                        });
                        if (_hookedFishes.Count + _nCountOfApplyedTouchFishes >= _nCountOfHooks)
                        {
                            break;
                        }
                    }
                }
            }
        }

        public override void OnChangeScene()
        {
            if (!HookFsm.CurrentState.GetType().IsAssignableFrom(typeof(Idle)))
            {
                _toEscapeFishes.Clear();
                foreach (var item in _waitForResultFishs)
                {
                    if (item.Value.SHook != null)
                        item.Value.SHook.fish = null;
                    //item.Value.SetEscapePath();
                    item.Value.OnEscape();
                    _level.fishMgr.AddEscaped(item.Value);
                }
                _waitForResultFishs.Clear();
                foreach (var item in _hookedFishes)
                {
                    item.Value.SHook.fish = null;
                    //item.Value.SetEscapePath();
                    item.Value.OnEscape();
                    _level.fishMgr.AddEscaped(item.Value);
                }
                _hookedFishes.Clear();
                float bakcPeriod = (Vector3.Distance(transform.position, _transHook.position) - _fOriginalDistance) / _fSpeedWhenChangeScene;
                _fBackf = 1 / bakcPeriod;
                _fBackI = 0;
                _vec3StartBackLocalPos = _transHook.localPosition;
                HookFsm.ChangeState<Back>();
                NetWorkManager.Instance.ntfBackScore();
            }
        }

        private void _raiseLaunchHookSuccessEvent()
        {
            if (launchHookSuccessEvent != null)
            {
                launchHookSuccessEvent();
            }
        }

        private void _raiseEnterStruggleEvent()
        {
            if (enterStruggleEvent != null)
            {
                enterStruggleEvent();
            }
        }

        private void _raiseIdleAgainEvent()
        {
            if (idleAgainEvent != null)
            {
                idleAgainEvent();
            }
        }
    }
}

