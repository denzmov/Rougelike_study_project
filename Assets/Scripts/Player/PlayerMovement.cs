using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        private enum ControlType
        {
            Keyboard,
            Joystick
        }

        [Header("Movement")]
        [SerializeField] private float _moveSpeed;
        [SerializeField] private ControlType _controlType = ControlType.Keyboard;

        [Header("References")]
        [SerializeField] private Joystick _joystick;
        [SerializeField] private Animator _animator;

        private Vector3 _movement;

        public Vector3 Movement => _movement;

        private void Update()
        {
            Move();
            UpdateAnimation();
        }

        private void Move()
        {
            float horizontal;
            float vertical;

            if (_controlType == ControlType.Joystick && _joystick != null)
            {
                horizontal = _joystick.Horizontal;
                vertical = _joystick.Vertical;
            }
            else
            {
                horizontal = Input.GetAxisRaw("Horizontal");
                vertical = Input.GetAxisRaw("Vertical");
            }

            _movement = new Vector3(horizontal, vertical, 0f);

            // Чтобы скорость по диагонали не была выше обычной
            if (_movement.sqrMagnitude > 1f)
            {
                _movement.Normalize();
            }

            transform.position += _movement *
                                  (_moveSpeed * Time.deltaTime);
        }

        private void UpdateAnimation()
        {
            if (_animator == null)
            {
                return;
            }

            _animator.SetFloat("Horizontal", _movement.x);
            _animator.SetFloat("Vertical", _movement.y);
            _animator.SetFloat("Speed", _movement.sqrMagnitude);
        }
    }
}