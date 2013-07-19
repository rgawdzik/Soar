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
    public class Explosion
    {
        #region Properties and Variables

        public ExplosionType ExplosionType;
        public Vector3 Position { get; set; }
        public byte Index { get; set; }

        #endregion

        #region Constructor(s)

        public Explosion(Vector3 position, byte index)
        {
            Position = position;
            Index = index;
            ExplosionType = ExplosionType.Big;
        }


        public Explosion(Vector3 position, byte index, ExplosionType type)
        {
            Position = position;
            Index = index;
            ExplosionType = type;
        }

        public void UpdatePosition(Vector3 pos)
        {
            Position = pos;
        }

        #endregion
    }
}
