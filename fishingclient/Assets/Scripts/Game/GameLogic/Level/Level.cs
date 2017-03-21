using UnityEngine;
using System.Collections;
using Pomelo.DotNetClient;
using Fishing.Net;
using JsonFx.Json;
using System.Collections.Generic;
using System;
using Fishing.UI;

namespace Fishing
{
    public interface ILevelObj
    {
        Level level { get; set; }
    }

    public enum HookPos { left, right };

    public class LotteryChange
    {
        public Vector3 stratPos;
        public int nDelta;
    }

    public class Level
    {
        public FishManager fishMgr;
        public FishPoolManager fishPoolMgr;
        //Code by Xhj. Add hook object of player
        public Hook hook;
        //Add hook object of opponent player
        public OHook oHook;
        //uid and Hook map
        public Dictionary<int, BaseHook> uIDHookMap = new Dictionary<int, BaseHook>();
        //Player infomation
        private Player _player;
        private Player _oPlayer;
        public Dictionary<int, Player> uIDPlayerMap = new Dictionary<int, Player>();
        //DeskID
        public int tableID;
        public string tableName;
        public int tablePower;
        //Has the player ever launched the hook?
        private bool _bHasLaunched;
        //flag about whether vibration is able (Its value will not be reset when enter level)
        public bool bVibration = true;
        //flag about whether it is in scene changing
        private bool _bInSceneChanging;
        //
        public bool bForbidChat = false;

        public bool BInSceneChanging { get { return _bInSceneChanging; } }

        //Event to UI
        public event Action<bool> hookStruggleEvent;
        public event Action<HookPos, int> scoreChangeEvent;
        public event Action<HookPos, int, LotteryChange> lotteryChangeEvent;
        public event Action<int> hookPowerEvent;
        public event Action<HookPos, bool> playerChangeEvent;
        //Player and Hook have initialized but player has not launch the hook.
        public event Action<HookPos> playerInitCompleteEvent;
        public event Action firstLaunchEvent;
        public event Action<HookPos, float, Action<Vector3>> drawingLotteryEvent;

        private int iPassTime = 0;

        public int PassTime
        {
            get { return this.iPassTime; }
            set { this.iPassTime = value; }
        }

        public void Init(ArgList args)
        {
            fishMgr = new FishManager(this);
            fishPoolMgr = new FishPoolManager(this);
            fishPoolMgr.Init();
            tableID = args.GetAt<int>(0);
            tableName = args.GetAt<string>(1);
            tablePower = args.GetAt<int>(2);
            _player = Game.Instance.player;
            _oPlayer = new Player();
            uIDPlayerMap.Add(_player.UID, _player);

            Game.Instance.StartCoroutine(_setSceneBg(args.GetAt<int>(3)));
        }

        public void Reset(ArgList args)
        {
            GC.Collect();

            AddNetListenEvent();

            tableID = args.GetAt<int>(0);
            tableName = args.GetAt<string>(1);
            tablePower = args.GetAt<int>(2);
            _bHasLaunched = false;
            _bInSceneChanging = false;

            Game.Instance.StartCoroutine(_setSceneBg(args.GetAt<int>(3)));
        }

        private IEnumerator _setSceneBg(int sceneId)
        {
            yield return new WaitForEndOfFrame();
            SceneMgr.Instance.SetSceneTexture(sceneId);
        }

        public void AddNetListenEvent()
        {
            NetWorkManager.Instance.AddGameEvent(NotifyMsg.ON_NEWENTER, NewEnter);
            NetWorkManager.Instance.AddGameEvent(NotifyMsg.ON_ADDFISH, AddFish);
            NetWorkManager.Instance.AddGameEvent(NotifyMsg.ON_FISHSTATUS, FishStatus);
            NetWorkManager.Instance.AddGameEvent(NotifyMsg.ON_PLAYERQUIT, OPlayerQuit);
            NetWorkManager.Instance.AddGameEvent(NotifyMsg.ON_SHOOT, Shoot);
            NetWorkManager.Instance.AddGameEvent(NotifyMsg.ON_TOUCHFISH, TouchFish);
            NetWorkManager.Instance.AddGameEvent(NotifyMsg.ON_FISHRESULT, FishResult);
            NetWorkManager.Instance.AddGameEvent(NotifyMsg.ON_CHANGEHOOK, ChangeHook);
            NetWorkManager.Instance.AddGameEvent(NotifyMsg.ON_CHANGESCENE, ChangeScene);
            NetWorkManager.Instance.AddGameEvent(NotifyMsg.ON_BACKSCORE, WithDrawScore);
        }

        public void RemoveNetListenEvent()
        {
            //Caution! If u foget to remove the relative event, then you may handle one event twice or more when you re-enter level.
            NetWorkManager.Instance.RemoveEvent(NotifyMsg.ON_NEWENTER);
            NetWorkManager.Instance.RemoveEvent(NotifyMsg.ON_ADDFISH);
            NetWorkManager.Instance.RemoveEvent(NotifyMsg.ON_FISHSTATUS);
            NetWorkManager.Instance.RemoveEvent(NotifyMsg.ON_PLAYERQUIT);
            NetWorkManager.Instance.RemoveEvent(NotifyMsg.ON_SHOOT);
            NetWorkManager.Instance.RemoveEvent(NotifyMsg.ON_TOUCHFISH);
            NetWorkManager.Instance.RemoveEvent(NotifyMsg.ON_FISHRESULT);
            NetWorkManager.Instance.RemoveEvent(NotifyMsg.ON_CHANGEHOOK);
            NetWorkManager.Instance.RemoveEvent(NotifyMsg.ON_CHANGESCENE);
            NetWorkManager.Instance.RemoveEvent(NotifyMsg.ON_BACKSCORE);
        }

        public void Clear()
        {
            fishMgr.Clear();
            fishPoolMgr.Clear();
            UILotteryMgr.Instance.Clear();

            RemoveNetListenEvent();

            //Add by Xhj. Clear stuff
            uIDHookMap.Clear();
            hook = null;
            oHook = null;

            if (_oPlayer != null)
                uIDPlayerMap.Remove(_oPlayer.UID);
        }

        public void UpDate()
        {
            LevelTimeForward();

            foreach (var item in uIDHookMap)
            {
                item.Value.UpDate();
            }
            fishMgr.UpDate();
        }

        #region network event
        public void AddFish(Message msg)
        {
            //change the value of flag
            if (_bInSceneChanging)
                _bInSceneChanging = false;

            MessageFishInfo fishInfo = JsonReader.Deserialize<MessageFishInfo>(msg.rawString);
            int iCount = fishInfo.fishList.Count;
            int iCurLevelTime = -1;
            for (int i = 0; i < iCount; i++)
            {
                FishInfo info = fishInfo.fishList[i];
                //*2 test
                IFish fish = SpawnFish(info);
                if (fish != null)
                    fishMgr.Add(info.fishId, fish);

                iCurLevelTime = info.createTime;
            }

            if (iCurLevelTime != -1)
                this.PassTime = iCurLevelTime;
        }

        public void FishStatus(Message msg)
        {
            fishMgr.ClearAllActiveFish();

            MessageFishStatus fishStatus = JsonReader.Deserialize<MessageFishStatus>(msg.rawString);
            this.PassTime = fishStatus.currentTime;

            int iCount = fishStatus.fishList.Count;
            for (int i = 0; i < iCount; i++)
            {
                FishInfo info = fishStatus.fishList[i];
                if (!fishMgr.fishes.ContainsKey(info.fishId))
                {
                    IFish fish = SpawnFish(info);
                    fishMgr.Add(info.fishId, fish);
                }
            }
        }

        public void LevelTimeForward()
        {
            this.PassTime += (int)(Time.deltaTime * 1000);
        }

        //1. get hook gameObject
        //2. add component
        //3. activate
        //4. Init
        //5. OnEnter
        //6. Add it into Map
        public void NewEnter(Message msg)
        {
            MessageSycHook allhookInfo = JsonReader.Deserialize<MessageSycHook>(msg.rawString);
            int iCount = allhookInfo.playerList.Count;
            if (iCount == 1)
            {
                HookInfo info = allhookInfo.playerList[0];
                this.PassTime = info.enterTime;
                HookPos hookPos = (HookPos)Enum.Parse(typeof(HookPos), info.chairId);
                AllStuffNeeded allStuff = SceneMgr.Instance.hookPosAllStuffMap[hookPos];
                hook = allStuff.gmObjRealPoleEnd.AddComponent<Hook>();

                allStuff.gmObjBoat.SetActive(true);
                FloatEffect floatEffect = allStuff.gmObjBoat.AddComponent<FloatEffect>();
                floatEffect.Init();
                floatEffect.StartEffect();

                hook.gameObject.SetActive(true);
                hook.Init(hookPos);
                hook.OnEnter(info.enterTime, _player, allStuff);
                uIDHookMap.Add(info.uid, hook);

                _raisePlayerInitCompleteEvent(hook.hookPos);
                _raisePlayerChangeEvent(hook.hookPos, true);
                _raiseScoreChangeEvent(hook.hookPos, _player.Score);
                raiseLotteryChangeEvent(hook.hookPos, _player.Lottery);
                raiseHookPowerEvent(1);
            }
            else if (iCount == 2)
            {
                this.PassTime = Mathf.Max(allhookInfo.playerList[0].enterTime, allhookInfo.playerList[1].enterTime);
                for (int i = 0; i < iCount; i++)
                {
                    HookInfo info = allhookInfo.playerList[i];
                    if (info.uid == Game.Instance.player.UID)
                    {
                        if (hook == null)
                        {
                            this.PassTime = info.enterTime;
                            HookPos hookPos = (HookPos)Enum.Parse(typeof(HookPos), info.chairId);
                            AllStuffNeeded allStuff = SceneMgr.Instance.hookPosAllStuffMap[hookPos];
                            hook = allStuff.gmObjRealPoleEnd.AddComponent<Hook>();

                            allStuff.gmObjBoat.SetActive(true);
                            FloatEffect floatEffect = allStuff.gmObjBoat.AddComponent<FloatEffect>();
                            floatEffect.Init();
                            floatEffect.StartEffect();

                            hook.gameObject.SetActive(true);
                            hook.Init(hookPos);
                            hook.OnEnter(info.enterTime, _player, allStuff);
                            uIDHookMap.Add(info.uid, hook);

                            _raisePlayerInitCompleteEvent(hook.hookPos);
                            _raisePlayerChangeEvent(hook.hookPos, true);
                            _raiseScoreChangeEvent(hook.hookPos, _player.Score);
                            raiseLotteryChangeEvent(hook.hookPos, _player.Lottery);
                            raiseHookPowerEvent(1);
                        }
                    }
                    else if (info.uid != Game.Instance.player.UID)
                    {
                        HookPos hookPos = (HookPos)Enum.Parse(typeof(HookPos), info.chairId);
                        AllStuffNeeded allStuff = SceneMgr.Instance.hookPosAllStuffMap[hookPos];
                        if (oHook == null)
                        {
                            oHook = allStuff.gmObjRealPoleEnd.AddComponent<OHook>();
                            //Assume that player can't change position in level
                            oHook.Init(hookPos);

                            FloatEffect floatEffect = allStuff.gmObjBoat.AddComponent<FloatEffect>();
                            floatEffect.Init();
                            floatEffect.StartEffect();
                        }
                        allStuff.gmObjBoat.SetActive(true);
                        oHook.gameObject.SetActive(true);
                        uIDHookMap.Add(info.uid, oHook);

                        //Init oPlayer(no definition, just intialize it)
                        _oPlayer.UID = info.uid;
                        _oPlayer.Score = info.score;
                        _oPlayer.Lottery = info.lottery;
                        uIDPlayerMap.Add(info.uid, _oPlayer);

                        oHook.OnEnter(info.enterTime, _oPlayer, allStuff);


                        oHook.ChangeHooks(info.hook);
                        _raisePlayerChangeEvent(oHook.hookPos, true);
                        _raiseScoreChangeEvent(oHook.hookPos, _oPlayer.Score);
                        raiseLotteryChangeEvent(oHook.hookPos, _oPlayer.Lottery);
                    }
                }
            }
            else
            {
                DebugLogger.LogWarning(eLogType.LT_LOGIC, iCount + " hooks, out of bound!");
            }
        }

        public void OPlayerQuit(Message msg)
        {
            MessagePlayerInfo info = JsonReader.Deserialize<MessagePlayerInfo>(msg.rawString);
            GameObject gObjPoleEnd = oHook.gameObject;
            //Remove item in uid and hook map (also means that remove oHook out of "Update" function)
            uIDHookMap.Remove(info.uid);
            oHook.OnQuit();
            //Remove oPlay in the map
            uIDPlayerMap.Remove(_oPlayer.UID);
            //hide hook gameObject           
            gObjPoleEnd.SetActive(false);
            //hide boat
            SceneMgr.Instance.hookPosAllStuffMap[oHook.hookPos].gmObjBoat.SetActive(false);
            //hide UI
            _raisePlayerChangeEvent(oHook.hookPos, false);
        }

        public void Shoot(Message msg)
        {
            MessageShootInfo info = JsonReader.Deserialize<MessageShootInfo>(msg.rawString);
            BaseHook whichHook = uIDHookMap[info.uid];
            whichHook.LaunchHook(info.angle);
            uIDPlayerMap[info.uid].Score = info.score;
            _raiseScoreChangeEvent(whichHook.hookPos, info.score);
            if (!_bHasLaunched && info.uid == _player.UID)
            {
                _raiseFirstLaunchEvent();
                _bHasLaunched = true;
            }
        }

        public void TouchFish(Message msg)
        {
            MessageTouchInfo info = JsonReader.Deserialize<MessageTouchInfo>(msg.rawString);
            uIDHookMap[info.uid].GotFish(info.fishId);
        }

        public void FishResult(Message msg)
        {
            MessageFishResult result = JsonReader.Deserialize<MessageFishResult>(msg.rawString);
            BaseHook whichHook = uIDHookMap[result.uid];
            whichHook.NailDownFish(result.fishId, result.isCatch, result.lottery);
            //Move this part into "NailDownFish"
            //uIDPlayerMap[result.uid].LastDeltaLottery += result.lottery - uIDPlayerMap[result.uid].Lottery;
            //uIDPlayerMap[result.uid].Lottery = result.lottery;
        }

        public void ChangeHook(Message msg)
        {
            MessageChangeHook result = JsonReader.Deserialize<MessageChangeHook>(msg.rawString);
            oHook.ChangeHooks(result.hook);
        }

        public void ChangeScene(Message msg)
        {
            MessageChangeScene result = JsonReader.Deserialize<MessageChangeScene>(msg.rawString);

            fishMgr.AllFishesEscape();
            foreach (var item in uIDHookMap)
            {
                item.Value.OnChangeScene();
            }

            SceneMgr.Instance.changeSceneTexture(result.sceneId, 1f, 2f, 1f);

            //change the value of flag
            _bInSceneChanging = true;
        }

        public void WithDrawScore(Message msg)
        {
            MessageBackScore result = JsonReader.Deserialize<MessageBackScore>(msg.rawString);
            uIDPlayerMap[result.uid].Score = result.score;
            _raiseScoreChangeEvent(uIDHookMap[result.uid].hookPos, result.score);
        }

        #endregion network event
        //Changed by Xhj. use info as Parameter
        public IFish SpawnFish(FishInfo info)
        {
            //iType = 23;
            IFish fish = fishPoolMgr.Spawn(info.fishType);
            if (fish == null)
            {
                DebugLogger.LogWarning(eLogType.LT_LOGIC, info.fishType.ToString() + " fish spawn failed");
                return null;
            }
            fish.Init();
            fish.FishID = info.fishId;
            fish.LifeTime = info.expireTime;
            fish.Lottery = info.lottery;
            fish.CreateTime = info.createTime;
            fish.EscapeTime = info.escapePlace / 100f;
            ILevelObj obj = fish as ILevelObj;
            obj.level = this;
            FishPath path = SceneMgr.Instance.GetFishPath(info.pathId - 1);
            if (path != null)
            {
                fish.SetPath(path.start.position, path.end.position);
            }
            else
            {
                DebugLogger.LogWarning(eLogType.LT_LOGIC, "path not exist");
            }
            return fish;
        }

        #region raise kinds of Events
        public void raiseHookStruggleEvent(bool bFlag)
        {
            if (hookStruggleEvent != null)
                hookStruggleEvent(bFlag);
        }

        private void _raiseScoreChangeEvent(HookPos hookPos, int nScore)
        {
            if (scoreChangeEvent != null)
                scoreChangeEvent(hookPos, nScore);
        }

        public void raiseLotteryChangeEvent(HookPos hookPos, int nLottery, LotteryChange change = null)
        {
            if (lotteryChangeEvent != null)
                lotteryChangeEvent(hookPos, nLottery, change);
        }

        public void raiseHookPowerEvent(int number)
        {
            if (hookPowerEvent != null)
                hookPowerEvent(number * tablePower);
        }

        private void _raisePlayerChangeEvent(HookPos hookPos, bool bFlag)
        {
            if (playerChangeEvent != null)
                playerChangeEvent(hookPos, bFlag);
        }

        private void _raisePlayerInitCompleteEvent(HookPos hookPos)
        {
            if (playerInitCompleteEvent != null)
                playerInitCompleteEvent(hookPos);
        }

        private void _raiseFirstLaunchEvent()
        {
            if (firstLaunchEvent != null)
                firstLaunchEvent();
        }

        public void raiseDrawingLotteryEvent(HookPos hookPos, Action<Vector3> callBack)
        {
            if (drawingLotteryEvent != null)
                drawingLotteryEvent(hookPos, 4f, callBack);
        }

        #endregion
    }
}
