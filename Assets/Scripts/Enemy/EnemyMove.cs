using System.Collections;
using Player;
using UnityEngine;
using Zenject;

namespace Enemy
{
    public class EnemyMove : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed;
        //[SerializeField] private float _freezeTimer; // пока уберем, т.к. не используем.
        [SerializeField] private Animator _animator;
        //private Vector3 _direction; // оставляем локальной переменной внутри метода Move
        private PlayerMovement _playerMovement;
        private WaitForSeconds _checkTime =  new WaitForSeconds(3f);
        private Coroutine _distanceToHide;
        
        private float _slowMultiplier = 1f; // 1 — обычная скорость, 0.5 — замедление
        private int _slowSources;           // счётчик, сколько эффектов сейчас тормозят

        private void OnEnable()
        {
            _distanceToHide = StartCoroutine(CheckDistanceToHide());
            _slowMultiplier = 1f;
            _slowSources = 0;
        }

        private void OnDisable() => StopCoroutine(_distanceToHide);

        private void Update() => Move();

        private void Move()
        {
            Vector3 direction = (_playerMovement.transform.position - transform.position).normalized;
            //transform.position += direction * (_moveSpeed * Time.deltaTime); //без замедления
            transform.position += direction * (_moveSpeed * _slowMultiplier * Time.deltaTime); // с учетом замедления
            _animator.SetFloat("Horizontal", direction.x);
            _animator.SetFloat("Vertical", direction.y);
        }

        // Исчезновение противников на удаленной дситанции от игрока
        // Раз в некоторое время проверяем дистанцию
        private IEnumerator CheckDistanceToHide()
        {
            while (true)
            {
                float distance = Vector3.Distance(transform.position, _playerMovement.transform.position);
                if (distance > 20f)
                    gameObject.SetActive(false);
                yield return _checkTime;
            }
        }
        
        
        
        public void ApplySlow(float multiplier) // применить замедление (вызывает аура при входе врага в зону)
        {
            _slowMultiplier = multiplier;
            _slowSources++;
        }
        
        public void RemoveSlow() // снять замедление (аура при выходе врага из зоны)
        {
            _slowSources = Mathf.Max(0, _slowSources - 1);
            if (_slowSources == 0)
                _slowMultiplier = 1f; // никого не тормозим, обычная скорость
        }
        
        

        [Inject] private void Construct(PlayerMovement playerMovement) => _playerMovement = playerMovement;
    }
}