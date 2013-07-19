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
    public class EndGameMenu : GameScreen
    {
        public static event EventHandler Selected;
        public static event EventHandler MenuSelected;
        public static event EventHandler Restart;

        Save _save;
        SpriteFont _HUDFont;

        public EndGameMenu(Texture2D[] panels, Texture2D[] backgroundAssets, Save save, SpriteFont hudFont)
            :base(panels, backgroundAssets)
        {
            //This is the starting point of the menu.
            EndGameMenu.Selected += (object sender, EventArgs args) =>
            {
                this.CurrentState = State.Shown;
                this._delay = 0;
            };

            MenuEntry menu = new MenuEntry("Mission Select");
            MenuEntry retry = new MenuEntry("Retry");
            menu.Enter += (o, e) =>
                {
                    this.CurrentState = State.Hidden;
                    MenuSelectedEvent();
                    PlayMenu.SelectedEvent();
                };
            retry.Enter += (o, e) =>
                {
                    this.CurrentState = State.Hidden;
                    RestartEvent();
                };

            this.MenuEntryList.Add(menu);
            this.MenuEntryList.Add(retry);

            _save = save;
            _HUDFont = hudFont;
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

                if(_save.Config.Win)
                {
                    spriteBatch.DrawString(_HUDFont, "Good Job! Reinforcements are on the way!", new Vector2(device.Viewport.Width / 2 - 200, device.Viewport.Height / 2 - 200), Color.White);
                }
                else
                {
                    spriteBatch.DrawString(_HUDFont, "We have failed our mission.", new Vector2(device.Viewport.Width / 2 - 200, device.Viewport.Height / 2 - 200), Color.White);
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

        public void MenuSelectedEvent()
        {
            if (MenuSelected != null)
                MenuSelected(null, EventArgs.Empty);
        }

        public void RestartEvent()
        {
            if (Restart != null)
                Restart(null, EventArgs.Empty);
        }
    }
}
