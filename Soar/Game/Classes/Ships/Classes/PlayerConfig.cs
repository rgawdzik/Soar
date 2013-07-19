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

namespace Paradox.Game.Classes.Ships
{
    public class PlayerConfig
    {
        public BasicModel ShipModel { get; set; }
        public int Life { get; set; }
        public Vector3 CameraFollowPosition { get; set; }
        public WeaponSystem2D WeaponSystem2D;
        public MissleSystem2D MissleSystem2D;
        public int AmountMissles;
        public float MaxShipSpeed;
        public float MinShipSpeed;
        public float Acceleration;
        public float TurningSpeed;
        public float MaxTurningSpeed;
        public float MinTurningSpeed;

        public PlayerConfig(BasicModel model, int life, Vector3 cameraPosition, WeaponSystem2D wS, MissleSystem2D mS, int amtMissles, float maxSS, float minSS, float accl, float tS, float maxTS, float minTS)
        {
            ShipModel = model;
            Life = life;
            CameraFollowPosition = cameraPosition;
            WeaponSystem2D = wS;
            MissleSystem2D = mS;
            AmountMissles = amtMissles;
            MaxShipSpeed = maxSS;
            MinShipSpeed = minSS;
            Acceleration = accl;
            TurningSpeed = tS;
            MaxTurningSpeed = maxTS;
            MinTurningSpeed = minTS;
        }
    }
}
