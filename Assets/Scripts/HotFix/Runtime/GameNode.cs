using Framework;
using UnityEngine;

namespace HotFix
{
    public class GameNode : Singleton<GameNode>
    {
        private GameObject _battleRoot;

        private GameObject _bulletRoot;

        private GameObject _dropRoot;
        
        private GameObject _effectParent;

        private GameObject _modelParent;

        private GameObject _monsterRoot;
        private GameObject _mapItemRoot;
        
        
        public GameObject EffectParent
        {
            get
            {
                if (_effectParent == null)
                {
                    _effectParent = new GameObject("EffectParent");
                    Object.DontDestroyOnLoad(_effectParent);
                }
                return _effectParent;
            }
        }

        public void OnWorldToMain()
        {
            Object.Destroy(_battleRoot);
            _battleRoot = null;
            _monsterRoot = null;
            _bulletRoot = null;
            _dropRoot = null;
        }
        
        public GameObject ModelParent
        {
            get
            {
                if (_modelParent == null)
                {
                    _modelParent = new GameObject("modelParent");
                    _modelParent.transform.position = new Vector3(1000, 1000, 1000);
                     Object.DontDestroyOnLoad(_modelParent);
                }

                return _modelParent;
            }
        }

        public GameObject BattleRoot
        {
            get
            {
                if (_battleRoot == null)
                {
                    _battleRoot = new GameObject("BattleRoot");
                    Object.DontDestroyOnLoad(_battleRoot);
                }
                return _battleRoot;
            }
        }

        public GameObject EntityRoot
        {
            get
            {
                if (_monsterRoot == null)
                {
                    _monsterRoot = new GameObject("entityRoot");
                    _monsterRoot.transform.SetParentAndReset(BattleRoot.transform);
                }

                return _monsterRoot;
            }
        }

        public GameObject MapItemRoot
        {
            get
            {
                if (_mapItemRoot == null)
                {
                    _mapItemRoot = new GameObject("mapItemRoot");
                    _mapItemRoot.transform.SetParentAndReset(BattleRoot.transform);
                }

                return _mapItemRoot;
            }
        }

        public GameObject BulletRoot
        {
            get
            {
                if (_bulletRoot == null)
                {
                    _bulletRoot = new GameObject("bulletRoot");
                    _bulletRoot.transform.SetParentAndReset(BattleRoot.transform);
                }

                return _bulletRoot;
            }
        }

        public Transform HpParent { get; private set; }

        public GameObject DropRoot
        {
            get
            {
                if (_dropRoot == null)
                {
                    _dropRoot = new GameObject("dropRoot");
                    _dropRoot.transform.SetParentAndReset(BattleRoot.transform);
                }

                return _dropRoot;
            }
        }

        public void SetHpParent(Transform t)
        {
            HpParent = t;
        }

        #region 屏幕正上方（可能给Boss血条使用等）

        public RectTransform ScreenTopNode { get; private set; }

        public void SetScreenTop(RectTransform t)
        {
            ScreenTopNode = t;
        }
        
        //经验条位置，目前和boss 血条共享一个位置
        public RectTransform ScreenBarNode { get; private set; }

        public void SetScreenBarNode(RectTransform t)
        {
            ScreenBarNode = t;
        }

        #endregion
    }
}