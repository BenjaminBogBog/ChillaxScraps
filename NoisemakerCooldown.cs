using System;
using System.Collections.Generic;
using System.Text;

namespace Chillax.Bastard.BogBog
{
    public class NoisemakerCooldown : NoisemakerProp
    {
        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            if(noiseAudio.isPlaying || noiseAudioFar.isPlaying) return;

            base.ItemActivate(used, buttonDown);
        }
    }
}
