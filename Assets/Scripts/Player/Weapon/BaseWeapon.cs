using System.Collections.Generic;
using Enemy;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Player.Weapon
{
    public abstract class BaseWeapon : MonoBehaviour
    {
        [SerializeField] private List<WeaponStats> _weaponStats = new List<WeaponStats>();
        [SerializeField] private float _damage;
        private DiContainer _diContainer;
        private int _currentLevel = 1; // текущий изначальный уровень прокачки оружия
        private int _maxLevel = 8; // максимальный уровень прокачки оружия

        public List<WeaponStats> WeaponStats => _weaponStats;
        public float Damage => _damage;
        public int CurrentLevel => _currentLevel;
        public int MaxLevel => _maxLevel;

        protected virtual void Awake() => _diContainer.Inject(this);

        protected virtual void Start() => SetStats(0);

        public virtual void LevelUp()
        {
            if(CurrentLevel < _maxLevel)
                _currentLevel++;
            SetStats(_currentLevel-1);
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.TryGetComponent(out EnemyHealth enemy))
            {
                float damage = Random.Range(_damage / 2f, _damage * 1.5f);
                enemy.TakeDamage(damage);
            }
        }

        protected virtual void SetStats(int value) => _damage = _weaponStats[value].Damage; // устанавиваем для поля damage то значение урона, которое содержит тот или иной уровень

        [Inject] public void Construct(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }
        
    }
}