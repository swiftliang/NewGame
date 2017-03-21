using UnityEngine;
using System.Collections.Generic;
using Fishing.Info;
using System.Collections;
namespace Fishing
{
    [System.Serializable]
    public class FishPath
    {
        public Transform start;
        public Transform end;
        public FishPath(Transform start, Transform end)
        {
            this.start = start;
            this.end = end;
        }
    }
    public class AllStuffNeeded
    {
        public GameObject gmObjRealPoleEnd;
        public Transform transFakePoleEnd;
        public GameObject gmObjBoat;
        public Animator animatorCh;
        public Vector3 vec3ChestPos;
    }
    public class SceneMgr : SingletonBehaviour<SceneMgr>
    {
        public WaterEffect water;
        [Header("Fish Path")]
        public List<FishPath> fishPath = new List<FishPath>();
        public Transform fishes;
        public Transform spawnPoints;
        public Scene currentScene { get { return _currentScene; } }
        protected Scene _currentScene;
        //Code by Xhj. Add position for hook to mount
        [Header("Stuff")]
        public GameObject leftRealPoleEnd;
        public Transform leftFakePoleEnd;
        public GameObject leftBoat;
        public Animator leftAnimatorCh;
        public GameObject rightRealPoleEnd;
        public Transform rightFakePoleEnd;
        public GameObject rightBoat;
        public Animator rightAnimatorCh;
        public Transform transLeftChestPos;
        public Transform transRightChestPos;
        public Dictionary<HookPos, AllStuffNeeded> hookPosAllStuffMap = new Dictionary<HookPos, AllStuffNeeded>();
        private ParticleSystem _parSysBubble;
#if UNITY_EDITOR
        [Header("Scene Editor Mode")]
        public bool EditorMode = false;
#endif
        protected override void Init()
        {
            if (water != null)
            {
                water.Init(FishingCamera.Instance.PlayCamera);
                LoadScene("Scene1");
                LoadFishPath();
                LoadAllStuff();
                _loadParticles();
            }
        }
        public void LoadScene(string strSceneName)
        {
#if UNITY_EDITOR
            if(EditorMode)
            {
                _currentScene = transform.FindChild("Scene1").GetComponent<Scene>();
            }
            else
            {
                _currentScene = Utils.Instantiate<Scene>("Scenes/" + strSceneName, this.transform);
            }
#else
            _currentScene = Utils.Instantiate<Scene>("Scenes/" + strSceneName, this.transform);
#endif

        }
        public void SetSceneTexture(int sceneId)
        {
            _currentScene.SetBackBgTexture(sceneId);
            water.SetBgTexture(_currentScene.GetBackBgTexture());
        }
        public void changeSceneTexture(int nNewSceneId, float fPeriod, float fChangeDelay, float fParticleDelay)
        {
            StartCoroutine(_changeSceneTexture(nNewSceneId, fPeriod, fChangeDelay));
            StartCoroutine(_playBubbleParticle(fParticleDelay));
        }
        private IEnumerator _changeSceneTexture(int nNewSceneId, float fPeriod, float fChangeDelay)
        {
            yield return new WaitForSeconds(fChangeDelay);
            _currentScene.ChangeBgTexture(nNewSceneId, fPeriod);
            water.SetBgTexture(_currentScene.GetBackBgTexture());
        }
        private IEnumerator _playBubbleParticle(float fParticleDelay)
        {
            yield return new WaitForSeconds(fParticleDelay);
            _parSysBubble.Play();
            //Sound. Temporary solution because it has to fit the timing when scene change.
            Utils.PlaySound(AudioNames.Bubble);
        }
        public FishPath GetFishPath(int iIndex)
        {
            int iCount = this.fishPath.Count;
            if (iIndex >= 0 && iIndex < iCount)
                return this.fishPath[iIndex];
            else
                return null;
        }
        //Load Paths from infoMgr
        public void LoadFishPath()
        {
            List<ResFishPath> fishPathinfos = InfoManager.Instance.FishPaths;
            int i;
            for (i = 0; i < fishPathinfos.Count; i++)
            {
                var point = new GameObject();
                point.name = "point" + (i + 1);
                point.transform.SetParent(spawnPoints, false);
                var start = new GameObject();
                start.name = "start";
                start.transform.SetParent(point.transform);
                var end = new GameObject();
                end.name = "end";
                end.transform.SetParent(point.transform);
                float len = fishPathinfos[i].len;
                float y = fishPathinfos[i].y;
                float z = fishPathinfos[i].z;//Random.Range(0f, 50f);
                if (fishPathinfos[i].bornPoint == "left")
                {
                    start.transform.localPosition = new Vector3(-len, y, z);
                    end.transform.localPosition = new Vector3(len, y, z);
                }
                else if (fishPathinfos[i].bornPoint == "right")
                {
                    start.transform.localPosition = new Vector3(len, y, z);
                    end.transform.localPosition = new Vector3(-len, y, z);
                }
                fishPath.Add(new FishPath(start.transform, end.transform));
            }
        }
        public void LoadAllStuff()
        {
#if UNITY_EDITOR
            if (!EditorMode)
            {
                _loadAllStuff();
            }
#else
            _loadAllStuff();
#endif
        }
        private void _loadAllStuff()
        {
            AllStuffNeeded leftStuff = new AllStuffNeeded();
            leftStuff.gmObjRealPoleEnd = leftRealPoleEnd;
            leftStuff.gmObjRealPoleEnd.SetActive(false);
            leftStuff.gmObjBoat = leftBoat;
            leftStuff.gmObjBoat.SetActive(false);
            leftStuff.transFakePoleEnd = leftFakePoleEnd;
            leftStuff.animatorCh = leftAnimatorCh;
            leftStuff.vec3ChestPos = transLeftChestPos.position;
            hookPosAllStuffMap.Add(HookPos.left, leftStuff);
            AllStuffNeeded rightStuff = new AllStuffNeeded();
            rightStuff.gmObjRealPoleEnd = rightRealPoleEnd;
            rightStuff.gmObjRealPoleEnd.SetActive(false);
            rightStuff.gmObjBoat = rightBoat;
            rightStuff.gmObjBoat.SetActive(false);
            rightStuff.transFakePoleEnd = rightFakePoleEnd;
            rightStuff.animatorCh = rightAnimatorCh;
            rightStuff.vec3ChestPos = transRightChestPos.position;
            hookPosAllStuffMap.Add(HookPos.right, rightStuff);
        }
        private void _loadParticles()
        {
            Transform transParticles = transform.FindChild("Particles");
            _parSysBubble = transParticles.FindChild("VFX_Bubble_ShuaXin").GetComponent<ParticleSystem>();
        }
    }
}
