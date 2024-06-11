using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

namespace Chillax.Bastard.BogBog
{
    public class DeathNote : NoisemakerProp
    {
        private NetworkVariable<bool> canUseDeathNote = new NetworkVariable<bool>(true);
        [SerializeField] private AudioClip killSfx;
        [SerializeField] private GameObject canvasPrefab;

        private float _currentCooldown;
        [SerializeField] private float checkRate;
        [SerializeField] private Renderer deathNoteRenderer;
        
        [SerializeField] private Material[] materials;
        [SerializeField] private string[] textNodes;
        [SerializeField] private Sprite[] icons;
        
        [SerializeField] private ScanNodeProperties scanNodeProperties;

        private DeathNoteCanvas _temp;

        public List<PlayerControllerB> playerList;
        public List<EnemyAI> enemyList;
        private bool _opened;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            canUseDeathNote = new NetworkVariable<bool>(true);
            playerList = new List<PlayerControllerB>();
            enemyList = new List<EnemyAI>();

            Item newItem = Instantiate(itemProperties);
            itemProperties = newItem;

            UpdateList();
            
            _opened = false;
            
            InvokeRepeating(nameof(ProcessCooldown), checkRate, checkRate);

            if (IsHost)
            {
                canUseDeathNote.Value = Config.canUseDeathNote.Value;
                UpdateVisuals(0);
                UpdateVisualsClientRpc(0);
            }
        }

        [ClientRpc]
        private void UpdateVisualsClientRpc(int index)
        {
            UpdateVisuals(index);
        }
        private void UpdateVisuals(int index)
        {
            if (deathNoteRenderer.material != materials[index])
            {
                deathNoteRenderer.material = materials[index];
            }

            scanNodeProperties.headerText = textNodes[index];
            itemProperties.itemName = textNodes[index];
            itemProperties.itemIcon = icons[index];
        }

        [ServerRpc(RequireOwnership = false)]
        private void StartCooldownServerRpc()
        {
            Random rand = new Random();
            float randValue = rand.Next(600, 1800);
            _currentCooldown = randValue;
            StartCooldownClientRpc(randValue);
        }

        [ClientRpc]
        private void StartCooldownClientRpc(float cooldown)
        {
            _currentCooldown = cooldown;
        }

        private void ProcessCooldown()
        {
            if (_currentCooldown > 0)
            {
                _currentCooldown -= checkRate;
                UpdateVisuals(1);
            }
            else
            {
                UpdateVisuals(0);
            }
        }

        private void UpdateList()
        {
            List<PlayerControllerB> rawList = FindObjectsOfType<PlayerControllerB>().ToList();

            List<PlayerControllerB> updatedList = new List<PlayerControllerB>(rawList);

            foreach (var p in rawList)
            {
                if (p.playerSteamId <= 0)
                {
                    updatedList.Remove(p);
                }
            }

            playerList = updatedList;

            enemyList = FindObjectsOfType<EnemyAI>().ToList();
        }

        private void OnDisable()
        {
            if(_temp != null)
                _temp.Close();
        }

        public override void DiscardItem()
        {
            base.DiscardItem();

            if(_temp != null)
                _temp.Close();
        }

        public override void PocketItem()
        {
            base.PocketItem();
            
            if(_temp != null)
                _temp.Close();
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            if (!canUseDeathNote.Value) return;
            
            if (_opened || _currentCooldown > 0) return;
            
            base.ItemActivate(used, buttonDown);

            if (playerHeldBy.IsOwner)
            {
                UpdateList();
                _temp = Instantiate(canvasPrefab, transform).GetComponent<DeathNoteCanvas>();
                _temp.Initialize(this);
                _opened = true;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                
                _temp.onExit += OnExit;
            }
        }

        private void OnExit()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            
            _opened = false;
        }

        public void ActivateDeathNote(GameObject objectToKill)
        {
            OnExit();

            if (objectToKill.GetComponent<PlayerControllerB>())
            {
                KillPlayer(objectToKill.GetComponent<PlayerControllerB>());
            }
            
            if(objectToKill.GetComponent<EnemyAI>())
            {
                KillEnemy(objectToKill.GetComponent<EnemyAI>());
            }
            
            StartCooldownServerRpc();
        }

        private void KillPlayer(PlayerControllerB controller)
        {
            if (controller == null) return;
            Debug.Log("[CHILLAX] [DEATH NOTE] killing off player: " + controller.playerUsername);
            DeathNoteServerRpc(controller.playerClientId, controller.OwnerClientId);
        }
        
        private void KillEnemy(EnemyAI enemy)
        {
            if (enemy == null) return;
            Debug.Log("[CHILLAX] [DEATH NOTE] killing off enemy: " + enemy.enemyType.enemyName);
            enemy.KillEnemyServerRpc(false);
        }

        [ServerRpc(RequireOwnership = false)]
        private void DeathNoteServerRpc(ulong playerID, ulong clientID)
        {
            PlayerControllerB controller = StartOfRound.Instance.allPlayerObjects[playerID].GetComponent<PlayerControllerB>();

            Debug.Log("[DEATH NOTE] Receive Server RPC, player to kill: " + controller.playerUsername);

            ClientRpcParams clientRpcParams = new ClientRpcParams()
                { Send = new ClientRpcSendParams() { TargetClientIds = new[] { clientID } } };
            DeathNoteClientRpc(controller.playerClientId, clientRpcParams);
        }

        [ClientRpc]
        private void DeathNoteClientRpc(ulong playerID, ClientRpcParams clientRpcParams = default)
        {
            PlayerControllerB controller = StartOfRound.Instance.allPlayerObjects[playerID].GetComponent<PlayerControllerB>();
            Debug.Log("[DEATH NOTE] Receive Client RPC, player to kill: " + controller.playerUsername);
            Debug.Log("[DEATH NOTE] Killing: " + controller.playerUsername);
            
            StartCoroutine(KilLDelay(controller));
        }

        private IEnumerator KilLDelay(PlayerControllerB controller)
        {
            if (controller.IsOwner)
            {
                AudioSource tempSource = controller.gameObject.AddComponent<AudioSource>();
                tempSource.PlayOneShot(killSfx);
                
                Destroy(tempSource, 3f); 
            }
            yield return new WaitForSeconds(3f);
            controller.KillPlayer(Vector3.up * 10);
        }
    }
}