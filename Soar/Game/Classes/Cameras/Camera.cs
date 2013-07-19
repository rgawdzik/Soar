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

using Paradox.Game.Classes.Ships;

namespace Paradox.Game.Classes.Cameras
{
    public class Camera
    {
        #region Class-Level Properties and Variables

        //Spectator Variables
        private const int SpectatorChange = 600;
        private int _spectatorTime;
        private int _followShip;
        private int _cameraSpeed;

        //Camera Matrices
        public Matrix View { get; set; }
        public Matrix Projection { get; set; }
        public Quaternion CameraRotation { get; set; } //For a Cockpit View, CameraRotation is not used.

        /* To create a view matrix, you need to have the following properties:
         * cameraPosition = position of the camera
         * cameraTarget = coordinate of what the camera is looking at.
         * cameraUpVector = Vector indicating which direction is up.
         */

        public Vector3 CameraPosition;
        public Vector3 CameraDirection { get; set; }
        public Vector3 CameraUp { get; set; }

        public Vector3 CameraFollowPosition { get; set; }

        private Vector3 _defaultCameraUp;
        private Random _randomVar;

        public float CameraShakeIntensity;

        #endregion

        #region Constructors

        public Camera(GraphicsDevice device, Vector3 cameraFollowPosition)
        {
            CameraRotation = Quaternion.Identity;
            View = Matrix.Identity;


            DefaultProjectionMatrix(device); //Sets the Projection Matrix to the default params.
            CameraFollowPosition = cameraFollowPosition;

            SetDefaultCameraUp();
            CameraUp = _defaultCameraUp;

            _randomVar = new Random();
            CameraShakeIntensity = 0f;
        }

        

        public Camera(GraphicsDevice device, Vector3 cameraFollowPosition, Vector3 defaultCameraUp)
        {
            CameraRotation = Quaternion.Identity;
            View = Matrix.Identity;

            DefaultProjectionMatrix(device); //Sets the Projection Matrix to the default params.
            CameraFollowPosition = cameraFollowPosition;

            _defaultCameraUp = defaultCameraUp;

            _randomVar = new Random();
            CameraShakeIntensity = 0;
        }

        

        public Camera(float FieldOfView, float AspectRatio, float NearPlaneDistance, float FarPlaneDistance, Vector3 cameraFollowPosition, Vector3 defaultCameraUp)
        {
            CameraRotation = Quaternion.Identity;
            View = Matrix.Identity;

            Projection = Matrix.CreatePerspectiveFieldOfView(FieldOfView, AspectRatio, NearPlaneDistance, FarPlaneDistance);
            CameraFollowPosition = cameraFollowPosition;

            _defaultCameraUp = defaultCameraUp;
        }

        #endregion

        #region Update Object Method(s)

        //This Update tells the Camera to follow an object, a chase view camera update.
        public void Update(Vector3 objectPosition, Quaternion objectRotation)
        {
            
            CameraRotation = Quaternion.Lerp(CameraRotation, objectRotation, 0.12f); //Adds Camera Delay.

            CameraPosition = CameraFollowPosition; //Resets the CameraPosition to default values.
            CameraPosition = Vector3.Transform(CameraPosition, Matrix.CreateFromQuaternion(CameraRotation)); //Transforms the Camera Position according to the object rotation and the previous camera rotation (delay)

            CameraPosition += objectPosition; //Repositions the camera to the object position.

            CameraUp = _defaultCameraUp;
            CameraUp = Vector3.Transform(CameraUp, Matrix.CreateFromQuaternion(CameraRotation)); //Modifies the Up Vector according to the Camera Rotation.

             
            View = Matrix.CreateLookAt(CameraPosition, CameraShakeProjection(objectPosition, CameraShakeIntensity), CameraUp);
        }

        //This Update tells the Camera that it is in Cockpit view.
        public void Update(Quaternion cameraRotation, Vector3 objectPosition) //The Camera Rotation is provided, most likely by Mouse Input Params.  This allows the camera to point to any area as the player pleases.
        {
            CameraRotation = cameraRotation;
            CameraPosition = CameraFollowPosition;

            CameraPosition = Vector3.Transform(CameraPosition, Matrix.CreateFromQuaternion(CameraRotation)); //Transforms the Camera Position according to the Camera Rotation.

            CameraPosition += objectPosition; //Repositions the camera to the object position.

            CameraUp = _defaultCameraUp;
            CameraUp = Vector3.Transform(CameraUp, Matrix.CreateFromQuaternion(CameraRotation)); //Modifies the Up Vector according to the Camera Rotation.

            View = Matrix.CreateLookAt(CameraPosition, objectPosition, CameraUp);

        }

        /// <summary>
        /// Creates a camera that Randomly spectates different Friends.
        /// </summary>
        /// <param name="EnemyList"></param>
        public void SpectatorUpdate(List<Friend> FriendList)
        {

            _spectatorTime++;
            //The time to start spectating another ship has occured.
            if (_spectatorTime >= SpectatorChange)
            {
                _followShip = _randomVar.Next(FriendList.Count);
                _cameraSpeed += _randomVar.Next(-20, 20);
                _spectatorTime = 0;
            }

            //The Ship has been destroyed, let's get another one to follow.
            if (_followShip >= FriendList.Count)
            {
                _followShip = _randomVar.Next(FriendList.Count);
            }

            CameraRotation = FollowShip(FriendList[_followShip].ShipPosition, 0.2f);

            CameraPosition = CameraFollowPosition; //Resets the CameraPosition to default values.
            CameraPosition = Vector3.Transform(CameraPosition, Matrix.CreateFromQuaternion(CameraRotation)); //Transforms the Camera Position according to the object rotation and the previous camera rotation (delay)

            CameraPosition += FriendList[_followShip].ShipPosition;//Repositions the camera to the object position.
            //CameraPosition  = Vector3.Lerp(CameraPosition, CameraPosition + FriendList[_followShip].ShipPosition, 0.2f) ;
            //Moves the camera in a random speed.
            CameraPosition.X += _cameraSpeed;
            CameraPosition.Y += _cameraSpeed;
            CameraPosition.Z += _cameraSpeed;

            //Increments the camera speed.
            _cameraSpeed += _cameraSpeed;

            CameraUp = _defaultCameraUp;
            CameraUp = Vector3.Transform(CameraUp, Matrix.CreateFromQuaternion(CameraRotation)); //Modifies the Up Vector according to the Camera Rotation.


            View = Matrix.CreateLookAt(CameraPosition, FriendList[_followShip].ShipPosition, CameraUp);

        }

        #endregion

        #region Helper Methods

        private void DefaultProjectionMatrix(GraphicsDevice device)
        {
            //Sets the Projection Matrix to the default params.
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, device.Viewport.AspectRatio, 0.2f, 3300f); //Previously 2400f
        }

        private void SetCameraFollowPosition(Vector3 cameraFollowPosition)
        {
            CameraFollowPosition = cameraFollowPosition;
        }

        public Quaternion FollowShip(Vector3 position, float lerpValue)
        {
            Vector3 desiredDirection = Vector3.Normalize(CameraPosition - position);
            Quaternion ShipDirection = Quaternion.CreateFromRotationMatrix(Matrix.CreateWorld(CameraPosition, desiredDirection, CameraUp));
            return Quaternion.Lerp(CameraRotation, ShipDirection, lerpValue);
        }

        private void SetDefaultCameraUp()
        {
            _defaultCameraUp = new Vector3(0, 1, 0);
        }

        private Vector3 CameraShakeProjection(Vector3 objectFollowPosition, float intesity)
        {
            return new Vector3((float)Math.Sin(_randomVar.Next(-30, 30) * intesity) + objectFollowPosition.X, (float)Math.Sin(_randomVar.Next(-30, 30) * intesity) + objectFollowPosition.Y, 
                objectFollowPosition.Z);
            //The Z axis should not even be a variable.
        }

        #endregion
    }
}
