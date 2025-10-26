using System.Collections.Concurrent;
using System;
using System.Collections.Generic;
using System.Reflection;

public interface IResettable
{
    /// <summary>
    /// 清理或重置对象状态，以便下次复用。
    /// </summary>
    void Reset();
}

public static class Pool
{
    // 缓存区
    static readonly Dictionary<string, Func<object>> _creatorCache = new();

    /// <summary>
    /// 根据完全限定类名，从 Pool&lt;T&gt; 取得一个实例。
    /// 该类型必须实现 IResettable 并有无参构造函数。
    /// </summary>
    public static object CreateByClassName(string className)
    {
        if (_creatorCache.TryGetValue(className, out var creator))
        {
            if (creator != null)
            {
                return creator();
            }
            else
            {
                return null;
            }
        }

        // 第一次：反射构造委托
        var type = Type.GetType(className);
        if (type == null)
        {
            _creatorCache[className] = null;
            return null;
        }

        var poolType = typeof(Pool<>).MakeGenericType(type);
        var getMethod = poolType.GetMethod("Get", BindingFlags.Public | BindingFlags.Static);
        // 把反射调用转成委托
        var del = (Func<object>)Delegate.CreateDelegate(typeof(Func<object>), getMethod);

        _creatorCache[className] = del;
        return del();
    }

    /// <summary>
    /// 泛型版：同时指定返回类型约束
    /// </summary>
    public static T CreateByClassName<T>(string className) where T : class, IResettable
    {
        var obj = CreateByClassName(className);
        if (obj != null)
        {
            if (obj is T t)
                return t;

            throw new InvalidCastException($"从池里取出的 {className} 不能转换到 {typeof(T).Name}");
        }
        else
        {
            return null;
        }
    }
}

public static class Pool<T> where T : class, IResettable, new()
{
    // 线程安全的栈，用于存放可复用实例
    private static readonly ConcurrentStack<T> _stack = new ConcurrentStack<T>();

    /// <summary>
    /// 从池中获取一个对象；若池中无可用实例，则 new 出一个新的。
    /// </summary>
    public static T Get()
    {
        if (_stack.TryPop(out var item))
            return item;
        return new T();
    }

    /// <summary>
    /// 将对象归还池中，并调用 Reset() 重置其状态。
    /// </summary>
    public static void Release(T item)
    {
        item.Reset();
        _stack.Push(item);
    }
}