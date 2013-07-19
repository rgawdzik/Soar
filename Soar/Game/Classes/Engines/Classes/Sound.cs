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
    public class Sound
    {
        #region Variables and Properties
        public SoundType SoundType { get; protected set; }
        public Vector3 Position { get; set; }
        public float Volume { get; set; }
        public float Pitch { get; set; }
        public bool IsLooped { get; protected set; }
        public bool IsCreated { get; set; }

        private Random _randomVar;

        #endregion
        #region Constructor(s)
        public Sound(SoundType type, Vector3 pos, bool isLooped)
        {
            _randomVar = new Random();
            SoundType = type;
            Position = pos;
            Volume = 0;
            IsLooped = isLooped;
            IsCreated = true; //This Sound was just created.
            Pitch = GeneratePitch();
            
        }

        public Sound(SoundType type, Vector3 pos, float pitch, bool isLooped)
        {
            _randomVar = new Random();
            SoundType = type;
            Position = pos;
            Volume = 0;
            IsLooped = isLooped;
            IsCreated = true; //This Sound was just created.
            Pitch = pitch;
            
        }

        #endregion
        #region Update
        public void UpdateVolume(Vector3 playerPosition)
        {
            float volumeCaculation = 0.40f - Extension.DistanceVector3(Position, playerPosition) / 30f;
            if (volumeCaculation < 0)
            {
                Volume = 0;
            }
            else
            {
                Volume = volumeCaculation;
            }
        }
        #endregion

        #region Helper Methods

        public float GeneratePitch()
        {
            return (float)Math.Sin(_randomVar.Next(-90, 90));
        }

        #endregion
    }
}
