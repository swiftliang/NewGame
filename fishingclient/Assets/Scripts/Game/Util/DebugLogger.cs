using UnityEngine;
using System.Collections;
using Fishing;
public enum eLoglevel
{
    LL_CLOSE = 0,
    LL_NORMAL = 1 << 0,
    LL_WARNING = 1 << 1,
    LL_ERROR = 1 << 2,
    LL_ALL = LL_NORMAL | LL_WARNING | LL_ERROR
}
public enum eLogType
{
    LT_LOGIC = 1 << 0,
    LT_NET = 1 << 1,
    LT_UI = 1 << 2,
    LT_SECEN = 1 << 3,
    LT_ALL = LT_LOGIC | LT_NET | LT_SECEN | LT_UI
}
public static class DebugLogger
{
    public static eLoglevel eLogLevel = eLoglevel.LL_NORMAL;
    public static eLogType eLogType = eLogType.LT_LOGIC/* | eLogType.LT_NET*/;
    public static bool CheckLog(eLoglevel elevel, eLogType eType)
    {
        if (DebugLogger.eLogLevel == eLoglevel.LL_CLOSE)
            return false;
        if ((DebugLogger.eLogLevel & elevel) == elevel)
        {
            if ((DebugLogger.eLogType & eType) == eType)
            {
                return true;
            }
        }
        return false;
    }
    public static void Log(eLogType eType, string str/*, params object[] args*/)
    {
        if (!CheckLog(eLoglevel.LL_NORMAL, eType))
        {
            return;
        }
        Debug.Log(":-)  " + str);
    }
    public static void LogError(eLogType eType, string str/*, params object[] args*/)
    {
        if (!CheckLog(eLoglevel.LL_ERROR, eType))
        {
            return;
        }
        Debug.LogError("(¯ □ ¯)   " + str);
    }
    public static void LogWarning(eLogType eType, string str/*, params object[] args*/)
    {
        if (!CheckLog(eLoglevel.LL_WARNING, eType))
        {
            return;
        }
        Debug.LogWarning("(‧_‧？)    " + str);
    }
}
