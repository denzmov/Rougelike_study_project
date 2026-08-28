using System;
using Player;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class HealthUIUpdater : MonoBehaviour
{
    [SerializeField] private Image _playerHealthImage;
    private PlayerHealth _playerHealth;

    private void OnEnable() => _playerHealth.OnHealthChanged += UpdateHealthBar;
    private void OnDisable() => _playerHealth.OnHealthChanged -= UpdateHealthBar;

    private void UpdateHealthBar()
    {
        _playerHealthImage.fillAmount = _playerHealth.CurrentHealth /  _playerHealth.MaxHealth;
        _playerHealthImage.fillAmount = Mathf.Clamp01(_playerHealthImage.fillAmount);
    }
    
    [Inject] private void Construct(PlayerHealth playerHealth) => _playerHealth = playerHealth;
}