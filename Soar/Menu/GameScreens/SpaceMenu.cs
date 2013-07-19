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

namespace Paradox.Menu.GameScreens
{
    public class SpaceMenu : GameScreen
    {
        public static event EventHandler Selected;
        public static event EventHandler SpaceBattle;

        SpriteFont _aboutFont;

        public SpaceMenu(Texture2D[] panels, Texture2D[] backgroundAssets, SpriteFont aboutFont)
            :base(panels, backgroundAssets)
        {
            _aboutFont = aboutFont;
            //This is the starting point of the menu.
            SpaceMenu.Selected += (object sender, EventArgs args) =>
            {
                this.CurrentState = State.Shown;
                this._delay = 0;
            };

            MenuEntry spacebattle = new MenuEntry("Let's do it!");
            MenuEntry back = new MenuEntry("I am weak. [back]");

            spacebattle.Enter += (object sender, EventArgs args) =>
                {
                    this.CurrentState = State.Hidden;
                    SpaceBattleEvent();
                };

            back.Enter += (object sender, EventArgs args) =>
                {
                    this.CurrentState = State.Hidden;
                    MainMenu.SelectedEvent();
                };

            this.MenuEntryList.Add(spacebattle);
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

                spriteBatch.DrawString(_aboutFont, "A massive space battle over Earth, 250 vs. 250 reinforcements.", new Vector2(device.Viewport.Width / 2 - 330, device.Viewport.Height / 2 - 300 + 50), Color.White);
                spriteBatch.DrawString(_aboutFont, "This epic battle has leaderboards. Try your luck to take the top spot.", new Vector2(device.Viewport.Width / 2 - 330, device.Viewport.Height / 2 - 300 + 125), Color.White);

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

        public void SpaceBattleEvent()
        {
            if (SpaceBattle != null)
                SpaceBattle(null, EventArgs.Empty);
        }
    }
}
