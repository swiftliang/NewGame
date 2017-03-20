using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Helps
{
    public static T LoadPrefab<T>(string prefabname) where T : MonoBehaviour
    {
        return (T)Resources.Load("prefab" + prefabname, typeof(T));
    }

    public static void ShowTipsWindow(string strTip)
    {
        TipsWindow tw = LoadPrefab<TipsWindow>("/ui/tipsWindows");
        tw.SetTip(strTip);
        tw.startDestroy(3);
    }
}
