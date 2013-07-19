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

namespace Paradox.Game.Classes.Ships
{
    public class Friend : Collidable
    {
        #region Variables and Properties
        public Vector3 ShipPosition;
        public Matrix ShipWorld;
        public Quaternion ShipRotation;
        public float ShipSpeed;
        public float MaxShipSpeed;
        public float MinShipSpeed;
        public float Acceleration;
        public float TurningSpeed;
        public float DistanceThresholdFriend;
        public float DistanceThresholdEnemy;
        public int Life;
        public Vector3 ClosestEnemyPosition;
        public float ClosestEnemyDistance;

        private Random _randomVar;

        private const int _deadLockTime = 30;
        private int _deadLockDelay; //If a deadlock occurs, the alternate direction must be followed for 15 frames.
        private bool _deadLock;
        private Quaternion _alternateRotation;

        #endregion

        #region Constructor

        public Friend(CollidableType type, float boundingRadius, Vector3 shipPosition, Quaternion shipRotation, float minShipSpeed, float maxShipSpeed, float acceleration, float turningSpeed, float distanceThresholdFriend, float distanceThresholdEnemy, int life)
        {
            ShipPosition = shipPosition;
            ShipWorld = Matrix.CreateTranslation(shipPosition);
            ShipRotation = shipRotation;

            ShipSpeed = 0;
            MinShipSpeed = minShipSpeed;
            MaxShipSpeed = maxShipSpeed;
            Acceleration = acceleration;
            TurningSpeed = turningSpeed;

            DistanceThresholdFriend = distanceThresholdFriend;
            DistanceThresholdEnemy = distanceThresholdEnemy;

            this.CollidableType = type;
            this.BoundingSphereRadius = boundingRadius;
            Life = life;

            _randomVar = new Random();
        }
        #endregion

        #region Update

        public void UpdateFriend(List<Friend> friendList, List<Enemy> enemyList, Vector3 playerPosition, GameTime gameTime)
        {
            //ShipRotation = Quaternion.CreateFromRotationMatrix(RotateToFace(ShipPosition, playerPosition, ShipWorld.Up));
            ClosestEnemyPosition = ClosestEnemy(enemyList);
            //ShipRotation = FollowShip(ClosestEnemyPosition, 0.1f);

            MoveShip(AIGenerateInputList(friendList, playerPosition, ClosestEnemyPosition, gameTime), gameTime);

            this.BoundingSphereCenter = ShipPosition;

        }

        #endregion

        #region Helper Methods

        public Vector3 ClosestEnemy(List<Enemy> enemyList)
        {
            Vector3 closestEnemy = Vector3.Zero;
            Vector3 cloestEnemy2 = Vector3.Zero;
            float closestDistance = float.MaxValue;
            foreach (Enemy enemy in enemyList)
            {
                float pos = DistanceVector3(ShipPosition, enemy.ShipPosition);
                if (pos < closestDistance)
                {
                    cloestEnemy2 = closestEnemy;
                    closestEnemy = enemy.ShipPosition;
                    closestDistance = pos;
                }
            }
            ClosestEnemyDistance = closestDistance;
            //The Friend Object attacks the second closest enemy, which makes things interesting.
            if (cloestEnemy2 == Vector3.Zero)
            {
                return closestEnemy;
            }
            return cloestEnemy2;
        }

        public void MoveShip(InputActionEnemy action, GameTime gameTime)
        {
            float second = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float currentTurningSpeed = second * TurningSpeed;

            if (_deadLockDelay > 0)
            {
                ShipRotation = FollowShip(_alternateRotation, 0.025f);
            }
            else
            {
                ShipRotation = FollowShip(ClosestEnemyPosition, 0.025f);
            }

            if (Acceleration < 1f)
                Acceleration += 0.034f;

                switch (action)
                {

                    case InputActionEnemy.Forward:
                        if (ShipSpeed != MaxShipSpeed && Acceleration >= 1f)
                        {
                            ShipSpeed = MaxShipSpeed;
                            Acceleration = 0;
                        }
                        break;
                    case InputActionEnemy.Reverse:
                        if (ShipSpeed != MinShipSpeed && Acceleration >= 1f)
                        {
                            ShipSpeed = MinShipSpeed;
                            Acceleration = 0;
                        }
                        break;
                    case InputActionEnemy.Stop:
                        if (ShipSpeed != 0 && Acceleration >= 1f)
                        {
                            ShipSpeed = 0;
                            Acceleration = 0;
                        }
                        break;
                }

            ShipPosition += Vector3.Transform(Vector3.UnitZ, ShipRotation) * (ShipSpeed * Acceleration * second);

            ShipWorld = Matrix.CreateFromQuaternion(ShipRotation) * Matrix.CreateTranslation(ShipPosition);
        }




        public InputActionEnemy AIGenerateInputList(List<Friend> friendList, Vector3 playerPosition, Vector3 enemyPosition, GameTime gameTime)
        {
            _deadLock = false;

            //Decrements the deadlockdelay variable (Should last 15 frames)
            if (_deadLockDelay > 0)
                _deadLockDelay--;

            float distanceFriendEnemy = DistanceVector3(ShipPosition, enemyPosition);
            //float distanceRotationEnemyPlayer = DistanceZVector3(ShipPosition, playerPosition, distanceEnemyPlayer);

            List<float> DistanceBetweenEachShip = new List<float>();

            foreach (Friend friend in friendList)
            {
                //Doesn't matter if this object gets added to the list, since it will be 0, but Object.Equals checking occurs to make sure no error occurs.
                DistanceBetweenEachShip.Add(DistanceVector3(friend.ShipPosition, ShipPosition));
            }

            //Adds the Player to the List, making him appear as a friend (Which he should be.
            DistanceBetweenEachShip.Add(DistanceVector3(playerPosition, ShipPosition));

            bool possibleDeadlock = false;


                Vector3 newFriendPositionSim = SimulateShipMovement(InputActionEnemy.Forward, gameTime);
                float distanceFriendEnemySim = DistanceVector3(newFriendPositionSim, enemyPosition);

                for (int z = 0; z < friendList.Count; z++)
                {
                    if (!Object.Equals(this, friendList[z]))
                    {
                        float newDistanceBetweenEachShip = DistanceVector3(newFriendPositionSim, friendList[z].ShipPosition);

                        if(DistanceThresholdEnemy > newDistanceBetweenEachShip) //The ship is too close to the enemy.
                        {
                            possibleDeadlock = true;
                            if(newDistanceBetweenEachShip < DistanceBetweenEachShip[z])
                            {
                                //The ship is getting closer, this action should not be tolerated.
                                break;
                            }
                        }

                        if (distanceFriendEnemy < distanceFriendEnemySim)
                        {
                            if (newDistanceBetweenEachShip > DistanceBetweenEachShip[z])
                            {
                                //The ship is getting closer, this action should not be tolerated.
                                break;
                            }
                        }
                    }

                //CHECK ON THIS...possibly r
                if (DistanceThresholdEnemy > distanceFriendEnemySim) //Distance between enemy and enemy 
                {
                    if (distanceFriendEnemy > distanceFriendEnemySim) //This action is making the ships closer.
                    {
                        _deadLock = true;
                        break;
                    }
                }
            }

            //Final Checks:
            if (possibleDeadlock)
            {
                Vector3 finalEnemyPositionSim = SimulateShipMovement(InputActionEnemy.Forward, gameTime);
                float finalDistanceEnemyPlayerSim = DistanceVector3(finalEnemyPositionSim, enemyPosition);

                for (int i = 0; i < friendList.Count; i++)
                {
                    if (!Object.Equals(this, friendList[i]))
                    {
                        float newDistanceBetweenEachShip = DistanceVector3(finalEnemyPositionSim, friendList[i].ShipPosition);
                        if (DistanceThresholdEnemy > newDistanceBetweenEachShip)
                        {
                            _deadLock = true;
                            break;
                        }
                    }
                }
            }

            if (_deadLock && _deadLockDelay <= 0)
            {
                _alternateRotation = FindRandomQuaternion(friendList, enemyPosition, gameTime);
                _deadLockDelay = _deadLockTime;
            }

            return InputActionEnemy.Forward;
        }

        public float DistanceVector3(Vector3 v1, Vector3 v2)
        {
            return (float)Math.Sqrt(Math.Abs(Math.Pow(v1.X - v2.X, 2)) + Math.Abs(Math.Pow(v1.Y - v2.Y, 2)) + Math.Abs(Math.Pow(v1.Z - v2.Z, 2)));
        }



        public Vector3 SimulateShipMovement(InputActionEnemy action, GameTime gameTime)
        {
            float second = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float currentTurningSpeed = second * TurningSpeed;

            float ShipSpeedTest = ShipSpeed;


                switch (action)
                {
                    case InputActionEnemy.Forward:
                        ShipSpeedTest = MaxShipSpeed;
                        break;
                    case InputActionEnemy.Reverse:
                        ShipSpeedTest = MinShipSpeed;
                        break;
                    case InputActionEnemy.Stop:
                        ShipSpeedTest = 0;
                        break;
                }
            

            Vector3 ShipPositionSim = ShipPosition;

            ShipPositionSim += Vector3.Transform(Vector3.UnitZ, ShipRotation) * (ShipSpeedTest * second);

            return ShipPositionSim;
        }




        public override void Collision(CollidableType objectCollidedWithType)
        {
            switch (objectCollidedWithType)
            {
                case CollidableType.EnemyBullet:
                    Life--;
                    break;
                case CollidableType.EnemyShip:
                    Life = 0;
                    break;
                case CollidableType.Station:
                    Life--;
                    break;
                case CollidableType.EnemyBase:
                    Life = 0;
                    break;
                case CollidableType.Asteroid:
                    Life = 0;
                    break;
            }
        }

        public Quaternion FindRandomQuaternion(List<Friend> friendList, Vector3 enemyPosition, GameTime gameTime)
        {
            Quaternion possibleRotation = Quaternion.Identity;
            for (int i = 0; i < 5; i++) //This method has 5 attempts to find a rotation that does not collide with any of the enemies, and does not get closer to the Friend.
            {
                bool _working = true;
                possibleRotation = GenerateRotation();

                Vector3 newThisEnemyPosition = SimulateShipMovement(possibleRotation, gameTime);
                float DistanceThisEnemyFriendSim = DistanceVector3(newThisEnemyPosition, enemyPosition);
                float DistanceThisEnemyFriend = DistanceVector3(this.ShipPosition, enemyPosition);

                if (DistanceThresholdFriend > DistanceThisEnemyFriendSim) //The Enemy is within the threshold.
                {
                    if (DistanceThisEnemyFriend > DistanceThisEnemyFriendSim) //This simulation is making the enemy closer than before.
                    {
                        _working = false;
                    }
                }

                if (_working)//This means the enemy is getting farther from the friend.
                {
                    foreach (Friend friend in friendList)
                    {
                        float DistanceThisEnemyEnemy = DistanceVector3(this.ShipPosition, friend.ShipPosition);
                        float DistanceThisEnemyEnemySim = DistanceVector3(newThisEnemyPosition, friend.ShipPosition);

                        if (DistanceThresholdEnemy > DistanceThisEnemyEnemySim) //This Enemy is within the Enemy Threshold.
                        {
                            if (DistanceThisEnemyEnemy > DistanceThisEnemyEnemySim) //This simulation is making the enemies closer to each other.
                            {
                                _working = false;//This rotation does not work.
                            }
                        }

                    }
                }

                if (_working)
                {
                    return possibleRotation;
                }
            }

            return possibleRotation; //If the code reaches this point, the 5 times given to generate a possible rotation failed.
        }

        public Quaternion FollowShip(Vector3 playerPosition, float lerpValue)
        {
            Vector3 desiredDirection = Vector3.Normalize(ShipPosition - playerPosition);
            Quaternion ShipDirection = Quaternion.CreateFromRotationMatrix(Matrix.CreateWorld(ShipPosition, desiredDirection, ShipWorld.Up));
            return Quaternion.Lerp(ShipRotation, ShipDirection, lerpValue);

        }

        public Quaternion FollowShip(Quaternion alternateRotation, float lerpValue)
        {
            return Quaternion.Lerp(ShipRotation, alternateRotation, lerpValue);
        }

        public Quaternion GenerateRotation()
        {
            return Quaternion.CreateFromYawPitchRoll((float)_randomVar.Next(-10, 10), (float)_randomVar.Next(-10, 10), (float)_randomVar.Next(-10, 10));
        }

        public Vector3 SimulateShipMovement(Quaternion possibleRotation, GameTime gameTime) //This method only makes the Ship Go Forward.
        {
            float second = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float currentTurningSpeed = second * TurningSpeed;

            float TestShipSpeed = MaxShipSpeed;

            Vector3 ShipPositionSim = ShipPosition;

            ShipPositionSim += Vector3.Transform(Vector3.UnitZ, possibleRotation) * (TestShipSpeed * Acceleration * second);

            return ShipPositionSim;
        }

        #endregion
    }
}
