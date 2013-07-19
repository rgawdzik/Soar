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

using Paradox.Game;
using Paradox.Game.Classes;
using Paradox.Game.Classes.Ships;
using Paradox.Game.Classes.Levels;
using Paradox.Game.Classes.Engines;
using Paradox.Game.Classes.Inputs;
using Paradox.Menu;
using Paradox.Menu.GameScreens;

namespace Paradox
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Paradox : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private GraphicsDevice _device;
        private SpriteFont _menuFont;

        private List<GameScreen> _gameScreenList;

        private GameVideo _gameIntroVideo;
        private bool _introEnded;

        private bool _gameActive;
        private bool _gameLoaded;
        private bool _loadingScreenActive;

        private ParadoxGame _game;

        private Save _save;

        /// <summary>
        /// Constructor and the Starting point of the game
        /// </summary>
        /// <param name="screenProperties">Represents the screen properties that were passed from the thread creation</param>
        public Paradox(object[] screenProperties)
        {
            int width = (int)screenProperties[0];
            int height = (int)screenProperties[1];
            bool fullScreen = (bool)screenProperties[2];
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _graphics.PreferredBackBufferWidth = width;
            _graphics.PreferredBackBufferHeight = height;


            _graphics.ApplyChanges();

            if (fullScreen)
            {
                _graphics.ToggleFullScreen();
            }

            _save = Save.LoadFile();
        }

        public static void RunGame(object properties)
        {
            //Since this is the starting point of a new thread, you need to pass an object if you have a thread with a parameter.
            object[] screenProperties = properties as object[];
            using (Paradox game = new Paradox(screenProperties))
            {
                game.Run();
            }
            /*
            try
            {
             using (ParadoxGame game = new ParadoxGame(screenProperties))
                {
                    game.Run();
                }   

            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString());
            }
            */

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            _gameScreenList = new List<GameScreen>();
            _device = GraphicsDevice;
            _spriteBatch = new SpriteBatch(_device);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            string[] descriptions = new string[3];
            descriptions[0] = "Earth is under attack! You|" + 
                               "must protect Earth at all|" +
                               "cost. The machines view earth|" +
                               "as a strategic outpost. We|" + 
                               "are counting on you to|" +
                               "complete certain objectives|" +
                               "that ensure help arrives.|" +
                               "Good Luck. Battlestation out.";
            descriptions[1] = "Dradis, our mining colony is|" +
                               "now threatened by the machines.|" +
                               "This mining colony provides fuel|" +
                               "for our ships.  Without it, we|" +
                               "are sitting ducks.  We must wait|" +
                               "for reinforcements to arrive.|" +
                               "We need you to complete your|" +
                               "objectives.|" +
                               "Good Luck. Battlestation out.";
            descriptions[2] = "We must finish the machines|" +
                               "off at Mars[<3 NASA & Curiosity]|" +
                               "Many citizens of|" +
                               "the Terran Empire are counting|" +
                               "on you for their protection.|" +
                               "Let's finish them off for good.|" +
                               "Complete the required|" +
                               "objectives.|" +
                               "For the Terran Empire!";
            Texture2D[] panels = {Content.Load<Texture2D>(@"Game/Images/Menu/Main"),
                                     Content.Load<Texture2D>(@"Game/Images/Menu/Panel")};
            Texture2D[] backgroundAssets = { Content.Load<Texture2D>(@"Game/Images/Menu/Logo") };

            MainMenu mainMenu = new MainMenu(panels, backgroundAssets);
            MainMenu.SelectedEvent();
            PlayMenu playMenu = new PlayMenu(panels, backgroundAssets, descriptions, Content.Load<SpriteFont>(@"Game/Fonts/Moire"), _save);
            #region Loading Screen
            GameVideo gV = new GameVideo(Content.Load<Video>(@"Game/Videos/GameLoading"));
            LoadingMenu loadingMenu = new LoadingMenu(panels, backgroundAssets, gV);
            OptionsMenu optionsMenu = new OptionsMenu(panels, backgroundAssets, _save);
            AboutMenu aboutMenu = new AboutMenu(panels, backgroundAssets, Content.Load<SpriteFont>(@"Game/Fonts/Moire"));
            SpaceMenu spaceMenu = new SpaceMenu(panels, backgroundAssets, Content.Load<SpriteFont>(@"Game/Fonts/Moire"));
            PauseMenu pauseMenu = new PauseMenu(panels, backgroundAssets);
            EndGameMenu endGameMenu = new EndGameMenu(panels, backgroundAssets, _save, Content.Load<SpriteFont>(@"Game/Fonts/Moire"));
            LeaderboardMenu leaderboardMenu = new LeaderboardMenu(panels, backgroundAssets, _save, Content.Load<SpriteFont>(@"Game/Fonts/Moire"));

            #endregion
            loadingMenu.StartLoading += (object sender, EventArgs args) =>
                {
                    if (!_gameActive)
                    {
                        Components.Add(_game = new ParadoxGame(this, _save));
                        _game.Initialize(); //Initializes the Game so that the loading can begin.
                        _gameActive = true;
                        _gameLoaded = false;
                    }
                };

            ParadoxGame.FinishedLoading += (object sender, EventArgs args) =>
                {
                    _gameLoaded = true;
                };

            //If the game is paused, we tell the Pause Menu that we select it.
            ParadoxGame.Pause += (object sender, EventArgs args) =>
            {
                PauseMenu.SelectedEvent();
                _gameActive = false;
            };

            EndGameMenu.Selected += (o, e) =>
                {
                    _gameActive = false;
                };

            //We are resuming the game.
            PauseMenu.Resume += (object sender, EventArgs args) =>
                {
                    _gameActive = true;
                };

            //We are leaving the game.
            PauseMenu.Exit += (object sender, EventArgs args) =>
                {
                    //Save the game.
                    _save.SaveFile();
                    _gameActive = false;
                    //Let's remove the game.
                    Components.Remove(_game);
                };

            EndGameMenu.MenuSelected += (o, e) =>
                {
                    _save.SaveFile();
                    _gameActive = false;
                    Components.Remove(_game);
                };

            EndGameMenu.Restart += (object sender, EventArgs args) =>
                {
                    _save.SaveFile();
                    Components.Remove(_game);
                    //Let's restart.
                    _gameActive = false;
                    _gameLoaded = false;
                    _loadingScreenActive = true;
                    _save.Config.SpaceBattle = false;
                    LoadingMenu.SelectedEvent();
                };

            SpaceMenu.SpaceBattle += (object sender, EventArgs args) =>
                {
                    _gameActive = false;
                    _gameLoaded = false;
                    _loadingScreenActive = true;
                    _save.Config.SpaceBattle = true;
                    LoadingMenu.SelectedEvent();
                };

            _gameScreenList.Add(mainMenu);
            _gameScreenList.Add(playMenu);
            _gameScreenList.Add(loadingMenu);
            _gameScreenList.Add(optionsMenu);
            _gameScreenList.Add(aboutMenu);
            _gameScreenList.Add(spaceMenu);
            _gameScreenList.Add(pauseMenu);
            _gameScreenList.Add(endGameMenu);
            _gameScreenList.Add(leaderboardMenu);
            _menuFont = Content.Load<SpriteFont>(@"Game/Fonts/SpawnerFont");
            _gameIntroVideo = new GameVideo(Content.Load<Video>(@"Game/Videos/GameIntro"));
            _gameIntroVideo.StartVideo();

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (!_gameActive)
            {

                if (!_introEnded)
                {
                    _introEnded = _gameIntroVideo.VideoDone();
                    _gameIntroVideo.UpdateVideo();
                    if (_gameIntroVideo.RequestLeave)
                        _gameIntroVideo.StopVideo();
                }
                if (_introEnded)
                {
                    foreach (GameScreen gs in _gameScreenList)
                    {
                        gs.UpdateMenu();
                    }
                }
            }

            #region Loading Screen Stuff

            if (_gameActive) //This means the game is loading still, therefore the loading screen should be active. // if (!_gameLoaded && _gameActive)
            {

                LoadingMenu lM = _gameScreenList[2] as LoadingMenu;
                lM.UpdateMenu(_gameLoaded);
                _loadingScreenActive = !lM.IsDone();
                _game.LoadingScreenActive = _loadingScreenActive;

            }

            #endregion

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            //Update all important components of the game.




            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (!_gameActive)
            {
                if (_introEnded)
                {
                    foreach (GameScreen gs in _gameScreenList)
                    {
                        gs.Draw(_device, _spriteBatch, _menuFont);
                    }
                }
                else //Draw the intro video.
                {
                    _gameIntroVideo.Draw(_device, _spriteBatch);
                }
            }
            else if (_loadingScreenActive)
            {
                LoadingMenu lM = _gameScreenList[2] as LoadingMenu;
                lM.Draw(_device, _spriteBatch, _menuFont);

                _menuFont.Spacing = 0;

                if (!_gameLoaded)
                {
                    _spriteBatch.Begin();
                    _spriteBatch.DrawString(_menuFont, "Loading...", Vector2.Zero, Color.White);
                    _spriteBatch.End();
                }
                else
                {
                    _spriteBatch.Begin();
                    _spriteBatch.DrawString(_menuFont, "Loading...Done Press any key to continue", Vector2.Zero, Color.White);
                    _spriteBatch.End();
                }
            }


            base.Draw(gameTime);
        }
    }
}
