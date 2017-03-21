using UnityEngine;
using System.Collections;
using DG.Tweening;
public class WaterEffect : MonoBehaviour
{
    public bool EnableDebug;
    public float DebugAmplitude;
    protected Camera _camera;
    protected Material _edgeLightMtl;
    protected int _amplitudeProp;
    protected int _waveOffsetProp;
    protected Material _surfaceMtl;
    protected Material _edgeSurfaceMtl;
    protected WaitForFixedUpdate __waitfixedUpdate;
    protected Plane __mouseHitTestPlane;
    protected Tweener __tweener;
    public float waveAmplitude { get { return _getWaveAmplitude(); } set { _setWaveAmplitude(value); } }
    protected float _getWaveAmplitude() { return _waveAmplitude; }
    protected void _setWaveAmplitude(float value)
    {
        _waveAmplitude = value;
        _edgeLightMtl.SetFloat(_amplitudeProp, _waveAmplitude);
        _surfaceMtl.SetFloat(_amplitudeProp, _waveAmplitude);
        _edgeSurfaceMtl.SetFloat(_amplitudeProp, _waveAmplitude);
    }
    protected float _waveAmplitude;
    protected ParticleSystem _waterSplash;
    public void Init(Camera camera)
    {
        _camera = camera;
        _waterSplash = Utils.Instantiate<ParticleSystem>("Scenes/Water/Splash/Splash");
        _waterSplash.transform.SetParent(this.transform, true);
    }
    public void SetBgTexture(Texture texture)
    {
        if (_surfaceMtl == null)
        {
            var surfaceRender = this.GetChild<MeshRenderer>("Surface");
            _surfaceMtl = surfaceRender.material;
        }
        if (_surfaceMtl.mainTexture == texture) return;
        ///Resources.UnloadAsset(_surfaceMtl.mainTexture);
        Resources.UnloadUnusedAssets();
        _surfaceMtl.mainTexture = texture;
    }
    // Use this for initialization
    void Awake()
    {
        var nearEdge = this.transform.Find("EdgeNear");
        var edgeLightRender = nearEdge.GetChild<MeshRenderer>("EdgeLight");
        _edgeLightMtl = edgeLightRender.material;
        var surfaceRender = this.GetChild<MeshRenderer>("Surface");
        _surfaceMtl = surfaceRender.material;
        var edgeSurfaceRender = nearEdge.GetChild<MeshRenderer>("EdgeSurface");
        _edgeSurfaceMtl = edgeSurfaceRender.material;
        _amplitudeProp = Shader.PropertyToID("_Amplitude");
        waveAmplitude = 0;
        _waveOffsetProp = Shader.PropertyToID("_WaveOffset");
        _edgeLightMtl.SetFloat(_waveOffsetProp, 0.0f);
        _surfaceMtl.SetFloat(_waveOffsetProp, 0.0f);
        _edgeSurfaceMtl.SetFloat(_waveOffsetProp, 0.0f);
        __waitfixedUpdate = new WaitForFixedUpdate();
        __mouseHitTestPlane = new Plane(Vector3.back, 0);
    }
    public void Splash(float xPos, float fAmplitude = 0.2f)
    {
        if (__tweener != null) __tweener.Kill();
        _edgeLightMtl.SetFloat(_waveOffsetProp, xPos);
        _surfaceMtl.SetFloat(_waveOffsetProp, xPos);
        _edgeSurfaceMtl.SetFloat(_waveOffsetProp, xPos);
        waveAmplitude = fAmplitude;
        __tweener = DOTween.To(_getWaveAmplitude, _setWaveAmplitude, 0.0f, 4.0f).OnKill(() =>
        {
            __tweener = null;
        });
        __tweener.SetEase(Ease.OutQuint);
        Vector3 splashPos = new Vector3(xPos, 1.65f, -5.11f);
        _waterSplash.SetPosition(splashPos);
        _waterSplash.Stop();
        _waterSplash.Play();
        //SceneMgr.Instance.PlaySound(AudioNames.LeapFromWater);
    }
    // Update is called once per frame
    void Update()
    {
        if (EnableDebug && Input.GetMouseButtonDown(0))
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            float enter;
            __mouseHitTestPlane.Raycast(ray, out enter);
            var position = ray.GetPoint(enter);
            Splash(position.x, DebugAmplitude);
        }
    }
}
