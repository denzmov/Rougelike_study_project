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
            if(CurrentHealth <= 0)
                Debug.Log("Player is dead");
        }
        private IEnumerator Regeration()
        {
            while (true)
            {
                TakeHeal(_regenerationValue);
                OnHealthChanged?.Invoke();
                yield return _regenerationInterval;
            }
        }
        
    }
    
}