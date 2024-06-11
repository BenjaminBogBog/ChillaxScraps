using GameNetcodeStuff;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ChillaxMods
{
    public class DeathDanceCanvas : MonoBehaviour
    {
        [SerializeField] private Transform container;
        [SerializeField] private GameObject namePrefab;
        [SerializeField] private Button closeButton;

        public void Initialize(DeathDanceNote note)
        {
            Debug.Log("Initialized Canvas");
            closeButton.onClick.AddListener(note.Close);

            List<PlayerControllerB> playerList = note.playerList;

            foreach (var player in playerList)
            {
                DeathDanceName name = Instantiate(namePrefab, container).GetComponent<DeathDanceName>();
                name.Initialize(player, note);
            }
        }
    }
}
