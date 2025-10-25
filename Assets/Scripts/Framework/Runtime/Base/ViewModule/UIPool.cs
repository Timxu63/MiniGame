using UnityEngine;

namespace Framework.ViewModule
{
    public class UIPool : MonoBehaviour
    {
        //一些要放在场景的ViewModule的gameObject
        public GameObject m_checkAssetsUI = null;
        public GameObject m_netloadingUI = null;

        //public ComponentRegister m_itemFly;

        #region 按钮动画器

        public RuntimeAnimatorController m_buttonAnimatorController;

        #endregion

    }
}
