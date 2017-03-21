using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fishing.FSM;
using Fishing.Info;
using Fishing.BaseHookState;
using System;

namespace Fishing
{
    public class SingleHook
    {
        public Transform transHookTopEnd;
        public Transform transHookBottomEnd;
        public IFish fish;
        public Vector3 mouthLocalPositionInWorld;

        public SingleHook(Transform transHookTopEnd, Transform transHookBottomEnd)
        {
            this.transHookTopEnd = transHookTopEnd;
            this.transHookBottomEnd = transHookBottomEnd;
        }
    }

    public class BaseHook : MonoBehaviour
    {
        public FSM<BaseHook> HookFsm;
        public Material materialOfLine;
        public float fSwingCycle = 3;
        public float fForwardSpeed = 4;
        public float fBackSpeed = 6;
        public Transform transFakePoleEnd;
        public Animator animatorCh;

        protected Level _level;
        //Player reference
        public Player player;
        //used for Draw the line
        protected LineRenderer _line;
        protected SingleHook[] _allHooks;
        protected List<SingleHook> _activatedHooks;
        protected Transform _transHook;
        //used for Calculate the Angle of Z Axis of pole end
        protected float _fW;
        protected float _fP;
        protected float _fMaxDegree = 24;
        protected float _fHookPassTime;
        //Information of movement of speed
        protected Vector3 _vec3ForwardVelocity;
        protected Vector3 _vec3BackVelocity;
        protected float _fEmptyBackPeriod = 1f;
        protected float _fBackf;
        protected Vector3 _vec3OriginalHookLocalPos;
        protected float _fBackI;
        protected Vector3 _vec3StartBackLocalPos;
        protected float _fSpeedWhenChangeScene = 3;
        //Screen boundary
        protected float _fLeftBoundaryX;
        protected float _fRightBoundaryX;
        protected float _fBottomBoundaryY;
        //Distance between pole end and hook, used to check if hook is back to original position.
        protected float _fOriginalDistance;
        //Hook radius
        protected float _fHookRadius = 0.2f;
        //Number of Hooks(1~3)
        protected int _nCountOfHooks = 1;
        protected Dictionary<int, IFish> _hookedFishes = new Dictionary<int, IFish>();
        protected List<IFish> _toEscapeFishes = new List<IFish>();
        //Pause Time
        protected float _fPausePeriod = 0.5f;
        protected float _fPauseEndTime;
        //Hook is on the left or on the right?
        public HookPos hookPos;
        //handle of sound of closing hook
        private int _nHandleClosehook = -1;
        //Time becoming Idle
        private float _fTimeStartIdle;
        private int _nPeriodOfIdleToSaySth = 10;
        //Lottery of Chest
        protected int _nChestLottery;
        protected Vector3 _vec3ChestPos;

        public LineRenderer Line { get { return _line; } }

        public virtual void Init(HookPos hookPos)
        {
            _level = Game.Instance.level;
            _transHook = transform.FindChild("Hook");
            int i;
            _allHooks = new SingleHook[5];
            for (i = 0; i < 5; i++)
            {
                _allHooks[i] = new SingleHook(_transHook.GetChild(i), _transHook.GetChild(i).Find("Hook_Bottom_end"));
            }
            _lineInit();
            _fW = 2 * Mathf.PI / fSwingCycle / 1000;
            _fP = 0;
            _fBottomBoundaryY = -FishingCamera.Instance.HEIGHT / 200f;
            _fRightBoundaryX = FishingCamera.Instance.WIDTH / 200f;
            _fLeftBoundaryX = -_fRightBoundaryX;
            _fOriginalDistance = Vector3.Distance(transform.position, _transHook.position);
            _vec3OriginalHookLocalPos = _transHook.localPosition;

            HookFsm = new FSM<BaseHook>(this);

            //hook position("left" or "right"?)
            this.hookPos = hookPos;
        }

        #region interface
        public virtual void OnEnter(int enterTime, Player player, AllStuffNeeded allStuff)
        {

            //active hook list
            _activatedHooks = new List<SingleHook>();
            _activatedHooks.Add(_allHooks[2]);

            //hook pass time
            _fP = _level.PassTime - enterTime;
            _fHookPassTime = 0;

            //player
            this.player = player;

            //allstuff
            this.transFakePoleEnd = allStuff.transFakePoleEnd;
            this.animatorCh = allStuff.animatorCh;
            this._vec3ChestPos = allStuff.vec3ChestPos;

            RaiseLotteryChangeEvent();
            HookFsm.ChangeState<Idle>();
        }

        public virtual void UpDate()
        {
            _fHookPassTime += Time.deltaTime * 1000;
            HookFsm.Update();
            _drawLine();
        }

        public virtual void LaunchHook(int nAngle)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, nAngle / 1000f);
            HookFsm.ChangeState<Forward>();
        }
        #endregion

        public virtual void IdleSwing()
        {
            transform.rotation = Quaternion.Euler(0f, 0f, (float)(Mathf.Sin((_fHookPassTime + _fP) * _fW) * _fMaxDegree));
            if (_fHookPassTime > (_fTimeStartIdle + _nPeriodOfIdleToSaySth * 1000))
            {
                Utils.PlaySound(AudioNames.IdleForAWhile);
                animatorCh.SetTrigger("SayHi");
                _fTimeStartIdle = _fHookPassTime;
            }
        }

        public virtual void BeforeLaunch()
        {
            //Clear record of lottery of chest
            _nChestLottery = 0;

            //Calculate the velocity
            _vec3ForwardVelocity = (_transHook.position - transform.position).normalized * fForwardSpeed;

            //Play Sound
            Utils.PlaySound(AudioNames.HookForward);
            //Play Animation
            animatorCh.SetTrigger("LaunchHook");
        }

        public virtual void MoveForward()
        {
            if (_checkIsHookInScreen())
                _transHook.Translate(_vec3ForwardVelocity * Time.deltaTime, Space.World);
            else
                HookFsm.ChangeState<Pause>();
        }

        public virtual void GotFish(int fishid) { }

        public virtual void OnPause()
        {
            _fPauseEndTime = _fPausePeriod + _fHookPassTime;
        }

        public virtual void WaitPauseEnd()
        {
            if (_fHookPassTime > _fPauseEndTime)
                HookFsm.ChangeState<Back>();
        }

        public virtual void CalculateBackPeriod()
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
            }
            _fBackf = 1 / fRealBackPeriod;
            _fBackI = 0;
            _vec3StartBackLocalPos = _transHook.localPosition;
        }

        public void DragFish()
        {
            int i;
            for (i = 0; i < _activatedHooks.Count; i++)
            {
                SingleHook hook = _activatedHooks[i];
                if (hook.fish != null)
                    hook.fish.SetPos(hook.transHookBottomEnd.position - hook.mouthLocalPositionInWorld);
            }
        }

        public void StartBack()
        {
            if (_nHandleClosehook == -1)
            {
                _nHandleClosehook = Utils.PlaySound(AudioNames.HookBack, true);
                animatorCh.SetTrigger("PullPole");
            }
        }

        public void StopBackSound()
        {
            if (_nHandleClosehook != -1)
            {
                Utils.StopSound(_nHandleClosehook);
                _nHandleClosehook = -1;
            }
        }

        public void MoveBack()
        {
            if (_toEscapeFishes.Count > 0 && _fBackI > _toEscapeFishes[0].EscapeTime)
            {
                HookFsm.ChangeState<Struggle>();
            }
            else if (_fBackI < 1)
            {
                _fBackI += _fBackf * Time.deltaTime;
                _transHook.localPosition = Vector3.Lerp(_vec3StartBackLocalPos, _vec3OriginalHookLocalPos, _fBackI);
            }
            else
            {
                SceneMgr.Instance.water.Splash(_transHook.position.x, 0.65f);
                if (player != null && player.LastDeltaLottery != 0)
                {
                    LotteryChange change = new LotteryChange();
                    change.stratPos = new Vector3(_transHook.position.x, _transHook.position.y, _transHook.position.z);
                    change.nDelta = player.LastDeltaLottery;
                    if (change.nDelta != 0)
                    {
                        _level.raiseLotteryChangeEvent(hookPos, player.Lottery - _nChestLottery, change);
                    }
                    else
                    {
                        _level.raiseLotteryChangeEvent(hookPos, player.Lottery - _nChestLottery);
                    }
                    player.LastDeltaLottery = 0;
                }

                //Play Sound
                if (_hookedFishes.Count > 0)
                    Utils.PlaySound(AudioNames.GotFish);

                HookFsm.ChangeState<Idle>();
            }
        }

        public void RaiseLotteryChangeEvent()
        {
            //Print lottery on UI
            if (player != null)
                _level.raiseLotteryChangeEvent(hookPos, player.Lottery);
        }

        public virtual void ClearLastHookThenIdle()
        {

            //Play Animation
            animatorCh.SetTrigger("Idle");

            //record the time when enter "Idle" state
            _fTimeStartIdle = _fHookPassTime;

            _toEscapeFishes.Clear();

        }

        public virtual void OnEnterStruggle()
        {
            if (_toEscapeFishes.Count > 0)
                _toEscapeFishes[0].FishAnimator.speed = 3;
        }

        public virtual void Struggle()
        {

        }

        public virtual void OnQuitStruggle()
        {
        }

        public virtual void NailDownFish(int fishId, bool bIsCatch, int nLottery)
        {
        }

        public virtual void OnChangeScene()
        {
        }

        protected void _lineInit()
        {
            materialOfLine = Resources.Load<Material>("Hook/Line");
            // Add a Line Renderer to the GameObject
            _line = this.gameObject.AddComponent<LineRenderer>();
            // Set the width of the Line Renderer
            //_line.SetWidth(0.035F, 0.035F);
            _line.startWidth = 0.035F;
            _line.endWidth = 0.035F;
            // Set the number of vertex fo the Line Renderer
            //_line.SetVertexCount(2);
            _line.numPositions = 2;
            _line.material = materialOfLine;
        }

        protected bool _checkIsHookInScreen()
        {
            Vector3 vec3HookPosition = _transHook.position;
            if (vec3HookPosition.y > _fBottomBoundaryY && vec3HookPosition.x > _fLeftBoundaryX
                && vec3HookPosition.x < _fRightBoundaryX)
                return true;
            return false;
        }

        protected void _changeHooks(int number)
        {
            foreach (SingleHook hook in _activatedHooks)
            {
                hook.transHookTopEnd.SetActive(false);
            }
            _activatedHooks.Clear();
            switch (number)
            {
                case 1:
                    _activatedHooks.Add(_allHooks[2]);
                    _activatedHooks[0].transHookTopEnd.SetActive(true);
                    break;
                case 2:
                    _activatedHooks.Add(_allHooks[1]);
                    _activatedHooks.Add(_allHooks[3]);
                    foreach (SingleHook hook in _activatedHooks)
                    {
                        hook.transHookTopEnd.SetActive(true);
                    }
                    break;
                case 3:
                    _activatedHooks.Add(_allHooks[0]);
                    _activatedHooks.Add(_allHooks[2]);
                    _activatedHooks.Add(_allHooks[4]);
                    foreach (SingleHook hook in _activatedHooks)
                    {
                        hook.transHookTopEnd.SetActive(true);
                    }
                    break;
                default:
                    DebugLogger.LogError(eLogType.LT_LOGIC, "Number of Hooks error");
                    break;
            }
        }

        private void _drawLine()
        {
            _line.SetPosition(0, transFakePoleEnd.position);
            _line.SetPosition(1, _allHooks[2].transHookTopEnd.position);
        }
    }
}

