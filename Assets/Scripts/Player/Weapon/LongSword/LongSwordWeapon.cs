using System.Collections;
using GameCore;
using UnityEngine;

namespace Player.Weapon.LongSword
{
    public class LongSwordWeapon : BaseWeapon, IActivatable
    {
        [Header("Single")] // конфигурация одного меча
        [SerializeField] private Transform _swordContainer1X;   // контейнер первого меча
        [SerializeField] private Transform _swordPivot1X;       // точка вращения первого меча

        [Header("Double")] // конфигурация второго меча (появляется с 4 уровня)
        [SerializeField] private Transform _swordContainer2X;   // контейнер второго меча
        [SerializeField] private Transform _swordPivot2X;       // точка вращения второго меча

        // Уровень, с которого появляется второй меч.
        private const int SecondSwordLevel = 4;

        private Coroutine _swordCoroutine;
        private float _rotationSpeed; // скорость вращения, растёт с уровнем
        private float _range;         // радиус вращения меча

        // Второй меч активен начиная с 4 уровня.
        private bool SecondSwordActive => CurrentLevel >= SecondSwordLevel;

        protected override void Start()
        {
            SetStats(0);
            LevelUp();
            LevelUp();
            LevelUp();
            LevelUp();
            LevelUp();
            LevelUp();
            SetupWeapon();
            Activate();
        }

        public void Activate()
        {
            SetStats(0);
            SetupWeapon();

            // Не допускаем запуск нескольких корутин одновременно.
            if (_swordCoroutine != null)
                StopCoroutine(_swordCoroutine);

            _swordCoroutine = StartCoroutine(RotateSwords());
        }

        public void Deactivate()
        {
            if (_swordCoroutine != null)
            {
                StopCoroutine(_swordCoroutine);
                _swordCoroutine = null;
            }
        }

        public override void LevelUp()
        {
            base.LevelUp();
            SetupWeapon();     // при повышении уровня пересобираем конфигурацию мечей
        }

        protected override void SetStats(int value)
        {
            base.SetStats(value);

            // Скорость вращения и радиус берём из характеристик текущего уровня.
            _rotationSpeed = WeaponStats[CurrentLevel - 1].Speed;
            _range = WeaponStats[CurrentLevel - 1].Range;

            // Разносим мечи по радиусу.
            ApplyRange();
        }

        // Расставляет мечи на нужный радиус от центра вращения.
        private void ApplyRange()
        {
            if (_swordPivot1X != null)
                _swordPivot1X.localPosition = new Vector3(_range, 0f, 0f);

            // Второй меч ставим строго напротив первого (через 180 градусов).
            if (_swordPivot2X != null)
                _swordPivot2X.localPosition = new Vector3(-_range, 0f, 0f);
        }

        // Включает/выключает второй меч в зависимости от уровня.
        private void SetupWeapon()
        {
            bool second = SecondSwordActive;

            if (_swordContainer2X != null)
                _swordContainer2X.gameObject.SetActive(second);

            // Первый меч есть всегда.
            if (_swordContainer1X != null)
                _swordContainer1X.gameObject.SetActive(true);
        }

        // Вращает контейнеры мечей вокруг персонажа.
        private IEnumerator RotateSwords()
        {
            while (true)
            {
                // Вращаем контейнеры по оси Z. Скорость зависит от уровня.
                if (_swordContainer1X != null)
                    _swordContainer1X.Rotate(0f, 0f, _rotationSpeed * Time.deltaTime);

                if (SecondSwordActive && _swordContainer2X != null)
                    _swordContainer2X.Rotate(0f, 0f, _rotationSpeed * Time.deltaTime);

                yield return null;
            }
        }
        
    }
}