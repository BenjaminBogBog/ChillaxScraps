using GameNetcodeStuff;
using UnityEngine;

namespace Chillax.Bastard.BogBog;

public class Utilities
{
    public static void TeleportPlayer(int playerObj, Vector3 teleportPos)
    {
        PlayerControllerB allPlayerScript = StartOfRound.Instance.allPlayerScripts[playerObj];
        allPlayerScript.averageVelocity = 0.0f;
        allPlayerScript.velocityLastFrame = Vector3.zero;
        StartOfRound.Instance.allPlayerScripts[playerObj].TeleportPlayer(teleportPos);
        StartOfRound.Instance.allPlayerScripts[playerObj].beamOutParticle.Play();
        if (!((UnityEngine.Object) allPlayerScript == (UnityEngine.Object) GameNetworkManager.Instance.localPlayerController))
            return;
        HUDManager.Instance.ShakeCamera(ScreenShakeType.Big);
    }
}