using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public static class DrawLineTool
{
    #region DrawLine

#if !BATTLE_SERVER
    [Conditional("UnityLog")]
    public static void DrawLine(UnityEngine.Vector3 start, UnityEngine.Vector3 end)
    {
        UnityEngine.Debug.DrawLine(start, end);
    }

    [Conditional("UnityLog")]
    public static void DrawLine(UnityEngine.Vector3 start, UnityEngine.Vector3 end, UnityEngine.Color color)
    {
        UnityEngine.Debug.DrawLine(start, end, color);
    }

    [Conditional("UnityLog")]
    public static void DrawLine(UnityEngine.Vector3 start, UnityEngine.Vector3 end, UnityEngine.Color color, float duration)
    {
        UnityEngine.Debug.DrawLine(start, end, color, duration);
    }
#endif

    #endregion
}
