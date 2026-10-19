using System.Collections;
using GameCore.Pool;
using UnityEngine;
using Player;

namespace Player.Weapon.Suriken
{
    public class SurikenWeapon : BaseWeapon
    {
        [SerializeField] private ObjectPool _objectPool;
        [SerializeField] private Transform _container;
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private Transform _player; // Ссылка на игрока - к нему будет возвращаться сюрикен.
      
        private WaitForSeconds _timeBetweenAttack;
        private Coroutine _surikenCoroutine;
        private float _duration, _speed, _range, _returnDelay;
        private Vector3 _direction;

        public float Duration => _duration;
        public float Speed => _speed;
        public Vector3 Direction => _direction;
        
        public bool ReturnsToPlayer => CurrentLevel >= 5; // На уровнях 5-8 сюрикен возвращается к игроку.
        public float ReturnDelay => _returnDelay; // Задержка возврата берётся из характеристик текущего уровня.

        private void OnEnable()
        {
            Activate();
        }

        public void Activate()
        {
            SetStats(0);
            LevelUp();
            LevelUp();
            LevelUp();
            LevelUp();
            LevelUp();
            LevelUp();
            _surikenCoroutine = StartCoroutine(SpawnSuriken());
        }

        public void Deactivate()
        {
            if (_surikenCoroutine != null)
                StopCoroutine(_surikenCoroutine);
        }

        protected override void SetStats(int value)
        {
            base.SetStats(CurrentLevel -1);
            _timeBetweenAttack = new WaitForSeconds(WeaponStats[CurrentLevel - 1].TimeBetweenAttack);
            _speed = WeaponStats[CurrentLevel - 1].Speed;
            _range = WeaponStats[CurrentLevel - 1].Range;
            _duration = WeaponStats[CurrentLevel - 1].Duration;
            _returnDelay = WeaponStats[CurrentLevel - 1].ReturnDelay; // Задержка перед возвратом для текущего уровня.
        }

        private IEnumerator SpawnSuriken()
        {
            while (true)
            {
                Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, _range, _layerMask);
                if (enemiesInRange.Length > 0)
                {
                    Vector3 targetPosition = enemiesInRange[Random.Range(0, enemiesInRange.Length)].transform.position;
                    _direction = (targetPosition - transform.position).normalized;
                    float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
                    GameObject suriken = _objectPool.GetFromPool();
                    suriken.transform.SetParent(_container);
                    suriken.transform.position = transform.position;
                    suriken.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                    
                    // Передаём снаряду данные о полёте и о том, должен ли он вернуться.
                    // Это надёжнее инжекта, потому что объект берётся из пула.
                    if (suriken.TryGetComponent(out Suriken projectile))
                    {
                        projectile.Init(
                            _player,                 // к кому возвращаться
                            _direction,              // куда лететь сначала
                            _speed,                  // скорость
                            _returnDelay,            // через сколько секунд повернуть назад
                            ReturnsToPlayer          // включён ли возврат (уровни 5-8)
                        );
                    }
                    
                    yield return _timeBetweenAttack;
                }
                else
                {
                    yield return _timeBetweenAttack;
                }
            }
        }
        
    }
}