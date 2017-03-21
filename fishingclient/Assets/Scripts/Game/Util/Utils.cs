using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;
using Fishing;
using Fishing.UI;
public static class ComponentExt
{
    public static Vector3 GetPosition(this Component component)
    {
        return component.transform.position;
    }
    public static void SetPosition(this Component component, Vector3 position)
    {
        component.transform.position = position;
    }
    public static void SetVisible(this Component component, bool bVisible)
    {
        component.gameObject.SetActive(bVisible);
    }
    public static void SetActive(this Component component, bool bActive)
    {
        component.gameObject.SetActive(bActive);
    }
    public static Transform GetChild(this Component component, string strChildName)
    {
        var parent = component.transform;
        var child = parent.Find(strChildName);
        return child;
    }
    /// Get a child's component
    public static T GetChild<T>(this Component component, string strChildName) where T : Component
    {
        var parent = component.transform;
        var child = parent.Find(strChildName);
        Debug.Assert(child != null, "Utility.GetChild failed, child " + strChildName + " not found!");
        var temp = child.GetComponent<T>();
        Debug.Assert(component != null, strChildName + " is not a child of " + parent.name);
        return temp;
    }
}
public class Utils
{
    static public Sprite LoadSprite(string spritePath)
    {
        return Resources.Load<GameObject>(spritePath).GetComponent<Image>().sprite;
    }
    static public Texture LoadTexture(string texturePath)
    {
        return Resources.Load<GameObject>(texturePath).GetComponent<Renderer>().sharedMaterial.mainTexture;
    }
    static public void AddChild(Transform pParent, Transform pChild, bool bActive = true, bool bResetTransform = true)
    {
        pChild.SetParent(pParent.transform);
        if (bResetTransform)
        {
            pChild.transform.localPosition = Vector3.zero;
            pChild.localRotation = Quaternion.identity;
            pChild.localScale = Vector3.one;
        }
        pChild.gameObject.SetActive(bActive);
    }
    static public void AutoScaleCamera(Camera camera, float fLogicWidth, float fLogicHeight)
    {
        camera.rect = _calculateScaledRect(fLogicWidth, fLogicHeight);
    }
    static private Rect _calculateScaledRect(float fLogicWidth, float fLogicHeight)
    {
        //--- Screen resolution ---//
        float w = Screen.width;
        float h = Screen.height;
        //--- target logic resolution ---//
        float destW = fLogicWidth;
        float destH = fLogicHeight;
        float fx = 0f;
        float fy = 0f;
        float fw = 1f;
        float fh = 1f;
        //--- 现在宽高比 不足, 即 宽度不足，高度足够，那么按照 不足的为基准，调整足够的（按新的宽度来调） ---//
        if (destW / destH > w / h)
        {
            //高度缩小 ，宽度不变为1
            fw = 1f;
            fh = destH * w / destW / h;
            fy = (1f - fh) / 2f; //为了保证视口在屏幕中央， 需要调整视口高度
        }
        else if (destW / destH < w / h)
        {
            //宽度缩小，高度不变
            fh = 1f;
            fw = destW * h / destH / w;
            fx = (1f - fw) / 2f;
        }
        return new Rect(fx, fy, fw, fh);
    }
    static public T Instantiate<T>(string strResPath, Transform parent = null)
    {
        var obj = Instantiate(strResPath, parent);
        Debug.Assert(obj != null, "Resource " + strResPath + " load failed!");
        var requestType = obj.GetComponent<T>();
        Debug.Assert(requestType != null, "Resource " + strResPath + " doesn't contains " + typeof(T).Name + " component!");
        return requestType;
    }
    static public GameObject Instantiate(string strResPath, Transform parent = null)
    {
        var resObj = Resources.Load(strResPath) as GameObject;
        if (resObj == null)
        {
            return null;
        }
        var requestObj = GameObject.Instantiate(resObj, Vector3.zero, Quaternion.identity) as GameObject;
        if (parent != null)
        {
            requestObj.transform.SetParent(parent);
            requestObj.transform.localScale = Vector3.one;
            requestObj.transform.localPosition = Vector3.zero;
            requestObj.transform.localRotation = Quaternion.identity;
        }
        return requestObj;
    }
    static public System.DateTime GetDateTime(int seconds)
    {
        System.DateTime time = new System.DateTime(1970, 1, 1);
        time = time.AddSeconds(seconds);
        return time;
    }
    static public T ParseEnum<T>(string value)
    {
        return (T)System.Enum.Parse(typeof(T), value);
    }
    static public T GetRootObject<T>(string name) where T : Component
    {
        var rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (var obj in rootObjects)
        {
            if (obj.name == name)
            {
                return obj.GetComponent<T>();
            }
        }
        return null;
    }
    static public int PlaySound(string strSoundName, bool bLoop = false)
    {
        var clip = ResourceMgr.Instance.GetAudioClip(strSoundName);
        int nIndex = SoundMgr.Instance.PlayClip(clip, bLoop);
        return nIndex;
    }
    static public int PlaySound(string strName, float fVolume, bool bLoop = false)
    {
        var clip = ResourceMgr.Instance.GetAudioClip(strName);
        int nIndex = SoundMgr.Instance.PlayClip(clip, bLoop);
        if (nIndex != -1)
        {
            SoundMgr.Instance.SetVolume(nIndex, fVolume);
        }
        return nIndex;
    }
    static public void StopSound(int iSndIndex)
    {
        SoundMgr.Instance.StopChannel(iSndIndex);
    }
    static public void PauseSound(int iSndIndex)
    {
        SoundMgr.Instance.PauseChannel(iSndIndex);
    }
    static public void ResumeSound(int iSndIndex)
    {
        SoundMgr.Instance.ResumeChannel(iSndIndex);
    }
    static public void ShowMessageBox(int idTips)
    {
        UIManager.Instance.ShowWindow<MessageBox>("messageBox", ArgList.Create(InfoManager.Instance.GetTips(idTips)), false);
    }
}
