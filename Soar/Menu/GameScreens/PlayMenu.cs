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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Paradox.Game;


namespace Paradox.Menu.GameScreens
{
    public class PlayMenu : GameScreen
    {
        public static event EventHandler Selected;
        private string[] _descriptions;
        private SpriteFont _descFont;

        Save _save; //Points to actual config;

        /// <summary>
        /// Constructor
        /// </summary>
        public PlayMenu(Texture2D[] panels, Texture2D[] backgroundAssets, string[] descriptions, SpriteFont descFont, Save save)
            : base(panels, backgroundAssets)
        {
            _save = save;
            PlayMenu.Selected += (object sender, EventArgs args) =>
            {
                this.CurrentState = State.Shown;
                this._delay = 0;
            };

            MenuEntry play1 = new MenuEntry("Protecting Earth");
            MenuEntry play2 = new MenuEntry("Defending Dradis");
            MenuEntry play3 = new MenuEntry("The Final Stand");
            MenuEntry back = new MenuEntry("Back");

            play1.Enter += (object sender, EventArgs args) =>
            {
                _save.Config.Mission = 0;
                _save.Config.SpaceBattle = false;
                this.CurrentState = State.Hidden;
                LoadingMenu.SelectedEvent();
            };

            play2.Enter += (object sender, EventArgs args) =>
            {
                _save.Config.Mission = 1;
                _save.Config.SpaceBattle = false;
                this.CurrentState = State.Hidden;
                LoadingMenu.SelectedEvent();
            };

            play3.Enter += (object sender, EventArgs args) =>
                {
                    _save.Config.Mission = 2;
                    _save.Config.SpaceBattle = false;
                    this.CurrentState = State.Hidden;
                    LoadingMenu.SelectedEvent();
                };

            back.Enter += (object sender, EventArgs args) =>
                {
                    this.CurrentState = State.Hidden;
                    MainMenu.SelectedEvent();
                };
            this.MenuEntryList.Add(play1);
            this.MenuEntryList.Add(play2);
            this.MenuEntryList.Add(play3);
            this.MenuEntryList.Add(back);
            _descriptions = descriptions;
            _descFont = descFont;

        }

        public override void Draw(GraphicsDevice device, SpriteBatch spriteBatch, SpriteFont font)
        {

            if (CurrentState == State.Shown)
            {
                DrawBackground(spriteBatch, device);
                spriteBatch.Begin();
                //Draw Panel 1
                spriteBatch.Draw(_panels[1], new Vector2(device.Viewport.Width / 2 - 400, device.Viewport.Height / 2 - 300), Color.White);
                int distanceY = (600 - (20 * MenuEntryList.Count)) / (MenuEntryList.Count + 1);
                for (int i = 0; i < MenuEntryList.Count; i++)
                {
                    if (MenuEntryList[i].Hovered)
                    {
                        font.Spacing = 5;
                        spriteBatch.DrawString(font, MenuEntryList[i].Text, new Vector2(device.Viewport.Width / 2 - 350, device.Viewport.Height / 2 - 300 + (i + 1) * distanceY), Color.LightGreen);
                    }
                    else
                    {
                        font.Spacing = _defaultSpacing;
                        spriteBatch.DrawString(font, MenuEntryList[i].Text, new Vector2(device.Viewport.Width / 2 - 350, device.Viewport.Height / 2 - 300 + (i + 1) * distanceY), Color.White);
                    }
                }

                //Draw Panel 2
                spriteBatch.Draw(_panels[1], new Vector2(device.Viewport.Width / 2, device.Viewport.Height / 2 - 300), Color.White);
                for (int i = 0; i < _descriptions.Length; i++)
                {
                    if(_selectedEntry < _descriptions.Count())
                    {
                        string[] split = _descriptions[_selectedEntry].Split('|');

                        int distanceYDesc = (600 - (10 * split.Length)) / (split.Length + 1);
                        for(int x = 0; x < split.Length; x++)
                        {
                            spriteBatch.DrawString(_descFont, split[x], new Vector2(device.Viewport.Width / 2 + 25, device.Viewport.Height / 2 - 300 + (x + 1) * distanceYDesc), Color.White);
                        }
                    }
                }
                spriteBatch.End();
            }
        }


        public static void SelectedEvent()
        {
            if (Selected != null)
            {
                Selected(null, EventArgs.Empty); //Since we do not need to know what menu called us, we just pass null.
            }
        }
    }
}
