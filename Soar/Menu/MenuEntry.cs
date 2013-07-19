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

namespace Paradox.Menu
{
    public class MenuEntry
    {
        #region Fields
        public event EventHandler Enter;
        public string Text { get; set; }
        public bool Hovered { get; set; }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="text">Text of the Menu Entry</param>
        public MenuEntry(string text)
        {
            Text = text;
        }

        /// <summary>
        /// The Event starting point.
        /// </summary>
        public void EnterEvent()
        {
            if (Enter != null)
                Enter(this, EventArgs.Empty);
        }

    }
}
