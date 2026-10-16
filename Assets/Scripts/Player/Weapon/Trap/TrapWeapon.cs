using System.Collections;
using GameCore.Pool;
using UnityEngine;

namespace Player.Weapon.Trap
{
    public class TrapWeapon : BaseWeapon
    {
        [SerializeField] private ObjectPool _objectPool;
        [SerializeField] private Transform _container;
        private WaitForSeconds _timeBetweenAttack;
        private Coroutine _trapCoroutine;

        private void OnEnable() => Activate();
        
        public void Activate()
        {
            SetStats(0);
            LevelUp();
            LevelUp();
            LevelUp();
            LevelUp();
            LevelUp();
            _trapCoroutine =  StartCoroutine(SpawnTrap());
        }

        public void Deactivate()
        {
            if (_trapCoroutine != null)
                StopCoroutine(_trapCoroutine);
        }

        protected override void SetStats(int value)
        {
            base.SetStats(CurrentLevel -1);
            _timeBetweenAttack = new WaitForSeconds(WeaponStats[CurrentLevel - 1].TimeBetweenAttack);
        }

        private IEnumerator SpawnTrap()
        {
            while (true)
            {
                GameObject trap = _objectPool.GetFromPool();
                trap.transform.SetParent(_container);
                trap.transform.position = transform.position;
                yield return _timeBetweenAttack;
            }
        }
        
    }
}