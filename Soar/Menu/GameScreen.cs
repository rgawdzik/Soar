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
using Paradox.Game.Classes;

namespace Paradox.Menu
{
    public abstract class GameScreen
    {
        #region Enumerations
        /// <summary>
        /// An enumeration representing the available states a menu can be in.
        /// </summary>
        public enum State
        {
            Shown,
            Hidden
        };

        #endregion

        #region Fields

        protected Texture2D[] _panels;
        protected Texture2D[] _backgroundAssets;

        public List<InputActionMenu> InputActionsMenuList;
        public List<MenuEntry> MenuEntryList;
        public State CurrentState { get; set; }
        

        private KeyboardInput _keyboardInput;
        private GamePadInput _gamepadInput;
        protected int _selectedEntry;

        public int _delay;

        private int _moved;
        const int _maxMoved = 15;

        protected float _defaultSpacing;

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public GameScreen(Texture2D[] panels, Texture2D[] backgroundAssets)
        {
            _panels = panels;
            CurrentState = State.Hidden;
            MenuEntryList = new List<MenuEntry>();
            InputActionsMenuList = new List<InputActionMenu>();
            _keyboardInput = new KeyboardInput();
            _defaultSpacing = 0f;
            _backgroundAssets = backgroundAssets;
            _gamepadInput = new GamePadInput();
        }
        #endregion

        #region Update
        /// <summary>
        /// Updates the menu.
        /// </summary>
        public virtual void UpdateMenu()
        {
            if (_delay < 10 && CurrentState == State.Shown)
                _delay++;
            
            InputActionsMenuList.Clear();
            if (CurrentState == State.Shown && _delay >= 10)
            {
#if XBOX
                InputActionsMenuList.AddRange(_keyboardInput.GetKeyboardMenuActions());
#else
                InputActionsMenuList.AddRange(_gamepadInput.GetGamePadMenuActions());
#endif
                CheckMenu();
                //Makes sure the _selectedEntry is shown.
                if (MenuEntryList.Count > 0)
                    MenuEntryList[_selectedEntry].Hovered = true;
            }
        }
        #endregion

        #region Draw

        public virtual void Draw(GraphicsDevice device, SpriteBatch spriteBatch, SpriteFont font)
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

                spriteBatch.End();
            }

        }
        
        #endregion


        #region Helper Methods
        public void CheckMenu() 
        {
            if (_moved > 0)
                _moved--;
            foreach (InputActionMenu action in InputActionsMenuList)
            {
                switch (action)
                {
                    case InputActionMenu.Down:
                        if (_moved <= 0) //This prevents the user skipping two options in a frame.
                        {
                            MenuMoveDown();
                        }
                        break;
                    case InputActionMenu.Up:
                        if (_moved <= 0)
                        {
                            MenuMoveUp();
                        }
                        break;
                    case InputActionMenu.Enter:
                        if (_moved <= 0)
                        {
                            if (MenuEntryList.Count > 0 && _delay >= 10)
                            {
                                MenuEntryList[_selectedEntry].EnterEvent(); //Starts the enter event in the Menu Entry.
                                _moved = _maxMoved;
                            }
                        }
                        break;
                }
            }
        }

        public void MenuMoveDown()
        {
            _moved = _maxMoved;

            MenuEntryList[_selectedEntry].Hovered = false;
            if (_selectedEntry < MenuEntryList.Count() - 1)
            {
                MenuEntryList[++_selectedEntry].Hovered = true;
            }
            else
            {
                _selectedEntry = 0;
                MenuEntryList[0].Hovered = true; //The Menu Entry is reset to 0.
            }
        }

        public void MenuMoveUp()
        {
            _moved = _maxMoved;

            MenuEntryList[_selectedEntry].Hovered = false;
            if (_selectedEntry > 0)
            {
                MenuEntryList[--_selectedEntry].Hovered = true;
            }
            else
            {
                MenuEntryList[MenuEntryList.Count() - 1].Hovered = true; //The Menu Entry is reset to the bottom of the list.
                _selectedEntry = MenuEntryList.Count() - 1;
            }
        }


        public void DrawBackground(SpriteBatch batch, GraphicsDevice device)
        {
            batch.Begin();
            batch.Draw(_backgroundAssets[0], new Vector2(0, device.Viewport.Height - 600), Color.White);

            batch.End();

        }


        #endregion
    }
}
