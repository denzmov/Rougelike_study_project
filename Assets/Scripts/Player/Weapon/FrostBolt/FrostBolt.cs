using Enemy;
using UnityEngine;
using Zenject;

namespace Player.Weapon.FrostBolt
{
    public class FrostBolt : Projectile
    {
        
        private const int FreezeLevelThreshold = 5; // Порог уровня, с которого работает заморозка.
        private const float FreezeChance = 0.5f;  // Вероятность заморозки.
        private const float FreezeDuration = 2f; // Длительность полной остановки противника.
        private const float SlowMultiplier = 0.5f; // Коэффициент замедления на уровнях 1-4 (0.5 - скорость вдвое ниже).
        
        private FrostBoltWeapon _frostBoltWeapon;
        
        public void Init(FrostBoltWeapon weapon) // Задаётся оружием при выдаче снаряда.
        {
            _frostBoltWeapon = weapon;
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();
            Timer = new WaitForSeconds(_frostBoltWeapon.Duration);
            Damage = _frostBoltWeapon.Damage;
        }

        private void Update() => transform.position += transform.right * (_frostBoltWeapon.Speed * Time.deltaTime);

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            // Реагируем только на врагов.
            if (!other.gameObject.TryGetComponent(out EnemyHealth enemy))
                return;

            // 1. Урон - как и раньше.
            enemy.TakeDamage(Damage);

            // 2. Получаем компонент движения (с проверкой, чтобы не падать).
            bool hasMove = other.TryGetComponent(out EnemyMove enemyMove);

            // 3. Логика зависит от уровня оружия.
            if (_frostBoltWeapon.CurrentLevel <= 4)
            {
                // Уровни 1-4: замедляем врага и снаряд исчезает при попадании.
                if (hasMove)
                    enemyMove.ApplySlow(SlowMultiplier);

                gameObject.SetActive(false);
            }
            else
            {
                // Уровни 5-8: снаряд пролетает насквозь.
                // С шансом 50% дополнительно полностью останавливаем врага на 2 секунды.
                if (hasMove && Random.value < FreezeChance)
                    enemyMove.FreezeEnemy(FreezeDuration);
            }
        }

        [Inject]
        private void Construct(FrostBoltWeapon frostBoltWeapon) => 
            _frostBoltWeapon = frostBoltWeapon;
        
    }
}