using System.Collections;
using GameCore.Health;
using UnityEngine;

namespace Enemy
{
    
    //визуальная вспышка спрайта врага при получении урона. Подписывается на событие ObjectHealth.Damaged и кратко красит спрайт в красный.
    [RequireComponent(typeof(SpriteRenderer))]
    
    public class EnemyDamageFlash : MonoBehaviour
    {
        [Header("Renderer с визуальным спрайтом врага")]

        // НОВОЕ:
        // Здесь должен быть SpriteRenderer объекта BeeSprite,
        // а не SpriteRenderer корневого объекта Bee.
        [SerializeField] private SpriteRenderer _spriteRenderer;

        [Header("Настройки вспышки")]

        // Цвет, в который временно окрашивается враг.
        [SerializeField] private Color _flashColor = Color.red;

        // Длительность вспышки в секундах.
        [SerializeField] private float _flashDuration;

        private ObjectHealth _health;
        private Coroutine _flashCoroutine;
        private Color _originalColor;

        private void Awake()
        {
            // EnemyHealth находится на этом же объекте Bee.
            _health = GetComponent<ObjectHealth>();

            if (_health == null)
            {
                Debug.LogError(
                    $"{name}: компонент ObjectHealth не найден.",
                    this);
            }

            if (_spriteRenderer == null)
            {
                Debug.LogError(
                    $"{name}: SpriteRenderer не назначен в инспекторе.",
                    this);
            }
            else
            {
                // Сохраняем исходный цвет спрайта.
                _originalColor = _spriteRenderer.color;
            }
        }

        private void OnEnable()
        {
            if (_health != null)
            {
                // Защита от повторной подписки при использовании object pool.
                _health.Damaged -= OnDamaged;
                _health.Damaged += OnDamaged;
            }

            // Возвращаем обычный цвет при повторной активации врага.
            if (_spriteRenderer != null)
                _spriteRenderer.color = _originalColor;
        }

        private void OnDisable()
        {
            if (_health != null)
                _health.Damaged -= OnDamaged;

            // Останавливаем незавершённую вспышку.
            if (_flashCoroutine != null)
            {
                StopCoroutine(_flashCoroutine);
                _flashCoroutine = null;
            }

            // Не оставляем врага красным в пуле.
            if (_spriteRenderer != null)
                _spriteRenderer.color = _originalColor;
        }

        private void OnDamaged(float damage)
        {
            if (!isActiveAndEnabled || _spriteRenderer == null)
                return;

            // При частых попаданиях перезапускаем вспышку.
            if (_flashCoroutine != null)
                StopCoroutine(_flashCoroutine);

            _flashCoroutine = StartCoroutine(FlashRoutine());
        }

        private IEnumerator FlashRoutine()
        {
            // Красим именно визуальный спрайт BeeSprite.
            _spriteRenderer.color = _flashColor;

            yield return new WaitForSeconds(_flashDuration);

            // Объект мог отключиться во время ожидания.
            if (_spriteRenderer != null && gameObject.activeInHierarchy)
                _spriteRenderer.color = _originalColor;

            _flashCoroutine = null;
        }
    }
}