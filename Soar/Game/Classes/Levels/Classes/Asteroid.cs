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




namespace Paradox.Game.Classes.Levels
{
    public class Asteroid : Collidable
    {
        #region Properties and Variables
        public Matrix World { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public AsteroidType AsteroidType { get; set; }
        private float _rotationSpeed;
        #endregion

        #region Constructor
        public Asteroid(CollidableType type, float boundingRadius, AsteroidType asteroidType, Vector3 position, float rotationSpeed)
        {
            Position = position;
            AsteroidType = asteroidType;
            Rotation = Quaternion.Identity;
            _rotationSpeed = rotationSpeed;
            World = Matrix.CreateTranslation(position);

            this.CollidableType = type;
            this.BoundingSphereRadius = boundingRadius;
        }
        #endregion

        #region Update
        public void RotateAsteroid(GameTime gameTime)
        {
            //Gets how many seconds occured before the last Update, then adds a rotation to one of the axis.
            float second = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Rotation *= Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), second * _rotationSpeed);
            //Updates the world
            World = Matrix.CreateFromQuaternion(Rotation) * Matrix.CreateTranslation(Position);

            //Updates the bounding sphere
            this.BoundingSphereCenter = Position;
        }
        #endregion

        #region Helper Methods
        public override void Collision(CollidableType objectCollidedWithType)
        {
            switch (objectCollidedWithType)
            {
                case CollidableType.FriendlyShip:
                    break;
            }
        }
        #endregion
    }
}
