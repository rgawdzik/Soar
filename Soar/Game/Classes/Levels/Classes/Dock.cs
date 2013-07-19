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

using Paradox.Game.Classes.Cameras;

namespace Paradox.Game.Classes.Levels
{
    public class Dock : Collidable
    {
        BasicModel _model;
        Vector3 _position;

        public Dock(CollidableType type, float radius, BasicModel model, Vector3 pos, Quaternion rotation)
        {
            this.CollidableType = type;
            this.BoundingSphereRadius = radius;
            this.BoundingSphereCenter = pos;

            _model = model;
            _position = pos;
            _model.UpdateModel(Matrix.CreateTranslation(_position) * Matrix.CreateFromQuaternion(rotation));
        }

        public void Draw(Camera camera)
        {
            _model.DrawModel(camera);
        }

    }
}
