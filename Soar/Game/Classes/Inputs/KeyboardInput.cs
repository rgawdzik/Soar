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
using Microsoft.Xna.Framework.Input;


namespace Paradox.Game.Classes.Inputs
{
    public class KeyboardInput
    {
        #region Properties and Variables
        private List<InputAction> _inputList { get; set; }
        private List<InputActionMenu> _inputListMenu { get; set; }


        public KeyboardInput()
        {
            _inputList = new List<InputAction>();
            _inputListMenu = new List<InputActionMenu>();
        }
        #endregion

        #region Keyboard Method

        public List<InputAction> GetKeyboardActions() //This method returns a List that contains the player actions.
        {
            _inputList.Clear();

            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.A))
                _inputList.Add(InputAction.Left);

            if (keyboardState.IsKeyDown(Keys.D))
                _inputList.Add(InputAction.Right);

            if (keyboardState.IsKeyDown(Keys.W))
                _inputList.Add(InputAction.Up);

            if (keyboardState.IsKeyDown(Keys.S))
                _inputList.Add(InputAction.Down);

            if (keyboardState.IsKeyDown(Keys.Q))
                _inputList.Add(InputAction.LinearLeft);

            if (keyboardState.IsKeyDown(Keys.E))
                _inputList.Add(InputAction.LinearRight);

            if (keyboardState.IsKeyDown(Keys.Space))
                _inputList.Add(InputAction.Fire1);

            if (keyboardState.IsKeyDown(Keys.LeftAlt | Keys.RightAlt))
                _inputList.Add(InputAction.Fire2);

            if (keyboardState.IsKeyDown(Keys.Escape))
                _inputList.Add(InputAction.Pause);

            if(keyboardState.IsKeyDown(Keys.Up))
                _inputList.Add(InputAction.IncreaseSpeed);
            if(keyboardState.IsKeyDown(Keys.Down))
                _inputList.Add(InputAction.DecreaseSpeed);

            if (keyboardState.IsKeyDown(Keys.Left))
                _inputList.Add(InputAction.DecreaseTurningSpeed);
            if (keyboardState.IsKeyDown(Keys.Right))
                _inputList.Add(InputAction.IncreaseTurningSpeed);

            

            return _inputList;
        }

        /// <summary>
        /// Gets a List of Keyboard Menu Actions
        /// </summary>
        /// <returns>A list of keyboard menu actions</returns>
        public List<InputActionMenu> GetKeyboardMenuActions()
        {
            _inputListMenu.Clear();
            KeyboardState keyboardState = Keyboard.GetState();

            //WASD
            if (keyboardState.IsKeyDown(Keys.A))
                _inputListMenu.Add(InputActionMenu.Left);

            if (keyboardState.IsKeyDown(Keys.D))
                _inputListMenu.Add(InputActionMenu.Right);

            if (keyboardState.IsKeyDown(Keys.W))
                _inputListMenu.Add(InputActionMenu.Up);

            if (keyboardState.IsKeyDown(Keys.S))
                _inputListMenu.Add(InputActionMenu.Down);

            //Keys

            if (keyboardState.IsKeyDown(Keys.Left))
                _inputListMenu.Add(InputActionMenu.Left);

            if (keyboardState.IsKeyDown(Keys.Right))
                _inputListMenu.Add(InputActionMenu.Right);

            if (keyboardState.IsKeyDown(Keys.Up))
                _inputListMenu.Add(InputActionMenu.Up);

            if (keyboardState.IsKeyDown(Keys.Down))
                _inputListMenu.Add(InputActionMenu.Down);

            //Enter

            if (keyboardState.IsKeyDown(Keys.Space))
                _inputListMenu.Add(InputActionMenu.Enter);

            if (keyboardState.IsKeyDown(Keys.Enter))
                _inputListMenu.Add(InputActionMenu.Enter);

            //Exit

            if (keyboardState.IsKeyDown(Keys.Escape))
                _inputListMenu.Add(InputActionMenu.Exit);

            //Select
            if(keyboardState.IsKeyDown(Keys.F))
                _inputListMenu.Add(InputActionMenu.Select);

            return _inputListMenu;
        }


        #endregion
    }
}
