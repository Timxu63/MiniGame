#if UNITY_EDITOR
using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object; // 为 StringTool

public class EditorLogBackend : ILogBackend
{
    public bool LogEnabled
    {
        get { return Debug.unityLogger.filterLogType != LogType.Error; }
        set
        {
#if BATTLE_SERVER
            isLog = value;
#else
            if (value)
            {
                Debug.unityLogger.filterLogType = LogType.Log;
            }
            else
            {
                Debug.unityLogger.filterLogType = LogType.Error;
            }
#endif
        }
    }

    [DebuggerHidden, DebuggerStepThrough]
    public void Log(object message, string memberName, string filePath, int lineNumber, LogLevel level)
    {
        if (level == LogLevel.Exception && message is Exception ex)
        {
            Debug.LogException(ex);
            return;
        }

        var sb = StringTool.GetStringBuilder();
        var fileName = Path.GetFileName(filePath);
        sb.Append(message ?? "NULL").Append("\n").Append('[').Append(FrameCounter.CurrentFrame).Append(']').Append(fileName).Append(':').Append(lineNumber).Append("(").Append(memberName).Append(")");
        //sb.Append('[').Append(FrameCounter.CurrentFrame).Append("][").Append(memberName).Append("]").Append(message ?? "NULL").Append("\n").Append(filePath).Append(':').Append(lineNumber);
        string final = sb.ToString();
        switch (level)
        {
            case LogLevel.Warning:
                Debug.LogWarning(final);
                break;
            case LogLevel.Error:
            case LogLevel.Exception:
                Debug.LogError(final);
                break;
            default:
                Debug.Log(final);
                break;
        }
    }

    public void Log(object message, Object context, string memberName, string filePath, int lineNumber, LogLevel level)
    {
        if (level == LogLevel.Exception && message is Exception ex)
        {
            Debug.LogException(ex);
            return;
        }

        var sb = StringTool.GetStringBuilder();
        sb.Append('[').Append(FrameCounter.CurrentFrame).Append("][").Append(memberName).Append("]").Append(message ?? "NULL").Append("\n").Append(filePath).Append(':').Append(lineNumber);
        string final = sb.ToString();

        switch (level)
        {
            case LogLevel.Warning:
                Debug.LogWarning(final, context);
                break;
            case LogLevel.Error:
            case LogLevel.Exception:
                Debug.LogError(final, context);
                break;
            default:
                Debug.Log(final, context);
                break;
        }
    }
}
#endif