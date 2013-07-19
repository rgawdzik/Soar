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
using Microsoft.Xna.Framework.Content;

//Game Specific Imports:
using Paradox.Game.Classes.Cameras;

namespace Paradox.Game.Classes
{
    /* This class acts as the base for 3D.  It is not meant for anything other than drawing a model, and getting updates with a new World or Rotation.
     * 
     */
    public class BasicModel
    {
        #region Properties and Variables
        public Model ActualModel { get; protected set; }
        public Matrix World { get; set; }
        public Quaternion Rotation { get; set; }

        public float Scale { get; protected set; }

        public Vector3 Sun = new Vector3(-300, 1300, 1300);

        #endregion

        #region Constructor

        public BasicModel(Model model, float scale)
        {
            World = Matrix.Identity;
            Rotation = Quaternion.Identity;
            Scale = scale;

            ActualModel = model;
        }

        #endregion

        #region Update
        /* To draw a 3D model on screen, you have to specify a world (location of the model), a projection matrix (for the camera), and a view matrix (for the camera)
         * view = camera position, direction, and orientation
         * projection = camera angle, field of view, and 3D to 2D screen
         */

        //This method handles the instructions of what to do with the model.
        public void UpdateModel(Matrix world)
        {
            World = world;
        }
        #endregion

        #region Draw
        //This method draws the model
        public void DrawModel(Camera camera)
        {
            Matrix[] transforms = new Matrix[ActualModel.Bones.Count]; //Represents the position of each model bone.
            ActualModel.CopyAbsoluteBoneTransformsTo(transforms); //Models have a method that populates an array.

            foreach (ModelMesh mesh in ActualModel.Meshes)
            {
                //BasicEffect is a simplified version of it's parent class Effect.  Effects allow objects to be placed on screen.
                foreach (BasicEffect basicEffect in mesh.Effects)
                {
                    basicEffect.EnableDefaultLighting();
                    basicEffect.Projection = camera.Projection;
                    basicEffect.View = camera.View;
                    //Note, fix up the lighting in here, it getting too dark!

                    basicEffect.EmissiveColor = new Vector3(0.7f);
                    
                    //End Note.
                    
                    basicEffect.World = Matrix.CreateScale(Scale) * World * mesh.ParentBone.Transform; //Positions the mesh in the correct place relavent to the world.
                }
                mesh.Draw();
                
            }
        }

        #endregion
    }
}
