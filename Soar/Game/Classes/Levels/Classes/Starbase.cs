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
    public class Starbase : Collidable
    {
        #region Properties and Variables

        private BasicModel _baseModel { get; set; }
        public Vector3 Position;
        private Quaternion _rotation;
        private float _scale;

        #endregion

        #region Constructor
        public Starbase(CollidableType type, float radius, Model starBaseModel, Vector3 position, Quaternion rotation, float scale)
        {
            this.CollidableType = type;
            this.BoundingSphereRadius = radius;
            _baseModel = new BasicModel(starBaseModel, scale);
            _rotation = rotation;
            _scale = scale;
            Position = position;
        }
        #endregion

        #region Update
        public void Update()
        {
            //Updaates the Model position based on the rotation and position properties.
            _baseModel.UpdateModel(Matrix.CreateFromQuaternion(_rotation) * Matrix.CreateTranslation(Position));
            this.BoundingSphereCenter = Position;
            //_baseModel.UpdateModel(Matrix.CreateTranslation(Position)); 
        }

        public void DrawBase(Camera camera)
        {
            _baseModel.DrawModel(camera);
        }
        #endregion

        #region Helper Methods

        public override void Collision(CollidableType objectCollidedWithType)
        {
            
        }

        #endregion
    }
}
