using Chillax.Bastard.BogBog;
using GameNetcodeStuff;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Netcode;
using UnityEngine;

namespace ChillaxMods
{
    public class DeathDanceNote : NoisemakerProp
    {
        [SerializeField] DeathDanceCanvas canvasPrefab;
        [SerializeField] GameObject warningPrefab;

        [SerializeField] private Vector3 aoeCenter;
        [SerializeField] private float aoeRadius;

        [SerializeField] private GameObject glowingParticle;
        [SerializeField] private GameObject aoeParticle;

        public List<AudioClip> musicClips;

        [SerializeField] private GameObject danceMusic;
        private GameObject _tempMusic;
        

        public List<PlayerControllerB> playerList;

        DeathDanceCanvas _temp;

        private bool _opened;

        private NetworkVariable<int> playerDancing = new NetworkVariable<int>(-1);

        [SerializeField] private float transitionTime;
        [SerializeField] private float danceTime;

        private GameObject _tempGlow;

        private float _elapsedTransitionTime;
        private float _elapsedDanceTime;

        private bool _spawned;
        private bool _activated;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            _opened = false;
            _spawned = true;

            _elapsedDanceTime = danceTime;
            _elapsedTransitionTime = transitionTime;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.DrawSphere(transform.position + aoeCenter, aoeRadius);
        }

        public override void Update()
        {
            base.Update();

            if (!_spawned) return;

            if (StartOfRound.Instance.localPlayerController == null) return;

            int localId = (int)StartOfRound.Instance.localPlayerController.playerClientId;

            if(localId != playerDancing.Value) return;

            _elapsedTransitionTime -= Time.deltaTime;

            if (_elapsedTransitionTime > 0) return;

            _elapsedDanceTime -= Time.deltaTime;

            if (_elapsedDanceTime <= 0)
            {
                ResetServerRpc();
                AOEKills(StartOfRound.Instance.localPlayerController);
                return;
            }

            TargetedUpdate(StartOfRound.Instance.localPlayerController);
        }

        private void TargetedUpdate(PlayerControllerB player)
        {
            if (!_tempGlow)
            {
                PlayGlowingParticleServerRpc(player.playerClientId);
            }


            if (player.performingEmote) return;

            ResetServerRpc();

            Debug.Log("KILL PLAYER CUZ HE WAS NOT DANCING");

            Vector3 randomDirection = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
            player.KillPlayer(randomDirection * 10f);
        }

        private void AOEKills(PlayerControllerB player)
        {
            Debug.Log("AOE KILL");
            PlayAOEParticleServerRpc(player.playerClientId);

            foreach(var p in FindObjectsOfType<PlayerControllerB>())
            {
                if(p == player) continue;

                if(p.playerSteamId <= 0) continue;

                float distance = Vector3.Distance(player.thisPlayerBody.transform.position, p.thisPlayerBody.transform.position);
                Debug.Log($"Player {p.playerUsername}, Distance: {Vector3.Distance(player.thisPlayerBody.transform.position, p.thisPlayerBody.transform.position)}");

                if(distance > aoeRadius) continue;

                Vector3 direction = (p.thisPlayerBody.transform.position - player.thisPlayerBody.transform.position).normalized;
                p.KillPlayer(direction * 10f);
            }

            // Kill enemy
            foreach(var e in FindObjectsOfType<EnemyAI>())
            {
                if(e == null) continue;

                if (!e.enemyType.canDie) continue;

                float distance = Vector3.Distance(player.thisPlayerBody.transform.position, e.transform.position);

                if(distance > aoeRadius) continue;

                e.KillEnemy(false);
            }
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            if (_opened) return;

            base.ItemActivate(used, buttonDown);

            if (StartOfRound.Instance.localPlayerController != playerHeldBy) return;

            _opened = true;


            playerList = new List<PlayerControllerB>();
            playerList = UpdatePlayerList();

            _temp = Instantiate(canvasPrefab);
            _temp.Initialize(this);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public override void PocketItem()
        {
            base.PocketItem();

            Close();
        }

        public override void DiscardItem()
        {
            Close();
            base.DiscardItem();
        }

        public void Close()
        {
            if (StartOfRound.Instance.localPlayerController != playerHeldBy) return;

            if (_temp != null)
                Destroy(_temp.gameObject);

            _opened = false;

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        

        private List<PlayerControllerB> UpdatePlayerList()
        {
            List<PlayerControllerB> rawList = FindObjectsOfType<PlayerControllerB>().ToList();

            List<PlayerControllerB> finalList = new List<PlayerControllerB>(rawList);

            foreach (var p in rawList)
            {
                if (p.playerSteamId <= 0)
                {
                    finalList.Remove(p);
                }

                Debug.Log($"Players in-game: {p.playerUsername}");
            }

            return finalList;
        }

        public void ActivateDeathDance(PlayerControllerB player)
        {
            Close();
            if (_activated) return;
            _activated = true;

            Invoke(nameof(ResetActivate), transitionTime + danceTime);

            ActivateDeathDanceServerRpc(player.playerClientId);
        }

        private void ResetActivate()
        {
            _activated = false;
        }

        [ServerRpc(RequireOwnership =false)]
        private void ActivateDeathDanceServerRpc(ulong playerId)
        {
            playerDancing.Value = (int)playerId;

            Debug.Log("Player Dancing is: " + playerDancing.Value);

            int clipToPlay = UnityEngine.Random.Range(0, musicClips.Count);
            ActivateDeathDanceClientRpc(playerId, clipToPlay);
        }

        [ClientRpc]
        private void ActivateDeathDanceClientRpc(ulong playerId, int clipToPlay)
        {
            PlayerControllerB targetedPlayer = StartOfRound.Instance.allPlayerObjects[playerId].GetComponent<PlayerControllerB>();

            if (targetedPlayer)
            {
                StartCoroutine(PlayMusic(targetedPlayer, clipToPlay));
            }
            else
            {
                Debug.Log("TARGET PLAYER IS NULL");
            }
            

            if (playerId != StartOfRound.Instance.localPlayerController.playerClientId) return;
            Debug.Log("START DANCING");

            _elapsedTransitionTime = transitionTime;
            _elapsedDanceTime = danceTime;

            GameObject warning = Instantiate(warningPrefab);
            Destroy(warning, transitionTime + 1);
        }

        private IEnumerator PlayMusic(PlayerControllerB targetedPlayer, int clipToPlay)
        {
            // Play Music
            if (danceMusic != null)
            {
                Debug.Log("Spawning Music");
                _tempMusic = Instantiate(danceMusic, targetedPlayer.thisPlayerBody.transform.position + Vector3.up, Quaternion.identity, targetedPlayer.transform);

                _tempMusic.GetComponent<AudioSource>().clip = musicClips[clipToPlay];
                _tempMusic.GetComponent<AudioSource>().loop = false;
                _tempMusic.GetComponent<AudioSource>().Play();
            }
            else
            {
                Debug.Log("MUSIC NOT AVAILABLE");
            }

            Invoke(nameof(DestroyTempMusic), transitionTime + danceTime);
            yield return null;
        }

        private void DestroyTempMusic()
        {
            if (_tempMusic != null)
            {
                Debug.Log("Destroy Music");
                Destroy(_tempMusic);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void ResetServerRpc()
        {
            playerDancing.Value = -1;
        }

        [ServerRpc(RequireOwnership = false)]
        private void PlayGlowingParticleServerRpc(ulong playerId)
        {
            PlayGlowingParticleClientRpc(playerId);
        }

        [ClientRpc]
        private void PlayGlowingParticleClientRpc(ulong playerId)
        {
            if(_tempGlow == null)
            {
                return;
            }
            PlayerControllerB targetedPlayer = StartOfRound.Instance.allPlayerObjects[playerId].GetComponent<PlayerControllerB>();
            if(targetedPlayer != null)
            {
                _tempGlow = Instantiate(glowingParticle, targetedPlayer.thisPlayerBody.transform.position + Vector3.up, Quaternion.identity, targetedPlayer.transform);
                Debug.Log("PLAYING GLOWING PARTICLE");

                Destroy(_tempGlow.gameObject, transitionTime + danceTime);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void PlayAOEParticleServerRpc(ulong playerId)
        {
            PlayAOEParticleClientRpc(playerId);
        }

        [ClientRpc]
        private void PlayAOEParticleClientRpc(ulong playerId)
        {
            PlayerControllerB targetedPlayer = StartOfRound.Instance.allPlayerObjects[playerId].GetComponent<PlayerControllerB>();
            if(targetedPlayer != null)
            {
                GameObject particle = Instantiate(aoeParticle, targetedPlayer.thisPlayerBody.transform.position + Vector3.up, Quaternion.identity);
                Destroy(particle, 10);
                Debug.Log("PLAY AOE PARTICLE");
            }
        }
        
    }
}
