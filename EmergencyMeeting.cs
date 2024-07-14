using GameNetcodeStuff;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Netcode;
using ChillaxMods;
using System.Collections;

namespace Chillax.Bastard.BogBog
{
    public class EmergencyMeeting : NoisemakerProp
    {
        [SerializeField] private GameObject meetingCanvas;

        private bool _used;
        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            if(_used) return;
            if (!playerHeldBy.isInsideFactory) return;
            Vector3 position = playerHeldBy.transform.position;

            List<PlayerControllerB> players = GetOtherPlayers();

            _used = true;
            Invoke(nameof(Reset), 5f);
            StartMeetingServerRpc(players.Select(p => p.playerClientId).ToArray(), position);

            StartCoroutine(DestroyDelay());

            base.ItemActivate(used, buttonDown);
        }

        private void Reset()
        {
            _used = false;
        }

        [ServerRpc(RequireOwnership = false)]
        public void StartMeetingServerRpc(ulong[] players, Vector3 position)
        {
            foreach(var p in players)
            {
                StartMeetingClientRpc(p, position);
            }
        }

        [ClientRpc]
        public void StartMeetingClientRpc(ulong clientId, Vector3 postion)
        {
            StartMeeting(clientId, postion);
        }

        public void StartMeeting(ulong clientId, Vector3 position)
        {
            Debug.Log("Meeting started!");

            Utilities.TeleportPlayer((int)clientId, position);
            GameObject meeting = Instantiate(meetingCanvas);
            Destroy(meeting, 5f);
        }

        public List<PlayerControllerB> GetOtherPlayers()
        {
            List<PlayerControllerB> rawList = FindObjectsOfType<PlayerControllerB>().ToList();

            List<PlayerControllerB> finalList = new List<PlayerControllerB>(rawList);

            foreach(var p in rawList)
            {
                if(p.playerSteamId <= 0)
                {
                    finalList.Remove(p);
                }

                if (!p.isInsideFactory)
                {
                    finalList.Remove(p);
                }

                Debug.Log($"Players in-game: {p.playerUsername}");
            }

            return finalList;
        }

        private IEnumerator DestroyDelay()
        {
            yield return new WaitForSeconds(0.1f);

            DestroyObjectInHand(playerHeldBy);
        }
    }
}
