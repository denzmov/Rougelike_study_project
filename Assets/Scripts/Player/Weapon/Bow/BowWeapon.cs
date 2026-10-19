using System;
using System.Collections;
using System.Collections.Generic;
using GameCore.Pool;
using UnityEngine;
using Zenject;
using UnityEngine.InputSystem;

namespace Player.Weapon.Bow
{
    public class BowWeapon : BaseWeapon
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _container, _shootPoint, _weaponTransform;
        [SerializeField] private ObjectPool _objectPool;
        [SerializeField] private Animator _animator;
        
        [Header("Charge")]
        [SerializeField] private float _maxChargeTime = 1.5f; // сколько держать для полного заряда
        [SerializeField] private float _minForce = 0.5f;       // множитель силы при мгновенном клике
        [SerializeField] private float _maxForce = 2f;         // множитель силы при полном заряде

        [Header("Input")]
        [SerializeField] private PlayerInput _playerInput;     // ссылка на PlayerInput игрока
        private InputAction _attackAction;
        
        private WaitForSeconds _timeBetweenAttack;
        private PlayerMovement _playerMovement;
        private Coroutine _bowCoroutine;
        private Vector3 _direction;
        private float _duration, _speed, _range;
        
        private float _charge; // Текущий накопленный заряд 0..1.
        private bool _isCharging; // Идет ли сейчас накопление заряда.

        public float Duration => _duration;
        public float Speed => _speed;

        //private void OnEnable() => Activate();
        
        private void OnEnable()
        {
            // Получаем действие Attack из карты Player.
            _attackAction = _playerInput.actions["Attack"];

            // Начинаем заряд при нажатии и выпускаем стрелу при отпускании.
            _attackAction.started += OnAttackStarted;
            _attackAction.canceled += OnAttackCanceled;

            Activate();
        }

        private void OnDisable()
        {
            // Отписываемся, чтобы не накопить обработчики при выключении объекта.
            if (_attackAction != null)
            {
                _attackAction.started -= OnAttackStarted;
                _attackAction.canceled -= OnAttackCanceled;
            }

            Deactivate();
        }
        

        private void Update()
         {
            // 1. Прицеливание: мышь или геймпад.
            UpdateAim();

            // 2. Накопление заряда, пока удерживают кнопку.
            if (_isCharging)
                _charge = Mathf.Clamp01(_charge + Time.deltaTime / _maxChargeTime);

            // 3. Визуализация: оттягиваем оружие назад пропорционально заряду.
            if (_weaponTransform != null)
                _weaponTransform.localPosition = Vector3.back * (_charge * 0.2f);
        }

        // Определяет направление прицела в зависимости от активной схемы управления.
        private void UpdateAim()
        {
            Vector3 aimDirection;

            bool gamepadScheme = _playerInput != null &&
                                 _playerInput.currentControlScheme == "Gamepad";

            if (gamepadScheme)
            {
                Vector2 look = _playerInput.actions["Look"].ReadValue<Vector2>();
                if (look.sqrMagnitude < 0.01f)
                    return;
                aimDirection = new Vector3(look.x, look.y, 0f); // Z уже 0
            }
            else
            {
                // Для мыши берём точку и обнуляем Z.
                Vector3 mouseWorld = _camera.ScreenToWorldPoint(Input.mousePosition);
                mouseWorld.z = transform.position.z;            // выравниваем по плоскости игрока
                aimDirection = mouseWorld - transform.position;
            }

            // НОВОЕ: жёстко обнуляем Z, чтобы вектор всегда был строго в плоскости XY.
            aimDirection.z = 0f;

            _direction = aimDirection.normalized;

            if (_weaponTransform != null)
            {
                float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
                _weaponTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }

        protected override void SetStats(int value)
        {
            base.SetStats(value);
            _timeBetweenAttack = new WaitForSeconds(WeaponStats[CurrentLevel - 1].TimeBetweenAttack);
            _speed = WeaponStats[CurrentLevel - 1].Speed;
            _duration = WeaponStats[CurrentLevel - 1].Duration;
            _range = WeaponStats[CurrentLevel - 1].Range; // дальность растёт с уровнем
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
            //_bowCoroutine = StartCoroutine(StartThrowArrow());
        }

        public void Deactivate()
        {
            if (_bowCoroutine != null)
                StopCoroutine(_bowCoroutine);
        }

        // Вызывается при нажатии Attack.
        private void OnAttackStarted(InputAction.CallbackContext context)
        {
            _isCharging = true;
            _charge = 0f;
        }

        // Вызывается при отпускании Attack.
        private void OnAttackCanceled(InputAction.CallbackContext context)
        {
            if (!_isCharging)
                return;

            _isCharging = false;
            ThrowArrow(_charge);
            _charge = 0f;
        }

        // Выстрел. force — множитель от 0..1 (заряд).
        public void ThrowArrow(float force01)
        {
            GameObject arrow = _objectPool.GetFromPool();
            arrow.transform.SetParent(_container);
            arrow.transform.position = _shootPoint.position;

            // Направление полёта = текущий прицел.
            arrow.transform.rotation = Quaternion.FromToRotation(Vector3.right, _direction);

            // Передаём стреле её параметры: скорость, урон, дальность + заряд.
            if (arrow.TryGetComponent(out Arrow arrowComp))
            {
                float forceMultiplier = Mathf.Lerp(_minForce, _maxForce, force01);
                arrowComp.Init(
                    _speed * forceMultiplier,       // скорость растёт с зарядом
                    Damage * forceMultiplier,       // урон растёт с зарядом
                    _range,                         // дальность по уровню
                    _direction                      // направление полёта
                );
            }

            _animator.SetTrigger("Attack");
        }
        
        // private IEnumerator StartThrowArrow()
        // {
        //     while (true)
        //     {
        //         if (_playerMovement.Movement != Vector3.zero)
        //             _animator.SetTrigger("Attack");
        //         yield return _timeBetweenAttack;
        //     }
        // }

        [Inject]
        private void Construct(PlayerMovement playerMovement)
        {
            _playerMovement = playerMovement;
        }
        
    }
}