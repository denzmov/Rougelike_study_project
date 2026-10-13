using System.Collections;
using System.Collections.Generic;
using GameCore;
using GameCore.Pool;
using UnityEngine;

namespace Player.Weapon.Suriken
{
    public class SurikenWeapon : BaseWeapon
    {
        [SerializeField] private ObjectPool _objectPool;
        [SerializeField] private Transform _container;
        [SerializeField] private LayerMask _layerMask;
        private WaitForSeconds _timeBetweenAttack;
        private Coroutine _surikenCoroutine;
        private float _duration, _speed, _range;
        private Vector3 _direction;

        public float Duration => _duration;
        public float Speed => _speed;
        public Vector3 Direction => _direction;

        private void OnEnable()
        {
            Activate();
        }

        public void Activate()
        {
            SetStats(0);
            //LevelUp();
            //LevelUp();
            //LevelUp();
            //LevelUp();
            //LevelUp();
            //LevelUp();
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
                    float angle = Mathf.Atan2(_direction.y, -_direction.x) * Mathf.Rad2Deg;
                    GameObject suriken = _objectPool.GetFromPool();
                    suriken.transform.SetParent(_container);
                    suriken.transform.position = transform.position;
                    suriken.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
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