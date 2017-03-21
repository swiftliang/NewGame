using UnityEngine;
using System.Collections;
//using Holoville.HOTween;
using System.Diagnostics;
public sealed class Helper
{
    public static T LoadPrefab<T>(string prefabName) where T : MonoBehaviour
    {
        return (T)Resources.Load("Prefab/" + prefabName, typeof(T));
    }

    public static GameObject LoadPrefab(string prefabName)
    {
        //UnityEngine.Debug.Log("prefabName=" + prefabName);
        DebugLogger.Log(eLogType.LT_LOGIC, "prefabName=" + prefabName);
        return (GameObject)Resources.Load("Prefab/" + prefabName, typeof(GameObject));
    }
    public static void Normalize(Transform t)
    {
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
        t.localScale = Vector3.one;
    }
    public static IEnumerator Wait(GameObject obj)
    {
        while (obj)
        {
            yield return 0;
        }
    }
    public static IEnumerator DelayAction(float delay, System.Action action)
    {
        yield return new WaitForSeconds(delay);
        action();
    }
    //public static IEnumerator WaitForTweener(Tweener tw, float time)
    //{
    //    while (Time.realtimeSinceStartup < time)
    //    {
    //        yield return 0;
    //    }
    //    if( tw != null )
    //        tw.Complete();
    //}
    public static IEnumerator WaitUntill(float time)
    {
        while (Time.realtimeSinceStartup < time)
        {
            yield return 0;
        }
    }
    public static void SetActive(GameObject obj, bool state)
    {
        obj.SetActive(state);
    }
    public static string Color2Hex(Color c)
    {
        return "[" + ((int)(c.r * 255)).ToString("X2") + ((int)(c.g * 255)).ToString("X2") + ((int)(c.b * 255)).ToString("X2") + "]";
    }
    public static bool HasLongKey(string key)
    {
        string keyHI = key + "HIWORD";
        string keyLO = key + "LOWORD";
        return PlayerPrefs.HasKey(keyHI) && PlayerPrefs.HasKey(keyLO);
    }
    public static void SetLong(string key, long value)
    {
        string keyHI = key + "HIWORD";
        string keyLO = key + "LOWORD";
        int hi = (int)(value >> 32);
        int lo = (int)(value & 0xffffffff);
        PlayerPrefs.SetInt(keyHI, hi);
        PlayerPrefs.SetInt(keyLO, lo);
        PlayerPrefs.Save();
    }
    public static long GetLong(string key, long defValue)
    {
        string keyHI = key + "HIWORD";
        string keyLO = key + "LOWORD";
        int dhi = (int)(defValue >> 32);
        int dlo = (int)(defValue & 0xffffffff);
        uint hi = (uint)PlayerPrefs.GetInt(keyHI, dhi);
        uint lo = (uint)PlayerPrefs.GetInt(keyLO, dlo);
        return ((long)hi << 32) | (long)lo;
    }
    public static byte[] BitArrayToByteArray(BitArray bits)
    {
        byte[] ret = new byte[bits.Length / 8];
        bits.CopyTo(ret, 0);
        return ret;
    }
    public static ushort MakeWord(byte b1, byte b2)
    {
        return (ushort)(b1 << 8 | b2);
    }
    public static byte HByte(ushort s)
    {
        return (byte)(s >> 8);
    }
    public static byte LByte(ushort s)
    {
        return (byte)(s & 0x00ff);
    }
    public static void LogLine(int skipFrames = 1)
    {
        StackFrame callStack = new StackFrame(skipFrames, true);
        UnityEngine.Debug.Log(callStack.GetFileName() + ":" + callStack.GetFileLineNumber());
    }
    public static void LogFunc(int skipFrames = 1)
    {
        StackFrame callStack = new StackFrame(skipFrames, true);
        UnityEngine.Debug.Log(callStack.GetMethod().DeclaringType.Name + ":" + callStack.GetMethod().Name);
    }
    public static void LogVar<T>(T var)
    {
        if (var == null) return;
        var param = typeof(T).GetProperties()[0];
        UnityEngine.Debug.Log("Parameter: '" + param.Name + "' = " + param.GetValue(var, null));
    }
    public static void LogWithFunc(string msg, int skipFrames = 1)
    {
        StackFrame callStack = new StackFrame(skipFrames, true);
        UnityEngine.Debug.Log(callStack.GetMethod().DeclaringType.Name + ":" + callStack.GetMethod().Name + ":" + msg);
    }
    public sealed class GameObjectGroup
    {
        GameObject[] gameObjects;
        public GameObjectGroup(params GameObject[] gameObjects)
        {
            this.gameObjects = gameObjects;
        }
        public void SetActive(GameObject gameObject, bool active)
        {
            foreach (var go in gameObjects)
            {
                if (go == gameObject)
                    go.SetActive(active);
                else
                    go.SetActive(!active);
            }
        }
        public void SetActive(string name, bool active)
        {
            foreach (var go in gameObjects)
            {
                if (go.name == name)
                    go.SetActive(active);
                else
                    go.SetActive(!active);
            }
        }
    }
}
