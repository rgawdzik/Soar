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

using Paradox.Game.Classes.Engines;
using Paradox.Game.Classes.Cameras;
using Paradox.Game.Classes.Levels;

namespace Paradox.Game.Classes.Ships
{
    public class EnemyFrigate : Enemy
    {
        #region Variables and Properties
        private BasicModel _model;

        private Random _randomVar;

        private List<WeaponSystem2D> _weaponsList;

        #endregion

        #region Constructor

        public EnemyFrigate(BasicModel model, CollidableType type, float boundingRadius, Vector3 shipPosition, Quaternion shipRotation, float minShipSpeed, float maxShipSpeed,
            float turningSpeed, float distanceThresholdEnemy, float distanceThresholdFriend, int life, List<WeaponSystem2D> weaponSystemList)
        {
            ShipPosition = shipPosition;
            ShipWorld = Matrix.CreateTranslation(shipPosition);
            ShipRotation = shipRotation;

            ShipSpeed = 1f;
            MinShipSpeed = minShipSpeed;
            MaxShipSpeed = maxShipSpeed;
            TurningSpeed = turningSpeed;

            DistanceThresholdEnemy = distanceThresholdEnemy;
            DistanceThresholdFriend = distanceThresholdFriend;

            this.CollidableType = type;
            this.BoundingSphereRadius = boundingRadius;
            IsFrigate = true;
            Life = life;

            _model = model;
            _momentum = 0f;
            _shipStopped = true;

            _weaponsList = weaponSystemList;

            _randomVar = new Random();
        }

        #endregion

        #region Update

        public override void UpdateEnemy(List<Enemy> enemyList, List<Friend> friendList, List<Objective> objectiveList, Vector3 playerPosition, List<Sound> soundDump, List<Explosion> explosionList, GameTime gameTime)
        {
            //ShipRotation = Quaternion.CreateFromRotationMatrix(RotateToFace(ShipPosition, playerPosition, ShipWorld.Up));
            ClosestFriendPosition = ClosestFriend(friendList, objectiveList, playerPosition);

            //Makes the ship follow the Closest Friend Position.
            //ShipRotation = FollowShip(ClosestFriendPosition, 0.1f);

            MoveShip(AIGenerateInputList(enemyList, ClosestFriendPosition, gameTime), gameTime);

            this.BoundingSphereCenter = ShipPosition;

            //Updates the ship world
            _model.UpdateModel(ShipWorld);


            foreach (WeaponSystem2D ws in _weaponsList)
            {
                ws.UpdateWeapons(this.ShipPosition, soundDump, explosionList, gameTime);
                if (ClosestDistance < 250f)
                {
                    ws.RequestFire(ClosestFriendPosition);
                }
            }
        }
        #endregion

        #region Draw

        /// <summary>
        /// Draws the Model of the Enemy Frigate.
        /// </summary>
        /// <param name="camera">Represents the player camera variable.</param>
        public void Draw(Camera camera)
        {
            _model.DrawModel(camera);

            //Draw Weapons:
            foreach (WeaponSystem2D ws in _weaponsList)
            {
                ws.DrawWeapons(camera);
            }
        }

        #endregion

        #region Overrides

        public override void MoveShip(InputActionEnemy action, GameTime gameTime)
        {
            //0.001f
            float second = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float currentTurningSpeed = second * TurningSpeed;

            if (_deadLockDelay > 0)
            {
                ShipRotation = FollowShip(_alternateRotation, 0.0005f);
            }
            else
            {
                ShipRotation = FollowShip(ClosestFriendPosition, 0.0005f);
            }

            //Increments the Acceleration Variable.  Acceleration should occur for 0.5 seconds.  This means the ship has only 0.5 seconds to react to another movement.
            if (ShipSpeed == MaxShipSpeed)
            {
                if (_xIndexSpeed < 30)
                {
                    _momentum = CalculateAcceleration(++_xIndexSpeed);

                }
            }
            else if (ShipSpeed == MinShipSpeed)
            {
                if (_xIndexSpeed > -30)
                {
                    _momentum = CalculateAcceleration(--_xIndexSpeed) * -1;
                }
            }
            else if (_shipStopped)
            {
                if (_xIndexSpeed < 0)
                    _momentum = CalculateAcceleration(++_xIndexSpeed);
                else if (_xIndexSpeed > 0)
                    _momentum = CalculateAcceleration(--_xIndexSpeed);
            }


            switch (action)
            {
                case InputActionEnemy.Forward:
                    //The Enemy cannot move until it finalizes it's acceleration.
                    if (ShipSpeed != MaxShipSpeed)
                    {
                        ShipSpeed = MaxShipSpeed;
                        _shipStopped = false;
                    }
                    break;
                case InputActionEnemy.Reverse:
                    if (ShipSpeed != MinShipSpeed)
                    {
                        ShipSpeed = MinShipSpeed;
                        _shipStopped = false;
                    }
                    break;
                case InputActionEnemy.Stop:
                    if (!_shipStopped)
                    {
                        _shipStopped = true;
                    }
                    break;
            }

            //Updates ShipPosition.
            ShipPosition += Vector3.Transform(Vector3.UnitZ, ShipRotation) * (ShipSpeed * _momentum * second);
            //Updates ShipWorld.
            ShipWorld = Matrix.CreateFromQuaternion(ShipRotation) * Matrix.CreateTranslation(ShipPosition);
        }

        public override void Collision(CollidableType objectCollidedWithType)
        {
            switch (objectCollidedWithType)
            {
                case CollidableType.PlayerBullet:
                    Life--;
                    break;
                case CollidableType.FriendBullet:
                    Life--;
                    break;
                case CollidableType.FriendlyShip:
                    Life = 0;
                    break;
                case CollidableType.Station:
                    Life--;
                    break;
                case CollidableType.Asteroid:
                    Life--;
                    break;
            }
        }

        #endregion

    }
}
