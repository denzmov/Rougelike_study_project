using Player;
using Player.Weapon;
using Player.Weapon.FrostBolt;
using Player.Weapon.Suriken;
using UnityEngine;
using Zenject;

namespace DI
{
    public class PlayerInstaller: MonoInstaller
    {
        [SerializeField] private PlayerMovement _playerMovement;
        [SerializeField] private PlayerHealth _playerHealth;
        
        // ссылки на оружия игрока со сцены
        [SerializeField] private FireBallWeapon _fireBallWeapon;
        [SerializeField] private AuraWeapon _auraWeapon;
        [SerializeField] private SurikenWeapon _surikenWeapon;
        [SerializeField] private FrostBoltWeapon _frostBoltWeapon;
        
        public override void InstallBindings()
        {
            Container.Bind<PlayerMovement>().FromInstance(_playerMovement).AsSingle().NonLazy();
            Container.Bind<PlayerHealth>().FromInstance(_playerHealth).AsSingle().NonLazy();

            
            // биндим каждое оружие как BaseWeapon, а Zenject соберёт оба бинда в List<BaseWeapon> для WeaponLevelView
            Container.Bind<BaseWeapon>().FromInstance(_fireBallWeapon).AsCached().NonLazy();
            Container.Bind<BaseWeapon>().FromInstance(_auraWeapon).AsCached().NonLazy();
            Container.Bind<SurikenWeapon>().FromInstance(_surikenWeapon).AsSingle().NonLazy();
            Container.Bind<FrostBoltWeapon>().FromInstance(_frostBoltWeapon).AsSingle().NonLazy();
            // !!!важно - если добавляется новое оружие у игрока - его нужно добавить сюда!!!
            // Container.Bind<BaseWeapon>().FromInstance(оружие).AsCached();
            
        }
    }
}