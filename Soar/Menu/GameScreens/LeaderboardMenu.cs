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
    public class LeaderboardMenu : GameScreen
    {
        public static event EventHandler Selected;

        private SpriteFont _HUDFont;
        Save _save;

        public LeaderboardMenu(Texture2D[] panels, Texture2D[] backgroundAssets, Save save, SpriteFont hudFont)
            :base(panels, backgroundAssets)
        {
            //This is the starting point of the menu.
            LeaderboardMenu.Selected += (object sender, EventArgs args) =>
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
                        spriteBatch.DrawString(font, MenuEntryList[i].Text, new Vector2(device.Viewport.Width / 2 - 100, device.Viewport.Height / 2 - 200 + (i + 1) * distanceY), Color.LightGreen);
                    }
                    else
                    {
                        font.Spacing = _defaultSpacing;
                        spriteBatch.DrawString(font, MenuEntryList[i].Text, new Vector2(device.Viewport.Width / 2 - 100, device.Viewport.Height / 2 - 200 + (i + 1) * distanceY), Color.White);
                    }
                }

                spriteBatch.DrawString(_HUDFont, "Leaderboards: Top 10", new Vector2(device.Viewport.Width / 2 - 150, 250), Color.White);

                Sort();

                int max;
                if (_save.Leaderboards.Count >= 10)
                    max = 10;
                else
                    max = _save.Leaderboards.Count;
                for (int i = 0; i < max; i++)
                {
                    Posting posting = _save.Leaderboards[i];
                    spriteBatch.DrawString(_HUDFont, (i + 1) + ". " + posting.Name + " => " + posting.Score, new Vector2(device.Viewport.Width / 2 - 100, 275 + (i * 27)), Color.White);
                }

                spriteBatch.End();
            }

        }

        public void Sort()
        {
            bool unsorted = true;

            while (unsorted)
            {
                unsorted = false;
                for (int i = 0; i < _save.Leaderboards.Count - 1; i++)
                {
                    if (_save.Leaderboards[i].Score < _save.Leaderboards[i + 1].Score)
                    {
                        unsorted = true;
                        Posting tempPost = _save.Leaderboards[i];
                        _save.Leaderboards[i] = _save.Leaderboards[i + 1];
                        _save.Leaderboards[i + 1] = tempPost;
                    }
                }
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
