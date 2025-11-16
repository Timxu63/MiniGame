
using System.Collections.Generic;
using Game.Logic.BattleModule.Entity;

namespace Game.Logic.BattleModule.Component.Weapon
{
    /// <summary>
    /// 武器组件
    /// </summary>
    public class WeaponComponent : EntityComponent
    {
        /// <summary>
        /// 当前装备的武器
        /// </summary>
        public Weapon CurrentWeapon { get; private set; }

        /// <summary>
        /// 武器背包
        /// </summary>
        private Dictionary<int, Weapon> _weaponInventory = new Dictionary<int, Weapon>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="owner">组件所属的实体</param>
        public WeaponComponent(BaseEntity owner) : base(owner)
        {
        }

        /// <summary>
        /// 装备武器
        /// </summary>
        /// <param name="weapon">要装备的武器</param>
        public void EquipWeapon(Weapon weapon)
        {
            if (weapon == null) return;

            // 如果已有武器，先卸下
            if (CurrentWeapon != null)
            {
                UnequipCurrentWeapon();
            }

            // 装备新武器
            CurrentWeapon = weapon;

            // 如果武器不在背包中，添加到背包
            if (!_weaponInventory.ContainsKey(weapon.Id))
            {
                _weaponInventory[weapon.Id] = weapon;
            }
        }

        /// <summary>
        /// 卸下当前武器
        /// </summary>
        public void UnequipCurrentWeapon()
        {
            if (CurrentWeapon == null) return;

            CurrentWeapon = null;
        }

        /// <summary>
        /// 添加武器到背包
        /// </summary>
        /// <param name="weapon">要添加的武器</param>
        public void AddWeaponToInventory(Weapon weapon)
        {
            if (weapon == null) return;

            _weaponInventory[weapon.Id] = weapon;
        }

        /// <summary>
        /// 从背包移除武器
        /// </summary>
        /// <param name="weaponId">武器ID</param>
        /// <returns>是否成功移除</returns>
        public bool RemoveWeaponFromInventory(int weaponId)
        {
            // 如果是当前装备的武器，先卸下
            if (CurrentWeapon != null && CurrentWeapon.Id == weaponId)
            {
                UnequipCurrentWeapon();
            }

            return _weaponInventory.Remove(weaponId);
        }

        /// <summary>
        /// 从背包获取武器
        /// </summary>
        /// <param name="weaponId">武器ID</param>
        /// <returns>武器实例，如果不存在则返回null</returns>
        public Weapon GetWeaponFromInventory(int weaponId)
        {
            if (_weaponInventory.ContainsKey(weaponId))
            {
                return _weaponInventory[weaponId];
            }

            return null;
        }

        /// <summary>
        /// 获取背包中的所有武器
        /// </summary>
        /// <returns>武器列表</returns>
        public List<Weapon> GetAllWeaponsFromInventory()
        {
            List<Weapon> weapons = new List<Weapon>();

            foreach (var weapon in _weaponInventory.Values)
            {
                weapons.Add(weapon);
            }

            return weapons;
        }

        /// <summary>
        /// 尝试使用当前武器攻击
        /// </summary>
        /// <param name="target">目标实体</param>
        /// <returns>是否成功攻击</returns>
        public bool TryAttackWithCurrentWeapon(BaseEntity target)
        {
            if (CurrentWeapon == null || target == null)
            {
                return false;
            }

            return CurrentWeapon.TryAttack(target);
        }
    }
}
