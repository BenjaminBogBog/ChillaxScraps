using LethalThings.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;
using static System.TimeZoneInfo;

namespace ChillaxMods
{
    public class TotemOfUndying : GrabbableObject
    {
        private bool _spawned = false;

        private int _holdCount = 0;

        private NetworkVariable<bool> canPickUp = new NetworkVariable<bool>(false);

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            _spawned = true;

            if (IsHost)
                canPickUp.Value = true;

            canPickUp.OnValueChanged += (oldValue, newValue) =>
            {
                grabbableToEnemies = newValue;
                grabbable = newValue;

                if (!newValue && playerHeldBy != null)
                {
                    playerHeldBy.DropItem(this);
                }
            };

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
            playerHeldBy.DropItem(this);

            if (IsHost)
            {
                canPickUp.Value = false;
            }
            else
            {
                DisablePickupServerRpc();
            }


            yield return new WaitForSeconds(1f);

            if (IsHost)
            {
                NetworkObject.Despawn(true);
            }
            else
            {
                DestroyServerRpc();
            }

        }

        [ServerRpc(RequireOwnership = false)]
        private void DisablePickupServerRpc()
        {
            canPickUp.Value = false;
        }

        [ServerRpc(RequireOwnership = false)]
        private void DestroyServerRpc()
        {
            NetworkObject.Despawn(true);
        }
    }
}
