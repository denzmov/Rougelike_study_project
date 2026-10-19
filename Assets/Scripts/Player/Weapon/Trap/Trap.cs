using System.Collections;
using Enemy;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace Player.Weapon.Trap
{
    public class Trap : Projectile
    {
        [SerializeField] private CircleCollider2D _collider;
        
        // Пороги уровней: с 3 - замедление, с 6 - оглушение.
        private const int SlowLevelThreshold = 3;
        private const int StunLevelThreshold = 6;
        private const float SlowMultiplier = 0.5f; // скорость вдвое ниже
        private const float SlowDuration = 3f;     // сколько держится замедление
        private const float StunDuration = 2f;     // длительность оглушения
        
        private WaitForSeconds _checkInterval =  new WaitForSeconds(3f);
        private PlayerHealth _playerHealth;
        private TrapWeapon _trapWeapon;
        
        // НОВОЕ: храним ссылки на корутины, чтобы их можно было остановить.
        private Coroutine _checkDistanceCoroutine;
        private Coroutine _removeSlowCoroutine;

        protected override void OnEnable()
        {
            Damage = _trapWeapon.Damage;
            _collider.enabled = false;

            // Сохраняем ссылку, а не запускаем "в никуда".
            _checkDistanceCoroutine = StartCoroutine(CheckDistance());
        }

        // НОВОЕ: останавливаем корутины при уходе в пул.
        protected void OnDisable()
        {
            if (_checkDistanceCoroutine != null)
            {
                StopCoroutine(_checkDistanceCoroutine);
                _checkDistanceCoroutine = null;
            }

            if (_removeSlowCoroutine != null)
            {
                StopCoroutine(_removeSlowCoroutine);
                _removeSlowCoroutine = null;
            }
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.TryGetComponent(out EnemyHealth enemy))
                return;

            enemy.TakeDamage(Damage);
            if (enemy.gameObject.activeSelf)
                enemy.Burn(Damage);

            ApplyControlEffect(other);
            gameObject.SetActive(false);
        }

        private void ApplyControlEffect(Collider2D other)
        {
            if (!other.TryGetComponent(out EnemyMove enemyMove))
                return;

            if (_trapWeapon.CurrentLevel >= StunLevelThreshold)
            {
                enemyMove.FreezeEnemy(StunDuration);
            }
            else if (_trapWeapon.CurrentLevel >= SlowLevelThreshold)
            {
                enemyMove.ApplySlow(SlowMultiplier);

                // Снимаем замедление корутиной, ссылку сохраняем.
                _removeSlowCoroutine = StartCoroutine(RemoveSlowAfter(enemyMove, SlowDuration));
            }
        }

        private IEnumerator RemoveSlowAfter(EnemyMove enemyMove, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (enemyMove != null)
                enemyMove.RemoveSlow();
            _removeSlowCoroutine = null;
        }

        public void ActivateTrap() => _collider.enabled = true;

        private IEnumerator CheckDistance()
        {
            while (true)
            {
                if (Vector3.Distance(transform.position, _playerHealth.gameObject.transform.position) > 15f)
                    gameObject.SetActive(false);
                yield return _checkInterval;
            }
        }
        
        [Inject]
        private void Construct(PlayerHealth playerHealth, TrapWeapon trapWeapon)
        {
            _playerHealth = playerHealth;
            _trapWeapon = trapWeapon;
        }

    }
}