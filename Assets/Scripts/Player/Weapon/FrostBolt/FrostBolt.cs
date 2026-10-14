using Enemy;
using UnityEngine;
using Zenject;

namespace Player.Weapon.FrostBolt
{
    public class FrostBolt : Projectile
    {
        private FrostBoltWeapon _frostBoltWeapon;

        protected override void OnEnable()
        {
            base.OnEnable();
            Timer = new WaitForSeconds(_frostBoltWeapon.Duration);
            Damage = _frostBoltWeapon.Damage;
        }

        private void Update() => transform.position += transform.right * (_frostBoltWeapon.Speed * Time.deltaTime);

        private protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.TryGetComponent(out EnemyHealth enemy))
            {
                enemy.TakeDamage(Damage);
                enemy.GetComponent<EnemyMove>().FreezeEnemy();
            }

            if (_frostBoltWeapon.CurrentLevel <= 4)
                gameObject.SetActive(false); // Если уровень оружия будет больше 4, то выстрелы будут пролетать насквозь врагов.
            {
                
            }
        }

        [Inject]
        private void Construct(FrostBoltWeapon frostBoltWeapon) => 
            _frostBoltWeapon = frostBoltWeapon;
        
    }
}