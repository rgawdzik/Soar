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
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Paradox.Game.Classes.Cameras;
using Paradox.Game.Classes.Levels;
using Paradox.Game.Classes;
using Paradox.Game.Classes.Ships;

namespace Paradox.Game.Classes.Engines
{
    public class GameEngine
    {
        #region Properties and Variables

        public int AmountEnemies { get { return EnemySquadron.ShipsArsenal; } }
        public int AmountFriends { get { return FriendSquadron.ShipsArsenal; } }

        public Level Level;
        EnemySquadron EnemySquadron;
        FriendSquadron FriendSquadron;
        PlayerShip PlayerShip;

        ExplosionEngine ExplosionEngine;

        public static List<Collidable> CollidableList = new List<Collidable>();


        List<Explosion> ExplosionList;
        List<Sound> _soundDump;
        List<SoundVoice> _soundVoiceDump;

        SoundEngine SoundEngine;

        #endregion

        #region Constructor

        public GameEngine(PlayerShip playerShip, Level level, EnemySquadron enemySquadron, FriendSquadron friendSquadron, ExplosionEngine explosionEngine, SoundEngine soundEngine)
        {
            Level = level;
            EnemySquadron = enemySquadron;
            FriendSquadron = friendSquadron;
            PlayerShip = playerShip;
            ExplosionEngine = explosionEngine;
            _soundDump = new List<Sound>();
            ExplosionList = new List<Explosion>();
            SoundEngine = soundEngine;
            _soundVoiceDump = new List<SoundVoice>();


            CollidableList.Add(PlayerShip);
        } 

        #endregion

        #region Update
        public void Update(GameTime gameTime)
        {
            _soundDump.Clear(); //Clears the sound list before dumping into the sound engine.
            ExplosionList.Clear(); //Clears the Explosion List that gets added to the Explosion Engine.  
            _soundVoiceDump.Clear();

            PlayerShip.UpdateShip(_soundDump, FriendSquadron.FriendList, EnemySquadron.EnemyList, ExplosionList, gameTime); //Adds the PlayerShip so it can collide.

            //Updates World Properties.
            UpdateGravity();
            UpdateProjectiles();

            //Updates all World Properties and adds them to the collidableList where they can collide.
            Level.Update(gameTime, FriendSquadron, EnemySquadron, _soundVoiceDump);
            EnemySquadron.UpdateEnemySquadron(FriendSquadron.FriendList, PlayerShip.ShipPosition, ExplosionList, _soundDump, Level.ObjectiveList, gameTime);
            FriendSquadron.UpdateEnemySquadron(PlayerShip.ShipPosition, EnemySquadron.EnemyList, ExplosionList, _soundDump, gameTime);

            Collidable[] CollidableArray = CollidableList.ToArray();
            //Adds CheckCollisions to the ThreadPool using a Worker object, so Collisions can be checked using multiple threads.
            ThreadPool.QueueUserWorkItem(o => CheckCollisions(CollidableArray));

            //Updates the Explosion engine, then adds any newly created explosions to the list.
            ExplosionEngine.Update();
            ExplosionEngine.AddToList(ExplosionList);

            SoundEngine.ImportSound(_soundDump);
            SoundEngine.ImportSoundVoice(_soundVoiceDump);
            //Updates the Sound Engine
            SoundEngine.UpdateSound(PlayerShip.ShipPosition, gameTime);
        } 
        #endregion

        #region Draw
        public void Draw(Camera camera)
        {
            Level.Draw(camera);
            EnemySquadron.DrawEnemySquadron(camera);
            FriendSquadron.DrawEnemySquadron(camera);

            ExplosionEngine.Draw(camera);

        }
        #endregion

        #region Helper Methods

        public void AddToCollidableList(List<Collidable> collidableList)
        {
            CollidableList.AddRange(collidableList);
        }

        public void AddToCollidableList(Collidable collidableObject)
        {
            CollidableList.Add(collidableObject);
        }

        public void UpdateGravity()
        {

        }

        public void UpdateProjectiles()
        {
            
        }

        public void CheckCollisions(Collidable[] CollidableList)
        {
            foreach (Collidable collidableObject in CollidableList)
            {
                if (collidableObject != null)
                {
                    BoundingSphere sphere1 = new BoundingSphere(collidableObject.BoundingSphereCenter, collidableObject.BoundingSphereRadius);
                    foreach (Collidable collidableObject2 in CollidableList)
                    {
                        if (collidableObject2 != null)
                        {
                            if (!Object.Equals(collidableObject, collidableObject2))
                            {
                                BoundingSphere sphere2 = new BoundingSphere(collidableObject2.BoundingSphereCenter, collidableObject2.BoundingSphereRadius);
                                if (sphere1.Intersects(sphere2))
                                {
                                    if(collidableObject.BoundingSphereCenter != Vector3.Zero || collidableObject2.BoundingSphereCenter != Vector3.Zero)
                                        collidableObject.Collision(collidableObject2.CollidableType);
                                }
                            }
                        }
                    }
                }
            }
        }

        

        public List<Enemy> RetrieveEnemyList()
        {
            return EnemySquadron.EnemyList;
        }

        public List<Friend> RetrieveFriendList()
        {
            return FriendSquadron.FriendList;
        }

        public PlayerShip RetrievePlayerShip()
        {
            return PlayerShip;
        }

        public SoundEngine RetrieveSoundEngine()
        {
            return SoundEngine;
        }

        #endregion
    }
}
