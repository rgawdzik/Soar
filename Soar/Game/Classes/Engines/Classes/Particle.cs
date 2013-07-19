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
    //A lightweight implementation of a Particle, for the Particle Engine.
    public class Particle
    {
        #region Properties and Variables

        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public float Speed;
        public float Distance { get; set; }
        public int Life;
        public byte Index { get; set; }

        #endregion

        #region Constructor(s)

        public Particle(Vector3 position, Quaternion rotation, float speed)
        {
            Position = position;
            Index = 0;
            Rotation = rotation;
            Speed = speed;
        }

        public void Update()
        {
            Position = Vector3.Transform(new Vector3(0, 0, 1), Rotation) * Speed;
            Index++;
        }

        public void Reset(Vector3 position, Quaternion rotation)
        {
            Index = 0;
            Position = position;
            Rotation = rotation;
        }

        #endregion
    }
}
