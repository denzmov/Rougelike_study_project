using UnityEngine;
using Zenject;

namespace Player.Weapon.Bow
{
    public class Arrow : Projectile
    {
        private BowWeapon _bowWeapon;
        
        // Параметры конкретного выстрела (задаются в Init).
        private float _speed;
        private float _damage;
        private float _range;
        private Vector3 _direction;
        
        // Стартовая точка - для расчёта дальности.
        private Vector3 _startPosition;
        
        // Инициализация стрелы при выдаче из пула.
        public void Init(float speed, float damage, float range, Vector3 direction)
        {
            _speed = speed;
            _damage = damage;
            _range = range;

            // гарантируем, что стрела летит строго в плоскости XY.
            direction.z = 0f;
            _direction = direction.normalized;

            _startPosition = transform.position;

            // Поворот
            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            // Фолбэк, если Init ещё не вызван.
            if (_speed <= 0f)
            {
                Timer = new WaitForSeconds(_bowWeapon.Duration);
                Damage = _bowWeapon.Damage;
            }
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            base.OnTriggerEnter2D(other);

            // До 4 уровня стрела исчезает при попадании, с 5 — пробивает насквозь.
            if (_bowWeapon.CurrentLevel <= 4)
                gameObject.SetActive(false);
        }

        private void Update()
        {
            // Движение строго в плоскости XY.
            transform.position += _direction * (_speed * Time.deltaTime);

            // страховка - не даём позиции уползти по Z.
            Vector3 pos = transform.position;
            pos.z = 0f;
            transform.position = pos;

            if (_range > 0f &&
                Vector3.Distance(_startPosition, transform.position) > _range)
            {
                gameObject.SetActive(false);
            }
        }

        [Inject]
        private void Construct(BowWeapon bowWeapon) => _bowWeapon = bowWeapon;
    }
}