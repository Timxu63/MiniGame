using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Profiling;

public static class P
{
    [Conditional("ProfilerControl")]
    public static void OpenProfiler(string profilerName)
    {
#if !BATTLE_SERVER
        Profiler.enabled = true;
        Profiler.BeginSample(profilerName);
#endif
    }

    [Conditional("ProfilerControl")]
    public static void CloseProfiler()
    {
#if !BATTLE_SERVER
        Profiler.EndSample();
        Profiler.enabled = false;
#endif
    }

    [Conditional("ProfilerBinaryLog")]
    public static void OpenBinaryLog()
    {
#if !BATTLE_SERVER
        Profiler.logFile = Application.persistentDataPath + "/profilerLog.raw"; // 指定日志文件路径
        Profiler.enableBinaryLog = true;
#endif
    }

    [Conditional("ProfilerBinaryLog")]
    public static void CloseBinaryLog()
    {
#if !BATTLE_SERVER
        Profiler.enableBinaryLog = false;
        Profiler.logFile = null;
#endif
    }

    [Conditional("Profiler")]
    public static void BeginSample(string name)
    {
#if !BATTLE_SERVER
        Profiler.BeginSample(name);
#endif
    }

    [Conditional("Profiler")]
    public static void EndSample()
    {
#if !BATTLE_SERVER
        Profiler.EndSample();
#endif
    }

    [Conditional("ProfilerGetType")]
    public static void BeginSampleGetType(string title, object o, string tag)
    {
#if !BATTLE_SERVER
        Profiler.BeginSample(o.GetType().Name + tag);
#endif
    }

    [Conditional("ProfilerGetType")]
    public static void BeginSampleGetType(object o, string tag)
    {
#if !BATTLE_SERVER
        Profiler.BeginSample(o.GetType().Name + tag);
#endif
    }

    [Conditional("ProfilerGetType")]
    public static void EndSampleGetType()
    {
#if !BATTLE_SERVER
        Profiler.EndSample();
#endif
    }

    [Conditional("ProfilerGetType")]
    public static void BeginSampleDelegate(string tag, Delegate handler)
    {
#if !BATTLE_SERVER
        if (handler == null)
        {
            return;
        }

        // 获取委托指向的方法信息
        var methodInfo = handler.Method;

        // 获取委托目标类实例（如果是静态方法则为null）
        var targetInstance = handler.Target;

        // 如果目标类实例存在，获取其类型
        Type targetType = targetInstance?.GetType();
        string typeName;
        string methodName = methodInfo?.Name;
        if (targetType != null)
        {
            typeName = targetType.Name;
        }
        else
        {
            typeName = "Static";
        }

        typeName = StringTool.StringBuilder(typeName, ":", methodName);
        Profiler.BeginSample(tag + typeName);
#endif
    }

    [Conditional("ProfilerGetType")]
    public static void EndSampleDelegate()
    {
#if !BATTLE_SERVER
        Profiler.EndSample();
#endif
    }
}