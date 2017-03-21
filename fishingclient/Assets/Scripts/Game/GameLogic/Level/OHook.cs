using UnityEngine;
using System.Collections;
using Fishing.FSM;
using Fishing.BaseHookState;
using Fishing.Info;
namespace Fishing
{
    //Hook of Opponent player
    public class OHook : BaseHook
    {
        //Remember to reset rotation and position
        public override void OnEnter(int enterTime, Player player, AllStuffNeeded allStuff)
        {
            base.OnEnter(enterTime, player, allStuff);
            _transHook.localPosition = _vec3OriginalHookLocalPos;
            int i;
            for (i = 0; i < 5; i++)
            {
                if (i == 2)
                    _allHooks[i].transHookTopEnd.SetActive(true);
                else
                    _allHooks[i].transHookTopEnd.SetActive(false);
            }
        }
        public override void LaunchHook(int nAngle)
        {
            if (!HookFsm.CurrentState.GetType().IsAssignableFrom(typeof(Idle)))
            {
                _transHook.localPosition = _vec3OriginalHookLocalPos;
                HookFsm.ChangeState<Idle>();
            }
            base.LaunchHook(nAngle);
        }
        public override void BeforeLaunch()
        {
            base.BeforeLaunch();
            //In case that hook of Opponent has not been reset.
            _transHook.localPosition = _vec3OriginalHookLocalPos;
        }
        public override void GotFish(int fishId)
        {
            if (_level.fishMgr.fishes.ContainsKey(fishId))
            {
                IFish fish = _level.fishMgr.fishes[fishId];
                _level.fishMgr.fishes.Remove(fishId);
                _hookedFishes.Add(fishId, fish);
                fish.OnGotFish();
                //fish.RemoveLottery();
                SingleHook hook = _activatedHooks[_hookedFishes.Count - 1];
                hook.fish = fish;
                fish.SHook = hook;
                Vector3 vec3DirectionToTop = Vector3.Normalize(hook.transHookTopEnd.position - hook.transHookBottomEnd.position);
                fish.SetRotaion(Quaternion.LookRotation(vec3DirectionToTop, Vector3.back));
                hook.mouthLocalPositionInWorld = fish.Mouth.position - (fish as MonoBehaviour).transform.position;
            }
        }
        public override void NailDownFish(int fishId, bool bIsCatch, int nLottery)
        {
            //Judge if fish in _hookedFishes
            if (_hookedFishes.ContainsKey(fishId))
            {
                int objType = _hookedFishes[fishId].ObjType;
                int size = InfoManager.Instance.GetFishInfo(objType).size;
                ResSizeInfo sizeInfo = InfoManager.Instance.GetSizeInfo(size);
                float struggleTime = sizeInfo.struggleTime;
                if (bIsCatch)
                {
                    //Ensure that certain fish has been caught.
                    if (objType != 27)
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
                        if (_toEscapeFishes.Count > 0 && _toEscapeFishes[0].FishID == fishId)
                        {
                            //Recover normal speed of swimming
                            _toEscapeFishes[0].FishAnimator.speed = 1;
                            _toEscapeFishes.RemoveAt(0);
                        }
                    }
                }
                else
                {
                    //Judge whether it is a fish that can struggle
                    if (struggleTime > 0)
                    {
                        if (_toEscapeFishes.Count > 0 && _toEscapeFishes[0].FishID == fishId)
                        {
                            //Recover normal speed of swimming
                            _toEscapeFishes[0].FishAnimator.speed = 1;
                            _hookedFishes.Remove(_toEscapeFishes[0].FishID);
                            //Decouple fish and hook
                            _toEscapeFishes[0].SHook.fish = null;
                            //_toEscapeFishes[0].SetEscapePath();
                            _toEscapeFishes[0].OnEscape();
                            _level.fishMgr.AddEscaped(_toEscapeFishes[0]);
                            //Sound
                            Utils.PlaySound(AudioNames.FishEscape);
                            if (Random.Range(0, 1f) > 0.5f)
                            {
                                Utils.PlaySound(AudioNames.FishSayBye);
                            }
                            _toEscapeFishes.RemoveAt(0);
                        }
                    }
                }

                if (HookFsm.CurrentState.GetType().IsAssignableFrom(typeof(Struggle)))
                {
                    HookFsm.ChangeState<Back>();
                }
            }
        }
        public override void ClearLastHookThenIdle()
        {
            base.ClearLastHookThenIdle();
            foreach (var item in _hookedFishes)
            {
                if (item.Value.ObjType == 27)
                {
                    _level.raiseDrawingLotteryEvent(hookPos, postion =>
                    {
                        if (player != null)
                        {
                            LotteryChange change = new LotteryChange();
                            change.stratPos = postion;
                            change.nDelta = _nChestLottery;
                            _level.raiseLotteryChangeEvent(hookPos, player.Lottery, change);
                        }
                        Utils.PlaySound(AudioNames.GotReward);
                    });
                }
                ResFishInfo info = InfoManager.Instance.GetFishInfo(item.Value.ObjType);
                _level.fishPoolMgr.Recycle(info.name, item.Value);
            }
            _hookedFishes.Clear();
        }
        //Invocation when opponent quits
        public void OnQuit()
        {
            RaiseLotteryChangeEvent();
            ClearLastHookThenIdle();
            _activatedHooks.Clear();
        }
        public void ChangeHooks(int number)
        {
            if (!HookFsm.CurrentState.GetType().IsAssignableFrom(typeof(Idle)))
            {
                _transHook.localPosition = _vec3OriginalHookLocalPos;
                HookFsm.ChangeState<Idle>();
            }
            _changeHooks(number);
        }
        public override void OnChangeScene()
        {
            if (!HookFsm.CurrentState.GetType().IsAssignableFrom(typeof(Idle)))
            {
                _toEscapeFishes.Clear();
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
            }
        }
    }
}
