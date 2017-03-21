using UnityEngine;
using System.Collections;
using Fishing.FSM;
using UnityEngine.SceneManagement;
using Fishing.UI;
using Fishing.Net;
namespace Fishing.GameState
{
    public class Loading : State<Game>
    {
        private AsyncOperation async;
        private LoadingWindow _wnd;
        private int _tableId;
        private string _tableName;
        private int _tableRate;
        private int _sceneId;
        public override void OnEnter(ArgList args)
        {
            _wnd = UIManager.Instance.ShowWindow<LoadingWindow>("LoadingWindow", null, false);
            _wnd.CCB = OnCompelete;
            LoadConfig();
            this.Fsm.Owner.StartCoroutine(LoadScene(args.GetAt<string>(0)));
            this._tableId = args.GetAt<int>(1);
            this._tableName = args.GetAt<string>(2);
            this._tableRate = args.GetAt<int>(3);
            this._sceneId = args.GetAt<int>(4);
            //this._strPos = args.GetAt<string>(2);
        }
        IEnumerator LoadScene(string strName)
        {
            yield return new WaitForEndOfFrame();
            async = SceneManager.LoadSceneAsync(strName);
            async.allowSceneActivation = false;
            yield return async;
        }
        IEnumerator WaitEndFrame()
        {
            yield return new WaitForEndOfFrame();
            Game.Instance.Play(ArgList.Create(_tableId, _tableName, _tableRate, _sceneId));
        }
        void LoadConfig()
        {
            //InfoManager.Instance.Init();
        }
        void OnCompelete()
        {
            if (async != null)
                async.allowSceneActivation = true;
            this.Fsm.Owner.StartCoroutine(WaitEndFrame());
        }
        public override void OnExit()
        {
            base.OnExit();
            UIManager.Instance.DestroyWindow(_wnd);
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
            if (async != null)
            {
                _wnd.OnProgress(async.progress);
            }
        }
    }
    public class Login : State<Game>
    {
        private Window _wnd;
        public override void OnEnter(ArgList args)
        {
            base.OnEnter(args);
            InfoManager.Instance.Init();
            _wnd = UIManager.Instance.ShowWindow<LoginWindow>("LoginWindow", args, false);
        }
        public override void OnExit()
        {
            base.OnExit();
            UIManager.Instance.DestroyWindow(_wnd);
        }
    }
    public class SelectMode : State<Game>
    {
    }
    public class PlayGame : State<Game>
    {
        private Window _wnd;
        public override void OnEnter(ArgList args)
        {
            base.OnEnter(args);
            this.Fsm.Owner.LevelStart(args);
            _wnd = UIManager.Instance.ShowWindow<PlayWindow>("PlayWindow", args, false);
            Owner.bgPlayer.PlayLevelBG();
        }
        public override void OnExit()
        {
            base.OnExit();
            UIManager.Instance.DestroyWindow(_wnd);
            //Caution! All sounds will be stopped when quit level.
            SoundMgr.Instance.StopAllSounds();
        }
    }
    public class Hall : State<Game>
    {
        private HallWindow _wnd;
        public override void OnEnter(ArgList args)
        {
            base.OnEnter(args);
            SceneManager.LoadSceneAsync("Hall");
            _wnd = UIManager.Instance.ShowWindow<HallWindow>("HallWindow", args, false);
        }
        public override void OnExit()
        {
            base.OnExit();
            UIManager.Instance.DestroyWindow(_wnd);
        }
        public override void OnUpdate()
        {
            _wnd.UpDate();
        }
    }
}
