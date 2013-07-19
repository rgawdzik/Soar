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
    public class Planet : Collidable
    {
        #region Variables and Properties
        private BasicModel _planetModel { get; set; }
        private Vector3 _position;
        private Quaternion _rotation;
        private float _scale;
        private float _rotationSpeed;

        #endregion

        #region Constructor

        public Planet(Model planetModel, Vector3 position, Quaternion rotation, float rotationSpeed, float scale)
        {
            _planetModel = new BasicModel(planetModel, scale);
            _position = position;
            _rotation = rotation;
            _rotationSpeed = rotationSpeed;
            _scale = scale;
        }

        #endregion

        #region Update
        public void Update(GameTime gameTime)
        {
            //Gets how many seconds occured after the last update.
            float second = (float)gameTime.ElapsedGameTime.TotalSeconds;
            //Updates the rotation on one of the axis.
            _rotation *= Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), second * _rotationSpeed);
            //Updates the model while creating a world.
            _planetModel.UpdateModel(Matrix.CreateFromQuaternion(_rotation) * Matrix.CreateTranslation(_position));
        }
        #endregion

        #region Draw
        public void DrawPlanet(Camera camera)
        {
            //Draws the planet.
            _planetModel.DrawModel(camera);
        }
        #endregion

        #region Helper Methods
        public override void Collision(CollidableType objectCollidedWithType)
        {

        }
        #endregion
    }
}
