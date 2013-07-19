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
using Paradox.Game.Classes.Levels;

namespace Paradox.Game.Classes.Ships
{
    public class EnemySquadron
    {
        #region Variables and Properties
        public List<Collidable> CollidableReference;

        private CollidableType _type;
        private float _boundingRadius;

        private BasicModel _enemyModel;

        public List<Enemy> EnemyList;

        private Vector3 _basePosition; //Represents the spawn point for all Enemies.
        private WeaponSystem2D _enemyWeaponSystem;

        private Random _randomVar;
        private Enemy _enemyTemplate;

        private int _defaultLife;
        private int _amountShips;

        public int ShipsArsenal;
        public int ShipsDestroyedPlayer;
        public int FrigatesDestroyedPlayer;

        #endregion

        #region Constructor

        public EnemySquadron(List<Collidable> collidableListRef, CollidableType type, float boundingRadius, BasicModel enemyModel, List<Enemy> enemyList, List<EnemyFrigate> enemyFrigateList, WeaponSystem2D enemyWeaponSystem, Vector3 basePosition, int defaultLife, int amtShips, int shipsArsenal)
        {
            CollidableReference = collidableListRef;
            EnemyList = enemyList;
            _basePosition = basePosition;
            _enemyModel = enemyModel;
            _enemyWeaponSystem = enemyWeaponSystem;

            _type = type;
            _boundingRadius = boundingRadius;

            _randomVar = new Random();

            _enemyTemplate = new Enemy(EnemyList[0].CollidableType, EnemyList[0].BoundingSphereRadius, EnemyList[0].ShipPosition, EnemyList[0].ShipRotation, EnemyList[0].MinShipSpeed,
                EnemyList[0].MaxShipSpeed, EnemyList[0].TurningSpeed, EnemyList[0].DistanceThresholdFriend, EnemyList[0].DistanceThresholdEnemy, EnemyList[0].Life);

            _defaultLife = defaultLife;

            _amountShips = amtShips;

            ShipsArsenal = shipsArsenal;

            EnemyList.AddRange(enemyFrigateList);

            CollidableReference.AddRange(EnemyList);
        }

        #endregion

        #region Update

        public void UpdateEnemySquadron(List<Friend> friendList, Vector3 playerPosition, List<Explosion> ExplosionList, List<Sound> soundDump, List<Objective> objectiveList, GameTime gameTime)
        {

            //Updates the enemy, and if the enemy is close enough, the enemy requests to fire.  This method also removes and generates enemies accordingly.
            for (int i = 0; i < EnemyList.Count(); i++)
            {
                EnemyList[i].UpdateEnemy(EnemyList, friendList, objectiveList, playerPosition, soundDump, ExplosionList, gameTime);

                if (EnemyList[i].ClosestDistance < 250f)
                {
                    _enemyWeaponSystem.RequestFire(EnemyList[i].ShipPosition, EnemyList[i].ShipRotation);
                }

                /*
                //Random Error: {"Index was out of range. Must be non-negative and less than the size of the collection.\r\nParameter name: index"}
                if (Math.Abs(EnemyList[i].ShipPosition.X) > 1500 || Math.Abs(EnemyList[i].ShipPosition.Y) > 1500 || Math.Abs(EnemyList[i].ShipPosition.Z) > 1500)
                {
                    EnemyList.Remove(EnemyList[i]);
                }
                 * */

                if (EnemyList[i].Life <= 0)
                {
                    if (EnemyList[i].IsFrigate)
                        FrigatesDestroyedPlayer++;
                    ExplosionList.Add(new Explosion(EnemyList[i].ShipPosition, 0));
                    soundDump.Add(new Sound(SoundType.Explosion, EnemyList[i].ShipPosition, false));

                    CollidableReference.Remove(EnemyList[i]);
                    EnemyList.Remove(EnemyList[i--]);
                }
            }



            if (EnemyList.Count() < _amountShips && ShipsArsenal > 0)
            {
                GenerateEnemyAtBase();
                ShipsArsenal--;
            }
            _enemyWeaponSystem.UpdateWeapons(soundDump, ExplosionList, gameTime);
        }

        #endregion

        #region Draw

        public void DrawEnemySquadron(Camera camera)
        {
            foreach (Enemy enemy in EnemyList)
            {
                if (!enemy.IsFrigate)
                {
                    _enemyModel.World = enemy.ShipWorld;
                    _enemyModel.DrawModel(camera);
                }
                else
                {
                    //The enemy is a frigate. Let's draw it.
                    (enemy as EnemyFrigate).Draw(camera);
                }
            }


            _enemyWeaponSystem.DrawWeapons(camera);
        }

        #endregion

        #region Helper Methods

        public void GenerateEnemyAtBase()
        {
            Enemy enemy1 = new Enemy(_enemyTemplate.CollidableType, _enemyTemplate.BoundingSphereRadius, _enemyTemplate.ShipPosition, _enemyTemplate.ShipRotation, _enemyTemplate.MinShipSpeed,
                _enemyTemplate.MaxShipSpeed, _enemyTemplate.TurningSpeed, _enemyTemplate.DistanceThresholdFriend, _enemyTemplate.DistanceThresholdEnemy, _enemyTemplate.Life);
            enemy1.Life = _defaultLife;
            enemy1.ShipPosition = new Vector3(_basePosition.X + _randomVar.Next(300), _basePosition.Y + _randomVar.Next(300), _basePosition.Z + _randomVar.Next(300));

            //If the enemy is destroyed by a player, it is added to the ShipsDestroyedPlayer variable.
            
            enemy1.DestroyedByPlayer += (object sender, EventArgs args) =>
                {
                    ShipsDestroyedPlayer++;
                };

            CollidableReference.Add(enemy1);
            EnemyList.Add(enemy1);
        }

        #endregion
    }
}
