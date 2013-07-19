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

using Paradox.Menu.GameScreens;
using Paradox.Game.Classes.Cameras;
using Paradox.Game.Classes.Levels;
using Paradox.Game.Classes.Ships;
using Paradox.Game.Classes.Engines;

namespace Paradox.Game.Classes.Levels
{
    public class Level
    {
        #region Variables and Properties
        List<Collidable> CollidableReference;

        public TimeSpan TimeRemaining;

        public bool GameEnded;
        public event EventHandler StartGame;
        public event EventHandler WinGame;
        public event EventHandler LoseGame;

        private BasicModel[] _asteroidModels;
        private List<Asteroid> _asteroidList;

        private Skybox _skybox;
        private Dock _dock;
        private Sun _sun;

        private Planet _levelPlanet;

        public Starbase PlayerStarBase;
        public Starbase EnemyStarBase;

        private Random _randomVar;

        private float[] _radiusArray;

        public List<Objective> ObjectiveList;
        GraphicsDevice _device;

        List<SoundVoice> _levelSoundVoiceDump;

        Save _save;

        public int Score { get; private set; }

        #endregion

        #region Constructor
        public Level(List<Collidable> collidableRef, float[] radiusArray, BasicModel[] asteroidModels, Planet levelPlanet, 
            Starbase playerStarBase, Starbase enemyStarBase, Skybox skybox, Dock dock, Sun sun, PlayerShip pShip, int amtAsteroids, 
            List<Objective> objectiveList, GraphicsDevice device, TimeSpan time, Vector3 minArea, Vector3 maxArea, Save save)
        {
            CollidableReference = collidableRef;
            _radiusArray = radiusArray;

            _asteroidList = new List<Asteroid>();
            _skybox = skybox;
            _asteroidModels = asteroidModels;
            _dock = dock;
            _randomVar = new Random();

            for (int i = 0; i < amtAsteroids; i++)
            {
                GenerateAsteroid(minArea, maxArea);
            }

            _levelSoundVoiceDump = new List<SoundVoice>();

            foreach (Objective obj in objectiveList)
            {
                obj.Completed += (object sender, EventArgs args) =>
                    {
                        //Vector3.Zero is used because position is irrelevant.
                        _levelSoundVoiceDump.Add(new SoundVoice(SoundType.None, Vector3.Zero, false, SoundVoiceType.Completed));
                    };
                obj.Failed += (object sender, EventArgs args) =>
                    {
                        _levelSoundVoiceDump.Add(new SoundVoice(SoundType.None, Vector3.Zero, false, SoundVoiceType.Failed));
                    };
            }

            pShip.LeftBattlefield += (object sender, EventArgs args) =>
                {
                    _levelSoundVoiceDump.Add(new SoundVoice(SoundType.None, Vector3.Zero, false, SoundVoiceType.Leaving));
                };


            _levelPlanet = levelPlanet;

            PlayerStarBase = playerStarBase;
            EnemyStarBase = enemyStarBase;

            ObjectiveList = objectiveList;
            _device = device;

            TimeRemaining = time;

            _sun = sun;
            StartGame += (object sender, EventArgs args) =>
                {
                    _levelSoundVoiceDump.Add(new SoundVoice(SoundType.None, Vector3.Zero, false, SoundVoiceType.Start));
                };

            WinGame += (object sender, EventArgs args) =>
                {
                    _levelSoundVoiceDump.Add(new SoundVoice(SoundType.None, Vector3.Zero, false, SoundVoiceType.Win));
                };

            LoseGame += (object sender, EventArgs args) =>
                {
                    _levelSoundVoiceDump.Add(new SoundVoice(SoundType.None, Vector3.Zero, false, SoundVoiceType.Lose));
                };
            StartGameEvent();


            CollidableReference.Add(PlayerStarBase);
            CollidableReference.Add(EnemyStarBase);
            CollidableReference.AddRange(_asteroidList);
            CollidableReference.Add(_dock);

            foreach(Objective obj in ObjectiveList)
            {
                CollidableReference.Add(obj);
            }

            _save = save;
        }
        #endregion

        #region Update
        public void Update(GameTime gameTime, FriendSquadron friendSquadron, EnemySquadron enemySquadron, List<SoundVoice> soundVoiceDump)
        {
            Score = enemySquadron.ShipsDestroyedPlayer + enemySquadron.FrigatesDestroyedPlayer;
            if (_levelSoundVoiceDump.Count > 0)
            {

                if (!_levelSoundVoiceDump[0].IsCreated) //These are old sounds, let's ignore em.
                {
                    _levelSoundVoiceDump.Clear();
                }

                soundVoiceDump.AddRange(_levelSoundVoiceDump);
            }

            //Updates each asteroid
            foreach (Asteroid asteroid in _asteroidList)
            {
                asteroid.RotateAsteroid(gameTime);
            }


            //Updates the planet
            _levelPlanet.Update(gameTime);
            //Updates the Star Bases.
            PlayerStarBase.Update();
            EnemyStarBase.Update();


            //Update Objectives
            foreach (Objective obj in ObjectiveList)
            {
                obj.Update(gameTime, enemySquadron, friendSquadron);
            }

            if (TimeRemaining > TimeSpan.Zero)
            {
                TimeRemaining -= gameTime.ElapsedGameTime;
            }

            CheckEndGame();
        }

        #endregion

        #region Draw

        public void Draw(Camera camera)
        {
            _sun.Draw(camera);

            //Draws each Asteroid dependant on the type of asteroid they are.  This ensures as little memory is used.
            foreach (Asteroid asteroid in _asteroidList)
            {
                _asteroidModels[(int)asteroid.AsteroidType].World = asteroid.World;
                _asteroidModels[(int)asteroid.AsteroidType].Rotation = asteroid.Rotation;
                _asteroidModels[(int)asteroid.AsteroidType].DrawModel(camera);
            }

            _levelPlanet.DrawPlanet(camera);
            PlayerStarBase.DrawBase(camera);
            EnemyStarBase.DrawBase(camera);
            _dock.Draw(camera);

            foreach (Objective obj in ObjectiveList)
            {
                obj.Draw(_device, camera);
            }
        }

        #endregion

        #region Helper Methods
        public void GenerateAsteroid(Vector3 AreaMinimum, Vector3 AreaMaximum) //The two Vector3 coordinates tell the general area of where to spawn asteroids.
        {
            //Only the first asteroid is really any good.
            int randInt = 1;
            float boundingRadius = _radiusArray[randInt];


            _asteroidList.Add(new Asteroid(CollidableType.Asteroid, boundingRadius, (AsteroidType)randInt, new Vector3(_randomVar.Next((int)AreaMinimum.X, (int)AreaMaximum.X),
                _randomVar.Next((int)AreaMinimum.X, (int)AreaMaximum.X), _randomVar.Next((int)AreaMinimum.X, (int)AreaMaximum.X)), (float)_randomVar.NextDouble()));
        }

        public void StartGameEvent()
        {
            if (StartGame != null)
                StartGame(this, EventArgs.Empty);
        }
        public void LoseGameEvent()
        {
            if (LoseGame != null)
                LoseGame(this, EventArgs.Empty);
        }
        public void WinGameEvent()
        {
            if (WinGame != null)
                WinGame(this, EventArgs.Empty);
        }

        public void CheckEndGame()
        {
            if (TimeRemaining <= TimeSpan.Zero && !GameEnded)
            {
                GameEnded = true;

                int completedObj = 0;

                foreach (Objective obj in ObjectiveList)
                {
                    if (!obj.IsFailed)
                        completedObj++;
                }

                //To win the game, the amount of objectives not failed must equal the objective count.
                if (completedObj == ObjectiveList.Count)
                {
                    WinGameEvent();
                    _save.Config.Win = true;
                    _save.Config.End = true;
                    _save.Leaderboards.Add(new Posting("", Score));
                }
                else
                {
                    LoseGameEvent();
                    _save.Config.Win = false;
                    _save.Config.End = true;
                    _save.Leaderboards.Add(new Posting("", Score));
                }

                //Let's bring up the endgame menu.
                EndGameMenu.SelectedEvent();
            }
        }

        #endregion
    }
}
