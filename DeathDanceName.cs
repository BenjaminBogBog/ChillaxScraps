using GameNetcodeStuff;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ChillaxMods
{
    public class DeathDanceName : MonoBehaviour
    {
        [SerializeField] private TMP_Text playerName;
        [SerializeField] private Button activateButton;
        public void Initialize(PlayerControllerB player, DeathDanceNote note)
        {
            playerName.text = player.playerUsername;
            activateButton.onClick.AddListener(() =>
            {
                note.ActivateDeathDance(player);
            });
        }
    }
}
