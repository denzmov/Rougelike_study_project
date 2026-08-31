using Player;
using UnityEngine;

namespace Enemy
{
    public class EnemyCollision : MonoBehaviour
    {
        [SerializeField] private float _damage; // знаечение урона противника
        // Настройки DoT в Inspector:
        [SerializeField] private bool _applyBleeding;      // включён ли DoT у проивника
        [SerializeField] private float _dotDamagePerTick; // урон за один тик
        [SerializeField] private float _dotTickInterval;  // интервал между тиками
        [SerializeField] private float _dotDuration;      // сколько всего длится эффект
        // -----

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.TryGetComponent(out PlayerHealth player))
            {
                player.TakeDamage(_damage); // враг наносит игроку урон (= _damage)
                // запуск кровотечения на игроке (DoT)
                if (_applyBleeding && _dotDamagePerTick > 0f)
                    player.ApplyDoT(_dotDamagePerTick, _dotTickInterval, _dotDuration);
                // -------
                //player.OnHealthChanged?.Invoke(); //дубляж из PlayerHealth
                gameObject.SetActive(false);
            }
        }
    }
}