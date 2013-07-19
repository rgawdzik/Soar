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

namespace Paradox.Game.Classes.Engines
{
    public class ExplosionEngine
    {
        #region Properties and Variables

        private TextureQuad _textureQuad;
        private TextureQuad _smallTextureQuad;
        private List<Explosion> _explosionList;
        private int _maxIndex;
        private GraphicsDevice _device;

        #endregion

        #region Constructors

        public ExplosionEngine(GraphicsDevice device, Texture2D[] explosionArray, int size, int smallSize)
        {
            _textureQuad = new TextureQuad(device, explosionArray, size, size);
            _smallTextureQuad = new TextureQuad(device, explosionArray, smallSize, smallSize);
            _explosionList = new List<Explosion>();
            _maxIndex = explosionArray.Count();
            _device = device;
        }

        #endregion

        #region Update

        public void Update()
        {
            for (int i = 0; i < _explosionList.Count(); i++)
            {
                _explosionList[i].Index++; //Increments the explosion texture variable

                if (_explosionList[i].Index >= _maxIndex) //Checks if the index is over the max index of the 
                {
                    _explosionList.Remove(_explosionList[i]);
                }
            }
        }

        #endregion

        #region Draw

        public void Draw(Camera camera)
        {
            //Sets the CullMode to none and blendstate to additive (Explosions have a black background)
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            _device.RasterizerState = rs;
            _device.BlendState = BlendState.Additive;

            foreach (Explosion explosion in _explosionList)
            {
                switch (explosion.ExplosionType)
                {
                    case ExplosionType.Big:
                        _textureQuad.SetTexture(explosion.Index);
                        _textureQuad.Draw(camera, explosion.Position);
                        break;
                    case ExplosionType.Small:
                        _smallTextureQuad.SetTexture(explosion.Index);
                        _smallTextureQuad.Draw(camera, explosion.Position);
                        break;
                }
            }

            //Resets the state of the BlendState and Rasterizer State
            RasterizerState rs2 = new RasterizerState();
            rs2.CullMode = CullMode.CullCounterClockwiseFace;
            _device.RasterizerState = rs2;
            _device.BlendState = BlendState.NonPremultiplied;
        }

        #endregion

        #region Accessor Methods

        public void AddToList(List<Explosion> ExplosionsToAdd)
        {
            _explosionList.AddRange(ExplosionsToAdd);
        }

        public void AddToList(Explosion explosionToAdd)
        {
            _explosionList.Add(explosionToAdd);
        }

        #endregion
    }
}
