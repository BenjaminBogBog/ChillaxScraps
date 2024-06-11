using System;
using GameNetcodeStuff;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Chillax.Bastard.BogBog
{
    public class DeathNoteCanvas : MonoBehaviour
    {
        [SerializeField] private Button closeButton;

        [SerializeField] private GameObject namesPrefab;
        [SerializeField] private Transform contentContainer;

        private string _chosenName;
        private DeathNote _deathNote;

        public Action onExit;

        public void Initialize(DeathNote deathNote)
        {
            foreach (Transform t in contentContainer)
            {
                Destroy(t.gameObject);
            }
            
            closeButton.onClick.AddListener(Close);

            _deathNote = deathNote;
            
            onExit += OnExit;

            foreach (var controller in deathNote.playerList)
            {
                if (controller.isPlayerDead) continue;
                
                DeathNoteName deathNoteName = Instantiate(namesPrefab, contentContainer).GetComponent<DeathNoteName>();
                deathNoteName.Initialize(controller.gameObject, _deathNote, this);
            }

            foreach (var enemy in deathNote.enemyList)
            {
                if(enemy.isEnemyDead || !enemy.enemyType.canDie) continue;
                
                DeathNoteName deathNoteName = Instantiate(namesPrefab, contentContainer).GetComponent<DeathNoteName>();
                deathNoteName.Initialize(enemy.gameObject, _deathNote, this);
            }

        }

        private void OnExit()
        {
            Destroy(gameObject);
        }

        public void Close()
        {
            onExit?.Invoke();
        }
    }
}