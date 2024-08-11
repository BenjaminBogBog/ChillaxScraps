using System;
using System.Collections.Generic;
using System.Text;

namespace ChillaxMods
{
    public class FixedScrapValue : GrabbableObject
    {
        public int newScrapValue = 0;
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            SetScrapValue(newScrapValue);
        }
    }
}
