using System.Collections.Generic;

namespace Fishing.FSM
{
    public abstract class State<T>
    {
        public T Owner;
        public FSM<T> Fsm;
        public virtual void OnEnter(ArgList args)
        {
        }
        public virtual void OnExit()
        {
        }
        public virtual void OnUpdate()
        {
        }
        public virtual void OnEvent(Event evt)
        {
        }
    }
    public class FSM<O>
    {
        public O Owner;
        public FSM(O o)
        {
            Owner = o;
        }
        public T ChangeState<T>()
            where T : State<O>, new()
        {
            return ChangeState<T>(null, false);
        }
        public T ChangeState<T>(ArgList args, bool force)
            where T : State<O>, new()
        {
            if (!force && CurrentState is T)
            {
                return CurrentState as T;
            }
            PopState();
            return PushState<T>(args);
        }
        public T PushState<T>()
            where T : State<O>, new()
        {
            return PushState<T>(null);
        }
        public T PushState<T>(ArgList args)
            where T : State<O>, new()
        {
            var t = new T() { Owner = Owner, Fsm = this };
            mStates.Push(t);
            t.OnEnter(args);
            return t;
        }
        public State<O> PopState()
        {
            if (mStates.Count > 0)
            {
                var s = mStates.Pop();
                s.OnExit();
                return s;
            }
            return null;
        }
        public void Clear()
        {
            while (mStates.Count > 0)
            {
                PopState();
            }
        }
        public State<O> CurrentState
        {
            get
            {
                if (mStates.Count > 0)
                    return mStates.Peek();
                else
                    return null;
            }
        }
        public void Update()
        {
            if (CurrentState != null)
                CurrentState.OnUpdate();
        }
        public void OnEvent(Event evt)
        {
            if (CurrentState != null)
                CurrentState.OnEvent(evt);
        }
        private Stack<State<O>> mStates = new Stack<State<O>>();
    }
}
