using System.Collections.Generic;
using Enemy;
using UnityEngine;

namespace Player.Weapon.LongSword
{
    // Скрипт одного клинка. Висит на объекте меча, где находится Collider2D.
    public class LongSword : MonoBehaviour
    {
        private LongSwordWeapon _weapon;

        // Время последнего удара по каждому врагу — чтобы не бить каждый кадр.
        private readonly Dictionary<EnemyHealth, float> _lastHitTime =
            new Dictionary<EnemyHealth, float>();

        [SerializeField] private float _hitCooldown = 0.5f;

        private void Awake()
        {
            // Ищем оружие в родительской иерархии — надёжнее инжекта
            // для объекта, который лежит на сцене и не создаётся Zenject.
            _weapon = GetComponentInParent<LongSwordWeapon>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(out EnemyHealth enemy))
                return;

            // Защита: если оружие почему-то не нашли — не падаем.
            if (_weapon == null)
            {
                Debug.LogWarning("LongSword: LongSwordWeapon не найден в родителях.");
                return;
            }

            if (_lastHitTime.TryGetValue(enemy, out float lastHit) &&
                Time.time - lastHit < _hitCooldown)
            {
                return;
            }

            _lastHitTime[enemy] = Time.time;
            enemy.TakeDamage(_weapon.Damage);
        }
    }
}