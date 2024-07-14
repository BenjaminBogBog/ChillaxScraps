using System.Collections;
using UnityEngine;

namespace Chillax.Bastard.BogBog
{
    public class Food : NoisemakerProp
    {
        [SerializeField] private int healAmount;
        [SerializeField] private float refuelAmount;

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            if (playerHeldBy == null) return;

            if (playerHeldBy.sprintMeter + refuelAmount > 1)
            {
                playerHeldBy.sprintMeter = 1;
            }
            else
            {
                playerHeldBy.sprintMeter += refuelAmount;
            }
            
            if (playerHeldBy.health + healAmount > 100)
            {
                playerHeldBy.health = 100;
            }
            else
            {
                playerHeldBy.health += healAmount;
            }

            StartCoroutine(DestroyDelay());

            ChillaxModPlugin.logger.LogInfo("[CHILLAX] EAT FOOD!!");

            playerHeldBy.MakeCriticallyInjured(false);

            base.ItemActivate(used, buttonDown);
        }

        private IEnumerator DestroyDelay()
        {
            yield return new WaitForSeconds(0.1f);
            DestroyObjectInHand(playerHeldBy);
        }
    }
}