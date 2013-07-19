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
    public class OptionsMenu : GameScreen
    {
        public static event EventHandler Selected;

        public OptionsMenu(Texture2D[] panels, Texture2D[] backgroundAssets, Save save)
            :base(panels, backgroundAssets)
        {
            //This is the starting point of the menu.
            OptionsMenu.Selected += (object sender, EventArgs args) =>
            {
                this.CurrentState = State.Shown;
                this._delay = 0;
            };

            MenuEntry music = new MenuEntry("Music On");
            if (!save.Config.MusicEnabled)
                music.Text = "Music Off";
            MenuEntry voice = new MenuEntry("Voices On");
            if (!save.Config.VoiceEnabled)
                voice.Text = "Voices Off";
            MenuEntry soundeffects = new MenuEntry("Sound Effects On");
            if (!save.Config.SoundEnabled)
                soundeffects.Text = "Sound Effects Off";
            MenuEntry back = new MenuEntry("Back");

            music.Enter += (object sender, EventArgs args) =>
                {
                    save.Config.MusicEnabled = !save.Config.MusicEnabled;
                    if (save.Config.MusicEnabled)
                        music.Text = "Music On";
                    else
                        music.Text = "Music Off"; 
                };

            voice.Enter += (object sender, EventArgs args) =>
            {
                save.Config.VoiceEnabled = !save.Config.VoiceEnabled;
                if (save.Config.VoiceEnabled)
                    voice.Text = "Voices On";
                else
                    voice.Text = "Voices Off";
            };

            soundeffects.Enter += (object sender, EventArgs args) =>
            {
                save.Config.SoundEnabled = !save.Config.SoundEnabled;
                if (save.Config.SoundEnabled)
                    soundeffects.Text = "Sound Effects On";
                else
                    soundeffects.Text = "Sound Effects Off";
            };

            back.Enter += (object sender, EventArgs args) =>
                {
                    this.CurrentState = State.Hidden;
                    MainMenu.SelectedEvent();
                };

            this.MenuEntryList.Add(music);
            this.MenuEntryList.Add(voice);
            this.MenuEntryList.Add(soundeffects);
            this.MenuEntryList.Add(back);
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
