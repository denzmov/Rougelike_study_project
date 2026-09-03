using System.Collections;
using System.Collections.Generic;
using Enemy;
using GameCore;
using Unity.VisualScripting;
using UnityEngine;

namespace Player.Weapon
{
    public class AuraWeapon : BaseWeapon, IActivatable
    {
        [SerializeField] private Transform _targetContainer;
        [SerializeField] private CircleCollider2D _collider;
        private List<EnemyHealth> _enemyInZone = new List<EnemyHealth>();
        private WaitForSeconds _timeBetweenAttack;
        private Coroutine _auraCoroutine;
        private float _range;
        
        // Замедление аурой
        private bool SlowActive => CurrentLevel >= 5; // Замедление начинает работать с 5-го уровня ауры.
        private bool _slowWasActive; // Запоминаем предыдущее состояние замедления. Это не позволяет несколько раз замедлить одного врага при повторном вызове SetStats().
        private const float SlowMultiplier = 0.5f; // Коэффициент скорости, 1f - обычная скорость, 0.5f - скорость уменьшена в два раза.

        protected override void Start()
        {
            SetStats(0);
            Activate();
            LevelUp();
            LevelUp();
            LevelUp();
            LevelUp();
            LevelUp();
        }


        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.TryGetComponent(out EnemyHealth enemy)) 
            {
                // Не добавляем одного и того же врага в список повторно.
                if (!_enemyInZone.Contains(enemy))
                    _enemyInZone.Add(enemy);
                // Если аура уже достигла 5-го уровня - сразу замедляем врага при входе в зону.
                if (SlowActive &&
                    enemy.TryGetComponent(out EnemyMove enemyMove))
                {
                    enemyMove.ApplySlow(SlowMultiplier);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.TryGetComponent(out EnemyHealth enemy))
            {
                // Снимаем замедление с врага при выходе из зоны.
                if (enemy.TryGetComponent(out EnemyMove enemyMove))
                    enemyMove.RemoveSlow();

                _enemyInZone.Remove(enemy);
            }
        }

        public void Activate()
        {
            SetStats(0);
            _auraCoroutine = StartCoroutine(CheckZone());
        }

        public void Deactivate()
        {
            if (_auraCoroutine != null)
            {
                StopCoroutine(_auraCoroutine);
                _auraCoroutine = null;
            }
            // Если аура отключается, снимаем замедление со всех врагов, которые ещё находятся в зоне.
            RemoveSlowFromEnemies();
        }
        
        protected override void SetStats(int value)
        {
            base.SetStats(value);
            _timeBetweenAttack =  new WaitForSeconds(WeaponStats[CurrentLevel - 1].TimeBetweenAttack);
            _range = WeaponStats[CurrentLevel - 1].Range;
            _targetContainer.transform.localScale = Vector3.one * _range;
            _collider.radius = _range / 3f;

            UpdateSlowEffects(); // После изменения уровня проверяем уже находящихся в зоне врагов и актуализируем замедление.
        }
        
        private void UpdateSlowEffects()
        {
            // Определяем, изменилось ли состояние замедления.
            bool slowStateChanged = SlowActive != _slowWasActive;

            // Если состояние не изменилось, повторно применять эффект не нужно. Иначе счётчик _slowSources будет увеличиваться при каждом вызове SetStats().
            if (!slowStateChanged)
                return;

            if (SlowActive)
            {
                ApplySlowToEnemies();
            }
            else
            {
                RemoveSlowFromEnemies();
            }
            _slowWasActive = SlowActive; // Запоминаем новое состояние.
        }

        // применяем замедление ко всем врагам, которые уже находятся внутри ауры.
        private void ApplySlowToEnemies()
        {
            for (int i = 0; i < _enemyInZone.Count; i++)
            {
                EnemyHealth enemy = _enemyInZone[i];

                if (enemy == null)
                    continue;

                if (enemy.TryGetComponent(out EnemyMove enemyMove))
                    enemyMove.ApplySlow(SlowMultiplier);
            }
        }

        // снимаем замедление со всех врагов, которые находятся в зоне ауры.
        private void RemoveSlowFromEnemies()
        {
            for (int i = 0; i < _enemyInZone.Count; i++)
            {
                EnemyHealth enemy = _enemyInZone[i];

                if (enemy == null)
                    continue;

                if (enemy.TryGetComponent(out EnemyMove enemyMove))
                    enemyMove.RemoveSlow();
            }
        }

        private IEnumerator CheckZone()
        {
            while (true)
            {
                for (int i = 0; i < _enemyInZone.Count; i++) 
                {
                    if (_enemyInZone[i] != null) // Проверяем, не был ли враг уничтожен или отключён object pool.
                        _enemyInZone[i].TakeDamage(_damage);
                }
                yield return _timeBetweenAttack;
            }
        }

        
    }
}