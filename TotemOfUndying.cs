using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ChillaxMods
{
    public class TotemOfUndying : GrabbableObject
    {
        private bool _spawned = false;

        private int _holdCount = 0;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            _spawned = true;

            _holdCount = 0;
        }


        public override void EquipItem()
        {
            base.EquipItem();

            if (playerHeldBy != StartOfRound.Instance.localPlayerController) return;

            StartOfRound.Instance.allowLocalPlayerDeath = false;

            _holdCount++;

            if (_holdCount > 2)
            {
                StartCoroutine(DestroyDelay());
            }
        }

        public override void PocketItem()
        {
            ResetInvincibility();

            base.PocketItem();
        }

        public override void DiscardItem()
        {
            ResetInvincibility();
            
            base.DiscardItem();
        }

        private void ResetInvincibility()
        {
            if (playerHeldBy == StartOfRound.Instance.localPlayerController)
            {
                StartOfRound.Instance.allowLocalPlayerDeath = true;
            }
        }

        private IEnumerator DestroyDelay()
        {
            yield return new WaitForSeconds(0.1f);
            DestroyObjectInHand(playerHeldBy);
        }
    }
}
