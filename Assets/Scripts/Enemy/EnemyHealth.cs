using System.Collections;
using GameCore.Health;
using UnityEngine;

namespace Enemy
{
    public class EnemyHealth : ObjectHealth
    {
        private WaitForSeconds _tick = new WaitForSeconds(1f);
        public override void TakeDamage(float damage)
        {
            base.TakeDamage(damage);
            if(CurrentHealth <= 0)
                gameObject.SetActive(false);
        }
        
        public void Burn(float damage) => StartCoroutine(StartBurn(damage));

        private IEnumerator StartBurn(float damage)
        {
            if(gameObject.activeSelf == false)
                yield break;
            float tickDamage =  damage / 3f;
            if(tickDamage < 1f)
                tickDamage = 1f;
            float roundDamage = Mathf.Round(tickDamage);
            for (int i = 0; i < 5; i++)
            {
                TakeDamage(roundDamage);
                yield return _tick;
            }
        }
        
    }
}