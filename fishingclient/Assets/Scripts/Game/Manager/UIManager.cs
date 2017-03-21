using UnityEngine;
using System.Collections;
namespace Fishing.UI
{
    public class UIManager : SingletonBehaviour<UIManager>
    {
        private Canvas _canvas;
        //private RectTransform _rct;
        private Window _window;
        public Window NowWindow { get { return _window; } }
        protected override void Init()
        {
            base.Init();
            //DontDestroyOnLoad(gameObject);
            if (_canvas == null)
            {
                _canvas = transform.GetComponent<Canvas>();
                //_rct = transform.GetComponent<RectTransform>();
            }
        }
        public void CloseAll()
        {
            var windows = GetComponentsInChildren<Window>();
            foreach (var w in windows)
                w.Close();
        }
        void StageChanged(string newStage)
        {
            CloseAll();
        }
        public T ShowWindow<T>(Fishing.ArgList args, bool fromAssetBundle = true) where T : Window
        {
            return ShowWindow<T>(typeof(T).ToString(), args, fromAssetBundle);
        }
        public T ShowWindow<T>(string wndName, Fishing.ArgList args, bool fromAssetBundle = true) where T : Window
        {
            GameObject prefab = null;
            if (fromAssetBundle)
            {
                //var layouts = AssetsManager.Instance.GetAssetBundle("layout");
                //prefab = layouts.Load(wndName, typeof(GameObject)) as GameObject;
            }
            else
            {
                prefab = Resources.Load("UI/" + wndName, typeof(GameObject)) as GameObject;
            }
            var go = Instantiate(prefab) as GameObject;
            go.transform.SetParent(transform);
            var rt = go.transform as RectTransform;
            Helper.Normalize(rt);
            //rt.offsetMin = Vector2.zero;
            //rt.offsetMax = Vector2.zero;
            T t = go.AddComponent<T>();
            //T t = go.AddComponent(System.Type.GetType(/*this.strNamespace + */wndName)) as T;
            t.gameObject.name = wndName;
            t.Show(args);
            _window = t;
            return t;
        }
        public T PopupWindow<T>(Fishing.ArgList args) where T : Window
        {
            T t = ShowWindow<T>(args);
            //BoxCollider bc = NGUITools.AddWidgetCollider(t.gameObject);
            //bc.center = new Vector3(bc.center.x, bc.center.y, 1);
            //bc.size = new Vector3(5000, 5000, 1);
            return t;
        }
        public void DestroyWindow(Window wnd)
        {
            if (wnd != null)
                Destroy(wnd.gameObject);
        }
        public void DestroyWindow(string wndName)
        {
            Transform ct = transform.Find(wndName);
            if (ct != null)
            {
                Destroy(ct.gameObject);
            }
        }
        public Vector2 WorldPos2UI(Vector3 worldPos)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            Vector3 uiPos = _canvas.worldCamera.ScreenToWorldPoint(screenPos);
            return new Vector2(uiPos.x, uiPos.y);
        }
        public Vector2 UI2WorldPos(Vector2 UIPos)
        {
            Vector3 vector = UIPos;
            Vector3 screenPos = _canvas.worldCamera.WorldToScreenPoint(vector);
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
            return worldPos;
        }
    }
}
