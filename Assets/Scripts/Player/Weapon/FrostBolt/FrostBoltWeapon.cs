using System.Collections;
using System.Collections.Generic;
using GameCore.Pool;
using UnityEngine;

namespace Player.Weapon.FrostBolt
{
    public class FrostBoltWeapon : BaseWeapon
    {
        [SerializeField] private ObjectPool _objectPool;
        [SerializeField] private Transform _container;
        [SerializeField] private List<Transform> _shootPoints =  new List<Transform>(); //для запуска из разных точек
        private WaitForSeconds _timeBetweenAttack;
        private Coroutine _frostBoltCoroutine;
        private float _duration, _speed;
        private Vector3 _direction;

        public float Speed => _speed;
        public Vector3 Direction => _direction;
        public float Duration => _duration;


        private void OnEnable() => Activate();

        public void Activate()
        {
            SetStats(0);
            LevelUp();
            LevelUp();
            LevelUp();
            LevelUp();
            LevelUp();
            _frostBoltCoroutine = StartCoroutine(StartThrowFrozenBolt());
        }

        public void Deactivate()
        {
            if (_frostBoltCoroutine != null)
                StopCoroutine(_frostBoltCoroutine);
        }

        protected override void SetStats(int value)
        {
            base.SetStats(CurrentLevel -1);
            _timeBetweenAttack = new WaitForSeconds(WeaponStats[CurrentLevel - 1].TimeBetweenAttack);
            _speed = WeaponStats[CurrentLevel - 1].Speed;
            _duration = WeaponStats[CurrentLevel - 1].Duration;
        }

        private IEnumerator StartThrowFrozenBolt()
        {
            while (true)
            {
                for (int i = 0; i < _shootPoints.Count; i++)
                {
                    _direction = (_shootPoints[i].position - transform.position).normalized;
                    float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
                    GameObject frozenbolt = _objectPool.GetFromPool();
                    frozenbolt.transform.SetParent(_container);
                    frozenbolt.transform.position = transform.position;
                    frozenbolt.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                }
                yield return _timeBetweenAttack;
            }
        }
        
    }
}