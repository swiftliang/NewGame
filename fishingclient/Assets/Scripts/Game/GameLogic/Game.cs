using UnityEngine;
using System.Collections.Generic;
using Fishing.FSM;
using Fishing.GameState;
using Fishing.UI;
using UnityEngine.SceneManagement;

namespace Fishing
{
    public class Game : SingletonBehaviour<Game>
    {
        [HideInInspector]
        public FSM<Game> GameFsm;
        public Level level;
        //Add by Xhj. Store player information.
        public Player player;
        //BG player
        public BGPlayer bgPlayer;
        public bool bAutoOperTest = false;
        //[Header("LOG CONFIG")]
        //public eLoglevel eLogLevel = eLoglevel.LL_ALL;
        //public eLogType eLogType = eLogType.LT_ALL;
        protected override void Init()
        {
            GameFsm = new FSM<Game>(this);
            SoundMgr.Instance.Init(this.gameObject);
            bgPlayer = new BGPlayer();
            NetWorkManager.Instance.OnReconnectStart += () =>
            {
                UIManager.Instance.ShowWindow<Wait2Connect>("WaittoConnect", ArgList.Create(InfoManager.Instance.GetTips(2005)), false);
            };
            NetWorkManager.Instance.OnReconnectResult += ret =>
            {
                if (!ret)
                {
                    //重新连接失败
                    //GameFsm.ChangeState<Login>();
                    SceneManager.LoadScene("Login");
                }
                UIManager.Instance.DestroyWindow("WaittoConnect");
            };
        }
        void Start()
        {
            //Init();
            GameFsm.ChangeState<Login>();
            //Load Sound because sound resources are not passed by infoMgr, so we have to load them positively.
            ResourceMgr.Instance.LoadSound();
            //Screen always on
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            if (bAutoOperTest)
                AutoOperTest();
        }
        void Update()
        {
            GameFsm.Update();
            if (level != null)
                level.UpDate();
        }
        public void Play(ArgList args)
        {
            GameFsm.ChangeState<PlayGame>(args, false);
            //notify server loading ok
            NetWorkManager.Instance.ntfLoadComplete();
        }
        public void LevelStart(ArgList args)
        {
            if (level == null)
            {
                level = new Level();
                level.Init(args);
            }
            level.Reset(args);
        }
        public void LevelClear()
        {
            if (level != null)
                level.Clear();
        }
        public void AutoOperTest()
        {
            
        }
    }
}
