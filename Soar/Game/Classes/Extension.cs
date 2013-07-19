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

namespace Paradox.Game.Classes
{
    #region TextureQuad
    //Extends the XNA Framework, provides a Point Sprite implementation without the use of HLSL Effects.
    public class TextureQuad
    {
        private static readonly Vector2 UpperLeft = new Vector2(0, 0);
        private static readonly Vector2 UpperRight = new Vector2(1, 0);
        private static readonly Vector2 BottomLeft = new Vector2(0, 1);
        private static readonly Vector2 BottomRight = new Vector2(1, 1);

        private VertexBuffer vertexBuffer;
        private BasicEffect _effect;

        private GraphicsDevice Device;

        private Texture2D[] _textureArray;

        public TextureQuad(GraphicsDevice graphicsDevice, Texture2D texture, float width, float height)
        {
            VertexPositionTexture[] vertices = CreateQuadVertices(width, height);
            vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionTexture), vertices.Length, BufferUsage.None);
            vertexBuffer.SetData(vertices);

            _effect = new BasicEffect(graphicsDevice) { TextureEnabled = true, Texture = texture };
            Device = graphicsDevice;
        }

        public TextureQuad(GraphicsDevice graphicsDevice, Texture2D[] textureArray, float width, float height)
        {
            VertexPositionTexture[] vertices = CreateQuadVertices(width, height);
            vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionTexture), vertices.Length, BufferUsage.None);
            vertexBuffer.SetData(vertices);

            _effect = new BasicEffect(graphicsDevice) { TextureEnabled = true, Texture = textureArray[0] };
            Device = graphicsDevice;
            _textureArray = textureArray;
        }

        public void SetTexture(byte index)
        {
            _effect.Texture = _textureArray[index];

        }

        private static VertexPositionTexture[] CreateQuadVertices(float width, float height)
        {
            float halfWidth = width / 2;
            float halfHeight = height / 2;

            VertexPositionTexture[] vertices = new VertexPositionTexture[4];

            vertices[0] = new VertexPositionTexture(new Vector3(-halfWidth, halfHeight, 0), UpperLeft);
            vertices[1] = new VertexPositionTexture(new Vector3(halfWidth, halfHeight, 0), UpperRight);
            vertices[2] = new VertexPositionTexture(new Vector3(-halfWidth, -halfHeight, 0), BottomLeft);
            vertices[3] = new VertexPositionTexture(new Vector3(halfWidth, -halfHeight, 0), BottomRight);

            return vertices;
        }

        public void ResizeTextureQuad(float width, float height)
        {
            VertexPositionTexture[] vertices = CreateQuadVertices(width, height);
            vertexBuffer = new VertexBuffer(Device, typeof(VertexPositionTexture), vertices.Length, BufferUsage.None);
            vertexBuffer.SetData(vertices);
        }

        /// <summary>
        /// Draws a billboard that rotates around a certain axis.
        /// </summary>

        public void Draw(Camera camera, Vector3 objectPosition, Quaternion rotateAxis)
        {
            _effect.GraphicsDevice.SetVertexBuffer(vertexBuffer);
            _effect.World = Matrix.CreateConstrainedBillboard(objectPosition, camera.CameraPosition, Vector3.Transform(Vector3.Zero, rotateAxis), null, null);
            //_effect.World = Matrix.CreateConstrainedBillboard(objectPosition, camera.CameraPosition, Vector3.Transform(Vector3.Zero, rotateAxis), Vector3.Transform(Vector3.Zero, camera.CameraRotation), Vector3.Transform(Vector3.Zero, rotateAxis));
            _effect.View = camera.View;
            _effect.Projection = camera.Projection;

            foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _effect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
            }
        }

        public void Draw(Camera camera, Vector3 objectPosition)
        {
            _effect.GraphicsDevice.SetVertexBuffer(vertexBuffer);
            _effect.World = Matrix.CreateBillboard(objectPosition, camera.CameraPosition, camera.CameraUp, Vector3.Transform(Vector3.Zero, camera.CameraRotation));

            _effect.View = camera.View;
            _effect.Projection = camera.Projection;

            foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _effect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
            }
        }

        public float Alpha
        {
            get { return _effect.Alpha; }
            set { _effect.Alpha = value; }
        }
    }
    public class Extension
    {
        public static float DistanceVector3(Vector3 v1, Vector3 v2)
        {
            return (float)Math.Sqrt(Math.Abs(Math.Pow(v1.X - v2.X, 2)) + Math.Abs(Math.Pow(v1.Y - v2.Y, 2)) + Math.Abs(Math.Pow(v1.Z - v2.Z, 2)));
        }
    }

    #endregion
}


