using System;
using System.Collections.Generic;
using System.Text;

namespace ChillaxMods
{
    public class TotemOfUndying : GrabbableObject
    {
        private bool _spawned = false;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            _spawned = true;
        }

        public override void EquipItem()
        {
            base.EquipItem();

            if (playerHeldBy != StartOfRound.Instance.localPlayerController) return;

            StartOfRound.Instance.allowLocalPlayerDeath = false;
        }

        public override void PocketItem()
        {
            base.PocketItem();

            if (playerHeldBy != StartOfRound.Instance.localPlayerController) return;

            StartOfRound.Instance.allowLocalPlayerDeath = true;
        }

        public override void DiscardItem()
        {
            if (playerHeldBy == StartOfRound.Instance.localPlayerController){
                StartOfRound.Instance.allowLocalPlayerDeath = true;
            }
            
            base.DiscardItem();
        }
    }
}
