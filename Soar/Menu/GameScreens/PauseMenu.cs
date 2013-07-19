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
    class PauseMenu : GameScreen
    {
        public static event EventHandler Selected;
        public static event EventHandler Resume;
        public static event EventHandler Exit;

        public PauseMenu(Texture2D[] panels, Texture2D[] backgroundAssets)
            :base(panels, backgroundAssets)
        {
            //This is the starting point of the menu.
            PauseMenu.Selected += (object sender, EventArgs args) =>
            {
                this.CurrentState = State.Shown;
                this._delay = 0;
            };

            

            MenuEntry resume = new MenuEntry("Resume");
            MenuEntry exit = new MenuEntry("Exit");
            

            resume.Enter += (object sender, EventArgs args) =>
                {
                    this.CurrentState = State.Hidden;
                    ResumeEvent();
                };

            exit.Enter += (object sender, EventArgs args) =>
                {
                    this.CurrentState = State.Hidden;
                    ExitEvent();
                    MainMenu.SelectedEvent();
                };

            this.MenuEntryList.Add(resume);
            this.MenuEntryList.Add(exit);
        }


        public static void SelectedEvent()
        {
            if (Selected != null)
            {
                Selected(null, EventArgs.Empty); //Since we do not need to know what menu called us, we just pass null.
            }
        }

        public static void ResumeEvent()
        {
            if (Resume != null)
                Resume(null, EventArgs.Empty);
        }

        public static void ExitEvent()
        {
            if (Exit != null)
                Exit(null, EventArgs.Empty);
        }
    }
}
