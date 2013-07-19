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
    public class Enemy : Collidable
    {
        #region Variables and Properties
        public Vector3 ShipPosition;
        public bool IsFrigate;

        public Matrix ShipWorld;
        public Quaternion ShipRotation;

        public Vector3 ClosestFriendPosition;
        public float ClosestDistance;
        public int Life;

        public float ShipSpeed;
        public float MaxShipSpeed;
        public float MinShipSpeed;
        public float TurningSpeed;
        public float DistanceThresholdEnemy;
        public float DistanceThresholdFriend;

        public bool _shipStopped;
        public float _momentum;
        protected int _xIndexSpeed;

        private Random _randomVar;

        private const int _deadLockTime = 30;
        protected int _deadLockDelay; //If a deadlock occurs, the alternate direction must be followed for 15 frames.
        protected bool _deadLock;
        protected Quaternion _alternateRotation;

        public event EventHandler DestroyedByPlayer;

        #endregion

        #region Constructor

        public Enemy()
        {
            _momentum = 0f;
            _shipStopped = true;

            _randomVar = new Random();
        } //This is called by any inherited classes.

        public Enemy(CollidableType type, float boundingRadius, Vector3 shipPosition, Quaternion shipRotation, float minShipSpeed, float maxShipSpeed, float turningSpeed, float distanceThresholdEnemy, float distanceThresholdFriend, int life)
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

            Life = life;

            _momentum = 0f;
            _shipStopped = true;

            _randomVar = new Random();
        }

        #endregion

        #region Update
        public virtual void UpdateEnemy(List<Enemy> enemyList, List<Friend> friendList, List<Objective> objectiveList, Vector3 playerPosition, List<Sound> soundList, List<Explosion> explosionList, GameTime gameTime)
        {
            //ShipRotation = Quaternion.CreateFromRotationMatrix(RotateToFace(ShipPosition, playerPosition, ShipWorld.Up));
            ClosestFriendPosition = ClosestFriend(friendList, objectiveList, playerPosition);

            //Makes the ship follow the Closest Friend Position.
            //ShipRotation = FollowShip(ClosestFriendPosition, 0.1f);

            MoveShip(AIGenerateInputList(enemyList, ClosestFriendPosition, gameTime), gameTime);

            this.BoundingSphereCenter = ShipPosition;

        }
        #endregion

        #region HelperMethods
        public Vector3 ClosestFriend(List<Friend> friendList, List<Objective> objectiveList, Vector3 playerPosition)
        {
            Vector3 closestFriend = Vector3.Zero;
            float closestDistance = float.MaxValue;
            foreach (Friend friend in friendList)
            {
                float pos = DistanceVector3(ShipPosition, friend.ShipPosition);
                if (pos < closestDistance)
                {
                    closestFriend = friend.ShipPosition;
                    closestDistance = pos;
                }
            }

            foreach (Objective obj in objectiveList)
            {
                if (obj.IsDrawable && !obj.IsDone)
                {
                    float pos = DistanceVector3(ShipPosition, obj.Position);
                    if (pos < closestDistance)
                    {
                        closestFriend = obj.Position;
                        closestDistance = pos;
                    }
                }
            }


            //Checks if the player is closer than any of the friends.
            if (DistanceVector3(ShipPosition, playerPosition) < closestDistance)
            {
                closestFriend = playerPosition;
                closestDistance = DistanceVector3(ShipPosition, playerPosition);
            }

            ClosestDistance = closestDistance;
            return closestFriend;
        }

        public virtual void MoveShip(InputActionEnemy action, GameTime gameTime)
        {
            float second = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float currentTurningSpeed = second * TurningSpeed;

            if (_deadLockDelay > 0)
            {
                ShipRotation = FollowShip(_alternateRotation, 0.025f);
            }
            else
            {
                ShipRotation = FollowShip(ClosestFriendPosition, 0.025f);
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



        public InputActionEnemy AIGenerateInputList(List<Enemy> enemyList, Vector3 enemyPosition, GameTime gameTime)
        {
            _deadLock = false;

            //Decrements the deadlockdelay variable (Should last 15 frames)
            if (_deadLockDelay > 0)
                _deadLockDelay--;

            float distanceFriendEnemy = DistanceVector3(ShipPosition, enemyPosition);
            //float distanceRotationEnemyPlayer = DistanceZVector3(ShipPosition, playerPosition, distanceEnemyPlayer);

            List<float> DistanceBetweenEachShip = new List<float>();

            foreach (Enemy enemy in enemyList)
            {
                //Doesn't matter if this object gets added to the list, since it will be 0, but Object.Equals checking occurs to make sure no error occurs.
                DistanceBetweenEachShip.Add(DistanceVector3(enemy.ShipPosition, ShipPosition));
            }


            bool possibleDeadlock = false;

            Vector3 newFriendPositionSim = SimulateShipMovement(InputActionEnemy.Forward, gameTime);
            float distanceFriendEnemySim = DistanceVector3(newFriendPositionSim, enemyPosition);

            for (int z = 0; z < enemyList.Count; z++)
            {
                if (!Object.Equals(this, enemyList[z]))
                {
                    float newDistanceBetweenEachShip = DistanceVector3(newFriendPositionSim, enemyList[z].ShipPosition);

                    if (DistanceThresholdEnemy > newDistanceBetweenEachShip) //The ship is too close to the enemy.
                    {
                        possibleDeadlock = true;
                        if (newDistanceBetweenEachShip < DistanceBetweenEachShip[z])
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
            }


            //CHECK ON THIS...possibly r
            if (DistanceThresholdEnemy > distanceFriendEnemySim) //Distance between enemy and enemy 
            {
                if (distanceFriendEnemy > distanceFriendEnemySim) //This action is making the ships closer.
                {
                    _deadLock = true;
                }
            }

            //Final Checks:
            if (possibleDeadlock)
            {
                Vector3 finalEnemyPositionSim = SimulateShipMovement(InputActionEnemy.Forward, gameTime);
                float finalDistanceEnemyPlayerSim = DistanceVector3(finalEnemyPositionSim, enemyPosition);

                for (int i = 0; i < enemyList.Count; i++)
                {
                    if (!Object.Equals(this, enemyList[i]))
                    {
                        float newDistanceBetweenEachShip = DistanceVector3(finalEnemyPositionSim, enemyList[i].ShipPosition);
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
                _alternateRotation = FindRandomQuaternion(enemyList, enemyPosition, gameTime);
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

            float TestShipSpeed = ShipSpeed;


            switch (action)
            {
                case InputActionEnemy.Forward:
                    TestShipSpeed = MaxShipSpeed;
                    break;
                case InputActionEnemy.Reverse:
                    TestShipSpeed = MinShipSpeed;
                    break;
                case InputActionEnemy.Stop:
                    TestShipSpeed = 0;
                    break;
            }

            Vector3 ShipPositionSim = ShipPosition;

            ShipPositionSim += Vector3.Transform(Vector3.UnitZ, ShipRotation) * (TestShipSpeed * CalculateAcceleration(_xIndexSpeed) * second);

            return ShipPositionSim;
        }




        public override void Collision(CollidableType objectCollidedWithType)
        {
            switch (objectCollidedWithType)
            {
                case CollidableType.PlayerBullet:
                    Life--;
                    if (Life == 0)
                        DestroyedPlayerEvent();
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
                case CollidableType.FriendBase:
                    Life = 0;
                    break;
                case CollidableType.FriendMissle:
                    Life = 0;
                    if (Life == 0)
                        DestroyedPlayerEvent();
                    break;
                case CollidableType.Asteroid:
                    Life = 0;
                    break;
            }
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

        public float CalculateAcceleration(int x)
        {
            return (float)(1 / 27000f * Math.Pow(x, 3));
        }

        public Quaternion FindRandomQuaternion(List<Enemy> enemyList, Vector3 FriendPosition, GameTime gameTime)
        {
            Quaternion possibleRotation = Quaternion.Identity;
            for (int i = 0; i < 5; i++) //This method has 5 attempts to find a rotation that does not collide with any of the enemies, and does not get closer to the Friend.
            {
                bool _working = true;
                possibleRotation = GenerateRotation();

                Vector3 newThisEnemyPosition = SimulateShipMovement(possibleRotation, gameTime);
                float DistanceThisEnemyFriendSim = DistanceVector3(newThisEnemyPosition, FriendPosition);
                float DistanceThisEnemyFriend = DistanceVector3(this.ShipPosition, FriendPosition);

                if (DistanceThresholdFriend > DistanceThisEnemyFriendSim) //The Enemy is within the threshold.
                {
                    if (DistanceThisEnemyFriend > DistanceThisEnemyFriendSim) //This simulation is making the enemy closer than before.
                    {
                        _working = false;
                    }
                }

                if (_working)//This means the enemy is getting farther from the friend.
                {
                    foreach (Enemy enemy in enemyList)
                    {
                        float DistanceThisEnemyEnemy = DistanceVector3(this.ShipPosition, enemy.ShipPosition);
                        float DistanceThisEnemyEnemySim = DistanceVector3(newThisEnemyPosition, enemy.ShipPosition);

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

            ShipPositionSim += Vector3.Transform(Vector3.UnitZ, possibleRotation) * (TestShipSpeed * CalculateAcceleration(_xIndexSpeed) * second);

            return ShipPositionSim;
        }

        public void DestroyedPlayerEvent()
        {
            if (DestroyedByPlayer != null)
                DestroyedByPlayer(this, EventArgs.Empty);
        }

        #endregion
    }
}
