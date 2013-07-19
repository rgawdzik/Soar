/*This code is managed under the Apache v2 license. 
To see an overview: 
http://www.tldrlegal.com/license/apache-license-2.0-(apache-2.0)

Author: Robert Gawdzik
www.github.com/rgawdzik/

THIS CODE HAS NO FORM OF ANY WARRANTY, AND IS CONSIDERED AS-IS.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Paradox.Game.Classes.Engines
{
    public class SoundVoice : Sound
    {
        public SoundVoiceType SoundVoiceType;

        public SoundVoice(SoundType soundType, Vector3 pos, bool isLooped, SoundVoiceType voiceType)
            :base(soundType, pos, isLooped)
        {
            SoundVoiceType = voiceType;
        }
    }
}
