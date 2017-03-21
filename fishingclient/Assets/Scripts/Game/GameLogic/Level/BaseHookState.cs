using UnityEngine;
using System.Collections;
using Fishing.FSM;
namespace Fishing.BaseHookState
{
    public class Idle : State<BaseHook>
    {
        public override void OnEnter(ArgList args)
        {
            //Owner.RaiseLotteryChangeEvent();
            Owner.ClearLastHookThenIdle();
            Owner.StopBackSound();
        }
        public override void OnUpdate()
        {
            Owner.IdleSwing();
        }
        //public override void OnExit()
        //{
        //    Owner.CalculateForwardVelocity();
        //}
    }
    public class Forward : State<BaseHook>
    {
        public override void OnEnter(ArgList args)
        {
            Owner.BeforeLaunch();
        }
        public override void OnUpdate()
        {
            Owner.MoveForward();
            //Owner.DetectFish();
            Owner.DragFish();
        }
    }
    public class Pause : State<BaseHook>
    {
        public override void OnEnter(ArgList args)
        {
            Owner.OnPause();
            Owner.CalculateBackPeriod();
        }
        public override void OnUpdate()
        {
            Owner.WaitPauseEnd();
        }
    }
    public class Back : State<BaseHook>
    {
        public override void OnEnter(ArgList args)
        {
            Owner.StartBack();
        }
        public override void OnUpdate()
        {
            Owner.MoveBack();
            Owner.DragFish();
        }
        //public override void OnExit()
        //{
        //    Owner.ClearFishes();
        //}
    }
    public class Struggle : State<BaseHook>
    {
        public override void OnEnter(ArgList args)
        {
            Owner.OnEnterStruggle();
        }
        public override void OnUpdate()
        {
            Owner.Struggle();
        }
        public override void OnExit()
        {
            Owner.OnQuitStruggle();
        }
    }

}
