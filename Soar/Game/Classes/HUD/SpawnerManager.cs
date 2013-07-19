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

using Paradox.Game.Classes.Inputs;

namespace Paradox.Game.Classes.HUD
{
    public class SpawnerManager
    {
        #region Fields
        //GUI Vars
        private Texture2D[] UIArrows;
        private Texture2D[] UIBars;
        private Texture2D UIOverlay;
        private Texture2D[] UIShipImages;
        private SpriteFont UIFont;

        public event EventHandler Selected;


        private List<ShipProfile> _shipProfiles;
        private int _amtShips;
        public int _selected { get; private set; }
        private byte inputTime;

        private KeyboardInput _keyboardInput;
        private GamePadInput _gamePadInput;
        private GraphicsDevice _device;

        #endregion

        #region Constructor

        public SpawnerManager(GraphicsDevice device, List<ShipProfile> shipProfiles, SpriteFont font, Texture2D[] arrows, Texture2D[] bars, Texture2D overlay, Texture2D[] shipImages, int amtShips)
        {
            _device = device;
            _shipProfiles = shipProfiles;
            UIFont = font;
            UIArrows = arrows;
            UIBars = bars;
            UIOverlay = overlay;
            UIShipImages = shipImages;
            _amtShips = amtShips;

            _selected = 0;
            _keyboardInput = new KeyboardInput();
            _gamePadInput = new GamePadInput();
        }

        #endregion

        #region Update

        public void UpdateManager()
        {
#if XBOX
            List<InputActionMenu> InputActionsMenu = _keyboardInput.GetKeyboardMenuActions();
#else
            List<InputActionMenu> InputActionsMenu = _gamePadInput.GetGamePadMenuActions();
#endif
            //Input time makes sure no other input occurs for 10 frames. This is because the IsKeyDown() method might register a keystroke twice.
            if (inputTime > 0)
                inputTime--;

            foreach (InputActionMenu IAM in InputActionsMenu)
            {
                if (inputTime <= 0)
                {
                    switch (IAM)
                    {
                        case InputActionMenu.Left:
                            MenuLeft();
                            break;
                        case InputActionMenu.Right:
                            MenuRight();
                            break;
                        case InputActionMenu.Select | InputActionMenu.Enter:
                            SelectedEvent();
                            break;
                    }
                }
            }
        }

        #endregion

        #region Draw

        public void DrawSpawner(SpriteBatch spriteBatch)
        {
            int overlayY = _device.Viewport.Height / 2 - 250;
            spriteBatch.Begin();
            //Draw Overlay
            spriteBatch.Draw(UIOverlay, new Vector2(0, overlayY), Color.White);
            //Draw Ship Image
            spriteBatch.Draw(UIShipImages[_selected], new Vector2(44, overlayY + 30), Color.White);
            
            //Ship Health
            spriteBatch.DrawString(UIFont, "Health", new Vector2(5, overlayY + 10 + 175), Color.White);
            for (int i = 0; i < _shipProfiles[_selected].Health; i++)
                spriteBatch.Draw(UIBars[0], new Vector2(120 + i * 18, overlayY + 10 + 175), Color.White);

            //Ship Weapons
            spriteBatch.DrawString(UIFont, "Weapons", new Vector2(5, overlayY + 10 + 175 + 43), Color.White);
            for (int i = 0; i < _shipProfiles[_selected].Weapons; i++)
                spriteBatch.Draw(UIBars[1], new Vector2(120 + i * 18, overlayY + 10 + 175 + 43), Color.White);

            //Ship Speed
            spriteBatch.DrawString(UIFont, "Speed", new Vector2(5, overlayY + 10 + 175 + 43 * 2), Color.White);
            for (int i = 0; i < _shipProfiles[_selected].Speed; i++)
                spriteBatch.Draw(UIBars[1], new Vector2(120 + i * 18, overlayY + 10 + 175 + 43 * 2), Color.White);

            //Ship Agility
            spriteBatch.DrawString(UIFont, "Agility", new Vector2(5, overlayY + 10 + 175 + 43  * 3), Color.White);
            for (int i = 0; i < _shipProfiles[_selected].Agility; i++)
                spriteBatch.Draw(UIBars[1], new Vector2(120 + i * 18, overlayY + 10 + 175 + 43 * 3), Color.White);

            //Left Button
            if (_selected == 0)
            {
                spriteBatch.Draw(UIArrows[0], new Vector2(29, overlayY + 412), Color.White);
            }
            else
            {
                spriteBatch.Draw(UIArrows[1], new Vector2(29, overlayY + 412), Color.White);
            }
            
            //Right Button
            if (_selected == _amtShips - 1)
            {
                spriteBatch.Draw(UIArrows[2], new Vector2(29 + 75 + 29, overlayY + 412), Color.White);
            }
            else
            {
                spriteBatch.Draw(UIArrows[3], new Vector2(29 + 75 + 29, overlayY + 412), Color.White);
            }

            spriteBatch.End();
        }

        #endregion

        #region Helper Methods

        void MenuLeft()
        {
            inputTime = 10;
            if (_selected > 0)
            {
                _selected--;
            }
        }

        void MenuRight()
        {
            inputTime = 10;
            if (_selected < _amtShips - 1)
            {
                _selected++;
            }
        }

        void SelectedEvent()
        {
            if (Selected != null)
                Selected(this, EventArgs.Empty); //Null is passed, there is no object.
        }
        #endregion
    }
}
