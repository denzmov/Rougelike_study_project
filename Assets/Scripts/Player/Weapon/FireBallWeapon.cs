using System;
using System.Collections;
using System.Collections.Generic;
using GameCore;
using UnityEngine;

namespace Player.Weapon
{
    public class FireBallWeapon : BaseWeapon, IActivatable
    {
        [Header("Single")]
        [SerializeField] private SpriteRenderer _spriteRenderer1X;
        [SerializeField] private Collider2D _collider1X;
        [SerializeField] private Transform _transformSprite1X, _targetCaontainer1X;
        
        [Header("Double")]
        [SerializeField] private List<SpriteRenderer> _spriteRenderer2X = new List<SpriteRenderer>();
        [SerializeField] private List<Collider2D> _collider2X;
        [SerializeField] private List<Transform> _transformSprite2X;
        [SerializeField] private Transform _targetCaontainer2X;
        
        [Header("Triple")]
        [SerializeField] private List<SpriteRenderer> _spriteRenderer3X = new List<SpriteRenderer>();
        [SerializeField] private List<Collider2D> _collider3X;
        [SerializeField] private List<Transform> _transformSprite3X;
        [SerializeField] private Transform _targetCaontainer3X;
        
        // Пробуем равномерно разместить шары по окружнсоти. Для двух шаров получится расположение справа и слева. Для трёх шаров — равномерное расположение через 120 градусов.
        private void PlaceBallsEvenly(
            List<Transform> ballTransforms,
            List<Collider2D> colliders,
            float radius)
        {
            int count = Mathf.Min(ballTransforms.Count, colliders.Count);

            if (count == 0)
                return;

            for (int i = 0; i < count; i++)
            {
                if (ballTransforms[i] == null || colliders[i] == null)
                    continue;

                float angle = i * (360f / count) * Mathf.Deg2Rad;

                float x = Mathf.Cos(angle) * radius;
                float y = Mathf.Sin(angle) * radius;

                Vector3 position = new Vector3(x, y, 0f);

                // Позиция визуального шара.
                ballTransforms[i].localPosition = position;

                //colliders[i].offset = new Vector2(x, y); // Если Collider2D находится на отдельном объекте-контейнере
                colliders[i].offset = Vector2.zero; // Если Collider2D находится на самом объекте шара
            }
        }
        
        
        private WaitForSeconds _interval, _duration, _timeBetweenAttack;
        private float _rotationSpeed, _range;
        private Coroutine _attackCoroutine;

        protected virtual void Start()
        {
            SetStats(0);
            SetupWeapon();
            
            LevelUp();
            LevelUp();
            LevelUp();
            LevelUp();
            LevelUp();
            LevelUp();
            LevelUp();
            
            Activate();
            
        }

        private void Update() => transform.Rotate(0, 0, _rotationSpeed * Time.deltaTime);
        
        
        // старый запуск и остановка корутины
        // public void Activate() => _attackCoroutine = StartCoroutine(WeaponLifeCycle()); 
        // public void Deactivate()
        // {
        //     if(_attackCoroutine != null)
        //         StopCoroutine(_attackCoroutine);
        // }
        
        // новый запуск корутины
        public void Activate()
        {
            // Не допускаем запуск нескольких WeaponLifeCycle одновременно
            if (_attackCoroutine != null)
                StopCoroutine(_attackCoroutine);

            _attackCoroutine = StartCoroutine(WeaponLifeCycle());
        }

        public void Deactivate()
        {
            if (_attackCoroutine != null)
            {
                StopCoroutine(_attackCoroutine);
                // Ссылка больше не указывает на остановленную корутину.
                _attackCoroutine = null;
            }
        }
        

        public override void LevelUp()
        {
            base.LevelUp();
            SetupWeapon();
        }

        protected override void SetStats(int value)
        {
            base.SetStats(value);
            _rotationSpeed = WeaponStats[CurrentLevel - 1].Speed;
            _range = WeaponStats[CurrentLevel - 1].Range;
            _duration = new WaitForSeconds(WeaponStats[CurrentLevel - 1].Duration);
            _timeBetweenAttack = new WaitForSeconds(WeaponStats[CurrentLevel - 1].TimeBetweenAttack);
        }

        // Вспомогательный метод. Включает или выключает все элементы группы одновременно. Возвращает новое состояние группы.
        private bool ToggleGroup(
            List<SpriteRenderer> renderers,
            List<Collider2D> colliders)
        {
            if (renderers == null ||
                renderers.Count == 0 ||
                renderers[0] == null)
            {
                return false;
            }

            bool newState = !renderers[0].enabled;

            for (int i = 0; i < renderers.Count; i++)
            {
                if (renderers[i] == null)
                    continue;

                renderers[i].enabled = newState;

                if (i < colliders.Count &&
                    colliders[i] != null)
                {
                    colliders[i].enabled = newState;
                }
            }

            return newState;
        }
        
        
        private void SetupWeapon() // новая версия реализации
        {
            if (CurrentLevel < 4)
            {
                _targetCaontainer1X.gameObject.SetActive(true);
                _targetCaontainer2X.gameObject.SetActive(false);
                _targetCaontainer3X.gameObject.SetActive(false);

                _transformSprite1X.localPosition =
                    new Vector3(_range, 0f, 0f);

                _collider1X.offset =
                    new Vector2(_range, 0f);
            }
            else if (CurrentLevel < 6)
            {
                _targetCaontainer1X.gameObject.SetActive(false);
                _targetCaontainer2X.gameObject.SetActive(true);
                _targetCaontainer3X.gameObject.SetActive(false);

                PlaceBallsEvenly(
                    _transformSprite2X,
                    _collider2X,
                    _range);
            }
            else
            {
                _targetCaontainer1X.gameObject.SetActive(false);
                _targetCaontainer2X.gameObject.SetActive(false);
                _targetCaontainer3X.gameObject.SetActive(true);

                PlaceBallsEvenly(
                    _transformSprite3X,
                    _collider3X,
                    _range);
            }
        }
        
        
        // private void SetupWeapon() // первая версия
        // {
        //     if (CurrentLevel < 4) // одиночный шар до 4 уровня
        //     {
        //         _targetCaontainer1X.gameObject.SetActive(true);
        //         _targetCaontainer2X.gameObject.SetActive(false);
        //         _targetCaontainer3X.gameObject.SetActive(false);
        //         _transformSprite1X.localPosition = new Vector3(_range, 0, 0);
        //         _collider1X.offset = new Vector2(_range, 0);
        //     }
        //     else if (CurrentLevel < 6) // два шара на уровнях 4-5
        //     {
        //         _targetCaontainer1X.gameObject.SetActive(false);
        //         _targetCaontainer2X.gameObject.SetActive(true);
        //         _targetCaontainer3X.gameObject.SetActive(false);
        //         for (int i = 0; i < _collider2X.Count; i++)
        //             _collider2X[i].gameObject.SetActive(true);
        //         _transformSprite2X[0].localPosition = new Vector3(_range, 0, 0);
        //         _transformSprite2X[1].localPosition = new Vector3(-_range, 0, 0);
        //         _collider2X[0].offset = new Vector2(_range, 0);
        //         _collider2X[1].offset = new Vector2(-_range, 0);
        //     }
        //     else // три шара на уровнях 6+
        //     {
        //         _targetCaontainer1X.gameObject.SetActive(false);
        //         _targetCaontainer2X.gameObject.SetActive(false);
        //         _targetCaontainer3X.gameObject.SetActive(true);
        //         for (int i = 0; i < _collider3X.Count; i++)
        //             _collider3X[i].gameObject.SetActive(true);
        //         _transformSprite3X[0].localPosition = new Vector3(_range, 0, 0);
        //         _transformSprite3X[1].localPosition = new Vector3(_range, 0, 0);
        //         _transformSprite3X[2].localPosition = new Vector3(-_range, 0, 0);
        //         _collider3X[0].offset = new Vector2(_range, 0);
        //         _collider3X[1].offset = new Vector2(-_range, 0);
        //         _collider3X[2].offset = new Vector2(-_range, 0);
        //     }
        // }
        
        // private IEnumerator WeaponLifeCycle() // старый WeaponLifeCycle
        // {
        //     while (true)
        //     {
        //         if (CurrentLevel < 4)
        //         {
        //             _spriteRenderer1X.enabled = !_spriteRenderer1X.enabled;
        //             _collider1X.enabled = !_collider1X.enabled;
        //         }
        //         else if (CurrentLevel < 6)
        //         {
        //             for (int i = 0; i < _spriteRenderer2X.Count; i++)
        //             {
        //                 _spriteRenderer2X[i].enabled = !_spriteRenderer2X[i].enabled;
        //                 _collider2X[i].enabled = !_collider2X[i].enabled;
        //             }
        //         }
        //         else
        //         {
        //             for (int i = 0; i < _spriteRenderer3X.Count; i++)
        //             {
        //                 _spriteRenderer3X[i].enabled = !_spriteRenderer3X[i].enabled;
        //                 _collider3X[i].enabled = !_collider3X[i].enabled;
        //             }
        //         }
        //         
        //         _interval = _spriteRenderer1X.enabled || _spriteRenderer2X[0] || _spriteRenderer3X[0] ? _duration : _timeBetweenAttack;
        //         yield return _interval;
        //     }
        // }
        
        private IEnumerator WeaponLifeCycle() // обновленный WeaponLifeCycle
        {
            while (true)
            {
                bool isAttacking;

                if (CurrentLevel < 4)
                {
                    _spriteRenderer1X.enabled =
                        !_spriteRenderer1X.enabled;

                    _collider1X.enabled =
                        _spriteRenderer1X.enabled;

                    isAttacking =
                        _spriteRenderer1X.enabled;
                }
                else if (CurrentLevel < 6)
                {
                    isAttacking = ToggleGroup(
                        _spriteRenderer2X,
                        _collider2X);
                }
                else
                {
                    isAttacking = ToggleGroup(
                        _spriteRenderer3X,
                        _collider3X);
                }
                // Теперь проверяется состояние SpriteRenderer, а не существование объекта в списке.
                _interval = isAttacking
                    ? _duration
                    : _timeBetweenAttack;

                yield return _interval;
            }
        }
        
    }
}