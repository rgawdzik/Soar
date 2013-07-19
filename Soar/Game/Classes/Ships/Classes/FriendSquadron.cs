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
using Paradox.Game.Classes.Engines;

namespace Paradox.Game.Classes.Ships
{
    public class FriendSquadron
    {
        #region Variables and Properties
        public List<Collidable> CollidableReference;

        private CollidableType _type;
        private float _boundingRadius;

        private BasicModel _friendModel;
        public List<Friend> FriendList;
        private Vector3 _basePosition; //Represents the spawn point for all Enemies.
        private WeaponSystem2D _weaponSystem;

        private Random _randomVar;

        private Friend _friendTemplate;

        private int _defaultLife;
        private int _amountShips;

        private List<Sound> _soundDump;

        public int ShipsArsenal;

        #endregion

        #region Constructor

        public FriendSquadron(List<Collidable> collidableList, CollidableType type, float boundingRadius, BasicModel friendModel, List<Friend> friendList, WeaponSystem2D weaponSystem, Vector3 basePosition, int defaultLife, int amtShips, int shipsArsenal)
        {
            CollidableReference = collidableList;
            FriendList = friendList;
            CollidableReference.AddRange(FriendList);
            _basePosition = basePosition;
            _friendModel = friendModel;
            _weaponSystem = weaponSystem;

            _type = type;
            _boundingRadius = boundingRadius;

            _randomVar = new Random();

            //Copies the first friend into a template, so a friend could be easily generated.
            _friendTemplate = new Friend(friendList[0].CollidableType, friendList[0].BoundingSphereRadius, friendList[0].ShipPosition, friendList[0].ShipRotation, friendList[0].MinShipSpeed,
                friendList[0].MaxShipSpeed, friendList[0].Acceleration, friendList[0].TurningSpeed, friendList[0].DistanceThresholdFriend, friendList[0].DistanceThresholdEnemy, friendList[0].Life);

            _defaultLife = defaultLife;

            _amountShips = amtShips;

            _soundDump = new List<Sound>();

            ShipsArsenal = shipsArsenal;
        }

        #endregion

        #region Update

        public void UpdateEnemySquadron(Vector3 playerPosition, List<Enemy> enemyList, List<Explosion> ExplosionList, List<Sound> soundDump, GameTime gameTime)
        {
            soundDump.AddRange(_soundDump);
            _soundDump.Clear();
            //Updates every Friend in the FriendList, and if an enemy is close, it requests to fire.
            for(int i = 0; i < FriendList.Count(); i++)
            {
                FriendList[i].UpdateFriend(FriendList, enemyList, playerPosition, gameTime);
                if (FriendList[i].ClosestEnemyDistance < 250f)
                {
                    _weaponSystem.RequestFire(FriendList[i].ShipPosition, FriendList[i].ShipRotation); //Requests the enemy to fire.
                }

                if (FriendList[i].Life <= 0)
                {
                    ExplosionList.Add(new Explosion(FriendList[i].ShipPosition, 0)); //Adds an explosion
                    soundDump.Add(new Sound(SoundType.Explosion, FriendList[i].ShipPosition, false));

                    CollidableReference.Remove(FriendList[i]);
                    FriendList.Remove(FriendList[i--]);
                }
            }

            if (FriendList.Count() < _amountShips && ShipsArsenal > 0)
            {
                GenerateEnemyAtBase(); //Generates a new Enemy
                ShipsArsenal--;
            }

            _weaponSystem.UpdateWeapons(_soundDump, ExplosionList, gameTime);
        }

        #endregion

        #region Draw

        public void DrawEnemySquadron(Camera camera)
        {
            foreach (Friend friend in FriendList)
            {
                _friendModel.World = friend.ShipWorld;
                _friendModel.DrawModel(camera);
            }

            _weaponSystem.DrawWeapons(camera);
        }

        #endregion

        #region Helper Methods

        public void GenerateEnemyAtBase()
        {
            Friend friend1 = new Friend(_friendTemplate.CollidableType, _friendTemplate.BoundingSphereRadius, _friendTemplate.ShipPosition, _friendTemplate.ShipRotation, _friendTemplate.MinShipSpeed,
                _friendTemplate.MaxShipSpeed, _friendTemplate.Acceleration, _friendTemplate.TurningSpeed, _randomVar.Next(3, (int)_friendTemplate.DistanceThresholdFriend + 8), _friendTemplate.DistanceThresholdEnemy, _friendTemplate.Life);
            friend1.Life = _defaultLife;
            friend1.ShipPosition = new Vector3(_basePosition.X + _randomVar.Next(300), _basePosition.Y + _randomVar.Next(300), _basePosition.Z + _randomVar.Next(300));
            CollidableReference.Add(friend1);
            FriendList.Add(friend1);
        }
        #endregion
    }
}
