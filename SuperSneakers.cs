using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ChillaxMods
{
    public class SuperSneakers : GrabbableObject
    {
        [SerializeField] private float jumpMultiplier = 2f;
        private float _originalJumpForce;

        public override void EquipItem()
        {
            base.EquipItem();

            _originalJumpForce = playerHeldBy.jumpForce;
            playerHeldBy.jumpForce *= jumpMultiplier;
        }

        public override void PocketItem()
        {
            base.PocketItem();

            playerHeldBy.jumpForce = _originalJumpForce;
        }

        public override void DiscardItem()
        {
            playerHeldBy.jumpForce = _originalJumpForce;
            base.DiscardItem();
        }
    }
}
