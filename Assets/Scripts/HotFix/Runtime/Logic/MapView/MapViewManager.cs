using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HotFix
{
    public class MapViewManager
    {
        public Transform mapRoot;
        public async Task Init(int resourceId)
        {
            var resourceConfig = GameTableProxy.Tables.TbResource.Get(resourceId);
            var loadOperation = await Addressables.LoadAssetAsync<GameObject>(resourceConfig.Path).Task;
            mapRoot = GameObject.Instantiate(loadOperation).transform;
            
            // 根据MapManager中的地图大小将mapRoot居中
            var mapManager = HotFixBattle.MapManager.Instance;
            if (mapManager != null && mapManager.IsInitialized)
            {
                // 计算地图中心位置
                float centerX = mapManager.Width * mapManager.CellSize * 0.5f;
                float centerZ = mapManager.Height * mapManager.CellSize * 0.5f;
                
                // 设置mapRoot位置为地图中心
                mapRoot.position = new Vector3(centerX, 0, centerZ);
            }
        }
    }
}