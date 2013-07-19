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

using Paradox.Game.Classes.Cameras;

namespace Paradox.Game.Classes.Ships
{
    public class ShipOverlay
    {
        #region Variables and Properties
        private TextureQuad _textureQuad;
        #endregion

        #region Constructor
        public ShipOverlay(Texture2D image, GraphicsDevice device, float size)
        {
            _textureQuad = new TextureQuad(device, image, size, size);
        }
        #endregion

        #region Draw
        public void OverlayDraw(Camera camera, Vector3 shipPosition)
        {
            _textureQuad.Draw(camera, shipPosition);
        }
        #endregion
    }
}
