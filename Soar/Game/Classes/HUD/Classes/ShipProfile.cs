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

namespace Paradox.Game.Classes.HUD
{
    public class ShipProfile
    {
        public byte Health { get; protected set; }
        public byte Weapons { get; protected set; }
        public byte Speed { get; protected set; }
        public byte Agility { get; protected set; }

        public ShipProfile(byte health, byte weapons, byte speed, byte agility)
        {
            Health = health;
            Weapons = weapons;
            Speed = speed;
            Agility = agility;
        }

    }
}
