using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using HotFix;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HotFixBattle
{
    public class CameraManager
    {
        private Transform cameraRoot;
        private Camera camera;
        public Camera CameraView => camera;
        private bool haveInit;
        public async Task InitCamera()
        {
            var resourceConfig = GameTableProxy.Tables.TbResource.Get(3);
            var loadOperation = await Addressables.LoadAssetAsync<GameObject>(resourceConfig.Path).Task;
            Debug.LogError("!2");
            cameraRoot = GameObject.Instantiate(loadOperation).transform;
            camera = cameraRoot.GetComponentInChildren<Camera>();
            haveInit = true;
        }
        public void OnLateUpdate(float deltaTime, float unscaledDeltaTime)
        {
            if (haveInit && EntityViewManager.Instance.PlayerViewData != null)
            {
                cameraRoot.transform.position =
                    EntityViewManager.Instance.PlayerViewData.View.transform.position;
            }
        }

        public void DeInit()
        {
            
        }
    }
}