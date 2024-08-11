using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;

namespace ChillaxMods
{
    public class DrainAllBatteries : GrabbableObject
    {
        [SerializeField] private AudioSource effectSource;

        public override void EquipItem()
        {
            base.EquipItem();

            foreach (var item in playerHeldBy.ItemSlots)
            {
                if (item == null) continue;
                if(item.insertedBattery != null)
                {
                    item.insertedBattery.charge = 0;
                    item.insertedBattery.empty = true;
                }
            }

            PlayEffectServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        public void PlayEffectServerRpc()
        {
            PlayEffectClientRpc();
        }

        [ClientRpc]
        public void PlayEffectClientRpc()
        {
            if(effectSource == null) return;
            effectSource.Play();
        }
    }
}
