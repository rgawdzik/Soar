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
    public class MainMenu : GameScreen
    {
        public static event EventHandler Selected;
        /// <summary>
        /// Constructor
        /// </summary>
        public MainMenu(Texture2D[] panels, Texture2D[] backgroundAssets)
            :base(panels, backgroundAssets)
        {
            //This is the starting point of the menu.
            MainMenu.Selected += (object sender, EventArgs args) =>
            {
                this.CurrentState = State.Shown;
                this._delay = 0;
            };

            MenuEntry play = new MenuEntry("Campaign");
            MenuEntry spacebattle = new MenuEntry("Space Battle");
            MenuEntry options = new MenuEntry("Options");
            MenuEntry leaderboards = new MenuEntry("Leaderboards");
            MenuEntry howtoplay = new MenuEntry("How To Play");
            MenuEntry about = new MenuEntry("About");

            play.Enter += (object sender, EventArgs args) =>
            {
                this.CurrentState = State.Hidden;
                PlayMenu.SelectedEvent();
            };

            options.Enter += (object sender, EventArgs args) =>
                {
                    this.CurrentState = State.Hidden;
                    OptionsMenu.SelectedEvent();
                };

            about.Enter += (object sender, EventArgs args) =>
                {
                    this.CurrentState = State.Hidden;
                    AboutMenu.SelectedEvent();
                };
            spacebattle.Enter += (object sender, EventArgs args) =>
                {
                    this.CurrentState = State.Hidden;
                    SpaceMenu.SelectedEvent();
                };
            leaderboards.Enter += (object sender, EventArgs args) =>
                {
                    this.CurrentState = State.Hidden;
                    LeaderboardMenu.SelectedEvent();
                };

            this.MenuEntryList.Add(play);
            this.MenuEntryList.Add(spacebattle);
            this.MenuEntryList.Add(leaderboards);
            this.MenuEntryList.Add(options);
            this.MenuEntryList.Add(about);
            this.MenuEntryList.Add(howtoplay);
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
