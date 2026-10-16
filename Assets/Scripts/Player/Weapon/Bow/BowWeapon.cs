using System;
using System.Collections;
using System.Collections.Generic;
using GameCore.Pool;
using UnityEngine;
using Zenject;

namespace Player.Weapon.Bow
{
    public class BowWeapon : BaseWeapon
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _container, _shootPoint, _weaponTransform;
        [SerializeField] private ObjectPool _objectPool;
        [SerializeField] private Animator _animator;
        private WaitForSeconds _timeBetwenAttack;
        private PlayerMovement _playerMovement;
        private Coroutine _bowCoroutine;
        private Vector3 _direction;
        private float _duration, _speed;

        public float Duration => _duration;
        public float Speed => _speed;

        private void OnEnable() => Activate();

        private void Update()
        {
            _direction = _camera.ScreenToWorldPoint(Input.mousePosition) -  transform.position;
            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            _weaponTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        protected override void SetStats(int value)
        {
            base.SetStats(value);
            _timeBetwenAttack = new WaitForSeconds(WeaponStats[CurrentLevel -1].TimeBetweenAttack);
            _speed = WeaponStats[CurrentLevel -1].Speed;
            _duration = WeaponStats[CurrentLevel -1].Duration;
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
            _bowCoroutine = StartCoroutine(StartThrowArrow());
        }

        public void Deactivate()
        {
            if(_bowCoroutine != null)
                StopCoroutine(_bowCoroutine);
        }

        public void ThrowArrow()
        {
            GameObject arrow = _objectPool.GetFromPool();
            arrow.transform.SetParent(_container);
            arrow.transform.position = _shootPoint.position;
            arrow.transform.rotation = transform.rotation;
            _animator.SetTrigger("Idle");
        }
        
        private IEnumerator StartThrowArrow()
        {
            while (true)
            {
                if (_playerMovement.Movement != Vector3.zero)
                    _animator.SetTrigger("Attack");
                yield return _timeBetwenAttack;
            }
        }

        [Inject]
        private void Construct(PlayerMovement playerMovement)
        {
            _playerMovement = playerMovement;
        }
        
    }
}