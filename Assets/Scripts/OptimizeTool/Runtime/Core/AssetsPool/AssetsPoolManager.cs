using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
#if UNITY_IOS
using UnityEngine.iOS;
#endif
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using Debug = UnityEngine.Debug;
using Task = System.Threading.Tasks.Task;
using Object = UnityEngine.Object;

public enum LocalOrWorld
{
    Local,
    World,
}


[Flags]
public enum StatusEnum
{
    /// <summary>
    /// 初次加载成功(一般只有在这种情况下，需要存入字典，或者GetAsset)
    /// </summary>
    FristLoadToSuccess,

    /// <summary>
    /// 初次加载失败
    /// </summary>
    FristLoadToFailed,

    /// <summary>
    /// 已经发起过加载了，等待加载直到成功
    /// </summary>
    WaitUntilToSuccess,

    /// <summary>
    /// 已经发起过加载了，等待加载直到失败
    /// </summary>
    WaitUntilToFailed,

    /// <summary>
    /// 已经加载过了，并且是成功的
    /// </summary>
    Success,

    /// <summary>
    /// 已经加载过了，并且是失败的
    /// </summary>
    Failed,

    /// <summary>
    /// 加载中
    /// </summary>
    Loading,

    /// <summary>
    /// 不可用
    /// </summary>
    UnValid,
}

#region AssetData

public struct AssetData
{
    public AssetData(string path, AsyncOperationHandle handle, bool isPersistence, bool isGroupAsset, int count = 1)
    {
        this.path = path;
        this.handle = handle;
        this.count = count;
        asset = null;
        this.isGroupAsset = isGroupAsset;
        this.isPersistence = isPersistence;
    }


    private string path;
    private AsyncOperationHandle handle;
    private UnityEngine.Object asset;
    private int count;
    private bool isGroupAsset;
    private bool isPersistence;

    public bool IsPersistence
    {
        get { return isPersistence; }
    }

    public bool IsGroupAsset
    {
        get { return isGroupAsset; }
    }

    public AsyncOperationHandle Handle
    {
        get { return handle; }
    }

    public string Path
    {
        get { return path; }
    }

    public UnityEngine.Object Result
    {
        get
        {
            if (asset)
            {
                return asset;
            }
            else
            {
                if (!isGroupAsset)
                {
                    if (handle.IsValid() && handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        asset = (UnityEngine.Object)handle.Result;
                    }
                }
                else
                {
                    // 对于组资源，需要从 handle.Result 中根据路径找到对应的资源
                    if (handle.IsValid() && handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        // 尝试通过 AssetsPoolManager 的 HandlePaths 获取路径索引映射
                        if (AssetsPoolManager.Instance.HandlePaths.TryGetValue(handle, out var pathIndexMap))
                        {
                            // 直接通过路径获取索引
                            if (pathIndexMap.TryGetValue(path, out int index))
                            {
                                // 从组结果中获取对应索引的资源
                                var groupResult = handle.Result;
                                if (groupResult is System.Collections.IList list && index < list.Count)
                                {
                                    asset = (UnityEngine.Object)list[index];
                                }
                            }
                        }
                    }
                }

                return asset;
            }
        }
        set { asset = value; }
    }

    public int Count
    {
        get { return count; }
    }

    public bool IsDone
    {
        get { return handle.IsDone; }
    }


    public AsyncOperationStatus Status
    {
        get { return handle.Status; }
    }

    public Exception OperationException
    {
        get { return handle.OperationException; }
    }

    public bool IsValid()
    {
        return handle.IsValid();
    }

    public void AddChannel()
    {
        count++;
    }

    public void RemoveChannel()
    {
        count--;
        if (count < 0)
        {
            count = 0;
        }
    }

    public void ClearChannel()
    {
        count = 0;
    }
}

public struct AssetData<T> where T : UnityEngine.Object
{
    public static implicit operator AssetData(AssetData<T> assetData)
    {
        return new AssetData();
    }
}

#endregion

public class AssetsPoolManager : MonoSingleton<AssetsPoolManager>
{
    [Header("对象池调试")] [Tooltip("获取对象")] public bool isOpenGetObjectDebug = false;
    [Tooltip("对象还回对象池")] public bool isOpenGiveBackObjectDebug = false;
    [Tooltip("对象彻底销毁")] public bool isOpenDestroyObjectDebug = false;
    [Header("资源调试")] public int loadingCount;
    [Tooltip("加载资源调试")] public bool isOpenLoadAssetDebug = false;
    [Tooltip("释放资源调试")] public bool isOpenReleaseAssetDebug = false;
    [Header("追踪资源调试")] public bool isOpenTraceAssetsDebug = false;
    [Tooltip("需要追踪的资源路径")] public List<string> traceAssetPaths = new List<string>();
    [Header("WebGL调试")] public bool isOpenWebGLDebug = false;
    [Header("内存调试")] public bool isOpenMemeryDebug = false;

    private float _nextPollTime = 0f;
    private float memoryCheckInterval = 5f; // 可配置，单位 秒
    private float _nextMemoryCheckTime = 0f;
    private float memoryThresholdMB = 1024f; // 可配置，单位 MB
    private float totalSystemMemoryMB = 2048f; // 设备总内存

    #region —— 为调试面板暴露的公有接口 ——

    /// <summary>
    /// 1. 拿到所有已缓存的 AssetData（原来 private Dictionary<string, AssetData> cachedResources）
    ///    只读地返回给调试窗口使用。
    /// </summary>
    public IReadOnlyDictionary<string, AssetData> CachedResources
    {
        get { return cachedResources; }
    }

    /// <summary>
    /// 2. 拿到“每个 AsyncOperationHandle 对应的所有路径列表”
    ///    （原来 private Dictionary<AsyncOperationHandle, List<string>> _handlePaths）
    ///    用于把同一个 Handle 下的多个路径归为一组。
    /// </summary>
    public IReadOnlyDictionary<AsyncOperationHandle, Dictionary<string, int>> HandlePaths
    {
        get { return _handlePaths; }
    }

    /// <summary>
    /// 3. 拿到当前正在 LRU 列表中管理的所有 Handle（原来 private Dictionary<AsyncOperationHandle, ...> _lruNodes 的 Keys）
    ///    调试窗口只需要知道某个 Handle 是否在 LRU 中，用来判断该资源是否“真正未被使用”。
    /// </summary>
    public IReadOnlyCollection<AsyncOperationHandle> LRUHandles
    {
        get { return _lruNodes.Keys; }
    }

    /// <summary>
    /// 4. 拿到实例池中“未使用”对象的超时时间阈值（原来 private float instanceTimeout）
    ///    调试窗口里要算“Countdown”时，需要知道这个值。
    /// </summary>
    public float InstanceTimeout
    {
        get { return instanceTimeout; }
    }

    /// <summary>
    /// 5. 拿到当前对象池（未使用）里的所有对象队列（原来 private Dictionary<string, Queue<(GameObject, float)>> _objectPool）
    ///    调试窗口需要遍历它来显示“未使用”实例列表。
    /// </summary>
    public IReadOnlyDictionary<string, Queue<(GameObject, float)>> ObjectPool
    {
        get { return _objectPool; }
    }

    /// <summary>
    /// 6. 拿到当前所有“正在使用”的实例对应的映射（原来 private Dictionary<int, string> _objectToPath）
    ///    键是 InstanceID，值是该实例对应的资源路径。调试窗口用它来找“正在使用”的 GameObject。
    /// </summary>
    public IReadOnlyDictionary<GameObject, string> ObjectToPath
    {
        get { return _objectToPath; }
    }

    /// <summary>
    /// 7. 把原来私有的 ReleaseAddressable(handle) 方法改为公有接口：ReleaseHandle(handle)
    ///    调试窗口点“×”或“Release”时直接调用它。
    /// </summary>
    public void ReleaseHandle(AsyncOperationHandle handle)
    {
        ReleaseAddressable(handle);
    }

    #endregion

    public void Awake()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            return;
        }
#endif
        trParentRoot = new GameObject("PoolManager").transform;
        trParentRoot.position = new Vector3(2000f, 2000f, 2000f);
        Object.DontDestroyOnLoad(trParentRoot);
        trParentRoot.gameObject.SetActive(false);

        InitializeMemoryThreshold();
        if (!isOpenTraceAssetsDebug)
        {
            traceAssetPaths.Clear();
        }
    }

    void Update()
    {
        SilentCache();
        float currentTime = Time.time;
        if (currentTime < _nextPollTime)
        {
            return;
        }

        _nextPollTime = currentTime + 1;
        CheckInstanceUseTime();
        //CheckMemory();
    }

    /// <summary>
    /// 初始化设备内存阈值
    /// </summary>
    public void InitializeMemoryThreshold()
    {
#if WX_MINIGAME
        totalSystemMemoryMB = 2048f;
        memoryThresholdMB = 1024f;
#elif UNITY_IOS
        totalSystemMemoryMB = GetIosSystemMemory();
        // iOS 设备基于型号分类
        memoryThresholdMB = GetIosMemoryThreshold();
#elif UNITY_ANDROID
        totalSystemMemoryMB = SystemInfo.systemMemorySize; // 获取总内存 (MB)
        // Android 设备基于系统提供的内存值来计算
        memoryThresholdMB = GetAndroidMemoryThreshold();
#else
        totalSystemMemoryMB = 3000f; // 获取总内存 (MB)
        // 其他平台，默认设置一个较保守的阈值
        memoryThresholdMB = Mathf.Min(totalSystemMemoryMB * 0.5f, 1024f); // 50% 或 1GB
#endif
        if (isOpenReleaseAssetDebug)
        {
            Debug.Log($"[Memory Management] 设备总内存: {totalSystemMemoryMB}MB, 设置内存阈值: {memoryThresholdMB}MB");
        }
    }
#if UNITY_IOS
    /// <summary>
    /// 获取 iOS 设备（iPhone / iPad）的 RAM 大小 (单位: MB)
    /// </summary>
    private float GetIosSystemMemory()
    {
        DeviceGeneration generation = Device.generation;

        switch (generation)
        {
            // **iPhone 设备**
            case DeviceGeneration.iPhone:
            case DeviceGeneration.iPhone3G:
            case DeviceGeneration.iPhone3GS:
                return 128f; // 128MB (旧设备)
            case DeviceGeneration.iPhone4:
            case DeviceGeneration.iPhone4S:
                return 512f; // 512MB
            case DeviceGeneration.iPhone5:
            case DeviceGeneration.iPhone5C:
            case DeviceGeneration.iPhone5S:
                return 1024f; // 1GB
            case DeviceGeneration.iPhone6:
            case DeviceGeneration.iPhone6Plus:
                return 1024f; // 1GB
            case DeviceGeneration.iPhone6S:
            case DeviceGeneration.iPhone6SPlus:
            case DeviceGeneration.iPhoneSE1Gen:
                return 2048f; // 2GB
            case DeviceGeneration.iPhone7:
            case DeviceGeneration.iPhone7Plus:
                return 2048f; // 2GB (7), 3GB (7 Plus)
            case DeviceGeneration.iPhone8:
            case DeviceGeneration.iPhoneX:
                return 3000f; // 3GB
            case DeviceGeneration.iPhone8Plus:
            case DeviceGeneration.iPhoneXS:
            case DeviceGeneration.iPhoneXSMax:
                return 4000f; // 4GB
            case DeviceGeneration.iPhoneXR:
            case DeviceGeneration.iPhone11:
                return 4000f; // 4GB
            case DeviceGeneration.iPhone11Pro:
            case DeviceGeneration.iPhone11ProMax:
                return 6000f; // 6GB
            case DeviceGeneration.iPhoneSE2Gen:
            case DeviceGeneration.iPhone12Mini:
                return 4000f; // 4GB
            case DeviceGeneration.iPhone12:
            case DeviceGeneration.iPhone12Pro:
            case DeviceGeneration.iPhone13Mini:
            case DeviceGeneration.iPhone13:
            case DeviceGeneration.iPhone14:
            case DeviceGeneration.iPhone14Plus:
                return 6000f; // 6GB
            case DeviceGeneration.iPhone12ProMax:
            case DeviceGeneration.iPhone13Pro:
            case DeviceGeneration.iPhone13ProMax:
            case DeviceGeneration.iPhone14Pro:
            case DeviceGeneration.iPhone14ProMax:
                return 8000f; // 8GB
            case DeviceGeneration.iPhoneSE3Gen:
                return 4000f; // 4GB
            case DeviceGeneration.iPhone15:
            case DeviceGeneration.iPhone15Plus:
                return 6000f; // 6GB
            case DeviceGeneration.iPhone15Pro:
            case DeviceGeneration.iPhone15ProMax:
                return 8000f; // 8GB

            // **iPad 设备**
            case DeviceGeneration.iPad1Gen:
                return 256f; // 256MB
            case DeviceGeneration.iPad2Gen:
            case DeviceGeneration.iPadMini1Gen:
                return 512f; // 512MB
            case DeviceGeneration.iPad3Gen:
            case DeviceGeneration.iPad4Gen:
                return 1024f; // 1GB
            case DeviceGeneration.iPadAir1:
            case DeviceGeneration.iPadMini2Gen:
            case DeviceGeneration.iPadMini3Gen:
                return 1024f; // 1GB
            case DeviceGeneration.iPadAir2:
            case DeviceGeneration.iPadMini4Gen:
                return 2048f; // 2GB
            case DeviceGeneration.iPad5Gen:
            case DeviceGeneration.iPad6Gen:
            case DeviceGeneration.iPadPro1Gen:
            case DeviceGeneration.iPadPro10Inch1Gen:
                return 3000f; // 3GB
            case DeviceGeneration.iPadPro2Gen:
            case DeviceGeneration.iPadPro10Inch2Gen:
                return 4000f; // 4GB
            case DeviceGeneration.iPadPro3Gen:
            case DeviceGeneration.iPadPro11Inch:
            case DeviceGeneration.iPadAir3Gen:
                return 6000f; // 6GB
            case DeviceGeneration.iPadMini5Gen:
                return 3000f; // 3GB
            case DeviceGeneration.iPad7Gen:
            case DeviceGeneration.iPad8Gen:
            case DeviceGeneration.iPad9Gen:
                return 3000f; // 3GB
            case DeviceGeneration.iPadPro4Gen:
            case DeviceGeneration.iPadPro11Inch2Gen:
                return 6000f; // 6GB
            case DeviceGeneration.iPadAir4Gen:
                return 4000f; // 4GB
            case DeviceGeneration.iPadMini6Gen:
                return 4000f; // 4GB
            case DeviceGeneration.iPadPro5Gen:
            case DeviceGeneration.iPadPro11Inch3Gen:
                return 8000f; // 8GB
            case DeviceGeneration.iPadAir5Gen:
                return 8000f; // 8GB
            case DeviceGeneration.iPadPro6Gen:
            case DeviceGeneration.iPadPro11Inch4Gen:
                return 16000f; // 16GB
            case DeviceGeneration.iPad10Gen:
                return 4000f; // 4GB
            default:
                return 4000f; // 默认 4GB
        }
    }
#endif
    /// <summary>
    /// 根据 iOS 设备类型设置合理的内存阈值
    /// </summary>
    private float GetIosMemoryThreshold()
    {
        if (totalSystemMemoryMB <= 3000) return 1024f; // iOS 3GB RAM -> 1GB
        if (totalSystemMemoryMB <= 4000) return 1200f; // iOS 4GB RAM -> 1.2GB
        if (totalSystemMemoryMB <= 6000) return 1600f; // iOS 6GB RAM -> 1.6GB
        if (totalSystemMemoryMB <= 8000) return 2200f; // iOS 8GB RAM -> 2.2GB
        return Mathf.Min(totalSystemMemoryMB * 0.4f, 3000f); // 40% 或最多 3GB
    }

    /// <summary>
    /// 根据 Android 设备的总内存动态设置合理的内存阈值
    /// </summary>
    private float GetAndroidMemoryThreshold()
    {
        if (totalSystemMemoryMB <= 2000) return 600f; // Android 2GB RAM -> 600MB
        if (totalSystemMemoryMB <= 3000) return 900f; // Android 3GB RAM -> 900MB
        if (totalSystemMemoryMB <= 4000) return 1200f; // Android 4GB RAM -> 1.2GB
        if (totalSystemMemoryMB <= 6000) return 1600f; // Android 6GB RAM -> 1.6GB
        if (totalSystemMemoryMB <= 8000) return 2200f; // Android 8GB RAM -> 2.2GB
        return Mathf.Min(totalSystemMemoryMB * 0.4f, 3000f); // 40% 或最多 3GB
    }

    #region ObjectPool

    private float instanceTimeout = 60f;
    public Transform trParentRoot;

    /// <summary>
    /// 外界没用到的
    /// </summary>
    private Dictionary<string, Queue<(GameObject, float)>> _objectPool = new Dictionary<string, Queue<(GameObject, float)>>();

    /// <summary>
    /// 外界用到的
    /// </summary>
    private Dictionary<GameObject, string> _objectToPath = new Dictionary<GameObject, string>(); // 对象与路径的映射

    public GameObject Get(GameObject prefab, Transform parent, bool isResetTransform = false)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            throw null;
        }
#endif

        //P.BeginSample("AssetsPoolManager.Get");
        // 检查对象池中是否有可用对象
        if (!_objectPool.TryGetValue(prefab.name, out var objectQueue))
        {
            objectQueue = new Queue<(GameObject, float)>();
            _objectPool.Add(prefab.name, objectQueue);
        }

        if (objectQueue.Count > 0)
        {
            var (target, _) = objectQueue.Dequeue();
            if (isResetTransform)
            {
                target.transform.SetParentAndReset(parent);
            }
            else
            {
                target.transform.SetParent(parent, false);
            }

            // 记录对象与路径的映射
            _objectToPath[target] = prefab.name;
            if (isOpenGetObjectDebug)
            {
                Debug.Log($"从池中获取实例:{prefab.name}".ToColor(ColorEnum.GreenYellow) + StringTool.NewLine() + StringTool.StackTrace());
            }

            return target;
        }
        else
        {
            var obj = Instantiate(prefab);
            DontDestroy(obj);
            if (isResetTransform)
            {
                obj.transform.SetParentAndReset(parent);
            }
            else
            {
                obj.transform.SetParent(parent, false);
            }

            // 记录对象与路径的映射
            _objectToPath[obj] = prefab.name;
            if (isOpenGetObjectDebug)
            {
                Debug.Log($"创建新实例:{prefab.name}".ToColor(ColorEnum.ForestGreen) + StringTool.NewLine() + StringTool.StackTrace());
            }

            return obj;
        }
    }

    public GameObject Get(string path, Transform parent, bool isResetTransform = false)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            throw null;
        }
#endif

        //P.BeginSample("AssetsPoolManager.Get");
        // 检查对象池中是否有可用对象
        if (!_objectPool.TryGetValue(path, out var objectQueue))
        {
            objectQueue = new Queue<(GameObject, float)>();
            _objectPool.Add(path, objectQueue);
        }

        if (objectQueue.Count > 0)
        {
            var (target, _) = objectQueue.Dequeue();
            if (isResetTransform)
            {
                target.transform.SetParentAndReset(parent);
            }
            else
            {
                target.transform.SetParent(parent, false);
            }

            // 记录对象与路径的映射
            _objectToPath[target] = path;
            if (isOpenGetObjectDebug)
            {
                Debug.Log($"从池中获取实例:{path}".ToColor(ColorEnum.GreenYellow) + StringTool.NewLine() + StringTool.StackTrace());
            }

            //P.EndSample();
            return target;
        }
        else
        {
            if (GetAsset(path, out var assetData, false))
            {
                if (assetData.Result == null)
                {
                    Logger.LogError($"Path:{path}为空");
                    return null;
                }

                var obj = Instantiate((GameObject)assetData.Result);
                DontDestroy(obj);
                if (isResetTransform)
                {
                    obj.transform.SetParentAndReset(parent);
                }
                else
                {
                    obj.transform.SetParent(parent, false);
                }

                // 记录对象与路径的映射
                _objectToPath[obj] = path;
                if (isOpenGetObjectDebug)
                {
                    Debug.Log($"创建新实例:{path}".ToColor(ColorEnum.ForestGreen) + StringTool.NewLine() + StringTool.StackTrace());
                }

                //P.EndSample();
                return obj;
            }
            else
            {
                Logger.LogError($"没有找到实例:{path}");
                //P.EndSample();
                return null;
            }
        }
    }

    public async Task<GameObject> GetAsyne(string path, Transform parent, bool isResetTransform = false)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            throw null;
        }
#endif

        if (!_objectPool.TryGetValue(path, out var objectQueue))
        {
            objectQueue = new Queue<(GameObject, float)>();
            _objectPool.Add(path, objectQueue);
        }

        if (objectQueue.Count > 0)
        {
            var (target, _) = objectQueue.Dequeue();
            if (isResetTransform)
            {
                target.transform.SetParentAndReset(parent);
            }
            else
            {
                target.transform.SetParent(parent, false);
            }

            _objectToPath[target] = path;
            if (isOpenGetObjectDebug)
            {
                Debug.Log($"从池中获取实例:{path}".ToColor(ColorEnum.GreenYellow) + StringTool.NewLine() + StringTool.StackTrace());
            }

            return target;
        }
        else
        {
            AssetData assetData = await LoadAssetAsync<GameObject>(path);
            if (assetData.Result == null)
            {
                Logger.LogError($"Path:{path}为空");
                return null;
            }

            var obj = Instantiate((GameObject)assetData.Result);
            DontDestroy(obj);
            if (isResetTransform)
            {
                obj.transform.SetParentAndReset(parent);
            }
            else
            {
                obj.transform.SetParent(parent, false);
            }

            _objectToPath[obj] = path;
            if (isOpenGetObjectDebug)
            {
                Debug.Log($"创建新实例:{path}".ToColor(ColorEnum.ForestGreen) + StringTool.NewLine() + StringTool.StackTrace());
            }

            return obj;
        }
    }

    public async Task<GameObject[]> GetAsyncBatch(string path, int count, Transform parent, bool isResetTransform = false)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            throw null;
        }
#endif
        // 检查对象池中是否有可用对象
        if (!_objectPool.TryGetValue(path, out var objectQueue))
        {
            objectQueue = new Queue<(GameObject, float)>();
            _objectPool.Add(path, objectQueue);
        }

        GameObject[] objects = new GameObject[count];

        while (objectQueue.Count > 0 && count > 0)
        {
            var (target, _) = objectQueue.Dequeue();
            if (isResetTransform)
            {
                target.transform.SetParentAndReset(parent);
            }
            else
            {
                target.transform.SetParent(parent, false);
            }

            objects[--count] = target;
            // 记录对象与路径的映射
            _objectToPath[target] = path;
        }

        if (count > 0)
        {
            AssetData assetData = await LoadAssetAsync<GameObject>(path);
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                throw null;
            }
#endif
            var objs = await InstantiateAsyncBatch((GameObject)assetData.Result, count);
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                throw null;
            }
#endif
            for (int i = 0; i < count; i++)
            {
                var target = objs[i];
                DontDestroy(target);
                if (isResetTransform)
                {
                    target.transform.SetParentAndReset(parent);
                }
                else
                {
                    target.transform.SetParent(parent, false);
                }

                objects[i] = target;
                // 记录对象与路径的映射
                _objectToPath[target] = path;
            }
        }

        return objects;
    }

    public void GiveBack(GameObject target)
    {
        if (!target) return;

        if (!_objectToPath.TryGetValue(target, out var path))
        {
            if (isOpenGiveBackObjectDebug || isOpenDestroyObjectDebug)
            {
                Debug.Log($"实例不归对象池管:{gameObject.name},直接销毁".ToColor(ColorEnum.Red));
            }
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif
            Object.Destroy(target);
            return;
        }

        target.transform.SetParent(trParentRoot, false);

        if (!_objectPool.TryGetValue(path, out var objectQueue))
        {
            objectQueue = new Queue<(GameObject, float)>();
            _objectPool[path] = objectQueue;
        }

        objectQueue.Enqueue((target, Time.time));
        _objectToPath.Remove(target);
        if (isOpenGiveBackObjectDebug)
        {
            Debug.Log($"归还成功:{path}".ToColor(ColorEnum.LightGreen));
        }
    }

    public async Task CacheInstances(string path)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            throw null;
        }
#endif
        if (!_objectPool.TryGetValue(path, out var objectQueue))
        {
            objectQueue = new Queue<(GameObject, float)>();
            _objectPool[path] = objectQueue;
        }
        else
        {
            return;
        }

        AssetData assetData = await LoadAssetAsync<GameObject>(path);

        var objs = Instantiate((GameObject)assetData.Result, trParentRoot, false);

        DontDestroy(objs);
        // 记录对象与路径的映射
        objectQueue.Enqueue((objs, Time.time));
        // 记录对象与路径的映射
        _objectToPath[objs] = path;
    }

    public async Task CacheInstances(string path, int count)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            throw null;
        }
#endif
        if (!_objectPool.TryGetValue(path, out var objectQueue))
        {
            objectQueue = new Queue<(GameObject, float)>();
            _objectPool[path] = objectQueue;
        }

        var len = count - objectQueue.Count;
        if (len > 0)
        {
            AssetData assetData = await LoadAssetAsync<GameObject>(path);

            var objs = await InstantiateAsyncBatch((GameObject)assetData.Result, len);

            foreach (var target in objs)
            {
                target.transform.SetParent(trParentRoot, false);
                DontDestroy(target);
                // 记录对象与路径的映射
                objectQueue.Enqueue((target, Time.time));
                // 记录对象与路径的映射
                _objectToPath[target] = path;
            }
        }
    }

    /// <summary>
    /// 清空未使用的实例
    /// </summary>
    /// <returns></returns>
    public void ClearUnUseInstances()
    {
        foreach (var objectList in _objectPool)
        {
            foreach (var (obj, _) in objectList.Value)
            {
                DestoryInstance(obj, objectList.Key);
            }
        }

        _objectPool.Clear();
    }

    /// <summary>
    /// 清空正在使用的实例
    /// </summary>
    public void GiveBackAllUsedInstances(bool isSetParent = true)
    {
        foreach (var v in _objectToPath)
        {
            // 更新对象最后使用时间并放回池中
            if (!_objectPool.TryGetValue(v.Value, out var objectQueue))
            {
                objectQueue = new Queue<(GameObject, float)>();
                _objectPool[v.Value] = objectQueue;
            }

            objectQueue.Enqueue((v.Key, Time.time));
            if (isSetParent && v.Key)
            {
                v.Key.transform.SetParent(trParentRoot, false);
            }
        }

        _objectToPath.Clear();
    }

    private void DestoryInstance(GameObject obj, string path)
    {
        if (isOpenDestroyObjectDebug)
        {
            Debug.Log($"实例被正常销毁:{path}".ToColor(ColorEnum.PaleGoldenRod));
        }

        Release(path);
        Object.Destroy(obj);
    }

    /// <summary>
    /// 轮询方法，需要在 Update 中调用
    /// </summary>
    private void CheckInstanceUseTime()
    {
        float currentTime = Time.time;
        // 处理对象池中的超时对象
        foreach (var kvp in _objectPool)
        {
            var objectStack = kvp.Value;

            while (objectStack.Count > 0)
            {
                var item = objectStack.Peek();
                var obj = item.Item1;
                var lastUsedTime = item.Item2;

                if (currentTime - lastUsedTime > instanceTimeout)
                {
                    objectStack.Dequeue();
                    string path = kvp.Key;
                    _objectToPath.Remove(obj);
                    DestoryInstance(obj, path);
                }
                else
                {
                    break;
                }
            }
        }
    }

    private void DontDestroy(GameObject target)
    {
#if UNITY_EDITOR && !EditorProfiler
        if (!Application.isPlaying)
        {
            return;
        }
#endif
        DontDestroyOnLoad(target);
    }

    private Task<GameObject[]> AwaitInstantiateOperationBatch(AsyncInstantiateOperation<GameObject> instantiateOperation)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            throw null;
        }
#endif
        var tcs = new TaskCompletionSource<GameObject[]>();
        instantiateOperation.completed += (operation) =>
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                throw null;
            }
#endif
            OnInstantiateOperationCompletedBatch(instantiateOperation, tcs);
        };
        return tcs.Task;
    }

    private async Task<GameObject[]> InstantiateAsyncBatch(GameObject prefab, int count)
    {
        return await AwaitInstantiateOperationBatch(Object.InstantiateAsync<GameObject>(prefab, count));
    }

    private GameObject Instantiate(GameObject prefab)
    {
        return Object.Instantiate(prefab);
    }

    private void OnInstantiateOperationCompletedBatch(AsyncInstantiateOperation<GameObject> instantiateOperation, TaskCompletionSource<GameObject[]> tcs)
    {
        tcs.SetResult(instantiateOperation.Result);
    }

    #endregion

    #region AssetsPool

    // 为每种类型定义单独的缓存容器
    private Dictionary<string, AssetData> cachedResources = new Dictionary<string, AssetData>();

    // 是否开启静默缓存的功能
    private bool isCanSilentCache = true;
    private Queue<string> silentCacheAssets = new Queue<string>();

    // 用来跟踪 LRU 顺序，最近访问的放在 First
    private LinkedList<AsyncOperationHandle> _lru = new LinkedList<AsyncOperationHandle>();

    private Dictionary<AsyncOperationHandle, LinkedListNode<AsyncOperationHandle>> _lruNodes = new Dictionary<AsyncOperationHandle, LinkedListNode<AsyncOperationHandle>>();

    // 用来跟踪每个组资源：Key 是 AsyncOperationHandle，值是路径到索引的映射
    private Dictionary<AsyncOperationHandle, Dictionary<string, int>> _handlePaths = new Dictionary<AsyncOperationHandle, Dictionary<string, int>>();


    /// <summary>
    /// 是否开启静默缓存的功能
    /// </summary>
    public bool IsCanSilentCache
    {
        set { isCanSilentCache = value; }
    }

    /// <summary>
    /// 添加需要静默缓存的资源
    /// </summary>
    /// <param name="path"></param>
    public void SilentCacheAsset(string path)
    {
        silentCacheAssets.Enqueue(path);
    }

    public async Task CacheAssetAsync(string address)
    {
        if (address.EndsWith(".json") || address.EndsWith(".bytes"))
        {
            await PrepareAsset<TextAsset>(address);
        }
        else if (address.EndsWith(".prefab"))
        {
            await PrepareAsset<GameObject>(address);
        }
        else if (address.EndsWith(".asset") || Regex.IsMatch(address, @"\[[^\]]*\]$"))
        {
            await PrepareAsset<ScriptableObject>(address);
        }
        else if (address.EndsWith(".spriteatlas"))
        {
            await PrepareAsset<SpriteAtlas>(address);
        }
        else if (address.EndsWith(".texture"))
        {
            await PrepareAsset<Texture2D>(address);
        }
        else if (address.EndsWith(".wav") || address.EndsWith(".mp3") || address.EndsWith(".ogg"))
        {
            await PrepareAsset<AudioClip>(address);
        }
        else
        {
            Logger.LogError($"未处理的资源类型：{address}");
        }
    }

    public async Task CacheAssetsGroupAsync(HashSet<string> group)
    {
        HashSet<string> textAsset = HashSetPool<string>.Get();
        HashSet<string> gameObjectAsset = HashSetPool<string>.Get();
        HashSet<string> scriptableObjectAsset = HashSetPool<string>.Get();
        HashSet<string> spriteAtlasAsset = HashSetPool<string>.Get();
        HashSet<string> textureAsset = HashSetPool<string>.Get();
        HashSet<string> audioAsset = HashSetPool<string>.Get();
        HashSet<string> materialAsset = HashSetPool<string>.Get();
        List<Task> tasks = ListPool<Task>.Get();
        foreach (var address in group)
        {
            if (string.IsNullOrEmpty(address))
            {
                continue;
            }

            if (address.EndsWith(".json") || address.EndsWith(".bytes"))
            {
                //tasks.Add(LoadAssetAsync<TextAsset>(address));
                textAsset.Add(address);
            }
            else if (address.EndsWith(".prefab"))
            {
                //tasks.Add(LoadAssetAsync<GameObject>(address));
                gameObjectAsset.Add(address);
            }
            else if (address.EndsWith(".asset") || Regex.IsMatch(address, @"\[[^\]]*\]$"))
            {
                //tasks.Add(LoadAssetAsync<ScriptableObject>(address));
                scriptableObjectAsset.Add(address);
            }
            else if (address.EndsWith(".spriteatlas") || address.EndsWith(".spriteatlasv2"))
            {
                //tasks.Add(LoadAssetAsync<SpriteAtlas>(address));
                spriteAtlasAsset.Add(address);
            }
            else if (address.EndsWith(".texture"))
            {
                //tasks.Add(LoadAssetAsync<Texture2D>(address));
                textureAsset.Add(address);
            }
            else if (address.EndsWith(".WAV") || address.EndsWith(".wav") || address.EndsWith(".mp3") || address.EndsWith(".ogg"))
            {
                //tasks.Add(LoadAssetAsync<AudioClip>(address));
                audioAsset.Add(address);
            }
            else if (address.EndsWith(".mat"))
            {
                //tasks.Add(LoadAssetAsync<Material>(address));
                materialAsset.Add(address);
            }
            else
            {
                Logger.LogError($"未处理的资源类型：{address}");
            }
        }


        tasks.Add(PrepareAssetGroup<TextAsset>(textAsset));
        tasks.Add(PrepareAssetGroup<GameObject>(gameObjectAsset));
        tasks.Add(PrepareAssetGroup<ScriptableObject>(scriptableObjectAsset));
        tasks.Add(PrepareAssetGroup<SpriteAtlas>(spriteAtlasAsset));
        tasks.Add(PrepareAssetGroup<Texture2D>(textureAsset));
        tasks.Add(PrepareAssetGroup<AudioClip>(audioAsset));
        tasks.Add(PrepareAssetGroup<Material>(materialAsset));
        await Task.WhenAll(tasks);
        ListPool<Task>.Release(tasks);
        HashSetPool<string>.Release(textAsset);
        HashSetPool<string>.Release(gameObjectAsset);
        HashSetPool<string>.Release(scriptableObjectAsset);
        HashSetPool<string>.Release(spriteAtlasAsset);
        HashSetPool<string>.Release(textureAsset);
        HashSetPool<string>.Release(audioAsset);
        HashSetPool<string>.Release(materialAsset);
    }

    private Dictionary<string, bool> _cacheGroup = new Dictionary<string, bool>();
    private MethodInfo _prepMethodDef;

    public async Task PrepareAssetGroup(string groupName, bool isPersistence = false, Action<Object> onComplete = null)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            throw null;
        }
#endif
        if (string.IsNullOrEmpty(groupName))
        {
            return;
        }

        if (_cacheGroup.TryGetValue(groupName, out var handle))
        {
            if (!handle)
            {
                await UniTask.WaitUntil(() => _cacheGroup.TryGetValue(groupName, out var loaded) && loaded);
            }
        }
        else
        {
            var _handle = Addressables.LoadResourceLocationsAsync(groupName, Addressables.MergeMode.Union);
            _cacheGroup[groupName] = false;
            await _handle.Task;
            if (_handle.Status != AsyncOperationStatus.Succeeded)
            {
                Addressables.Release(_handle);
                _cacheGroup.Remove(groupName);
                Logger.LogError("无法获取 DefaultLocalGroup 资源列表！");
                return;
            }

            IList<IResourceLocation> locations = _handle.Result;

            // 2. 按资源类型分组：Type -> HashSet<key>
            var typeToKeys = new Dictionary<Type, HashSet<string>>();

            foreach (var loc in locations)
            {
                Type resType = loc.ResourceType;

                if (!typeToKeys.TryGetValue(resType, out var set))
                {
                    set = new HashSet<string>();
                    typeToKeys[resType] = set;
                }

                set.Add(loc.PrimaryKey); // PrimaryKey 通常是 Address 或路径
            }

            if (_prepMethodDef == null)
            {
                _prepMethodDef = typeof(AssetsPoolManager).GetMethods(BindingFlags.Instance | BindingFlags.Public).First(m => m.Name == "PrepareAssetGroup" && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 1 && m.GetParameters().Length == 3);
            }

            // 反射拿泛型定义
            List<Task> tasks = ListPool<Task>.Get();
            // 3. 逐类型调用你的 LoadAssetsAsync<T>
            foreach (var kv in typeToKeys)
            {
                Type t = kv.Key;
                var keys = kv.Value;
                if (isOpenLoadAssetDebug)
                {
                    Debug.Log($"开始加载 {keys.Count} 个 {t.Name} 资源...".ToColor(ColorEnum.Bisque));
                }

                // 反射获取泛型方法并调用
                var mi = _prepMethodDef.MakeGenericMethod(t);

                var taskObj = (Task)mi.Invoke(this, new object[] { keys, isPersistence, onComplete });
                tasks.Add(taskObj);
            }

            await Task.WhenAll(tasks);

            if (isOpenLoadAssetDebug)
            {
                Debug.Log($"{groupName}下所有资源加载完毕".ToColor(ColorEnum.Chartreuse));
            }

            ListPool<Task>.Release(tasks);
            Addressables.Release(_handle);
            _cacheGroup[groupName] = true;
        }
    }

    public async Task PrepareAssetGroup<T>(HashSet<string> paths, bool isPersistence = false, Action<Object> onComplete = null) where T : UnityEngine.Object
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            throw null;
        }
#endif
        if (paths == null || paths.Count == 0)
        {
            return;
        }

        List<string> tempList = ListPool<string>.Get();
        HashSet<Task> handles = HashSetPool<Task>.Get();
        foreach (string path in paths)
        {
            if (cachedResources.TryGetValue(path, out AssetData op))
            {
                if (op.IsValid())
                {
                    if (!op.IsDone)
                    {
                        handles.Add(op.Handle.Task);
                    }
                    else
                    {
                        if (op.Status == AsyncOperationStatus.Succeeded)
                        {
                            onComplete?.Invoke(op.Result);
                        }
                        else
                        {
                            cachedResources.Remove(path);
                            tempList.Add(path);
                        }
                    }
                }
                else
                {
                    cachedResources.Remove(path);
                    tempList.Add(path);
                }
            }
            else
            {
                tempList.Add(path);
            }
        }

        if (tempList.Count == 0 && handles.Count == 0)
        {
            return;
        }

        AsyncOperationHandle<IList<T>> handle = new AsyncOperationHandle<IList<T>>();
        if (tempList.Count != 0)
        {
            handle = Addressables.LoadAssetsAsync<T>(tempList, onComplete, Addressables.MergeMode.Union);
            
            // 创建路径到索引的映射
            var pathIndexMap = new Dictionary<string, int>();
            for (int i = 0; i < tempList.Count; i++)
            {
                pathIndexMap[tempList[i]] = i;
            }

            _handlePaths[handle] = pathIndexMap;
            loadingCount++;
            for (int i = 0, len = tempList.Count; i < len; i++)
            {
                string path = tempList[i];
                AssetData op = new AssetData(path, handle, isPersistence, true, 0);
                cachedResources[path] = op;
                CheckAssetDebug(path, op, CheckAssetType.Prepare);
            }
        }

        if (handle.IsValid())
        {
            handles.Add(handle.Task);
        }


        await Task.WhenAll(handles);
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            throw null;
        }
#endif
        loadingCount--;

        if (handle.IsValid())
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                if (isOpenLoadAssetDebug)
                {
                    Debug.Log(("######批加载完成").ToColor(ColorEnum.Orange) + StringTool.NewLine() + StringTool.StackTrace());
                }

                var assets = handle.Result;
                for (int i = 0, len = assets.Count; i < len; i++)
                {
                    string path = tempList[i];
                    Object obj = assets[i];
                    if (GetFileName(path) != obj.name)
                    {
                        Logger.LogError("路径不存在Path:" + path + "-" + tempList.Count + "-" + assets.Count + "-FileName:" + GetFileName(path) + "-Asset:" + obj.name);
                        ReleaseAddressable(handle);
                    }
                    else
                    {
                        AssetData assetData = cachedResources[path];
                        assetData.Result = obj;
                        cachedResources[path] = assetData;
                        onComplete?.Invoke(obj);
                    }
                }

                Release(handle, false, false);
            }
            else
            {
                ReleaseAddressable(handle);
                Logger.LogError("资源批加载异常，需检查：" + handle.Status.ToString());
            }
        }

        ListPool<string>.Release(tempList);
        HashSetPool<Task>.Release(handles);
    }

    /// <summary>
    /// 按组加载
    /// </summary>
    /// <param name="paths"></param>
    /// <param name="onComplete"></param>
    /// <param name="isPersistence"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<List<AssetData>> LoadAssetsAsync<T>(HashSet<string> paths, bool isPersistence = false, Action<Object> onComplete = null) where T : UnityEngine.Object
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            throw null;
        }
#endif
        if (paths == null || paths.Count == 0)
        {
            return null;
        }

        await PrepareAssetGroup<T>(paths, isPersistence, onComplete);
        List<AssetData> list = new List<AssetData>();
        foreach (string path in paths)
        {
            if (GetAsset(path, out AssetData op, false))
            {
                list.Add(op);
            }
            else
            {
                Logger.LogError("资源不存在:" + path);
            }
        }

        return list;
    }

    private string GetFileName(string path)
    {
        if (path.Contains(".prefab["))
        {
            Match match = Regex.Match(path, @"\[(.*?)\]");
            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return string.Empty;
        }
        else
        {
            return Path.GetFileNameWithoutExtension(path);
        }
    }

    public async Task<StatusEnum> PrepareAsset<T>(string path, bool isPersistence = false, Action complete = null) where T : UnityEngine.Object
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            throw null;
        }
#endif
        if (string.IsNullOrEmpty(path))
        {
            Logger.LogError("path为空，请检查");
            return StatusEnum.UnValid;
        }

        if (cachedResources.TryGetValue(path, out var op))
        {
            switch (op.Status)
            {
                case AsyncOperationStatus.Succeeded:
                    if (isOpenLoadAssetDebug)
                    {
                        Debug.Log(("######加载完成Step2:" + path + ":" + op.Result.name).ToColor(ColorEnum.Orange) + StringTool.NewLine() + StringTool.StackTrace());
                    }

                    complete?.Invoke();
                    return StatusEnum.Success;
                case AsyncOperationStatus.Failed:
                    //在第一次加载的时候已经报异常了，这里不需要报了
                    return StatusEnum.Failed;
                default:
                    await op.Handle.Task;
#if UNITY_EDITOR
                    if (!Application.isPlaying)
                    {
                        throw null;
                    }
#endif
                    if (op.Status == AsyncOperationStatus.Succeeded)
                    {
                        if (isOpenLoadAssetDebug)
                        {
                            Debug.Log(("######加载完成Step3:" + path + ":" + op.Status + ":" + op.Result.name).ToColor(ColorEnum.Orange) + StringTool.NewLine() + StringTool.StackTrace());
                        }

                        complete?.Invoke();
                        return StatusEnum.WaitUntilToSuccess;
                    }
                    else
                    {
                        //在第一次加载的时候已经报异常了，这里不需要报了
                        return StatusEnum.WaitUntilToFailed;
                    }
            }
        }
        else
        {
            if (isOpenLoadAssetDebug)
            {
                Debug.Log(("######开始加载:" + path).ToColor(ColorEnum.Orange) + StringTool.NewLine() + StringTool.StackTrace());
            }

            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(path);
            _handlePaths[handle] = new Dictionary<string, int>() { { path, 0 } };
            op = new AssetData(path, handle, isPersistence, false, 0);
            cachedResources[path] = op;
            loadingCount++;

            CheckAssetDebug(path, op, CheckAssetType.Prepare);

            await handle.Task;
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                throw null;
            }
#endif

            loadingCount--;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                complete?.Invoke();
                if (isOpenLoadAssetDebug)
                {
                    Debug.Log(("######加载完成Step1:" + path + ":" + op.Result.name).ToColor(ColorEnum.Orange) + StringTool.NewLine() + StringTool.StackTrace());
                }

                Release(op, false, false);
                return StatusEnum.FristLoadToSuccess;
            }
            else
            {
                ReleaseAddressable(handle);
                Logger.LogError("资源加载异常，需检查：" + path);
                return StatusEnum.FristLoadToFailed;
            }
        }
    }

    public async Task<AssetData> LoadAssetAsync<T>(string path, bool isPersistence = false, Action<AssetData> complete = null) where T : UnityEngine.Object
    {
        await PrepareAsset<T>(path, isPersistence, null);
        if (GetAsset(path, out var op, false))
        {
            complete?.Invoke(op);
            return op;
        }
        else
        {
            return new AssetData();
        }
    }

    public async Task LoadSceneAsync(string path, LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true, int priority = 100)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            throw null;
        }
#endif
        if (isOpenLoadAssetDebug)
        {
            Debug.Log(("######开始切换场景:" + path).ToColor(ColorEnum.Blue));
        }

        var handle = Addressables.LoadSceneAsync(path, loadMode, activateOnLoad, priority);
        loadingCount++;
        await handle.Task;
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            throw null;
        }
#endif

        loadingCount--;
        if (isOpenLoadAssetDebug)
        {
            Debug.Log(("######完成切换场景:" + path).ToColor(ColorEnum.Blue));
        }
    }

    public StatusEnum PrepareAssetWaitForCompletion<T>(string path, bool isPersistence = false) where T : UnityEngine.Object
    {
        if (string.IsNullOrEmpty(path))
        {
            Logger.LogError("path为空，请检查");
            return StatusEnum.UnValid;
        }

        if (cachedResources.TryGetValue(path, out var op))
        {
            switch (op.Status)
            {
                case AsyncOperationStatus.Succeeded:
                    return StatusEnum.Success;
                case AsyncOperationStatus.Failed:
                    return StatusEnum.Failed;
                default:
                    op.Handle.WaitForCompletion();
                    if (op.Status == AsyncOperationStatus.Succeeded)
                    {
                        return StatusEnum.WaitUntilToSuccess;
                    }
                    else
                    {
                        return StatusEnum.WaitUntilToFailed;
                    }
            }
        }
        else
        {
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(path);
            _handlePaths[handle] = new Dictionary<string, int>() { { path, 0 } };
            handle.WaitForCompletion();
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                op = new AssetData(path, handle, isPersistence, false);
                cachedResources[path] = op;
                Release(op, false, false);
                return StatusEnum.FristLoadToSuccess;
            }
            else
            {
                ReleaseAddressable(handle);
                Logger.LogError("资源加载异常，需检查：" + path);
                return StatusEnum.FristLoadToFailed;
            }
        }
    }

    public AssetData LoadAssetWaitForCompletion<T>(string path, bool isPersistence = false) where T : UnityEngine.Object
    {
        PrepareAssetWaitForCompletion<T>(path, isPersistence);
        GetAsset(path, out var op);
        return op;
    }

    public bool IsHaveAsset(string path)
    {
        if (cachedResources.TryGetValue(path, out var op))
        {
            if (op.IsValid() && op.IsDone && op.Status == AsyncOperationStatus.Succeeded)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 非特殊情况，尽量别用该方法
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public bool GetAsset(string path, out AssetData assetData, bool isDebug = true)
    {
        bool result = cachedResources.TryGetValue(path, out assetData);

        if (result)
        {
            assetData.AddChannel();
            cachedResources[path] = assetData;
            TouchReference(assetData.Handle);
        }

        CheckAssetDebug(path, assetData, CheckAssetType.Get);
        if (isOpenLoadAssetDebug && isDebug)
        {
            Debug.Log(("GetAsset:" + path + ":IsContain:" + result.ToString()).ToColor(ColorEnum.Violet));
        }

        return result;
    }

    public bool CheckCached(string path)
    {
        return cachedResources.ContainsKey(path);
    }

    public void ReleaseGroup(List<AssetData> assets, bool isForce = false)
    {
        foreach (var VARIABLE in assets)
        {
            Release(VARIABLE, isForce);
        }
    }

    public void Release(string path, bool isForce = false)
    {
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        if (cachedResources.TryGetValue(path, out var assetData))
        {
            Release(assetData, isForce);
        }
    }

    private void Release(AsyncOperationHandle handle, bool isForce = false, bool isDebug = true)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            return;
        }
#endif
        if (isForce)
        {
            ReleaseAddressable(handle);
        }
        else
        {
            if (_handlePaths.TryGetValue(handle, out var paths))
            {
                // 清理所有单路径缓存
                foreach (var p in paths)
                {
                    AssetData a = cachedResources[p.Key];
                    if (a.IsPersistence)
                    {
                        return;
                    }

                    a.ClearChannel();
                    cachedResources[p.Key] = a;
                }
            }

            EnqueueLRU(handle, isDebug);
        }
    }

    private void EnqueueLRU(AsyncOperationHandle handle, bool isDebug = true)
    {
        if (_lruNodes.ContainsKey(handle)) return;
        var node = new LinkedListNode<AsyncOperationHandle>(handle);
        _lru.AddLast(node);
        _lruNodes[handle] = node;
        if (isOpenReleaseAssetDebug && isDebug)
        {
            Debug.Log(("######组资源进入LRU管理").ToColor(ColorEnum.Green));
        }
    }

    public void Release(AssetData assetData, bool isForce = false, bool isDebug = true)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            return;
        }
#endif

        if (isForce)
        {
            ReleaseAddressable(assetData.Handle);
        }
        else
        {
            if (string.IsNullOrEmpty(assetData.Path) || !cachedResources.ContainsKey(assetData.Path) || !assetData.IsValid())
            {
                return;
            }

            assetData = cachedResources[assetData.Path];
            if (assetData.Count <= 0 || assetData.IsPersistence)
            {
                return;
            }

            assetData.RemoveChannel();
            cachedResources[assetData.Path] = assetData;
            CheckAssetDebug(assetData.Path, assetData, CheckAssetType.Release, isDebug);
            if (isOpenReleaseAssetDebug && isDebug)
            {
                Debug.Log(("######尝试释放资源:" + assetData.Count.ToString() + "-" + assetData.IsPersistence + "-" + assetData.Path).ToColor(ColorEnum.Green) + StringTool.NewLine() + StringTool.StackTrace());
            }

            if (assetData.IsGroupAsset)
            {
                if (_handlePaths.TryGetValue(assetData.Handle, out var pathIndexMap))
                {
                    // 清理所有单路径缓存
                    foreach (var kvp in pathIndexMap)
                    {
                        string p = kvp.Key;
                        AssetData a = cachedResources[p];
                        if (a.Count > 0 || a.IsPersistence)
                        {
                            return;
                        }
                    }
                }
                else
                {
                    return;
                }
            }
            else
            {
                if (assetData.Count > 0 || assetData.IsPersistence)
                {
                    return;
                }
            }

            EnqueueLRU(assetData, isDebug);
        }
    }

    private void EnqueueLRU(AssetData assetData, bool isDebug = true)
    {
        if (_lruNodes.ContainsKey(assetData.Handle)) return;
        var node = new LinkedListNode<AsyncOperationHandle>(assetData.Handle);
        _lru.AddLast(node);
        _lruNodes[assetData.Handle] = node;
        if (isOpenReleaseAssetDebug && isDebug)
        {
            Debug.Log(("######资源进入LRU管理:" + assetData.Path).ToColor(ColorEnum.Green));
        }
    }

    private void TouchReference(AsyncOperationHandle key)
    {
        if (_lruNodes.TryGetValue(key, out var node))
        {
            _lru.Remove(node);
            _lruNodes.Remove(key);
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="percent"> [0,1] 的小数</param>
    public void ReleaseLRU(float percent)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            return;
        }
#endif
        int count = _lru.Count;
        int toRelease = Mathf.CeilToInt(count * Mathf.Min(percent, 1));
        if (isOpenReleaseAssetDebug)
        {
            Debug.Log($"开始释放资源,资源释放数量{toRelease.ToString()}".ToColor(ColorEnum.Khaki));
        }

        if (count == 0 || percent <= 0) return;
        for (int i = 0; i < toRelease; i++)
        {
            var node = _lru.First;
            if (node == null) break;
            var key = node.Value;
            _lru.RemoveFirst();
            _lruNodes.Remove(key);
            ReleaseAddressable(key);
        }
    }

    public void ForceReleaseAll()
    {
        foreach (var VARIABLE in _handlePaths)
        {
            Addressables.Release(VARIABLE.Key);
        }

        cachedResources.Clear();
        _lru.Clear();
        _lruNodes.Clear();
        _handlePaths.Clear();
        silentCacheAssets.Clear();
    }

    public void AddAssetDebug(string path)
    {
        isOpenTraceAssetsDebug = true;
        if (!traceAssetPaths.Contains(path))
        {
            traceAssetPaths.Add(path);
        }
    }

    private void ReleaseAddressable(AsyncOperationHandle handle)
    {
        if (!handle.IsValid())
        {
            if (_handlePaths.TryGetValue(handle, out var pathIndexMap))
            {
                // 清理所有单路径缓存
                foreach (var kvp in pathIndexMap)
                {
                    string p = kvp.Key;
                    cachedResources.Remove(p);
                    if (isOpenReleaseAssetDebug)
                    {
                        Debug.Log(("######释放资源step3:资源已经不可用:" + p).ToColor(ColorEnum.Orange));
                    }
                }

                _handlePaths.Remove(handle);
                _lru.Remove(handle);
                _lruNodes.Remove(handle);
            }

            return;
        }

        if (handle.Status == AsyncOperationStatus.Succeeded || handle.Status == AsyncOperationStatus.Failed)
        {
            if (_handlePaths.TryGetValue(handle, out var pathIndexMap))
            {
                // 清理所有单路径缓存
                foreach (var kvp in pathIndexMap)
                {
                    string p = kvp.Key;
                    cachedResources.Remove(p);
                    if (isOpenReleaseAssetDebug)
                    {
                        Debug.Log(("######释放资源step1:" + p).ToColor(ColorEnum.Red));
                    }
                }

                _handlePaths.Remove(handle);
                _lru.Remove(handle);
                _lruNodes.Remove(handle);
                // 已加载完成，直接释放
                Addressables.Release(handle);
            }
            else
            {
                Logger.LogError("handle不存在");
            }
        }
        else
        {
            // 正在加载，等待加载完成后释放
            handle.Completed += handle =>
            {
                if (_handlePaths.TryGetValue(handle, out var pathIndexMap))
                {
                    // 清理所有单路径缓存
                    foreach (var kvp in pathIndexMap)
                    {
                        string p = kvp.Key;
                        cachedResources.Remove(p);
                        if (isOpenReleaseAssetDebug)
                        {
                            Debug.Log(("######释放资源step2:" + p).ToColor(ColorEnum.Red));
                        }
                    }

                    // 已加载完成，直接释放
                    _handlePaths.Remove(handle);
                    Addressables.Release(handle);
                }
            };
        }
    }

    private void SilentCache()
    {
        if (isCanSilentCache && loadingCount <= 0 && silentCacheAssets.Count > 0)
        {
            CacheAssetAsync(silentCacheAssets.Dequeue());
        }
    }


    public enum CheckAssetType
    {
        Prepare,
        Get,
        Release,
    }

    private void CheckAssetDebug(string path, AssetData assetData, CheckAssetType checkAssetType, bool isDebug = true)
    {
        if (isOpenTraceAssetsDebug && isDebug)
        {
            if (traceAssetPaths.Contains(path))
            {
                switch (checkAssetType)
                {
                    case CheckAssetType.Prepare:
                        Debug.Log($"目标资源被加载:{path}-{assetData.Count}".ToColor(ColorEnum.GreenYellow));
                        break;
                    case CheckAssetType.Get:
                        Debug.Log($"目标资源被使用:{path}-{assetData.Count}".ToColor(ColorEnum.LightYellow));
                        break;
                    case CheckAssetType.Release:
                        Debug.Log($"目标资源被释放:{path}-{assetData.Count}".ToColor(ColorEnum.MediumVioletRed));
                        break;
                    default:
                        break;
                }
            }
        }
    }

    #endregion
}
