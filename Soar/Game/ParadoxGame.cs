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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Threading;

using Paradox.Game.Classes;
using Paradox.Game.Classes.Ships;
using Paradox.Game.Classes.Levels;
using Paradox.Game.Classes.Engines;
using Paradox.Game.Classes.HUD;
using Paradox.Game.Classes.Inputs;
using Paradox.Menu.GameScreens;

namespace Paradox.Game
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class ParadoxGame : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private Save _save;

        public static event EventHandler FinishedLoading;
        public static event EventHandler Pause;

        private bool _gameActive;
        public bool LoadingScreenActive { protected get; set; } //Represents the state of the loading video.  The game should not draw, but can update, when the video still plays.

        private SpriteBatch _spriteBatch;
        private GraphicsDevice _device;

        Skybox SkyboxTEST;
        GameEngine GameEngineTEST;
        HUDManager HUDManager;

        public ParadoxGame(Paradox game, Save save)
            : base(game)
        {
            _save = save;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            PlayerShip.Pause += (object sender, EventArgs args) =>
                {
                    _gameActive = false;
                    GameEngineTEST.RetrieveSoundEngine().StopMusic();
                    PauseEvent();
                };
            PauseMenu.Resume += (object sender, EventArgs args) =>
                {
                    _gameActive = true;
                };

            EndGameMenu.Selected += (o, e) =>
                {
                    LoadingScreenActive = true; //We still want the game to be updated, but not drawn.
                    GameEngineTEST.RetrieveSoundEngine().StopMusic();
                };

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _gameActive = false;
            Thread thread = new Thread(LoadingThread);
            thread.Start();

            // TODO: use this.Game.Content to load your game content here
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            if (_gameActive)
            {
                GameEngineTEST.Update(gameTime);

                HUDManager.Update(gameTime, GameEngineTEST.RetrievePlayerShip(), GameEngineTEST.RetrieveEnemyList(), GameEngineTEST.RetrieveFriendList(),
                    GameEngineTEST.Level.PlayerStarBase, GameEngineTEST.Level.EnemyStarBase, GameEngineTEST.AmountEnemies, GameEngineTEST.AmountFriends, GameEngineTEST.Level, GameEngineTEST.Level.TimeRemaining);
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (_gameActive && !LoadingScreenActive)
            {
                //The Skybox should be relative to the camera.
                SkyboxTEST.DrawSkybox(GameEngineTEST.RetrievePlayerShip().Camera, GameEngineTEST.RetrievePlayerShip().Camera.CameraPosition);

                GameEngineTEST.Draw(GameEngineTEST.RetrievePlayerShip().Camera);
                GameEngineTEST.RetrievePlayerShip().DrawShip();

                HUDManager.Draw(GameEngineTEST.RetrievePlayerShip().Camera, GameEngineTEST.RetrieveFriendList(), GameEngineTEST.RetrieveEnemyList(), GameEngineTEST.Level.ObjectiveList);
            }
        }

        public void LoadingThread()
        {
            GameEngine.CollidableList.Clear();
            _save.Config.Win = false;
            _save.Config.End = false;
            _device = GraphicsDevice;
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Texture2D[] Skywalls = { Game.Content.Load<Texture2D>(@"Game/Levels/Skybox/SkyBox_Back"), Game.Content.Load<Texture2D>(@"Game/Levels/Skybox/SkyBox_Bottom"), Game.Content.Load<Texture2D>(@"Game/Levels/Skybox/SkyBox_Front"), 
                                       Game.Content.Load<Texture2D>(@"Game/Levels/Skybox/SkyBox_Left"), Game.Content.Load<Texture2D>(@"Game/Levels/Skybox/SkyBox_Right"), Game.Content.Load<Texture2D>(@"Game/Levels/Skybox/SkyBox_Top") };
            SkyboxTEST = new Skybox(_device, Skywalls, Game.Content.Load<Model>(@"Game/Levels/Skybox/Skybox"));

            Texture2D[] ExplosionArray = { 
                                             Game.Content.Load<Texture2D>(@"Game/Images/Explosions/ExplosionSet1/screen0"),
                                             Game.Content.Load<Texture2D>(@"Game/Images/Explosions/ExplosionSet1/screen1"),
                                             Game.Content.Load<Texture2D>(@"Game/Images/Explosions/ExplosionSet1/screen2"),
                                             Game.Content.Load<Texture2D>(@"Game/Images/Explosions/ExplosionSet1/screen3"),
                                             Game.Content.Load<Texture2D>(@"Game/Images/Explosions/ExplosionSet1/screen4"),
                                             Game.Content.Load<Texture2D>(@"Game/Images/Explosions/ExplosionSet1/screen5"),
                                             Game.Content.Load<Texture2D>(@"Game/Images/Explosions/ExplosionSet1/screen6"),
                                             Game.Content.Load<Texture2D>(@"Game/Images/Explosions/ExplosionSet1/screen7"),
                                             Game.Content.Load<Texture2D>(@"Game/Images/Explosions/ExplosionSet1/screen8"),
                                             Game.Content.Load<Texture2D>(@"Game/Images/Explosions/ExplosionSet1/screen9"),
                                             Game.Content.Load<Texture2D>(@"Game/Images/Explosions/ExplosionSet1/screen10"),
                                             Game.Content.Load<Texture2D>(@"Game/Images/Explosions/ExplosionSet1/screen11"),
                                             Game.Content.Load<Texture2D>(@"Game/Images/Explosions/ExplosionSet1/screen12"),
                                             Game.Content.Load<Texture2D>(@"Game/Images/Explosions/ExplosionSet1/screen13")
                                         };

            Texture2D[] ExplosionArray2 = { 
                                             Game.Content.Load<Texture2D>(@"Game/Images/Explosions/ExplosionSet2/screen1"),
                                             Game.Content.Load<Texture2D>(@"Game/Images/Explosions/ExplosionSet2/screen2"),
                                             Game.Content.Load<Texture2D>(@"Game/Images/Explosions/ExplosionSet2/screen3"),
                                             Game.Content.Load<Texture2D>(@"Game/Images/Explosions/ExplosionSet2/screen4"),
                                             Game.Content.Load<Texture2D>(@"Game/Images/Explosions/ExplosionSet2/screen5"),
                                             Game.Content.Load<Texture2D>(@"Game/Images/Explosions/ExplosionSet2/screen6"),
                                             Game.Content.Load<Texture2D>(@"Game/Images/Explosions/ExplosionSet2/screen7"),
                                             Game.Content.Load<Texture2D>(@"Game/Images/Explosions/ExplosionSet2/screen8"),
                                             Game.Content.Load<Texture2D>(@"Game/Images/Explosions/ExplosionSet2/screen9"),
                                             Game.Content.Load<Texture2D>(@"Game/Images/Explosions/ExplosionSet2/screen10"),
                                             Game.Content.Load<Texture2D>(@"Game/Images/Explosions/ExplosionSet2/screen11"),
                                             Game.Content.Load<Texture2D>(@"Game/Images/Explosions/ExplosionSet2/screen12"),
                                             Game.Content.Load<Texture2D>(@"Game/Images/Explosions/ExplosionSet2/screen13"),
                                             Game.Content.Load<Texture2D>(@"Game/Images/Explosions/ExplosionSet2/screen14"),
                                             Game.Content.Load<Texture2D>(@"Game/Images/Explosions/ExplosionSet2/screen15")
                                         };
            #region Spawn Manager
            List<ShipProfile> shipProfiles = new List<ShipProfile>();
            shipProfiles.Add(new ShipProfile(6, 5, 2, 2));
            shipProfiles.Add(new ShipProfile(4, 5, 3, 3));
            shipProfiles.Add(new ShipProfile(2, 4, 6, 6));
            SpriteFont spawnerFont = Game.Content.Load<SpriteFont>(@"Game/Fonts/SpawnerFont");

            Texture2D[] arrows = { Game.Content.Load<Texture2D>(@"Game/Images/HUD/Spawner/ArrowLeft"),
                                  Game.Content.Load<Texture2D>(@"Game/Images/HUD/Spawner/ArrowLeftShaded"),
                                  Game.Content.Load<Texture2D>(@"Game/Images/HUD/Spawner/ArrowRight"),
                                  Game.Content.Load<Texture2D>(@"Game/Images/HUD/Spawner/ArrowRightShaded")
                                 };

            Texture2D[] bars = {
                                 Game.Content.Load<Texture2D>(@"Game/Images/HUD/Spawner/GreenBar"),
                                 Game.Content.Load<Texture2D>(@"Game/Images/HUD/Spawner/RedBar")
                             };

            Texture2D overlay = Game.Content.Load<Texture2D>(@"Game/Images/HUD/Spawner/Overlay");

            Texture2D[] shipImages = {
                                         Game.Content.Load<Texture2D>(@"Game/Images/HUD/Spawner/ShipImage"),
                                         Game.Content.Load<Texture2D>(@"Game/Images/HUD/Spawner/ShipImage"),
                                         Game.Content.Load<Texture2D>(@"Game/Images/HUD/Spawner/ShipImage")
                                     };

            SpawnerManager spawnerManager = new SpawnerManager(_device, shipProfiles, spawnerFont, arrows, bars, overlay, shipImages, 3);
            #endregion

            WeaponSystem2D weaponSystem2D = new WeaponSystem2D(GameEngine.CollidableList, CollidableType.PlayerBullet, 1f, SoundType.Player_Shoot, Game.Content.Load<Texture2D>(@"Game/Images/Weapons/bullet"),
                0.25f, _device, 10, new Vector3(0, 0, 0), 150, 300, 0.001f);
            WeaponSystem2D weaponSystem2D2 = new WeaponSystem2D(GameEngine.CollidableList, CollidableType.PlayerBullet, 1f, SoundType.Player_Shoot, Game.Content.Load<Texture2D>(@"Game/Images/Weapons/bullet"),
                0.25f, _device, 10, new Vector3(0, 0, 0), 100, 300, 0.001f);
            WeaponSystem2D weaponSystem2D3 = new WeaponSystem2D(GameEngine.CollidableList, CollidableType.PlayerBullet, 1f, SoundType.Player_Shoot, Game.Content.Load<Texture2D>(@"Game/Images/Weapons/bullet"),
                0.25f, _device, 10, new Vector3(0, 0, 0), 75, 300, 0.001f);
            MissleSystem2D missleSystem2D = new MissleSystem2D(GameEngine.CollidableList, CollidableType.FriendMissle, 1f, SoundType.PlayerMissle, Game.Content.Load<Texture2D>(@"Game/Images/Weapons/bullet"), 1f,
                _device, 10, new Vector3(0, 0, 0), 1400, 100, 0.0001f, 14, 600);

            Model playerModel1 = Game.Content.Load<Model>("Game/Models/Viper/bomber");
            float playermodelscale1 = 0.07f;

            //Player Configurations
            PlayerConfig[] playerConfigurations = new PlayerConfig[3];

            playerConfigurations[0] = new PlayerConfig(new BasicModel(playerModel1, playermodelscale1), 13, new Vector3(0, 0.5f, -7), weaponSystem2D, missleSystem2D, 15, 25, -25, 2, 1.5f, 1.5f, 1f);
            playerConfigurations[1] = new PlayerConfig(new BasicModel(Game.Content.Load<Model>(@"Game/Models/Viper/inceptor"), playermodelscale1), 10, new Vector3(0, 0.5f, -7), weaponSystem2D2, missleSystem2D, 5, 30, -30, 2, 2f, 2f, 1f);
            playerConfigurations[2] = new PlayerConfig(new BasicModel(Game.Content.Load<Model>(@"Game/Models/Viper/ghoul"), playermodelscale1), 5, new Vector3(0, 0.5f, -7), weaponSystem2D3, missleSystem2D, 2, 40, -40, 2f, 2.5f, 2.5f, 1f);


            PlayerShip playerShipTEST = new PlayerShip(GameEngine.CollidableList, CollidableType.FriendlyShip, playerModel1.Meshes.ToList()[0].BoundingSphere.Radius * playermodelscale1, playerModel1, new Vector3(800, 40, -200),
                _device, new Vector3(0, 0.5f, -7), playermodelscale1, -30, 30, 2, 2, 1.5f, 1f, weaponSystem2D, missleSystem2D, spawnerManager, playerConfigurations);

            

            Planet planet;
            //Setting up the planet for the mission.
            switch (_save.Config.Mission)
            {
                case 0:
                    planet = new Planet(Game.Content.Load<Model>("Game/Models/Planets/Planet2/earth"), new Vector3(2800, 0, 0), Quaternion.Identity, 0.01f, 15f);
                    break;
                case 1:
                    planet = new Planet(Game.Content.Load<Model>("Game/Models/Planets/Planet1/dradis"), new Vector3(-300, -2800, 1300), Quaternion.Identity, 0.01f, 120f);
                    break;
                case 2:
                    planet = new Planet(Game.Content.Load<Model>("Game/Models/Planets/Planet3/mars"), new Vector3(300, 2800, 1300), Quaternion.CreateFromAxisAngle(new Vector3(0,0,1), 2f), 0.01f, 60f);
                    break;
                default:
                    planet = new Planet(Game.Content.Load<Model>("Game/Models/Planets/Planet2/earth"), new Vector3(2800, 0, 0), Quaternion.Identity, 0.01f, 50f);
                    break;
            }

            Starbase playerstarbase;
            Model playerBaseModel;
            float playerBaseScale;
            //Setting up the playerbase for the mission.
            switch (_save.Config.Mission)
            {
                case 0:
                    playerBaseModel = Game.Content.Load<Model>("Game/Models/Starbase/PlayerBase/Station/Battlestation");
                    playerBaseScale = 25f;
                    playerstarbase = new Starbase(CollidableType.FriendBase, playerBaseModel.Meshes.ToList()[0].BoundingSphere.Radius * playerBaseScale, playerBaseModel, new Vector3(0, -300, -1000), Quaternion.Identity, playerBaseScale);
                    break;
                case 1:
                    playerBaseModel = Game.Content.Load<Model>("Game/Models/Starbase/PlayerBase/Station/Battlestation2");
                    playerBaseScale = 50f;
                    playerstarbase = new Starbase(CollidableType.FriendBase, playerBaseModel.Meshes.ToList()[0].BoundingSphere.Radius * playerBaseScale, playerBaseModel, new Vector3(0, -300, -1000), Quaternion.Identity, playerBaseScale);
                    break;
                case 2:
                    playerBaseModel = Game.Content.Load<Model>("Game/Models/Starbase/PlayerBase/Station/Battlestation2");
                    playerBaseScale = 50f;
                    playerstarbase = new Starbase(CollidableType.FriendBase, playerBaseModel.Meshes.ToList()[0].BoundingSphere.Radius * playerBaseScale, playerBaseModel, new Vector3(0, -300, -1000), Quaternion.Identity, playerBaseScale);
                    break;
                default: 
                    playerBaseModel = Game.Content.Load<Model>("Game/Models/Starbase/PlayerBase/Station/Battlestation");
                    playerBaseScale = 25f;
                    playerstarbase = new Starbase(CollidableType.FriendBase, playerBaseModel.Meshes.ToList()[0].BoundingSphere.Radius * playerBaseScale, playerBaseModel, new Vector3(0, -300, -1000), Quaternion.Identity, playerBaseScale);
                    
                    break;

            }
            Model enemyBaseModel = Game.Content.Load<Model>("Game/Models/Starbase/EnemyBase/Station/sleeper7");
            float enemyBaseScale = 40f;

            Starbase enemyStarBase = new Starbase(CollidableType.EnemyBase, enemyBaseModel.Meshes.ToList()[0].BoundingSphere.Radius * enemyBaseScale, enemyBaseModel, new Vector3(950, 950, 950), Quaternion.CreateFromAxisAngle(Vector3.Up, 90f), enemyBaseScale);


            Model asteroid1 = Game.Content.Load<Model>(@"Game/Models/Asteroids/smallAsteroid");
            float asteroid1Scale = 1f;

            Model asteroid3 = Game.Content.Load<Model>(@"Game/Models/Asteroids/largeAsteroid");
            float asteroid3Scale = 5f;

            BasicModel[] basicAsteroidModel = { new BasicModel(asteroid1, asteroid1Scale), 
                                                  new BasicModel(asteroid3, asteroid3Scale) };
            float[] radiusArray = { asteroid1.Meshes.ToList()[0].BoundingSphere.Radius * asteroid1Scale, asteroid3.Meshes.ToList()[0].BoundingSphere.Radius * asteroid3Scale };

            Model dockModel = Game.Content.Load<Model>(@"Game/Models/Dock/Dock");
            float dockScale = 0.15f;

            Dock dock = new Dock(CollidableType.Dock, dockModel.Meshes.ToList()[0].BoundingSphere.Radius * dockScale, new BasicModel(dockModel, dockScale), new Vector3(800, 40, -200), Quaternion.Identity);

            #region Objective Stuff
            List<Objective> objectiveList = new List<Objective>();

            Model Bstation = Game.Content.Load<Model>(@"Game/Models/Station/BStation");
            float BstationScale = 1f;

            switch (_save.Config.Mission)
            {
                case 0:
                    objectiveList.Add(new ObjectiveDestroyable(CollidableType.Station, Bstation.Meshes.ToList()[0].BoundingSphere.Radius * BstationScale, 0.05f, new BasicModel(Bstation, BstationScale),
                new Vector3(600, 600, 600), Quaternion.Identity, "Protect the Relay Station for 3:00 Minutes", 800, new TimeSpan(0, 3, 0)));
            objectiveList.Add(new ObjectiveEnemy("Eliminate 10 Enemies: ", 10));
            objectiveList.Add(new ObjectiveEnemyFrigate("Eliminate 1 Frigate: ", 1));
                    break;
                case 1:
                    objectiveList.Add(new ObjectiveDestroyable(CollidableType.Station, Bstation.Meshes.ToList()[0].BoundingSphere.Radius * BstationScale, 0.05f, new BasicModel(Bstation, BstationScale),
                new Vector3(600, 600, 600), Quaternion.Identity, "Protect the Relay Station for 5:00 Minutes", 800, new TimeSpan(0, 5, 0)));
                    objectiveList.Add(new ObjectiveDestroyable(CollidableType.Station, Bstation.Meshes.ToList()[0].BoundingSphere.Radius * BstationScale, 0.05f, new BasicModel(Bstation, BstationScale),
                new Vector3(800, 200, 500), Quaternion.CreateFromAxisAngle(new Vector3(0,0,1), 2f), "Protect the Secondary Station for 2:00 Minutes", 300, new TimeSpan(0, 2, 0)));
            objectiveList.Add(new ObjectiveEnemy("Eliminate 20 Enemies: ", 20));
            objectiveList.Add(new ObjectiveEnemyFrigate("Eliminate 2 Frigates: ", 2));
                    break;
                case 2:
                    objectiveList.Add(new ObjectiveDestroyable(CollidableType.Station, Bstation.Meshes.ToList()[0].BoundingSphere.Radius * BstationScale, 0.05f, new BasicModel(Bstation, BstationScale),
                new Vector3(600, 600, 600), Quaternion.Identity, "Protect the Relay Station for 6:00 Minutes", 800, new TimeSpan(0, 5, 0)));
                    objectiveList.Add(new ObjectiveDestroyable(CollidableType.Station, Bstation.Meshes.ToList()[0].BoundingSphere.Radius * BstationScale, 0.05f, new BasicModel(Bstation, BstationScale),
                new Vector3(800, 200, 500), Quaternion.CreateFromAxisAngle(new Vector3(0,0,1), 2f), "Protect the Secondary Station for 4:00 Minutes", 400, new TimeSpan(0, 2, 0)));
            objectiveList.Add(new ObjectiveEnemy("Eliminate 30 Enemies: ", 30));
            objectiveList.Add(new ObjectiveEnemyFrigate("Eliminate 3 Frigates: ", 3));
                    break;
                default:
                    objectiveList.Add(new ObjectiveDestroyable(CollidableType.Station, Bstation.Meshes.ToList()[0].BoundingSphere.Radius * BstationScale, 0.05f, new BasicModel(Bstation, BstationScale),
                new Vector3(600, 600, 600), Quaternion.Identity, "Protect the Relay Station for 3:00 Minutes", 800, new TimeSpan(0, 3, 0)));
            objectiveList.Add(new ObjectiveEnemy("Eliminate 10 Enemies: ", 10));
            objectiveList.Add(new ObjectiveEnemyFrigate("Eliminate 1 Frigate: ", 1));
                    break;
            }

            #endregion


            Sun sun;
            switch (_save.Config.Mission)
            {
                case 0:
                    sun = new Sun(_device, Game.Content.Load<Texture2D>(@"Game/Images/Other/sun"), new Vector3(-300, 1300, 1300), 1000);
                    break;
                case 1:
                    sun = new Sun(_device, Game.Content.Load<Texture2D>(@"Game/Images/Other/sun"), new Vector3(-300, -1300, -1300), 1000);
                    break;
                case 2:
                    sun = new Sun(_device, Game.Content.Load<Texture2D>(@"Game/Images/Other/sun"), new Vector3(300, -1300, 1300), 1000);
                    break;
                default:
                    sun = new Sun(_device, Game.Content.Load<Texture2D>(@"Game/Images/Other/sun"), new Vector3(-300, 1300, 1300), 1000);
                    break;
            }


            Level level = new Level(GameEngine.CollidableList, radiusArray, basicAsteroidModel, planet, playerstarbase, enemyStarBase, SkyboxTEST, dock, sun, playerShipTEST, 40, objectiveList, _device,
                new TimeSpan(0, 10, 0), new Vector3(0, 0, 0), new Vector3(1000, 1000, 1000), _save);

            #region Enemy Stuff
            Model enemyModel = Game.Content.Load<Model>("Game/Models/Raider/fighter1");
            float enemyScale = 1f;
            List<Enemy> enemyList = new List<Enemy>();

            enemyList.Add(new Enemy(CollidableType.EnemyShip, enemyModel.Meshes.ToList()[0].BoundingSphere.Radius * enemyScale, new Vector3(900, 900, 900), Quaternion.CreateFromAxisAngle(Vector3.UnitX, 10), -30, 30, 30, 50, 10, 3));


            WeaponSystem2D enemyWeaponSystem = new WeaponSystem2D(GameEngine.CollidableList, CollidableType.EnemyBullet, 1f, SoundType.Enemy_Shoot, Game.Content.Load<Texture2D>(@"Game/Images/Weapons/bulletBlue"), 0.25f, _device, 125, new Vector3(0, 0, 0), 300, 300, 0.001f);

            List<EnemyFrigate> enemyFrigateList = new List<EnemyFrigate>();
            Model enemyFrigateModel = Game.Content.Load<Model>("Game/Models/Frigate/PirateShip");
            float enemyFrigateScale = 3f;
            List<WeaponSystem2D> frigateweaponSystemList = new List<WeaponSystem2D>();

            switch (_save.Config.Mission)
            {
                case 0:
                    frigateweaponSystemList.Add(new WeaponSystem2D(GameEngine.CollidableList, CollidableType.EnemyBullet, 2.5f, SoundType.Enemy_Shoot, Game.Content.Load<Texture2D>(@"Game/Images/Weapons/bullet"),
                        2.5f, _device, 100, new Vector3(0, 0.2f, 0), 300, 300, 0.001f));
                    frigateweaponSystemList.Add(new WeaponSystem2D(GameEngine.CollidableList, CollidableType.EnemyBullet, 3f, SoundType.Enemy_Shoot, Game.Content.Load<Texture2D>(@"Game/Images/Weapons/bullet"),
                        2.5f, _device, 100, new Vector3(5f, 0.2f, 0), 300, 300, 0.001f));

                    enemyFrigateList.Add(new EnemyFrigate(new BasicModel(enemyFrigateModel, enemyFrigateScale), CollidableType.EnemyShip, enemyFrigateModel.Meshes.ToList()[0].BoundingSphere.Radius * enemyFrigateScale,
                        new Vector3(1000, 1000, 1000), Quaternion.Identity, -10, 10, 0.1f, 20, 10, 300, frigateweaponSystemList));
                    break;
                case 1:
                    frigateweaponSystemList.Add(new WeaponSystem2D(GameEngine.CollidableList, CollidableType.EnemyBullet, 2.5f, SoundType.Enemy_Shoot, Game.Content.Load<Texture2D>(@"Game/Images/Weapons/bullet"),
                        2.5f, _device, 100, new Vector3(0, 0.2f, 0), 300, 300, 0.001f));
                    frigateweaponSystemList.Add(new WeaponSystem2D(GameEngine.CollidableList, CollidableType.EnemyBullet, 3f, SoundType.Enemy_Shoot, Game.Content.Load<Texture2D>(@"Game/Images/Weapons/bullet"),
                        2.5f, _device, 100, new Vector3(5f, 0.2f, 0), 300, 300, 0.001f));

                    enemyFrigateList.Add(new EnemyFrigate(new BasicModel(enemyFrigateModel, enemyFrigateScale), CollidableType.EnemyShip, enemyFrigateModel.Meshes.ToList()[0].BoundingSphere.Radius * enemyFrigateScale,
                        new Vector3(1000, 1000, 1000), Quaternion.Identity, -10, 10, 0.1f, 20, 10, 300, frigateweaponSystemList));
                    enemyFrigateList.Add(new EnemyFrigate(new BasicModel(enemyFrigateModel, enemyFrigateScale), CollidableType.EnemyShip, enemyFrigateModel.Meshes.ToList()[0].BoundingSphere.Radius * enemyFrigateScale,
                        new Vector3(1400, 1400, 1400), Quaternion.Identity, -10, 10, 0.1f, 20, 10, 300, frigateweaponSystemList));
                    break;
                case 2:
                    frigateweaponSystemList.Add(new WeaponSystem2D(GameEngine.CollidableList, CollidableType.EnemyBullet, 2.5f, SoundType.Enemy_Shoot, Game.Content.Load<Texture2D>(@"Game/Images/Weapons/bullet"),
                        2.5f, _device, 100, new Vector3(0, 0.2f, 0), 300, 300, 0.001f));
                    frigateweaponSystemList.Add(new WeaponSystem2D(GameEngine.CollidableList, CollidableType.EnemyBullet, 3f, SoundType.Enemy_Shoot, Game.Content.Load<Texture2D>(@"Game/Images/Weapons/bullet"),
                        2.5f, _device, 100, new Vector3(5f, 0.2f, 0), 300, 300, 0.001f));

                    enemyFrigateList.Add(new EnemyFrigate(new BasicModel(enemyFrigateModel, enemyFrigateScale), CollidableType.EnemyShip, enemyFrigateModel.Meshes.ToList()[0].BoundingSphere.Radius * enemyFrigateScale,
                        new Vector3(1000, 1000, 1000), Quaternion.Identity, -10, 10, 0.1f, 20, 10, 300, frigateweaponSystemList));
                    enemyFrigateList.Add(new EnemyFrigate(new BasicModel(enemyFrigateModel, enemyFrigateScale), CollidableType.EnemyShip, enemyFrigateModel.Meshes.ToList()[0].BoundingSphere.Radius * enemyFrigateScale,
                        new Vector3(1400, 1400, 1400), Quaternion.Identity, -10, 10, 0.1f, 20, 10, 300, frigateweaponSystemList));
                    enemyFrigateList.Add(new EnemyFrigate(new BasicModel(enemyFrigateModel, enemyFrigateScale), CollidableType.EnemyShip, enemyFrigateModel.Meshes.ToList()[0].BoundingSphere.Radius * enemyFrigateScale,
                        new Vector3(1400, 900, 1400), Quaternion.Identity, -10, 10, 0.1f, 20, 10, 300, frigateweaponSystemList));
                    break;

                default:
                    frigateweaponSystemList.Add(new WeaponSystem2D(GameEngine.CollidableList, CollidableType.EnemyBullet, 2.5f, SoundType.Enemy_Shoot, Game.Content.Load<Texture2D>(@"Game/Images/Weapons/bullet"),
                        2.5f, _device, 100, new Vector3(0, 0.2f, 0), 300, 300, 0.001f));
                    frigateweaponSystemList.Add(new WeaponSystem2D(GameEngine.CollidableList, CollidableType.EnemyBullet, 3f, SoundType.Enemy_Shoot, Game.Content.Load<Texture2D>(@"Game/Images/Weapons/bullet"),
                        2.5f, _device, 100, new Vector3(5f, 0.2f, 0), 300, 300, 0.001f));

                    enemyFrigateList.Add(new EnemyFrigate(new BasicModel(enemyFrigateModel, enemyFrigateScale), CollidableType.EnemyShip, enemyFrigateModel.Meshes.ToList()[0].BoundingSphere.Radius * enemyFrigateScale,
                        new Vector3(1000, 1000, 1000), Quaternion.Identity, -10, 10, 0.1f, 20, 10, 300, frigateweaponSystemList));
                    break;
            }
            
            EnemySquadron enemySquadron = new EnemySquadron(GameEngine.CollidableList, CollidableType.EnemyShip, enemyModel.Meshes.ToList()[0].BoundingSphere.Radius * enemyScale, new BasicModel(enemyModel, enemyScale), enemyList, enemyFrigateList, enemyWeaponSystem, new Vector3(950, 950, 950), 10, 40, 300);

            #endregion

            #region Friend Stuff
            Model friendModel = Game.Content.Load<Model>("Game/Models/Viper/inceptor");
            float friendScale = 0.07f;

            

            List<Friend> friendList = new List<Friend>();

            friendList.Add(new Friend(CollidableType.FriendlyShip, friendModel.Meshes.ToList()[0].BoundingSphere.Radius * friendScale, new Vector3(-300, -300, -1000), Quaternion.Identity, -30, 30, 30, 2, 10, 50, 3));
            //new ParticleEngine(pEngine._device, pEngine._displacement, pEngine._textureArray, pEngine.size, pEngine.size, _particleEngine._delayReset)

            WeaponSystem2D friendWeaponSystem = new WeaponSystem2D(GameEngine.CollidableList, CollidableType.FriendBullet, 1f, SoundType.Friend_Shoot, Game.Content.Load<Texture2D>(@"Game/Images/Weapons/bulletRed"), 0.25f, _device, 125, new Vector3(0, 0, 0), 300, 300, 0.001f);



            FriendSquadron friendSquadron = new FriendSquadron(GameEngine.CollidableList, CollidableType.FriendlyShip, friendModel.Meshes.ToList()[0].BoundingSphere.Radius * friendScale, new BasicModel(friendModel, friendScale), friendList, friendWeaponSystem, new Vector3(0, -300, -1000), 15, 20, 100);

            #endregion

            

            ExplosionEngine explosionEngine = new ExplosionEngine(_device, ExplosionArray, 20, 5);
            SoundEffect[] soundEffectArray = { Game.Content.Load<SoundEffect>(@"Game/Audio/Effects/Player_Engine"), 
                                                 Game.Content.Load<SoundEffect>(@"Game/Audio/Effects/Player_Hit"),
                                                 Game.Content.Load<SoundEffect>(@"Game/Audio/Effects/Player_Shoot"),
                                                 Game.Content.Load<SoundEffect>(@"Game/Audio/Effects/Friend_Shoot"),
                                                 Game.Content.Load<SoundEffect>(@"Game/Audio/Effects/Enemy_Shoot"),
                                                 Game.Content.Load<SoundEffect>(@"Game/Audio/Effects/Explosion"),
                                                 Game.Content.Load<SoundEffect>(@"Game/Audio/Effects/Player_Missle")
                                             };

            SoundEffect[] voiceChatterSounds = new SoundEffect[30];
            for (int i = 0; i < 30; i++)
                voiceChatterSounds[i] = Game.Content.Load<SoundEffect>(@"Game/Audio/Voice/Chatter/" + (i + 1));

            SoundEffect[] musicSounds = new SoundEffect[6];

            for (int i = 0; i < 6; i++)
                musicSounds[i] = Game.Content.Load<SoundEffect>(@"Game/Audio/Music/" + (i + 1));

            #region Voices

            List<SoundEffect[]> SoundVoiceEffects = new List<SoundEffect[]>();
            SoundEffect[] start = {
                                      Game.Content.Load<SoundEffect>(@"Game/Audio/Voice/Battlestation/srt1"),
                                           Game.Content.Load<SoundEffect>(@"Game/Audio/Voice/Battlestation/srt2"),
                                           Game.Content.Load<SoundEffect>(@"Game/Audio/Voice/Battlestation/srt3")
                                  };
            SoundVoiceEffects.Add(start);

            SoundEffect[] stationHit = {
                                           Game.Content.Load<SoundEffect>(@"Game/Audio/Voice/Battlestation/sd1"),
                                           Game.Content.Load<SoundEffect>(@"Game/Audio/Voice/Battlestation/sd2"),
                                           Game.Content.Load<SoundEffect>(@"Game/Audio/Voice/Battlestation/sd3")
                                       };
            SoundVoiceEffects.Add(stationHit);

            SoundEffect[] leaving = {
                                        Game.Content.Load<SoundEffect>(@"Game/Audio/Voice/Battlestation/lb1"),
                                           Game.Content.Load<SoundEffect>(@"Game/Audio/Voice/Battlestation/lb2"),
                                           Game.Content.Load<SoundEffect>(@"Game/Audio/Voice/Battlestation/lb3")
                                    };
            SoundVoiceEffects.Add(leaving);

            SoundEffect[] lose = {
                                     Game.Content.Load<SoundEffect>(@"Game/Audio/Voice/Battlestation/bw1"),
                                           Game.Content.Load<SoundEffect>(@"Game/Audio/Voice/Battlestation/bw2"),
                                           Game.Content.Load<SoundEffect>(@"Game/Audio/Voice/Battlestation/bw3")
                                 };

            SoundVoiceEffects.Add(lose);

            SoundEffect[] win = {
                                    Game.Content.Load<SoundEffect>(@"Game/Audio/Voice/Battlestation/gw1"),
                                           Game.Content.Load<SoundEffect>(@"Game/Audio/Voice/Battlestation/gw2"),
                                           Game.Content.Load<SoundEffect>(@"Game/Audio/Voice/Battlestation/gw3")
                                };

            SoundVoiceEffects.Add(win);

            SoundEffect[] completed = {
                                          Game.Content.Load<SoundEffect>(@"Game/Audio/Voice/Battlestation/boc1"),
                                           Game.Content.Load<SoundEffect>(@"Game/Audio/Voice/Battlestation/boc2"),
                                           Game.Content.Load<SoundEffect>(@"Game/Audio/Voice/Battlestation/boc3")
                                      };
            SoundVoiceEffects.Add(completed);

            SoundEffect[] failed = {
                                       Game.Content.Load<SoundEffect>(@"Game/Audio/Voice/Battlestation/bof1"),
                                           Game.Content.Load<SoundEffect>(@"Game/Audio/Voice/Battlestation/bof2"),
                                           Game.Content.Load<SoundEffect>(@"Game/Audio/Voice/Battlestation/bof3")
                                   };
            SoundVoiceEffects.Add(failed);
            #endregion
            SoundEngine soundEngine = new SoundEngine(soundEffectArray, voiceChatterSounds, musicSounds, SoundVoiceEffects, _save);

            Texture2D[] hudOverlay = {
                                         Game.Content.Load<Texture2D>(@"Game/Images/HUD/Overlay/BottomLeftCorner"),
                                         Game.Content.Load<Texture2D>(@"Game/Images/HUD/Overlay/BottomRightCorner"),
                                         Game.Content.Load<Texture2D>(@"Game/Images/HUD/Overlay/TopLeftCorner"),
                                         Game.Content.Load<Texture2D>(@"Game/Images/HUD/Overlay/TopMiddle"),
                                         Game.Content.Load<Texture2D>(@"Game/Images/HUD/Overlay/TopRightCorner")
                                     };


            GameEngineTEST = new GameEngine(playerShipTEST, level, enemySquadron, friendSquadron, explosionEngine, soundEngine);

            Texture2D[] radarImages = { Game.Content.Load<Texture2D>(@"Game/Images/HUD/friend"), Game.Content.Load<Texture2D>(@"Game/Images/HUD/enemy"), Game.Content.Load<Texture2D>(@"Game/Images/HUD/friendBase"), Game.Content.Load<Texture2D>(@"Game/Images/HUD/enemyBase") };
            ShipOverlay enemyOverlay = new ShipOverlay(Game.Content.Load<Texture2D>(@"Game/Images/HUD/EnemyOverlay"), _device, 3.7f);
            ShipOverlay friendOverlay = new ShipOverlay(Game.Content.Load<Texture2D>(@"Game/Images/HUD/FriendOverlay"), _device, 3f);




            HUDManager = new HUDManager(_device, _spriteBatch, spawnerManager, Game.Content.Load<SpriteFont>(@"Game/Fonts/Segoe"), Game.Content.Load<SpriteFont>(@"Game/Fonts/HUDFont"),
                Game.Content.Load<SpriteFont>(@"Game/Fonts/SHUDFont"), radarImages, friendOverlay, enemyOverlay, hudOverlay, Game.Content.Load<Texture2D>(@"Game/Images/HUD/crosshairs"), _save);
            _gameActive = true;
            FinishedLoadingEvent();
        }

        private void FinishedLoadingEvent()
        {
            if (FinishedLoading != null)
            {
                FinishedLoading(this, EventArgs.Empty);
            }
        }
        private static void PauseEvent()
        {
            if (Pause != null)
                Pause(null, EventArgs.Empty);
        }
    }
}
