using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameNetcodeStuff;
using LethalThings;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Chillax.Bastard.BogBog;

public class UnoReverse : NoisemakerProp
{
    public AudioClip teleportClip;
    public List<PlayerControllerB> playerList;
    public List<EnemyAI> outsideEnemyList;
    public List<EnemyAI> insideEnemyList;

    private bool _isRed = false;

    [SerializeField] private Renderer renderer;

    [SerializeField] private Material blueUnoMat;
    [SerializeField] private Material redUnoMat;

    private bool _used;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        playerList = new List<PlayerControllerB>();
        _used = false;

        if (IsHost)
        {
            bool chance = Random.Range(0, 100f) > 50;
            _isRed = chance;
            ReplicateBoolClientRpc(chance);
        }
    }

    private void FixedUpdate()
    {
        if(renderer != null)
            renderer.material = _isRed ? redUnoMat : blueUnoMat;
    }

    [ClientRpc(Delivery = RpcDelivery.Reliable)]
    private void ReplicateBoolClientRpc(bool chance, ClientRpcParams clientRpcParams = default)
    {
        _isRed = chance;
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

        outsideEnemyList = new List<EnemyAI>();
        insideEnemyList = new List<EnemyAI>();

        foreach (var e in outsideEnemyList)
        {
            Debug.Log("[UNO REVERSE CARD] Outside Enemy: " + e.enemyType.enemyName);
        }
        
        foreach (var e in outsideEnemyList)
        {
            Debug.Log("[UNO REVERSE CARD] Inside Enemy: " + e.enemyType.enemyName);
        }
        
        outsideEnemyList.Clear();
        insideEnemyList.Clear();
        List<EnemyAI> enemies = FindObjectsOfType<EnemyAI>().ToList();

        foreach (var enemy in enemies)
        {
            if (enemy.enemyType.isOutsideEnemy)
            {
                outsideEnemyList.Add(enemy);
            }
            else
            {
                insideEnemyList.Add(enemy);
            }
        }
    }

    public override void ItemActivate(bool used, bool buttonDown = true)
    {
        if (!playerHeldBy.IsOwner) return;
        
        if (_used) return;
        
        base.ItemActivate(used, buttonDown);
        
        UpdateList();
        
        int randIndex = 0;

        if (_isRed)
        {
            // Enemy

            if (playerHeldBy.isInsideFactory)
            {
                randIndex = Random.Range(0, insideEnemyList.Count);
            
                if (insideEnemyList.Count > 0)
                {
                    foreach (var enemy in insideEnemyList)
                    {
                        Debug.Log($"[CHILLAX] [UNO REVERSE] Enemies in Game: {enemy.enemyType.enemyName}");
                    }
            
                    
                    SwapEnemyServerRpc(randIndex, false);
                    StartCoroutine(DestroyDelay());
                    return;
                }
            }
            else
            {
                randIndex = Random.Range(0, outsideEnemyList.Count);
        
                if (outsideEnemyList.Count > 0)
                {
                    foreach (var enemy in outsideEnemyList)
                    {
                        Debug.Log($"[CHILLAX] [UNO REVERSE] Enemies in Game: {enemy.enemyType.enemyName}");
                    }
            
                    SwapEnemyServerRpc(randIndex, true);
                    StartCoroutine(DestroyDelay());
                    return;
                }
            }
        }

        // Player

        foreach (var controller in playerList)
        {
            Debug.Log($"[CHILLAX] [UNO REVERSE] Players in Game: {controller.playerUsername}");
        }

        playerList.Remove(playerHeldBy);

        randIndex = Random.Range(0, playerList.Count);

        if (playerList.Count <= 0)
        {
            Debug.LogWarning("[CHILLAX] [UNO REVERSE] No players to swap with");
        }

        foreach (var controller in playerList)
        {
            Debug.Log($"[CHILLAX] [UNO REVERSE] Potential Swappers: {controller.playerUsername}");
        }
        
        PlayerControllerB other = playerList[randIndex];

        Vector3 firstPosition = GetPosition(playerHeldBy);
        Vector3 secondPosition = GetPosition(other);
        
        Debug.Log("Teleporting to: " + secondPosition + " From: " + firstPosition);
        _used = true;
        SwapServerRpc(firstPosition, secondPosition, other.actualClientId, other.playerClientId);
        StartCoroutine(DestroyDelay());
    }

    private Vector3 GetPosition(PlayerControllerB player)
    {
        Vector3 position = RoundManager.Instance.GetNavMeshPosition(player.transform.position, RoundManager.Instance.navHit, 2.7f);
        position = new Vector3(player.transform.position.x, position.y, player.transform.position.z);

        return position;
    }
    
    private Vector3 GetPosition(Vector3 pos)
    {
        Vector3 position = RoundManager.Instance.GetNavMeshPosition(pos, RoundManager.Instance.navHit, 2.7f);
        position = new Vector3(pos.x, position.y, pos.z);

        return position;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SwapServerRpc(Vector3 firstPosition, Vector3 secondPosition, ulong otherId, ulong otherPlayerId)
    {
        _used = true;
        Debug.Log("Teleporting... SERVER RPC");
        Teleport(firstPosition, secondPosition, otherId, otherPlayerId);
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void SwapEnemyServerRpc(int randIndex, bool outside = false)
    {
        _used = true;
        EnemyAI e = outside ? outsideEnemyList[randIndex] : insideEnemyList[randIndex];
        
        ClientRpcParams firstClientParams = new ClientRpcParams()
            { Send = new ClientRpcSendParams() { TargetClientIds = new[] { playerHeldBy.OwnerClientId } } };

        Vector3 playerPosition = GetPosition(playerHeldBy.transform.position);
        Vector3 enemyPosition = GetPosition(e.transform.position);
        
        Utilities.TeleportPlayer((int)playerHeldBy.playerClientId, enemyPosition);
        TeleportOnClientRpc(enemyPosition, playerHeldBy.isInElevator, playerHeldBy.isInHangarShipRoom, playerHeldBy.isInsideFactory, firstClientParams);
        LethalThings.Utilities.TeleportEnemy(e, playerPosition);
        TeleportEnemyClientRpc(randIndex, playerPosition, outside);
    }

    [ClientRpc]
    private void TeleportEnemyClientRpc(int randIndex, Vector3 playerPosition, bool outside = false)
    {
        EnemyAI e = outside ? outsideEnemyList[randIndex] : insideEnemyList[randIndex];
        LethalThings.Utilities.TeleportEnemy(e, playerPosition);
    }

    private void Teleport(Vector3 firstPosition, Vector3 secondPosition, ulong otherClientId, ulong otherPlayerId)
    {
        ClientRpcParams firstClientParams = new ClientRpcParams()
            { Send = new ClientRpcSendParams() { TargetClientIds = new[] { playerHeldBy.OwnerClientId } } };
        ClientRpcParams secondClientParams = new ClientRpcParams()
            { Send = new ClientRpcSendParams() { TargetClientIds = new[] { otherClientId } } };
        
        Utilities.TeleportPlayer((int)playerHeldBy.playerClientId, secondPosition);
        Utilities.TeleportPlayer((int)otherPlayerId, firstPosition);
        
        /*allPlayerScript.isInElevator = false;
        allPlayerScript.isInHangarShipRoom = false;
        allPlayerScript.isInsideFactory = true;*/

        if (playerHeldBy == null) return;

        PlayerControllerB firstPlayer = StartOfRound.Instance.allPlayerScripts[playerHeldBy.playerClientId];
        PlayerControllerB secondPlayer = StartOfRound.Instance.allPlayerScripts[otherPlayerId];


        TeleportOnClientRpc(secondPosition, secondPlayer.isInElevator, secondPlayer.isInHangarShipRoom, secondPlayer.isInsideFactory, firstClientParams);
        TeleportOnClientRpc(firstPosition, firstPlayer.isInElevator, firstPlayer.isInHangarShipRoom, firstPlayer.isInsideFactory, secondClientParams);
        
        bool player1IsInElevator = playerHeldBy.isInElevator;
        bool player1IsIsHangarShipRoom = playerHeldBy.isInHangarShipRoom;
        bool player1IsInsideFactory = playerHeldBy.isInsideFactory;
        
        firstPlayer.isInElevator = secondPlayer.isInElevator;
        firstPlayer.isInHangarShipRoom = secondPlayer.isInHangarShipRoom;
        firstPlayer.isInsideFactory = secondPlayer.isInsideFactory;
            
        secondPlayer.isInElevator = player1IsInElevator;
        secondPlayer.isInHangarShipRoom = player1IsIsHangarShipRoom;
        secondPlayer.isInsideFactory = player1IsInsideFactory;

        PlayExtraClipClientRpc(secondClientParams);
    }

    [ClientRpc]
    private void TeleportOnClientRpc(Vector3 position, bool inElevator, bool inHangar, bool inFactory, ClientRpcParams clientRpcParams = default)
    {
        PlayerControllerB player = StartOfRound.Instance.localPlayerController;

        player.isInElevator = inElevator;
        player.isInHangarShipRoom = inHangar;
        player.isInsideFactory = inFactory;
        
        _used = true;
        Utilities.TeleportPlayer((int)StartOfRound.Instance.localPlayerController.playerClientId, position);
    }

    [ClientRpc]
    private void PlayExtraClipClientRpc(ClientRpcParams clientRpcParams = default)
    {
        AudioSource tempSource = StartOfRound.Instance.localPlayerController.gameObject.AddComponent<AudioSource>();
        if (teleportClip == null) return;
        tempSource.PlayOneShot(teleportClip);
        Destroy(tempSource, 2f);
    }
    
    private IEnumerator DestroyDelay()
    {
        yield return new WaitForSeconds(1f);
        DestroyObjectInHand(playerHeldBy);
        DestroyServerRpc();
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void DestroyServerRpc()
    {
        DestroyObjectInHand(playerHeldBy);
        DestroyClientRpc();
    }

    [ClientRpc]
    private void DestroyClientRpc()
    {
        DestroyObjectInHand(playerHeldBy);
    }
}