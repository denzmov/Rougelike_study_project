using System.Collections.Generic;
using System.Text;
using Player.Weapon;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GameCore.UI
{
    // HUD-панель уровней для всего оружия игрока. Вешается на объект с Text на канвасе. Получает список оружий через Zenject.
    public class WeaponLevelView : MonoBehaviour
    {
        [SerializeField] private Text _text; // текстовое поле на канвасе
        private readonly List<BaseWeapon> _weapons = new List<BaseWeapon>();
        
        [Inject] private void Construct(List<BaseWeapon> weapons) // Zenject внедрит все зарегистрированные BaseWeapon списком
        {
            _weapons.AddRange(weapons);
        }

        private void OnEnable()
        {
            foreach (BaseWeapon weapon in _weapons) // защита от двойной подписки
            {
                weapon.LevelUpped -= OnLevelUpped;
                weapon.LevelUpped += OnLevelUpped;
            }

            Refresh(); // сразу показываем актуальные уровни
        }

        private void OnDisable()
        {
            foreach (BaseWeapon weapon in _weapons)
                weapon.LevelUpped -= OnLevelUpped;
        }

        private void OnLevelUpped() => Refresh();

        // собираем строки вида "FireBall — Lv 3" для каждого оружия
        private void Refresh()
        {
            StringBuilder builder = new StringBuilder();

            foreach (BaseWeapon weapon in _weapons)
            {
                if (builder.Length > 0)
                    builder.AppendLine(); // каждое оружие на своей строке

                builder.Append($"{weapon.DisplayName} — Lv {weapon.CurrentLevel}");
            }

            _text.text = builder.ToString();
        }
    }
}