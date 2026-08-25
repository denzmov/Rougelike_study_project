using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        /// <summary>
        /// Источник управления персонажем.
        /// </summary>
        private enum ControlType
        {
            Keyboard,
            Joystick
        }

        /// <summary>
        /// Текущее состояние персонажа.
        /// </summary>
        private enum PlayerState
        {
            Idle,
            Run,
            Interacting
        }

        [Header("Movement")]

        // Скорость движения персонажа.
        [SerializeField] private float _moveSpeed;

        // Выбор управления в Inspector.
        [SerializeField] private ControlType _controlType =
            ControlType.Keyboard;

        [Header("Input")]

        // Ссылка на Joystick Pack.
        // Заполняется, если выбран тип управления Joystick.
        [SerializeField] private Joystick _joystick;

        [Header("Animation")]

        // Ссылка на Animator персонажа.
        [SerializeField] private Animator _animator;

        // Минимальное значение ввода, которое считается движением.
        // Особенно полезно для экранного джойстика.
        [SerializeField] private float _inputDeadZone = 0.1f;

        // Текущее движение персонажа.
        private Vector3 _movement;

        // Последнее направление, в котором смотрел персонаж.
        // По умолчанию персонаж смотрит вниз.
        private Vector2 _lastDirection = Vector2.down;

        // Текущее состояние персонажа.
        private PlayerState _currentState = PlayerState.Idle;

        // Хэши параметров Animator.
        private static readonly int HorizontalHash =
            Animator.StringToHash("Horizontal");

        private static readonly int VerticalHash =
            Animator.StringToHash("Vertical");

        private static readonly int SpeedHash =
            Animator.StringToHash("Speed");

        private static readonly int IsInteractingHash =
            Animator.StringToHash("IsInteracting");

        /// <summary>
        /// Текущее направление движения персонажа.
        /// </summary>
        public Vector3 Movement => _movement;

        /// <summary>
        /// Возвращает true, если персонаж взаимодействует с объектом.
        /// </summary>
        public bool IsInteracting =>
            _currentState == PlayerState.Interacting;

        private void Update()
        {
            // Во время взаимодействия ввод не считываем,
            // чтобы персонаж не мог двигаться.
            if (_currentState != PlayerState.Interacting)
            {
                ReadInput();
                Move();
            }
            else
            {
                // На всякий случай обнуляем движение.
                _movement = Vector3.zero;
            }

            // Обновляем параметры Animator каждый кадр.
            UpdateAnimation();
        }

        /// <summary>
        /// Считывает ввод с клавиатуры или экранного джойстика.
        /// </summary>
        private void ReadInput()
        {
            float horizontal;
            float vertical;

            if (_controlType == ControlType.Joystick &&
                _joystick != null)
            {
                // Аналог Input.GetAxisRaw("Horizontal").
                horizontal = _joystick.Horizontal;

                // Аналог Input.GetAxisRaw("Vertical").
                vertical = _joystick.Vertical;
            }
            else
            {
                // Управление с клавиатуры.
                horizontal = Input.GetAxisRaw("Horizontal");
                vertical = Input.GetAxisRaw("Vertical");
            }

            Vector2 input = new Vector2(horizontal, vertical);

            // Игнорируем слишком слабое отклонение джойстика.
            if (input.magnitude < _inputDeadZone)
            {
                input = Vector2.zero;
            }

            // Не позволяем двигаться по диагонали быстрее,
            // чем по одной оси.
            if (input.sqrMagnitude > 1f)
            {
                input.Normalize();
            }

            // Преобразуем двумерный ввод в движение по XY.
            _movement = new Vector3(input.x, input.y, 0f);

            // Направление взгляда обновляется только во время движения.
            // Поэтому после остановки персонаж сохраняет последнее направление.
            if (input != Vector2.zero)
            {
                _lastDirection = GetAnimationDirection(input);
            }
        }

        /// <summary>
        /// Перемещает персонажа без использования физики.
        /// </summary>
        private void Move()
        {
            transform.position += _movement *
                                  (_moveSpeed * Time.deltaTime);
        }

        /// <summary>
        /// Передаёт направление и скорость в Animator.
        /// </summary>
        private void UpdateAnimation()
        {
            if (_animator == null)
            {
                return;
            }

            // Передаём последнее направление взгляда.
            // Эти параметры используются Blend Tree внутри Idle и Run.
            _animator.SetFloat(
                HorizontalHash,
                _lastDirection.x
            );

            _animator.SetFloat(
                VerticalHash,
                _lastDirection.y
            );

            // Speed используется для переходов:
            //
            // Idle → Run, если Speed > 0.1
            // Run → Idle, если Speed < 0.1
            float speed = _movement.magnitude;

            _animator.SetFloat(SpeedHash, speed);

            // Передаём состояние взаимодействия.
            _animator.SetBool(
                IsInteractingHash,
                _currentState == PlayerState.Interacting
            );
        }

        /// <summary>
        /// Выбирает одно из четырёх основных направлений
        /// для Blend Tree.
        /// </summary>
        private Vector2 GetAnimationDirection(Vector2 input)
        {
            // Если горизонтальный ввод сильнее вертикального,
            // выбираем направление влево или вправо.
            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            {
                return new Vector2(
                    Mathf.Sign(input.x),
                    0f
                );
            }

            // Иначе выбираем направление вверх или вниз.
            return new Vector2(
                0f,
                Mathf.Sign(input.y)
            );
        }

        /// <summary>
        /// Запускает состояние взаимодействия.
        /// Этот метод можно вызвать из другого компонента.
        /// </summary>
        public void StartInteraction()
        {
            if (_currentState == PlayerState.Interacting)
            {
                return;
            }

            // Останавливаем персонажа.
            _movement = Vector3.zero;

            // Переводим его в состояние взаимодействия.
            _currentState = PlayerState.Interacting;

            // Сразу обновляем Animator.
            UpdateAnimation();
        }

        /// <summary>
        /// Завершает взаимодействие.
        /// </summary>
        public void StopInteraction()
        {
            if (_currentState != PlayerState.Interacting)
            {
                return;
            }

            // После завершения взаимодействия
            // персонаж возвращается в состояние ожидания.
            _currentState = PlayerState.Idle;

            // Движение остаётся остановленным,
            // поэтому Speed будет равен нулю.
            _movement = Vector3.zero;

            // Сразу обновляем Animator.
            UpdateAnimation();
        }
    }
}