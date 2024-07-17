using GameNetcodeStuff;
using UnityEngine;

namespace Chillax.Bastard.BogBog
{
    public class Boink : NoisemakerProp
    {
        private Vector3 targetForce;
        private bool push;
        private float _elapsedTime;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            targetForce = Vector3.zero;
            push = false;
            _elapsedTime = 0;

            useCooldown = Config.boinkCooldown.Value;
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            if (playerHeldBy == null) return;

            Vector3 direction = (-transform.forward + Vector3.up);

            Push(direction.normalized * 500f, 0.5f);

            ChillaxModPlugin.logger.LogInfo("[CHILLAX] ACTIVATE BOINK!");

            base.ItemActivate(used, buttonDown);
        }

        public override void Update()
        {
            base.Update();

            if (playerHeldBy == null) return;

            if (!push) return;

            _elapsedTime -= Time.deltaTime;
            playerHeldBy.externalForces = Vector3.Lerp(playerHeldBy.externalForces, targetForce, Time.deltaTime * 5f);

            if (_elapsedTime <= 0)
            {
                push = false;
            }
        }

        private void Push(Vector3 targetForce, float duration)
        {
            if (playerHeldBy == null) return;

            this.targetForce = targetForce;
            _elapsedTime = duration;
            push = true;
        }
    }
}