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
using Microsoft.Xna.Framework.Input;

namespace Paradox.Game.Classes.Inputs
{
    class GamePadInput
    {
        #region Properties and Variables
        private List<InputAction> _inputList { get; set; }
        private List<InputActionMenu> _inputListMenu { get; set; }
        public Vector2 LeftThumbStick { get; private set; }
        public Vector2 RightThumbStick { get; private set; }

        public GamePadInput()
        {
            _inputList = new List<InputAction>();
            _inputListMenu = new List<InputActionMenu>();
        }
        #endregion

        #region Keyboard Method

        public List<InputAction> GetGamePadActions() //This method returns a List that contains the player actions.
        {
            _inputList.Clear();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

            LeftThumbStick = gamePadState.ThumbSticks.Left;
            RightThumbStick = gamePadState.ThumbSticks.Right;

            if (gamePadState.ThumbSticks.Left.X < 0f)
                _inputList.Add(InputAction.Left);

            if (gamePadState.ThumbSticks.Left.X > 0f)
                _inputList.Add(InputAction.Right);

            if (gamePadState.ThumbSticks.Left.Y > 0f)
                _inputList.Add(InputAction.Up);

            if (gamePadState.ThumbSticks.Left.Y < 0f)
                _inputList.Add(InputAction.Down);

            if (gamePadState.ThumbSticks.Right.X < 0f)
                _inputList.Add(InputAction.LinearLeft);

            if (gamePadState.ThumbSticks.Right.X > 0f)
                _inputList.Add(InputAction.LinearRight);

            if (gamePadState.Triggers.Right > 0f)
                _inputList.Add(InputAction.Fire1);

            if (gamePadState.Triggers.Left > 0f)
                _inputList.Add(InputAction.Fire2);

            if (gamePadState.Buttons.Start == ButtonState.Pressed || gamePadState.Buttons.Back == ButtonState.Pressed)
                _inputList.Add(InputAction.Pause);

            if (gamePadState.Buttons.RightShoulder == ButtonState.Pressed)
                _inputList.Add(InputAction.IncreaseSpeed);

            if (gamePadState.Buttons.LeftShoulder == ButtonState.Pressed)
                _inputList.Add(InputAction.DecreaseSpeed);

            if (gamePadState.DPad.Left == ButtonState.Pressed)
                _inputList.Add(InputAction.DecreaseTurningSpeed);
            if (gamePadState.DPad.Right == ButtonState.Pressed)
                _inputList.Add(InputAction.IncreaseTurningSpeed);

            

            return _inputList;
        }

        /// <summary>
        /// Gets a List of Keyboard Menu Actions
        /// </summary>
        /// <returns>A list of keyboard menu actions</returns>
        public List<InputActionMenu> GetGamePadMenuActions()
        {
            _inputListMenu.Clear();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

            //WASD
            if (gamePadState.ThumbSticks.Left.X < 0f)
                _inputListMenu.Add(InputActionMenu.Left);

            if (gamePadState.ThumbSticks.Left.X > 0f)
                _inputListMenu.Add(InputActionMenu.Right);

            if (gamePadState.ThumbSticks.Left.Y > 0f)
                _inputListMenu.Add(InputActionMenu.Up);

            if (gamePadState.ThumbSticks.Left.Y < 0f)
                _inputListMenu.Add(InputActionMenu.Down);

            //Keys

            if (gamePadState.DPad.Left == ButtonState.Pressed)
                _inputListMenu.Add(InputActionMenu.Left);

            if (gamePadState.DPad.Right == ButtonState.Pressed)
                _inputListMenu.Add(InputActionMenu.Right);

            if (gamePadState.DPad.Up == ButtonState.Pressed)
                _inputListMenu.Add(InputActionMenu.Up);

            if (gamePadState.DPad.Down == ButtonState.Pressed)
                _inputListMenu.Add(InputActionMenu.Down);

            //Enter

            if (gamePadState.Buttons.A == ButtonState.Pressed)
                _inputListMenu.Add(InputActionMenu.Enter);

            //Exit

            if (gamePadState.Buttons.Start == ButtonState.Pressed || gamePadState.Buttons.Back == ButtonState.Pressed)
                _inputListMenu.Add(InputActionMenu.Exit);

            //Select
            if (gamePadState.Buttons.A == ButtonState.Pressed)
                _inputListMenu.Add(InputActionMenu.Select);

            return _inputListMenu;
        }


        #endregion
    }
}
