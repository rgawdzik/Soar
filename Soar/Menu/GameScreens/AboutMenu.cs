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

using Paradox.Game;

namespace Paradox.Menu.GameScreens
{
    public class AboutMenu : GameScreen
    {
        public static event EventHandler Selected;

        SpriteFont _aboutFont;

        public AboutMenu(Texture2D[] panels, Texture2D[] backgroundAssets, SpriteFont aboutFont)
            :base(panels, backgroundAssets)
        {
            _aboutFont = aboutFont;
            //This is the starting point of the menu.
            AboutMenu.Selected += (object sender, EventArgs args) =>
            {
                this.CurrentState = State.Shown;
                this._delay = 0;
            };
            MenuEntry back = new MenuEntry("Back");

            back.Enter += (object sender, EventArgs args) =>
                {
                    this.CurrentState = State.Hidden;
                    MainMenu.SelectedEvent();
                };

            this.MenuEntryList.Add(back);
        }

        public override void Draw(GraphicsDevice device, SpriteBatch spriteBatch, SpriteFont font)
        {
            if (CurrentState == State.Shown)
            {
                DrawBackground(spriteBatch, device);
                spriteBatch.Begin();
                //Draw Main Panel
                spriteBatch.Draw(_panels[0], new Vector2(device.Viewport.Width / 2 - 400, device.Viewport.Height / 2 - 300), Color.White);
                int distanceY = (600 - (20 * MenuEntryList.Count)) / (MenuEntryList.Count + 1);
                for (int i = 0; i < MenuEntryList.Count; i++)
                {
                    if (MenuEntryList[i].Hovered)
                    {
                        font.Spacing = 5;
                        spriteBatch.DrawString(font, MenuEntryList[i].Text, new Vector2(device.Viewport.Width / 2 - 100, device.Viewport.Height / 2 - 300 + (i + 1) * distanceY), Color.LightGreen);
                    }
                    else
                    {
                        font.Spacing = _defaultSpacing;
                        spriteBatch.DrawString(font, MenuEntryList[i].Text, new Vector2(device.Viewport.Width / 2 - 100, device.Viewport.Height / 2 - 300 + (i + 1) * distanceY), Color.White);
                    }
                }

                spriteBatch.DrawString(_aboutFont, "Game created by Robert Gawdzik [Byte Code] www.bytecode.ca", new Vector2(device.Viewport.Width / 2 - 330, device.Viewport.Height / 2 - 300 + 50), Color.White);
                spriteBatch.DrawString(_aboutFont, "Some music made by clearsidemusic@gmail.com", new Vector2(device.Viewport.Width / 2 - 330, device.Viewport.Height / 2 - 300 + 125), Color.White);
                spriteBatch.DrawString(_aboutFont, "Some models made by www.solcommand.com", new Vector2(device.Viewport.Width / 2 - 330, device.Viewport.Height / 2 - 300 + 200), Color.White);

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
