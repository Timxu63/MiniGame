using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
#if !BATTLE_SERVER
using UnityEngine;
using Object = UnityEngine.Object;
#endif

public enum LogLevel
{
    Info,
    Warning,
    Error,
    Exception
}
public interface ILogBackend
{
    void Log(object message, string memberName, string filePath, int lineNumber, LogLevel level);
#if UNITY_EDITOR
    bool LogEnabled { get; set; }
    void Log(object message, Object context, string memberName, string filePath, int lineNumber, LogLevel level);
#endif
}
public static class Logger
{
    private static readonly List<ILogBackend> _backends = new List<ILogBackend>();

    static Logger()
    {
#if UNITY_EDITOR
        _backends.Add(new EditorLogBackend());
#endif
    }
    public static void Log(string message, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0) => InternalLog(message, LogLevel.Info, memberName, filePath, lineNumber);

    public static void EditorLog(string message, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0) => InternalLog(message, LogLevel.Info, memberName, filePath, lineNumber);

    // ======== Log Warning ========
    public static void LogWarning(string message, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0) => InternalLog(message, LogLevel.Warning, memberName, filePath, lineNumber);

    // ======== Log Error ========
    public static void LogError(string message, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0) => InternalLog(message, LogLevel.Error, memberName, filePath, lineNumber);
    private static void InternalLog(object message, LogLevel level, string memberName, string filePath, int lineNumber)
    {
        // 分发
        foreach (var bk in _backends)
            bk.Log(message, memberName, filePath, lineNumber, level);
    }
}