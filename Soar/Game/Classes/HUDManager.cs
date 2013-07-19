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

using Paradox.Game.Classes.Ships;
using Paradox.Game.Classes.Levels;
using Paradox.Game.Classes.Cameras;
using Paradox.Game.Classes.HUD;

namespace Paradox.Game.Classes
{
    public class HUDManager
    {
        #region Contact Struct

        struct Contact
        {
            public enum ContactState : byte { Friend = 0, Enemy, FriendBase, EnemyBase, Player };

            public ContactState State;
            public Vector2 Position;
            public Contact(ContactState state, Vector2 position)
            {
                State = state;
                Position = position;
            }
        }

        #endregion

        #region Variables and Properties

        private List<Contact> _contactList;

        private GraphicsDevice _device;

        private Texture2D[] _radarImages;

        private SpriteBatch _spriteBatch;
        private SpriteFont _font;
        private SpriteFont _HUDFont;
        private SpriteFont _HUDFontSmall;
        private string _life;
        private int _lifeInt;
        private string _shipPostion;
        private string _speed;
        private string _missles;

        private int _amountFriends;
        private int _amountEnemies;

        private TimeSpan _elapsedTimeSeconds;

        private ShipOverlay _friendOverlay;
        private ShipOverlay _enemyOverlay;

        private Texture2D[] _HUDImages;

        private Texture2D _crosshair;

        private SpawnerManager _spawnerManager;

        private bool _respawn;

        private bool _leavingBattlefield;

        private TimeSpan _timeLeft;

        private int _score;
        private bool _spaceBattle;

        #endregion

        #region Constructor
        public HUDManager(GraphicsDevice device, SpriteBatch spriteBatch, SpawnerManager spawnerManager, SpriteFont font, SpriteFont hudfont, SpriteFont hudfontsmall, Texture2D[] radarImages,
            ShipOverlay friendOverlay, ShipOverlay enemyOverlay, Texture2D[] hudImages, Texture2D crosshair, Save save)
        {
            _font = font;
            _spriteBatch = spriteBatch;
            _elapsedTimeSeconds = TimeSpan.Zero;
            _contactList = new List<Contact>();
            _radarImages = radarImages;
            _device = device;
            _life = "0";
            _speed = "";
            _missles = "";
            _friendOverlay = friendOverlay;
            _enemyOverlay = enemyOverlay;
            _HUDImages = hudImages;
            _HUDFont = hudfont;
            _spawnerManager = spawnerManager;
            _crosshair = crosshair;
            _spaceBattle = save.Config.SpaceBattle;

            _spawnerManager.Selected += (object sender, EventArgs args) =>
                {
                    //A ship has been selected, therefore respawn is false.
                    _respawn = false;
                };
            _respawn = false;
            _HUDFontSmall = hudfontsmall;
        }
        #endregion

        #region Update
        /// <summary>
        /// Updates the HUD with it's specified parameters that are important to the user.
        /// </summary>
        public void Update(GameTime gameTime, PlayerShip ship, List<Enemy> enemyList, List<Friend> friendList, Starbase friendBase, Starbase enemyBase, int amtEnemyLeft, int amtFriendLeft, Level level, TimeSpan timeLeft)
        {
            if (ship.Life <= 0)
            {
                //The ship is dead, we must show the respawn menu
                _respawn = true;
            }

            if (_respawn)
            {
                _spawnerManager.UpdateManager();
            }

            _life = ship.Life * 10 + "%";
            _lifeInt = ship.Life * 10;
            _shipPostion = "[PlayerPosition] " + ship.ShipPosition.ToString();
            _speed = ship._shipSpeed * 100 + " m/s";
            GenerateRadar(friendList, enemyList, ship, friendBase, enemyBase);
            _missles = ship.AmountMissles() + "M";
            _amountEnemies = amtEnemyLeft;
            _amountFriends = amtFriendLeft;
            _timeLeft = timeLeft;
            _leavingBattlefield = ship.LeftBattle;
            _score = level.Score;
        }
        #endregion

        #region Draw
        public void Draw(Camera camera, List<Friend> friendList, List<Enemy> enemyList, List<Objective> objectiveList)
        {
            if (!_respawn)
            {
                _spriteBatch.Begin();
                foreach (Contact contact in _contactList)
                {
                    _spriteBatch.Draw(_radarImages[(int)contact.State], new Vector2(_device.Viewport.Width - 140 + contact.Position.X, contact.Position.Y + 120), Color.White);
                }

                //Hard coded HUD Overlay Positions, dependant on the screenWidth and height.

                //Bottom Left Overlay
                _spriteBatch.Draw(_HUDImages[0], new Vector2(0, _device.Viewport.Height - 50), Color.White);
                if (_lifeInt > 70)
                    _spriteBatch.DrawString(_HUDFont, _life, new Vector2(0, _device.Viewport.Height - 45), Color.DarkGreen);
                else if (_lifeInt > 50)
                    _spriteBatch.DrawString(_HUDFont, _life, new Vector2(0, _device.Viewport.Height - 45), Color.Yellow);
                else
                    _spriteBatch.DrawString(_HUDFont, _life, new Vector2(0, _device.Viewport.Height - 45), Color.Red);
                _spriteBatch.DrawString(_HUDFont, _missles, new Vector2(110, _device.Viewport.Height - 45), Color.White);


                //Bottom Right Overlay
                _spriteBatch.Draw(_HUDImages[1], new Vector2(_device.Viewport.Width - 250, _device.Viewport.Height - 50), Color.White);
                _spriteBatch.DrawString(_HUDFont, _speed, new Vector2(_device.Viewport.Width - 200, _device.Viewport.Height - 45), Color.White);

                //Top Left Overlay
                _spriteBatch.Draw(_HUDImages[2], new Vector2(0, 0), Color.White);
                _spriteBatch.DrawString(_HUDFont, (int)_timeLeft.TotalMinutes + "m :" + (int)_timeLeft.Seconds + "s", new Vector2(45, 4), Color.White);

                //Middle Overlay
                _spriteBatch.Draw(_HUDImages[3], new Vector2(_device.Viewport.Width / 2 - 150, 0), Color.White);
                _spriteBatch.DrawString(_HUDFont, _amountFriends.ToString(), new Vector2(_device.Viewport.Width / 2 - 90, 0), Color.White); //Draws how many friends.
                _spriteBatch.DrawString(_HUDFont, _amountEnemies.ToString(), new Vector2(_device.Viewport.Width / 2 + 20, 0), Color.White); //Draws how many enemies.

                //Cross Hair
                _spriteBatch.Draw(_crosshair, new Vector2(_device.Viewport.Width / 2 - 18, _device.Viewport.Height / 2 - 110), Color.White);

                //Objective Drawing:
                _spriteBatch.DrawString(_HUDFont, "Objectives", new Vector2(0, 70), Color.White);

                for (int i = 0; i < objectiveList.Count; i++)
                {
                    if (objectiveList[i].IsDone && !objectiveList[i].IsFailed)
                        _spriteBatch.DrawString(_HUDFontSmall, objectiveList[i].Title, new Vector2(0, 70 + (i * 30 + 40)), Color.Green);
                    else if (objectiveList[i].IsDone)
                        _spriteBatch.DrawString(_HUDFontSmall, objectiveList[i].Title, new Vector2(0, 70 + (i * 30 + 40)), Color.Red);
                    else
                        _spriteBatch.DrawString(_HUDFontSmall, objectiveList[i].Title, new Vector2(0, 70 + (i * 30 + 40)), Color.White);
                }

                if (_leavingBattlefield)
                {
                    _spriteBatch.DrawString(_HUDFont, "You are leaving the Battlefield", new Vector2(_device.Viewport.Width / 2 - 300, _device.Viewport.Height - 300), Color.White);
                }

                if (_spaceBattle)
                {
                    _spriteBatch.DrawString(_HUDFont, "Score: " + _score, new Vector2(_device.Viewport.Width - 200, _device.Viewport.Height - 125), Color.White);
                }

                _spriteBatch.End();

                RasterizerState rs = new RasterizerState();
                rs.CullMode = CullMode.None;
                _device.RasterizerState = rs;
                _device.BlendState = BlendState.NonPremultiplied;

                foreach (Enemy enemy in enemyList)
                {
                    _enemyOverlay.OverlayDraw(camera, enemy.ShipPosition);
                }

                foreach (Friend friend in friendList)
                {
                    _friendOverlay.OverlayDraw(camera, friend.ShipPosition);
                }

                RasterizerState rs2 = new RasterizerState();
                rs2.CullMode = CullMode.CullCounterClockwiseFace;
                _device.RasterizerState = rs2;


            }
            else
            {
                _spawnerManager.DrawSpawner(_spriteBatch);

                _spriteBatch.Begin();
                //Middle Overlay
                _spriteBatch.Draw(_HUDImages[3], new Vector2(_device.Viewport.Width / 2 - 150, 0), Color.White);
                _spriteBatch.DrawString(_HUDFont, _amountFriends.ToString(), new Vector2(_device.Viewport.Width / 2 - 90, 0), Color.White); //Draws how many friends.
                _spriteBatch.DrawString(_HUDFont, _amountEnemies.ToString(), new Vector2(_device.Viewport.Width / 2 + 20, 0), Color.White); //Draws how many enemies.

                _spriteBatch.DrawString(_HUDFont, "Choose your ship, press F to spawn", new Vector2(_device.Viewport.Width / 2 - 350, _device.Viewport.Height - 300), Color.White);
                _spriteBatch.End();
            }

        }
        #endregion

        #region Helper Methods

        public void GenerateRadar(List<Friend> friendList, List<Enemy> enemyList, PlayerShip player, Starbase friendBase, Starbase enemyBase)
        {
            _contactList.Clear();

            //Adds Enemies to the radar.
            foreach (Enemy enemy in enemyList)
            {
                _contactList.Add(new Contact(Contact.ContactState.Enemy, new Vector2(enemy.ShipPosition.X / 10, enemy.ShipPosition.Z / 10)));
            }

            foreach (Friend friend in friendList)
            {
                _contactList.Add(new Contact(Contact.ContactState.Friend, new Vector2(friend.ShipPosition.X / 10, friend.ShipPosition.Z / 10)));
            }

            //Adds PlayerShip to the radar.
            _contactList.Add(new Contact(Contact.ContactState.Friend, new Vector2(player.ShipPosition.X / 10, player.ShipPosition.Z / 10)));

            //Add star bases to the radar
            _contactList.Add(new Contact(Contact.ContactState.FriendBase, new Vector2(friendBase.Position.X / 10, friendBase.Position.Z / 10)));
            _contactList.Add(new Contact(Contact.ContactState.EnemyBase, new Vector2(enemyBase.Position.X / 10, enemyBase.Position.Z / 10)));
        }
        #endregion
    }
}
