using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Pool;

public class ArrayTool
{
    public static T[] MergeArrays<T>(T[] a, T[] b)
    {
        if (a == null) return b;
        if (b == null) return a;
        return a.Concat(b).ToArray();
    }

// 辅助方法：合并两个列表（null 安全）
    public static List<T> MergeLists<T>(List<T> a, List<T> b)
    {
        if (a == null) return b;
        if (b == null) return a;
        var result = new List<T>(a);
        result.AddRange(b);
        return result;
    }

    /// <summary>
    /// 去重连接
    /// </summary>
    public static T[] MergeDistinctArrays<T>(T[] source, T[] other)
    {
        // source 为空或长度0，直接返回 other（可能为 null、也可能是已有数组）
        if (source == null || source.Length == 0)
            return other;

        // other 为空，直接返回 source
        if (other == null || other.Length == 0)
            return source;

        // 合并去重：用池化的 HashSet<T>
        var set = HashSetPool<T>.Get();
        try
        {
            // 先加 source，再加 other
            foreach (var v in source)
                set.Add(v);
            foreach (var v in other)
                set.Add(v);

            // 分配正好大小的结果数组
            var result = set.ToArray();
            return result;
        }
        finally
        {
            // 一定释放回池
            HashSetPool<T>.Release(set);
        }
    }
}