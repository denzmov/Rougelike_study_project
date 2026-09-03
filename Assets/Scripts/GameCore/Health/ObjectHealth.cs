using System;
using UnityEngine;

namespace GameCore.Health
{
    public abstract class ObjectHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] private float _maxHealth;
        [SerializeField] private float _currentHealth;
        
        public event Action<float> Damaged; // Событие вызывается после получения урона. В параметр передаётся размер нанесённого урона.
        
        public float MaxHealth => _maxHealth;
        public float CurrentHealth => _currentHealth;

        private void OnEnable() => _currentHealth =  _maxHealth;

        public virtual void TakeDamage(float damage)
        {
            if(damage <= 0)
                throw new ArgumentOutOfRangeException(nameof(damage));
            _currentHealth -= damage;
            Damaged?.Invoke(damage); // Уведомляем EnemyDamageFlash и других подписчиков о том, что объект получил урон.
        }

        public void TakeHeal(float value)
        {
            if(value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value));
            _currentHealth += value;
            if(_currentHealth > _maxHealth)
                _currentHealth = _maxHealth;
        }
    }
}