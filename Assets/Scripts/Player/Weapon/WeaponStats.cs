using UnityEngine;

namespace Player.Weapon
{
    public class WeaponStats
    {
        [SerializeField] private float _speed, _damage, _range, _timeBetweenAttack, _duration;

        public float Speed => _speed;
        public float Damage => _damage;
        public float Range => _range;
        public float TimeBetweenAttack => _timeBetweenAttack;
        public float Duration => _duration;
    }
}