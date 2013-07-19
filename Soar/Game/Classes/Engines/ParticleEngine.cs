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
    public class ParticleEngine
    {
        TextureQuad _textureQuad;
        GraphicsDevice _device;
        List<Particle> _particleList;
        Random _randVar;

        int amtTextures;

        public ParticleEngine(GraphicsDevice device, int maxParticles, Vector3 Plypos, Quaternion Plyrotation, Texture2D[] textureArray, float width)
        {
            _device = device;
            _particleList = new List<Particle>();
            _randVar = new Random();

            amtTextures = textureArray.Length;
            _textureQuad = new TextureQuad(device, textureArray, width, width);

            for (int i = 0; i < maxParticles; i++)
            {
                _particleList.Add(new Particle(Plypos, Plyrotation, (float)_randVar.NextDouble()));
            }
        }

        public void Update(Camera camera, Vector3 playerPos, Quaternion rotation)
        {
            foreach (Particle particle in _particleList)
            {
                if (particle.Index >= amtTextures - 1)
                {
                    particle.Reset(playerPos, rotation);
                }
                particle.Update();

                particle.Distance = Vector3.Distance(camera.CameraPosition, particle.Position);
            }
            SortByDistance();
        }

        public void Draw(Camera camera)
        {
            //Sets the CullMode to none and blendstate to additive (Explosions have a black background)
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            _device.RasterizerState = rs;
            _device.BlendState = BlendState.Additive;

            foreach (Particle particle in _particleList)
            {
                        _textureQuad.SetTexture(particle.Index);
                        _textureQuad.Draw(camera, particle.Position);
            }

            //Resets the state of the BlendState and Rasterizer State
            RasterizerState rs2 = new RasterizerState();
            rs2.CullMode = CullMode.CullCounterClockwiseFace;
            _device.RasterizerState = rs2;
            _device.BlendState = BlendState.NonPremultiplied;
        }

        public void SortByDistance()
        {
            bool unsorted = true;

            while (unsorted)
            {
                unsorted = false;
                for (int i = 0; i < _particleList.Count - 1; i++)
                {
                    if (_particleList[i].Distance < _particleList[i + 1].Distance)
                    {
                        unsorted = true;
                        Particle tempParticle = _particleList[i];
                        _particleList[i] = _particleList[i + 1];
                        _particleList[i + 1] = tempParticle;
                    }
                }
            }
        }
    }
}
