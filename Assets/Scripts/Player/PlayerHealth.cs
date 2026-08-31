using System;
using System.Collections;
using GameCore.Health;
using UnityEngine;

namespace Player
{
    public class PlayerHealth : ObjectHealth
    {
        public Action OnHealthChanged;
        private WaitForSeconds _regenerationInterval = new WaitForSeconds(5f);
        private float _regenerationValue = 1f;
        // сылка на активную корутину кровотечения
        private Coroutine _dotRoutine;
        // ---

        private void Start() => StartCoroutine(Regeration());

        public void Heal(float value)
        {
            TakeHeal(value);
            OnHealthChanged?.Invoke();
        }

        public override void TakeDamage(float damage)
        {
            base.TakeDamage(damage);
            OnHealthChanged?.Invoke();
            if (CurrentHealth <= 0)
            {
                // останавка DoT при смерти =====
                StopDoT();
                // ---
                Debug.Log("Player is dead");
            }
        }
        
        // DoT
        public void ApplyDoT(float damagePerTick, float interval, float duration)
        {
            // если есть кровотечение — перезапускается, а не складываем
            StopDoT();
            _dotRoutine = StartCoroutine(DoTRoutine(damagePerTick, interval, duration));
        }
        // ----
        
        private IEnumerator Regeration()
        {
            while (true)
            {
                TakeHeal(_regenerationValue);
                OnHealthChanged?.Invoke();
                yield return _regenerationInterval;
            }
        }
        
        // DoT - корутина периодического урона =====
        private IEnumerator DoTRoutine(float damagePerTick, float interval, float duration)
        {
            float elapsed = 0f;

            while (elapsed < duration && CurrentHealth > 0)
            {
                TakeDamage(damagePerTick);
                elapsed += interval;
                yield return new WaitForSeconds(interval);
            }

            _dotRoutine = null;
        }
        // -----

        // DoT - остановка кровотечения
        private void StopDoT()
        {
            if (_dotRoutine != null)
            {
                StopCoroutine(_dotRoutine);
                _dotRoutine = null;
            }
        }
        // ---
        
    }
    
}