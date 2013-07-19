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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Paradox.Game.Classes
{
    public abstract class Collidable
    {
        #region Variables and Properties

        public Vector3 BoundingSphereCenter = Vector3.Zero;
        public CollidableType CollidableType = CollidableType.None;
        public float BoundingSphereRadius = 0f;

        public virtual void Collision(CollidableType objCollided)
        {

        }
        #endregion
    }
}
