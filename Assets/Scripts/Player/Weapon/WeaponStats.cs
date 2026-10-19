using UnityEngine;

namespace Player.Weapon
{
    [System.Serializable]
    public class WeaponStats
    {
        [SerializeField] private float _speed, _damage, _range, _timeBetweenAttack, _duration;
        [SerializeField] private float _returnDelay; // сколько секунд сюрикен летит вперёд, прежде чем повернуть к игроку. Для остальных оружий можно оставить 0.

        public float Speed => _speed;
        public float Damage => _damage;
        public float Range => _range;
        public float TimeBetweenAttack => _timeBetweenAttack;
        public float Duration => _duration;
        public float ReturnDelay => _returnDelay; // Публичный доступ к задержке возврата.
    }
}