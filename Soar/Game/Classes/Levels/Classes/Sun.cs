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

namespace Paradox.Game.Classes.Levels
{
    public class Sun
    {
        private TextureQuad _textureQuad;
        public Vector3 Position;
        private GraphicsDevice _device;

        public Sun(GraphicsDevice device, Texture2D sunTexture, Vector3 position, int size)
        {
            _textureQuad = new TextureQuad(device, sunTexture, size, size);
            Position = position;
            _device = device;
        }

        public void Draw(Camera camera)
        {
            //This sets the BlendState to Additive, which allows blending to occur between backgrounds.
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            _device.RasterizerState = rs;
            _device.BlendState = BlendState.Additive;

            _textureQuad.Draw(camera, Position);

            RasterizerState rs2 = new RasterizerState();
            rs2.CullMode = CullMode.CullCounterClockwiseFace;
            _device.RasterizerState = rs2;
            _device.BlendState = BlendState.NonPremultiplied;
        }
    }
}
