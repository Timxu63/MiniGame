using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    [CreateAssetMenu] //可以直接在Project右键创建
    public class GameAppConfigData : ScriptableObject
    {
        public List<GameAppConfigInfo> m_gameAppConfigInfoList = new List<GameAppConfigInfo>();

        public Dictionary<string, GameAppConfigInfo> m_gameAppConfigInfoDic = null;

        private void OnEnable()
        {
            if (m_gameAppConfigInfoDic == null)
            {
                m_gameAppConfigInfoDic = new Dictionary<string, GameAppConfigInfo>(m_gameAppConfigInfoList.Count);
                for (int i = 0; i < m_gameAppConfigInfoList.Count; i++)
                {
                    GameAppConfigInfo gameAppConfigInfo = m_gameAppConfigInfoList[i];
                    m_gameAppConfigInfoDic.Add(gameAppConfigInfo.name, gameAppConfigInfo);
                }
            }
        }

        private void OnDestroy()
        {
            if (m_gameAppConfigInfoDic != null)
            {
                m_gameAppConfigInfoDic.Clear();
            }

            m_gameAppConfigInfoDic = null;
        }

        /// <summary>
        /// 获取游戏配置
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetConfigInfo(string name)
        {
            m_gameAppConfigInfoDic.TryGetValue(name, out var data);
            if (data == null) return string.Empty;
            return data.info;
        }

#if UNITY_EDITOR

        public void SetConfig(string name, string info)
        {
            bool isHave = false;
            for (int i = 0; i < m_gameAppConfigInfoList.Count; i++)
            {
                var data = m_gameAppConfigInfoList[i];
                if (string.Equals(data.name, name))
                {
                    data.info = info;
                    isHave = true;
                    break;
                }
            }

            if (!isHave)
            {
                GameAppConfigInfo data = new GameAppConfigInfo();
                data.name = name;
                data.info = info;
                m_gameAppConfigInfoList.Add(data);
            }
        }

        public void Save()
        {
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.Refresh();
        }

#endif
    }
    
    #region GameAppConfigInfo

    /// <summary>
    /// 配置信息
    /// </summary>
    [Serializable]
    public class GameAppConfigInfo
    {
        /// <summary>
        /// 配置名字
        /// </summary>
        public string name;

        /// <summary>
        /// 配置信息
        /// </summary>
        public string info;
    }

    #endregion
}