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
    public class Skybox
    {
        #region Properties and Variables
        private Model _skyboxModel;
        private GraphicsDevice _device;
        private Texture2D[] _skyboxWalls;

        #endregion

        #region Constructor
        public Skybox(GraphicsDevice device, Texture2D[] skyboxWalls, Model skyboxModel)
        {
            _device = device;
            _skyboxModel = skyboxModel;
            _skyboxWalls = skyboxWalls;
        }
        #endregion

        #region Draw
        public void DrawSkybox(Camera camera, Vector3 shipPosition)
        {
            //A TextureAddressMode.Clamp state removes the seams between the cube.
            SamplerState ss = new SamplerState();
            ss.AddressU = TextureAddressMode.Clamp;
            ss.AddressV = TextureAddressMode.Clamp;
            _device.SamplerStates[0] = ss;

            //Removes the ZBuffer so no size can be set for the skybox.
            DepthStencilState dss = new DepthStencilState();
            dss.DepthBufferEnable = false;
            _device.DepthStencilState = dss;

            Matrix[] transforms = new Matrix[_skyboxModel.Bones.Count]; //Represents the position of each model bone.
            _skyboxModel.CopyAbsoluteBoneTransformsTo(transforms); //Models have a method that populates an array.

            foreach (ModelMesh mesh in _skyboxModel.Meshes)
            {
                //BasicEffect is a simplified version of it's parent class Effect.  Effects allow objects to be placed on screen.
                foreach (BasicEffect basicEffect in mesh.Effects)
                {
                    basicEffect.Projection = camera.Projection;
                    basicEffect.View = camera.View;
                    basicEffect.World =  Matrix.CreateScale(80) * mesh.ParentBone.Transform * Matrix.CreateTranslation(shipPosition); //Positions the mesh in the correct place relavent to the world.
                   
                }
                mesh.Draw();
            }

            //Reenabling the ZBuffer.
            dss = new DepthStencilState();
            dss.DepthBufferEnable = true;
            _device.DepthStencilState = dss;
        }
        #endregion
    }
}
