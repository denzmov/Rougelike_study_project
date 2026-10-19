using UnityEngine;
using Zenject;

namespace Player.Weapon.Suriken
{
    public class Suriken : Projectile
    {
        [SerializeField] private Transform _sprite;
        
        // Скорость вращения спрайта для визуального эффекта.
        private const float SpinSpeed = 500f;
        // Дистанция, на которой считаем, что сюрикен долетел до игрока.
        private const float CatchRadius = 0.01f;
        
        private SurikenWeapon _surikenWeapon;
        
        // Данные конкретного броска (задаются в Init)
        private Transform _player;         // цель возврата
        private Vector3 _flyDirection;     // начальное направление полёта
        private float _speed;              // скорость снаряда
        private float _returnDelay;        // сколько лететь вперёд до возврата
        private bool _returnsToPlayer;     // включён ли возврат (уровни 5-8)
        
        // Таймер полёта «вперёд».
        private float _timer;
        // Текущая фаза: false - летим вперёд, true - возвращаемся.
        private bool _returning;
        
        // Вызывается оружием при выдаче снаряда из пула.
        // Переустанавливает состояние каждый раз, т.к. объект переиспользуется пулом.
        public void Init(Transform player, Vector3 direction, float speed,
            float returnDelay, bool returnsToPlayer)
        {
            _player = player;
            _flyDirection = direction;
            _speed = speed;
            _returnDelay = returnDelay;
            _returnsToPlayer = returnsToPlayer;

            _timer = 0f;
            _returning = false;

            // Сразу выставляем поворот по направлению полёта.
            // FromToRotation устойчивее ручного Atan2 и не зеркалит по X.
            if (_flyDirection.sqrMagnitude > 0.0001f)
                transform.rotation = Quaternion.FromToRotation(Vector3.right, _flyDirection);
        }
        

        protected override void OnEnable()
        {
            base.OnEnable();
            Timer = new WaitForSeconds(_surikenWeapon.Duration);
            Damage = _surikenWeapon.Damage;
        }

        private void Update()
        {
            // Вращаем спрайт для эффекта «вертушки».
            _sprite.transform.Rotate(0, 0, SpinSpeed * Time.deltaTime);

            // Считаем время полёта вперёд.
            _timer += Time.deltaTime;

            // Определяем, пора ли возвращаться:
            // возврат включён И прошла задержка И игрок ещё есть.
            if (_returnsToPlayer && !_returning && _timer >= _returnDelay && _player != null)
            {
                _returning = true;
            }

            if (_returning && _player != null)
            {
                // Фаза возврата: каждый кадр пересчитываем направление на игрока.
                Vector3 toPlayer = (_player.position - transform.position).normalized;

                // Разворачиваем спрайт по направлению к игроку.
                transform.rotation = Quaternion.FromToRotation(Vector3.right, toPlayer);

                transform.position += toPlayer * (_speed * Time.deltaTime);

                // Долетел до игрока — прячем (пул вернёт объект).
                if (Vector3.Distance(transform.position, _player.position) <= CatchRadius)
                {
                    gameObject.SetActive(false);
                }
            }
            else
            {
                // Фаза полёта вперёд: летим строго по начальному направлению.
                transform.position += _flyDirection * (_speed * Time.deltaTime);
            }
        }
        
        [Inject]
        private void Construct(SurikenWeapon surikenWeapon) =>
            _surikenWeapon = surikenWeapon;
    }
}