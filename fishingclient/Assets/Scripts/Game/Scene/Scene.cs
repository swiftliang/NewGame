using UnityEngine;
using System.Collections;
namespace Fishing
{
    public class Scene : MonoBehaviour
    {
        public Material MaterialFrontBg { get; protected set; }
        public Material MaterialBackBg { get; protected set; }
        private GameObject _gmObjFrontBg;
        // Use this for initialization
        void Awake()
        {
            _gmObjFrontBg = this.transform.Find("BgTexture").Find("FrontBgTexture").gameObject;
            MaterialFrontBg = _gmObjFrontBg.GetComponent<MeshRenderer>().material;
            MaterialBackBg = this.transform.Find("BgTexture").Find("BackBgTexture").GetComponent<MeshRenderer>().material;
            Debug.Assert(MaterialBackBg != null && MaterialFrontBg != null, "场景里没有找到背景图！！！");
        }
        void Start()
        {
            //if(GameApp.Instance.EditorMode)
            //{
            //    SceneMgr.Instance._SetCurrentScene(this);
            //}
        }
        public Texture GetBackBgTexture()
        {
            return MaterialBackBg.mainTexture;
        }
        public void SetBackBgTexture(int nSceneId)
        {
            MaterialBackBg.mainTexture = ResourceMgr.Instance.CreatSceneBgTexture(nSceneId);
        }
        public void ChangeBgTexture(int nNewSceneId, float fPeriod)
        {
            //Prepare to Start
            _gmObjFrontBg.SetActive(true);
            MaterialFrontBg.SetFloat("_Alpha", 1);
            MaterialFrontBg.mainTexture = MaterialBackBg.mainTexture;
            MaterialBackBg.mainTexture = ResourceMgr.Instance.CreatSceneBgTexture(nNewSceneId);
            SceneMgr.Instance.StartCoroutine(_graduallyChangingBgTexture(fPeriod));
        }
        private IEnumerator _graduallyChangingBgTexture(float fPeriod)
        {
            float fRate = 1 / fPeriod;
            float i = 0;
            float fAlpha = 1;
            while (i <= 1)
            {
                i += Time.deltaTime * fRate;
                fAlpha = 1 - i;
                MaterialFrontBg.SetFloat("_Alpha", fAlpha);
                yield return null;
            }
            _gmObjFrontBg.SetActive(false);
        }
    }
}
