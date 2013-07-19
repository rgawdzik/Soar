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


namespace Paradox.Game.Classes.Ships.Weapons
{
    public class Projectile : Collidable
    {
        #region Variables and Properties
        public Vector3 Position;
        public Quaternion Rotation;
        private float _bulletSpeed;
        public bool Destroyed;
        private Enemy ClosestEnemyPos;


        public int Life;
        #endregion

        #region Constructor
        public Projectile(CollidableType type, float boundingRadius, Vector3 position, Quaternion rotation, float bulletSpeed, int life = 0)
        {
            Position = position;
            Rotation = rotation;
            _bulletSpeed = bulletSpeed;
            this.BoundingSphereRadius = boundingRadius;
            this.CollidableType = type;
            ClosestEnemyPos = null;
            Life = life;
        }
        #endregion

        #region Update
        public void UpdateProjectile(GameTime gameTime)
        {
            float second = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position += Vector3.Transform(new Vector3(0, 0, 1), Rotation) * (_bulletSpeed * second);
            this.BoundingSphereCenter = Position;
        }

        public void UpdateProjectile(GameTime gameTime, List<Enemy> enemyList)
        {
            if (ClosestEnemyPos == null)
            {
                ClosestEnemyPos = ClosestActualEnemy(enemyList);
            }
            else
            {
                float second = (float)gameTime.ElapsedGameTime.TotalSeconds;
                Rotation = FollowShip(ClosestEnemyPos.ShipPosition, 0.05f);
                Position += Vector3.Transform(new Vector3(0, 0, 1), Rotation) * (_bulletSpeed * second);
            }
            this.BoundingSphereCenter = Position;
            Life--;
        }

        #endregion

        #region Helper Methods

        public override void Collision(CollidableType objectCollidedWithType)
        {
            switch (objectCollidedWithType)
            {
                case CollidableType.EnemyBullet:
                    if(this.CollidableType != CollidableType.EnemyBullet)
                    {
                        Destroyed = true;
                    }
                    break;
                case CollidableType.EnemyBase:
                    if (this.CollidableType != CollidableType.EnemyBullet)
                    {
                        Destroyed = true;
                    }
                    break;
                case CollidableType.FriendBase:
                    if (this.CollidableType != CollidableType.FriendBullet)
                    {
                        Destroyed = true;
                    }
                    break;
                case CollidableType.FriendlyShip:
                    if (this.CollidableType != CollidableType.FriendBullet 
                        && this.CollidableType != CollidableType.FriendMissle)
                    {
                        Destroyed = true;
                    }
                    break;
                case CollidableType.FriendBullet:
                    if (this.CollidableType != CollidableType.FriendBullet
                        && this.CollidableType != CollidableType.FriendMissle)
                    {
                        Destroyed = true;
                    }
                    break;
                case CollidableType.EnemyShip:
                    if (this.CollidableType != CollidableType.EnemyBullet)
                    {
                        Destroyed = true;
                    }
                    break;
                case CollidableType.Station:
                    Destroyed = true;
                    break;
                case CollidableType.Asteroid:
                    Destroyed = true;
                    break;
            }
        }

        public Quaternion FollowShip(Vector3 playerPosition, float lerpValue)
        {
            Vector3 desiredDirection = Vector3.Normalize(Position - playerPosition);
            Quaternion ShipDirection = Quaternion.CreateFromRotationMatrix(Matrix.CreateWorld(Position, desiredDirection, Vector3.Up));
            /*
            if (Rotation.X < 0 && ShipDirection.X > 0)
                System.Diagnostics.Debugger.Break();

             * /*/
            return Quaternion.Lerp(Rotation, ShipDirection, lerpValue);
        }

        public Quaternion FollowShip(Quaternion alternateRotation, float lerpValue)
        {
            return Quaternion.Lerp(Rotation, alternateRotation, lerpValue);
        }

        public Vector3 ClosestEnemy(List<Enemy> enemyList)
        {
            Vector3 closestEnemy = Vector3.Zero;
            float closestDistance = float.MaxValue;
            foreach (Enemy enemy in enemyList)
            {
                float pos = DistanceVector3(Position, enemy.ShipPosition);
                if (pos < closestDistance)
                {
                    closestEnemy = enemy.ShipPosition;
                    closestDistance = pos;
                }
            }
            return closestEnemy;
        }

        public Enemy ClosestActualEnemy(List<Enemy> enemyList)
        {
            Enemy closestEnemy = null;
            float closestDistance = float.MaxValue;
            foreach (Enemy enemy in enemyList)
            {
                float pos = DistanceVector3(Position, enemy.ShipPosition);
                if (pos < closestDistance)
                {
                    closestEnemy = enemy;
                    closestDistance = pos;
                }
            }
            return closestEnemy;
        }

        public float DistanceVector3(Vector3 v1, Vector3 v2)
        {
            return (float)Math.Sqrt(Math.Abs(Math.Pow(v1.X - v2.X, 2)) + Math.Abs(Math.Pow(v1.Y - v2.Y, 2)) + Math.Abs(Math.Pow(v1.Z - v2.Z, 2)));
        }

        #endregion
    }
}
