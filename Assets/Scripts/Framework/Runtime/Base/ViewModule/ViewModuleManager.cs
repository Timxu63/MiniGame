using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Framework.EventSystem;
using Framework.Logic.Modules;
using Framework.Runtime;
using UnityEngine;

namespace Framework.ViewModule
{
    public sealed partial class ViewModuleManager : MonoBehaviour
    {
        private Dictionary<int, ViewModuleData> m_viewModuleDatas = new Dictionary<int, ViewModuleData>();
        private Dictionary<int, UILayers> m_layers = new Dictionary<int, UILayers>();

        [SerializeField] private EventSystemManager m_eventSystemManager = null;
        [SerializeField] private ResourcesManager m_resourcesManager = null;
        [SerializeField] private Camera m_uiCamera;

        [Header("Mask Setting")] [SerializeField]
        private GameObject m_maskUI = null;

        private int m_maskCount;

        public Func<int, string> m_funcAnalysisViewName;
        
        private AndroidBackManager m_backManager = new AndroidBackManager();

        /// <summary>
        /// UI 摄像机
        /// </summary>
        public Camera UICamera
        {
            get { return m_uiCamera; }
            private set { m_uiCamera = value; }
        }

        /// <summary>
        /// UI 层级物体
        /// </summary>
        public GameObject[] m_layerObjects;
        
        /// <summary>
        /// 注册UI层
        /// </summary>
        /// <param name="viewModuleDatas"></param>
        public void RegisterViewModule(ViewModuleData viewModuleData)
        {
            if (viewModuleData == null) return;
            m_viewModuleDatas[viewModuleData.m_id] = viewModuleData;
        }

        /// <summary>
        /// 取消注册UI层
        /// </summary>
        /// <param name="viewModuleDatas"></param>
        public void UnRegisterViewModule(ViewModuleData viewModuleData)
        {
            if (viewModuleData == null) return;
            m_viewModuleDatas.Remove(viewModuleData.m_id);
        }

        /// <summary>
        /// 卸载所有的显示模块模块
        /// </summary>
        /// <param name="ignoreIDs">忽略的ID</param>
        public void UnRegisterAllViewModule(params int[] ignoreIDs)
        {
            List<ViewModuleData> unRegisters = new List<ViewModuleData>();
            foreach (var item in m_viewModuleDatas)
            {
                if (item.Value == null) continue;
                if (ignoreIDs != null && ignoreIDs.Contains(item.Key)) continue;
                unRegisters.Add(item.Value);
            }

            for (int i = 0; i < unRegisters.Count; i++)
            {
                var data = unRegisters[i];
                if (data == null) continue;
                UnRegisterViewModule(data);
            }
        }

        /// <summary>
        /// 通过界面ID获得界面基础数据
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns></returns>
        public ViewModuleData GetViewModuleData(int viewName)
        {
            m_viewModuleDatas.TryGetValue(viewName, out var viewModuleData);
            return viewModuleData;
        }

        /// <summary>
        /// 通过界面ID获得界面模块
        /// </summary>
        /// <param name="viewName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetViewModule<T>(int viewName) where T : BaseViewModule
        {
            T t = null;
            ViewModuleData viewModuleData = GetViewModuleData(viewName);
            t = viewModuleData.m_viewModule as T;
            return t;
        }

        /// <summary>
        /// 打开界面 等待Task 返回任务
        /// </summary>
        /// <param name="viewName">id</param>
        /// <param name="data">打开传入的数据</param>
        /// <param name="layer">层级</param>
        /// <param name="loadedCallBack">加载触发</param>
        /// <param name="openedCallBack">打开触发</param>
        public async Task OpenView(
            int viewName,
            object data = null,
            UILayers layer = UILayers.First,
            Action<GameObject> loadedCallBack = null,
            Action<GameObject> openedCallBack = null)
        {
            Logger.Log($"<color=red>[ViewModule]</color>OpenView = {viewName}");
            await OpenViewInternal(viewName, data, layer, loadedCallBack, openedCallBack);
        }

        private async Task OpenViewInternal(int viewName,
            object data = null,
            UILayers layer = UILayers.First,
            Action<GameObject> loadedCallBack = null,
            Action<GameObject> openedCallBack = null)
        {
            ViewModuleData _viewModuleData = null;
            m_viewModuleDatas.TryGetValue(viewName, out _viewModuleData);

            if (_viewModuleData == null)
            {
                Logger.LogError($"[ViewModule]OpenView viewModuleData is null , viewName = {viewName},UIlayers = {layer}");
                return;
            }

            if (_viewModuleData.m_viewState == ViewState.Loading || _viewModuleData.m_viewState == ViewState.Opened)
            {
#if UNITY_EDITOR                
            Logger.LogError("界面已经打开了，无法再次打开：  "+(ViewName)viewName);        
#endif                  
                return;
            }

            if (_viewModuleData.m_gameObject == null)
            {
                if (_viewModuleData.m_prefab == null)
                {
                    SetMask(true);
                    _viewModuleData.m_viewState = ViewState.Loading;
                    //加载
                    var _handler = m_resourcesManager.LoadAssetAsync<GameObject>(_viewModuleData.m_assetPath);
                    await _handler.Task;

                    if (_handler.Status != UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                    {
                        Logger.LogError($"[ViewModule]OpenView viewModuleData loading Failed , viewName = {viewName},UIlayers = {layer}");
                        SetMask(false);
                        return;
                    }
                    _viewModuleData.m_prefab = _handler.Result;
                    if (_viewModuleData.m_loader != null)
                    {
                        await _viewModuleData.m_loader.OnLoad(data);
                    }

                    SetMask(false);
                    if (loadedCallBack != null) loadedCallBack.Invoke(_viewModuleData.m_prefab);
                    if (_viewModuleData.m_viewState == ViewState.Closed)
                    {
                        if (_viewModuleData.m_isCanDestory)
                        {
                            if (_viewModuleData.m_loader != null)
                            {
                                _viewModuleData.m_loader.OnUnLoad();
                            }
                            if (_viewModuleData.m_prefab != null)
                            {
                                m_resourcesManager.Release(_viewModuleData.m_prefab);
                            }
                            _viewModuleData.m_prefab = null;
                        }
                        return;
                    }
                    //创建UI 并调用UI函数
                    _viewModuleData.m_gameObject = InstantiateByPrefab(_viewModuleData.m_id, _viewModuleData.m_prefab, layer, _viewModuleData.SetPadding);
                    _viewModuleData.m_viewModule = _viewModuleData.m_gameObject.GetComponent<BaseViewModule>();

                    if (_viewModuleData.m_viewModule == null)
                    {
                        Logger.LogError(
                            $"[ViewModule]OpenView viewModuleData not loading , viewName = {viewName},UIlayers = {layer} ,path = {_viewModuleData.m_assetPath} viewModule  is null!!!");
                        return;
                    }
                    
                    _viewModuleData.m_viewModule.SetViewData(_viewModuleData.m_id);
                    _viewModuleData.m_viewModule.SetLoader(_viewModuleData.m_loader);

                    _viewModuleData.m_viewModule.OnCreate(data);
                    OpenViewByGameObject(_viewModuleData, data, openedCallBack, layer);
                }
                else
                {
                    //创建UI 并调用UI函数
                    _viewModuleData.m_gameObject = InstantiateByPrefab(_viewModuleData.m_id, _viewModuleData.m_prefab, layer, _viewModuleData.SetPadding);
                    _viewModuleData.m_viewModule = _viewModuleData.m_gameObject.GetComponent<BaseViewModule>();
                    if (_viewModuleData.m_viewModule == null)
                    {
                        Logger.LogError(
                            $"[ViewModule]viewModuleData not loading , viewName = {viewName},UIlayers = {layer} ,path = {_viewModuleData.m_assetPath} viewModule  is null!!!");
                        return;
                    }

                    OpenViewByGameObject(_viewModuleData, data, openedCallBack, layer);
                }
            }
            else
            {
                _viewModuleData.m_viewModule = _viewModuleData.m_gameObject.GetComponent<BaseViewModule>();

                if (_viewModuleData.m_viewModule == null)
                {
                    Logger.LogError(
                        $"[ViewModule]OpenView viewModuleData not loading ," +
                        $"viewName = {viewName}," +
                        $"UIlayers = {layer} ," +
                        $"path = {_viewModuleData.m_assetPath} ," +
                        $"objname={_viewModuleData.m_gameObject.name}viewModule  is null!!!");
                    return;
                }

                OpenViewByGameObject(_viewModuleData, data, openedCallBack, layer);
            }

            var eventArgs = m_eventSystemManager.GetEvent<ViewEventArgs>().SetData(viewName);
            m_eventSystemManager.DispatchNow(LocalMessageName.CC_ViewOpen,eventArgs);
        }

        private GameObject InstantiateByPrefab(int viewName, GameObject prefab, UILayers layer, bool setPadding)
        {
            GameObject _obj = GameObject.Instantiate<GameObject>(prefab);
            SetLayer(viewName, _obj, layer, setPadding);
            return _obj;
        }

        private void SetLayer(int viewName, GameObject obj, UILayers layer, bool setPadding)
        {
            GameObject _layerObj = GetGameObjectByUILayers(layer);
            if (_layerObj == null)
            {
                Logger.LogError($"[ViewModule]layer gameObject is null ,viewName = {viewName},UIlayers = {layer}");
                return;
            }

            RectTransform _trans = (RectTransform)obj.transform;
            _trans.SetParent(_layerObj.transform);
            if (setPadding)
            {
                _trans.SetForPadding();
            }
            else
            {
                _trans.sizeDelta = Vector3.zero;
                _trans.localScale = Vector3.one;
                _trans.localPosition = Vector3.zero;
                _trans.localRotation = Quaternion.identity;
            }
            _trans.SetAsLastSibling();
            m_layers[viewName] = layer;
        }

        private void OpenViewByGameObject(ViewModuleData viewData, object data, Action<GameObject> openedCallBack, UILayers layer)
        {
            try
            {
                SetLayer(viewData.m_id, viewData.m_gameObject, layer, viewData.SetPadding);
                viewData.m_gameObject.SetActive(true);
                RectTransform _trans = (RectTransform)viewData.m_gameObject.transform;
                _trans.SetAsLastSibling();

                viewData.m_viewModule.RegisterEvents(m_eventSystemManager);
                viewData.m_viewModule.SetAddBack(viewData.m_viewModule.IsAddBack());
                m_backManager.AddBack(viewData.m_viewModule);
                viewData.m_viewState = ViewState.Opened;
                viewData.m_viewModule.OnOpen(data);
                if (openedCallBack != null) openedCallBack.Invoke(viewData.m_gameObject);
                
                //上报ViewModuleManager
                // GameApp.SDK?.Analyze.Track("ViewModuleManager", new Dictionary<string, object>()
                // {
                //     {"int0", viewData.m_id},
                //     {"int1", 1},
                // });
                
                Logger.Log($"<color=red>[ViewModule]</color>OpenView.Opened finided id={viewData.m_id},type= {viewData.m_gameObject.name} ");
            }
            catch (Exception ex)
            {
#if UNITY_EDITOR
                Logger.LogError($"OpenViewByGameObject has exception :{ex}, id={viewData.m_id},path= {viewData.m_assetPath}");
#endif
                throw ex;
            }
        }

        /// <summary>
        /// 通过界面ID关闭界面
        /// </summary>
        /// <param name="viewName"></param>
        public void CloseView(int viewName)
        {
            // GameApp.Sound.PlaySoundEffect("Assets/_Resources/Sound/Archersaga/UI/PopUI.mp3");
            Logger.Log($"<color=red>[ViewModule]</color>CloseView = {viewName}");

            m_viewModuleDatas.TryGetValue(viewName, out var viewModuleData);
            if (viewModuleData == null)
            {
                Logger.LogError($"[ViewModule]CloseView viewModuleData is null , id = {viewName}");
                return;
            }

            if (viewModuleData.m_viewState == ViewState.Loading)
            {
                //HLog.LogErrorFormat("change loading to  close -------------");
                viewModuleData.m_viewState = ViewState.Closed;
                return;
            }

            if (viewModuleData.m_gameObject == null)
            {
                Logger.LogError(
                    $"[ViewModule]CloseView viewModuleData gameObject is null , id = {viewName},path= {viewModuleData.m_assetPath}");
                viewModuleData.m_viewState = ViewState.Null;
                return;
            }

            if (viewModuleData.m_viewModule == null)
            {
                Logger.LogError(
                    $"[ViewModule]CloseView viewModuleData gameObject is null , viewName = {viewName},objName={viewModuleData.m_gameObject.name}");
                viewModuleData.m_viewState = ViewState.Null;
                return;
            }

            viewModuleData.m_viewModule.UnRegisterEvents(m_eventSystemManager);
            //关闭界面时，一定会从返回键list中移除
            m_backManager.RemoveBack(viewModuleData.m_viewModule);
            //状态提前置位Closed，因为OnClose的逻辑中可能会对UI的打开关闭状态进行检查
            viewModuleData.m_viewState = ViewState.Closed;
            viewModuleData.m_viewModule.OnClose();
            
            //上报ViewModuleManager
            // GameApp.SDK?.Analyze.Track("ViewModuleManager", new Dictionary<string, object>()
            // {
            //     {"int0", viewName},
            //     {"int1", 2},
            // });
            
            if (viewModuleData.m_isCanDestory)
            {
                viewModuleData.m_viewModule.OnDelete();
                if (viewModuleData.m_loader != null)
                {
                    viewModuleData.m_loader.OnUnLoad();
                }
                viewModuleData.m_viewModule.SetLoader(null);
                
                GameObject.Destroy(viewModuleData.m_gameObject);
                viewModuleData.m_gameObject = null;

                if (viewModuleData.m_prefab != null)
                {
                    m_resourcesManager.Release(viewModuleData.m_prefab);
                }

                viewModuleData.m_prefab = null;
                viewModuleData.m_viewModule = null;
            }
            else
            {
                viewModuleData.m_gameObject.SetActive(false);
            }

           

            
            var eventArgs = m_eventSystemManager.GetEvent<ViewEventArgs>();
            eventArgs.SetData(viewName);
            m_eventSystemManager.DispatchNow(LocalMessageName.CC_ViewClose,eventArgs);
        }

        public void ForceDestroyView(int viewName)
        {
            m_viewModuleDatas.TryGetValue(viewName, out var viewModuleData);
            if (viewModuleData == null)
            {
                return;
            }
            if (!viewModuleData.m_isCanDestory && viewModuleData.m_viewModule!=null)
            {
                viewModuleData.m_viewModule.OnDelete();
                if (viewModuleData.m_loader != null)
                {
                    viewModuleData.m_loader.OnUnLoad();
                }
                viewModuleData.m_viewModule.SetLoader(null);
                
                GameObject.Destroy(viewModuleData.m_gameObject);
                viewModuleData.m_gameObject = null;

                if (viewModuleData.m_prefab != null)
                {
                    m_resourcesManager.Release(viewModuleData.m_prefab);
                }
                viewModuleData.m_prefab = null;
                viewModuleData.m_viewModule = null;
            }
        }


        /// <summary>
        /// 获得层级的物体
        /// </summary>
        /// <param name="uilayer">层级ID</param>
        /// <returns></returns>
        public GameObject GetGameObjectByUILayers(UILayers uilayer)
        {
            GameObject _object = null;
            int _index = (int)uilayer;
            if (_index < m_layerObjects.Length)
            {
                _object = m_layerObjects[_index];
            }

            return _object;
        }

        /// <summary>
        /// 关闭所有UI,不包括忽略的ID
        /// </summary>
        /// <param name="ignoreIDs">忽略的ID</param>
        public void CloseAllView(int[] ignoreIDs = null)
        {
            int[] ids = m_viewModuleDatas.Keys.ToArray();
            for (int i = 0; i < ids.Length; i++)
            {
                int id = ids[i];
                bool _isOpened = IsOpened(id);
                if (_isOpened == false) continue;
                if (ignoreIDs != null && ignoreIDs.Contains(id)) continue;
                CloseView(id);
            }
            
            m_backManager.Clear();
        }

        /// <summary>
        /// 关闭指定层级的UI 
        /// </summary>
        /// <param name="layers">层级</param>
        /// <param name="ignoreIDs">忽略的ID</param>
        public void CloseAllView(UILayers[] layers, int[] ignoreIDs = null)
        {
            if (layers == null) return;
            int[] ids = m_viewModuleDatas.Keys.ToArray();
            for (int i = 0; i < ids.Length; i++)
            {
                int id = ids[i];
                bool _isOpened = IsOpened(id);
                if (_isOpened == false) continue;
                if (ignoreIDs != null && ignoreIDs.Contains(id)) continue;
                UILayers layer = UILayers.First;
                if (m_layers.TryGetValue(id, out layer) == false) continue;
                if (layers.Contains(layer) == false) continue;
                CloseView(id);
            }
            m_backManager.Clear();
        }

        /// <summary>
        /// 是否界面是打开状态
        /// </summary>
        /// <param name="viewName">界面ID</param>
        /// <returns></returns>
        public bool IsOpened(int viewName)
        {
            bool _isOpened = false;
            ViewModuleData _viewModuleData = null;
            m_viewModuleDatas.TryGetValue(viewName, out _viewModuleData);

            if (_viewModuleData == null)
            {
                Logger.LogError($"[ViewModule]CloseView viewModuleData is null , viewName = {viewName}");
                return false;
            }

            _isOpened = _viewModuleData.m_viewState == ViewState.Opened;
            return _isOpened;
        }

        /// <summary>
        /// 界面是否是加载状态
        /// </summary>
        /// <param name="viewName">界面ID</param>
        /// <returns></returns>
        public bool IsLoading(int viewName)
        {
            bool isLoading = false;
            m_viewModuleDatas.TryGetValue(viewName, out var viewModuleData);

            if (viewModuleData == null)
            {
                Logger.LogError($"[ViewModule]CloseView viewModuleData is null , viewName = {viewName}");
                return false;
            }

            isLoading = viewModuleData.m_viewState == ViewState.Loading;
            return isLoading;
        }

        /// <summary>
        /// 界面是否是加载状态或者打开状态
        /// </summary>
        /// <param name="viewName">界面ID</param>
        /// <returns></returns>
        public bool IsOpenedOrLoading(int viewName)
        {
            bool isLoadingOrOpen = false;
            m_viewModuleDatas.TryGetValue(viewName, out var viewModuleData);

            if (viewModuleData == null)
            {
                return false;
            }
            var state = viewModuleData.m_viewState;
            isLoadingOrOpen = state == ViewState.Loading || state == ViewState.Opened;
            return isLoadingOrOpen;
        }

        /// <summary>
        /// 界面Update运行，打开的界面通过注册顺序调用 
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <param name="unscaledDeltaTime"></param>
        public void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            foreach (var item in m_viewModuleDatas)
            {
                if (item.Value.m_viewModule != null
                    && item.Value.m_viewState == ViewState.Opened
                    && item.Value.m_gameObject != null)
                {
                    item.Value.m_viewModule.OnUpdate(deltaTime, unscaledDeltaTime);
                }
            }
            
            m_backManager.Update();
        }
        
        public void OnLateUpdate(float deltaTime, float unscaledDeltaTime)
        {
            foreach (var item in m_viewModuleDatas)
            {
                if (item.Value.m_viewModule != null
                    && item.Value.m_viewState == ViewState.Opened
                    && item.Value.m_gameObject != null)
                {
                    item.Value.m_viewModule.OnLateUpdate(deltaTime, unscaledDeltaTime);
                }
            }
        }

        /// <summary>
        /// 获取界面ID对应的名称
        /// <para>TGA埋点、Editor调试时会使用</para>
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns></returns>
        public string GetViewNameString(int viewName)
        {
            if (m_funcAnalysisViewName != null) return m_funcAnalysisViewName.Invoke(viewName);
            return viewName.ToString();
        }

        #region 货币条单独处理层级

        public void RefreshCurrencyLayer(int currencyViewName)
        {
            //货币条没打开，不处理层级
            if (IsOpened(currencyViewName) == false)
            {
                return;
            }
            int[] ids = m_viewModuleDatas.Keys.ToArray();
            //这里只检查是否有第二层的UI打开，3层和4层不管（因为都是系统层的，理论上逻辑层只能在1层或2层）
            bool containsSecond = false;
            for (int i = 0; i < ids.Length; i++)
            {
                int id = ids[i];
                //货币条跳过处理
                if (id == currencyViewName) continue;
                bool isOpenedOrLoading = IsOpenedOrLoading(id);
                if (isOpenedOrLoading == false) continue;
                //正在打开的UI
                if (m_layers[id] == UILayers.Second)
                {
                    //有第二层的UI打开了
                    containsSecond = true;
                    break;
                }
            }

            
            
            UILayers layer = !containsSecond ? UILayers.First : UILayers.Second;
            SetLayer(currencyViewName, m_viewModuleDatas[currencyViewName].m_gameObject, layer, m_viewModuleDatas[currencyViewName].SetPadding);
        }

        #endregion
    }
}