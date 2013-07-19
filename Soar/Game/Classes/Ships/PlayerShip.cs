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
using Paradox.Game.Classes.Inputs;
using Paradox.Game.Classes.Engines;
using Paradox.Game.Classes.HUD;

namespace Paradox.Game.Classes.Ships
{
    public class PlayerShip : Collidable
    {
        #region Variables and Properties
        List<Collidable> CollidableReference;


        public static event EventHandler Pause;

        private BasicModel _shipModel { get; set; }

        private Vector3 _dockingPosition { get; set; }

        private WeaponSystem2D _weaponSystem2D;
        private MissleSystem2D _missleSystem2D;

        public Vector3 ShipPosition;

        public int Life { get; set; }

        public Matrix ShipWorld;
        private Quaternion _shipRotation;

        private GraphicsDevice _device;

        public Camera Camera { get; set; }
        private KeyboardInput _keyboard { get; set; }
        private GamePadInput _gamePadInput { get; set; }

        public float _shipSpeed { get; private set; }
        private float _maxShipSpeed;
        private float _minShipSpeed;
        private float _acceleration;

        private float _turningSpeed;
        private float _maxTurningSpeed;
        private float _minTurningSpeed;

        private bool _docked;

        private List<Sound> _soundDump;

        private PlayerConfig[] _playerConfigurations;

        public event EventHandler LeftBattlefield;
        public bool LeftBattle;


        #endregion

        #region Constructor

        public PlayerShip(List<Collidable> collidableRef, CollidableType type, float boundingRadius, Model model, Vector3 shipPosition, GraphicsDevice device, Vector3 cameraFollowPosition, float scale, float minShipSpeed, float maxShipSpeed, float acceleration, float turningSpeed,
            float maxTurningSpeed, float minTurningSpeed, WeaponSystem2D weaponSystem2D, MissleSystem2D missleSystem2D, SpawnerManager spawnerManager, PlayerConfig[] playerConfigurations)
        {
            CollidableReference = collidableRef;

            _shipModel = new BasicModel(model, scale);

            ShipWorld = Matrix.CreateTranslation(shipPosition) * Matrix.CreateScale(scale);
            _dockingPosition = shipPosition;
            ShipPosition = shipPosition;

            _shipRotation = Quaternion.Identity;

            Camera = new Camera(device, cameraFollowPosition);

            _keyboard = new KeyboardInput();

            _minShipSpeed = minShipSpeed;
            _maxShipSpeed = maxShipSpeed;
            _acceleration = acceleration;
            _turningSpeed = turningSpeed;
            _gamePadInput = new GamePadInput();

            _weaponSystem2D = weaponSystem2D;
            _device = device;
            _shipRotation.Normalize();

            //Collidable Properties
            this.CollidableType = type;
            this.BoundingSphereRadius = boundingRadius;

            _maxTurningSpeed = maxTurningSpeed;
            _minTurningSpeed = minTurningSpeed;

            Life = 0;

            _soundDump = new List<Sound>();

            spawnerManager.Selected += (object sender, EventArgs args) =>
                {
                    SpawnerManager SM = sender as SpawnerManager;
                    Spawn(SM._selected);
                };

            _missleSystem2D = missleSystem2D;

            _playerConfigurations = playerConfigurations;
        }

        #endregion

        #region Update
        public void UpdateShip(List<Sound> soundDump, List<Friend> friendList, List<Enemy> enemyList, List<Explosion> explosionList, GameTime gameTime)
        {
            List<Collidable> collidableList = new List<Collidable>();
            soundDump.AddRange(_soundDump);
            _soundDump.Clear();
            if (Life > 0)
            {

#if XBOX
                //Updates all objects present in the PlayerShip class.
                MoveShip(_keyboard.GetKeyboardActions(), gameTime);
#else
                MoveShipGamePad(_gamePadInput.GetGamePadActions(), gameTime);
#endif
                _shipModel.UpdateModel(ShipWorld);
                Camera.Update(ShipPosition, _shipRotation);
                //_shipFlameParticleEngine.UpdateParticles(ShipPosition);
            }
            else
            {
                Camera.SpectatorUpdate(friendList);
            }
            //Sets the position of the ship to the bounding sphere center, so collision can be checked.
            this.BoundingSphereCenter = ShipPosition;
            _weaponSystem2D.UpdateWeapons(_soundDump, explosionList, gameTime);
            _missleSystem2D.UpdateWeapons(ShipPosition, enemyList, soundDump, explosionList, gameTime);
            CheckPlayerBounds();
        }
        #endregion

        #region Draw
        public void DrawShip()
        {
            //Draws all needed objects to be drawn in the PlayerShip class.
            _shipModel.DrawModel(Camera);
            _weaponSystem2D.DrawWeapons(Camera);
            _missleSystem2D.DrawWeapons(Camera);
        }
        #endregion

        #region Helper Methods

        public void MoveShip(List<InputAction> InputActionList, GameTime gameTime)
        {
            float second = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float currentTurningSpeed = second * _turningSpeed;
            float leftRightRotation = 0;
            float upDownRotation = 0;
            float linearLeftRightRotation = 0;

            //If the ship is docked, the ship can only go straight.
            if (_docked)
            {
                InputActionList.Clear();
                InputActionList.Add(InputAction.IncreaseSpeed);
                _docked = false; //If the ship collides with the dock again, then it will be true.
            }

            foreach (InputAction action in InputActionList)
            {
                switch (action)
                {
                    case InputAction.Left:
                        leftRightRotation -= currentTurningSpeed;
                        break;
                    case InputAction.Right:
                        leftRightRotation += currentTurningSpeed;
                        break;
                    case InputAction.Up:
                        upDownRotation += currentTurningSpeed;
                        break;
                    case InputAction.Down:
                        upDownRotation -= currentTurningSpeed;
                        break;
                    case InputAction.IncreaseSpeed:
                        if (_shipSpeed < _maxShipSpeed)
                            _shipSpeed += _acceleration;
                        break;
                    case InputAction.DecreaseSpeed:
                        if (_shipSpeed > _minShipSpeed)
                            _shipSpeed -= _acceleration;
                        break;
                    case InputAction.LinearLeft:
                        linearLeftRightRotation += currentTurningSpeed;
                        break;
                    case InputAction.LinearRight:
                        linearLeftRightRotation -= currentTurningSpeed;
                        break;
                    case InputAction.Fire1:
                        _weaponSystem2D.RequestFire(ShipPosition, _shipRotation);
                        break;
                    case InputAction.Fire2:
                        _missleSystem2D.RequestFire(ShipPosition, _shipRotation);
                        break;
                    case InputAction.IncreaseTurningSpeed:
                        if (_turningSpeed < _maxTurningSpeed)
                        {
                            _turningSpeed += 0.25f;
                        }
                        break;
                    case InputAction.DecreaseTurningSpeed:
                        if (_turningSpeed > _minTurningSpeed)
                        {
                            _turningSpeed -= 0.25f;
                        }
                        break;
                }

            }

            Quaternion currentRotation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, leftRightRotation) *
                Quaternion.CreateFromAxisAngle(Vector3.UnitX, upDownRotation) *
                Quaternion.CreateFromAxisAngle(Vector3.UnitY, linearLeftRightRotation);

            _shipRotation *= currentRotation;

            ShipPosition += Vector3.Transform(Vector3.UnitZ, _shipRotation) * (_shipSpeed * second);

            ShipWorld = Matrix.CreateFromQuaternion(_shipRotation) * Matrix.CreateTranslation(ShipPosition);



        }


        public void MoveShipGamePad(List<InputAction> InputActionList, GameTime gameTime)
        {
            float second = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float currentTurningSpeed = second * _turningSpeed;
            float leftRightRotation = 0;
            float upDownRotation = 0;
            float linearLeftRightRotation = 0;

            //If the ship is docked, the ship can only go straight.
            if (_docked)
            {
                InputActionList.Clear();
                InputActionList.Add(InputAction.IncreaseSpeed);
                _docked = false; //If the ship collides with the dock again, then it will be true.
            }

            foreach (InputAction action in InputActionList)
            {
                switch (action)
                {
                    case InputAction.Left:
                        leftRightRotation -= currentTurningSpeed * Math.Abs(_gamePadInput.LeftThumbStick.X);
                        break;
                    case InputAction.Right:
                        leftRightRotation += currentTurningSpeed * Math.Abs(_gamePadInput.LeftThumbStick.X);
                        break;
                    case InputAction.Up:
                        upDownRotation += currentTurningSpeed * Math.Abs(_gamePadInput.LeftThumbStick.Y);
                        break;
                    case InputAction.Down:
                        upDownRotation -= currentTurningSpeed * Math.Abs(_gamePadInput.LeftThumbStick.Y);
                        break;
                    case InputAction.IncreaseSpeed:
                        if (_shipSpeed < _maxShipSpeed)
                            _shipSpeed += _acceleration;
                        break;
                    case InputAction.DecreaseSpeed:
                        if (_shipSpeed > _minShipSpeed)
                            _shipSpeed -= _acceleration;
                        break;
                    case InputAction.LinearLeft:
                        linearLeftRightRotation += currentTurningSpeed * Math.Abs(_gamePadInput.RightThumbStick.X);
                        break;
                    case InputAction.LinearRight:
                        linearLeftRightRotation -= currentTurningSpeed * Math.Abs(_gamePadInput.RightThumbStick.X);
                        break;
                    case InputAction.Fire1:
                        _weaponSystem2D.RequestFire(ShipPosition, _shipRotation);
                        break;
                    case InputAction.Fire2:
                        _missleSystem2D.RequestFire(ShipPosition, _shipRotation);
                        break;
                    case InputAction.IncreaseTurningSpeed:
                        if (_turningSpeed < _maxTurningSpeed)
                        {
                            _turningSpeed += 0.25f;
                        }
                        break;
                    case InputAction.DecreaseTurningSpeed:
                        if (_turningSpeed > _minTurningSpeed)
                        {
                            _turningSpeed -= 0.25f;
                        }
                        break;
                    case InputAction.Pause:
                        //We asked to pause, let's do it.
                        PauseEvent();
                        break;
                }

            }

            Quaternion currentRotation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, leftRightRotation) *
                Quaternion.CreateFromAxisAngle(Vector3.UnitX, upDownRotation) *
                Quaternion.CreateFromAxisAngle(Vector3.UnitY, linearLeftRightRotation);

            _shipRotation *= currentRotation;

            ShipPosition += Vector3.Transform(Vector3.UnitZ, _shipRotation) * (_shipSpeed * second);

            ShipWorld = Matrix.CreateFromQuaternion(_shipRotation) * Matrix.CreateTranslation(ShipPosition);



        }

        public override void Collision(CollidableType objectCollidedWithType)
        {
            switch (objectCollidedWithType)
            {
                case CollidableType.EnemyBullet:
                    Life--;
                    break;
                case CollidableType.EnemyShip:
                    Life -= 5;
                    break;
                case CollidableType.Dock:
                    _docked = true;
                    break;
                case CollidableType.Asteroid:
                    Life -= 5;
                    break;
                case CollidableType.FriendlyShip:
                    Life -= 5;
                    break;
                case CollidableType.Station:
                    Life -= 5;
                    break;
                case CollidableType.FriendBase:
                    Life -= 5;
                    break;
            }
        }

        /// <summary>
        /// Sets the model, life, speed, etc variables to the chosen ship.
        /// </summary>
        public void Spawn(int _selected)
        {
            _shipModel = new BasicModel(_playerConfigurations[_selected].ShipModel.ActualModel, _playerConfigurations[_selected].ShipModel.Scale);
            _weaponSystem2D = _playerConfigurations[_selected].WeaponSystem2D;
            _missleSystem2D = _playerConfigurations[_selected].MissleSystem2D;
            _missleSystem2D.AmountMissles = _playerConfigurations[_selected].AmountMissles;
            Camera.CameraFollowPosition = _playerConfigurations[_selected].CameraFollowPosition;
            _maxShipSpeed = _playerConfigurations[_selected].MaxShipSpeed;
            _minShipSpeed = _playerConfigurations[_selected].MinShipSpeed;
            _acceleration = _playerConfigurations[_selected].Acceleration;
            _turningSpeed = _playerConfigurations[_selected].TurningSpeed;
            _maxTurningSpeed = _playerConfigurations[_selected].MaxTurningSpeed;
            _minTurningSpeed = _playerConfigurations[_selected].MinTurningSpeed;
            Life = _playerConfigurations[_selected].Life;
            ShipPosition = _dockingPosition;
            _shipRotation = Quaternion.Identity;
        }

        public int AmountMissles()
        {
            return _missleSystem2D.AmountMissles;
        }

        public void CheckPlayerBounds()
        {
            //Also you can use Math.Abs for the ShipPositions, if Math.Abs is a lightweight method call.
            if (ShipPosition.X > 1100 || ShipPosition.X < -200 || ShipPosition.Y > 1100 || ShipPosition.Y < -200 || ShipPosition.Z > 1100 || ShipPosition.Z < -200)
            {
                bool leftBattle = false;
                if (ShipPosition.X > 1200 || ShipPosition.X < -200)
                {
                    Camera.CameraShakeIntensity = Math.Abs(ShipPosition.X) * 0.000001f;
                    leftBattle = true;
                }
                else if (ShipPosition.Y > 1200 || ShipPosition.Y < -200)
                {
                    Camera.CameraShakeIntensity = Math.Abs(ShipPosition.Y) * 0.000001f;
                    leftBattle = true;
                }
                else if (ShipPosition.Z > 1200 || ShipPosition.Z < -200)
                {
                    Camera.CameraShakeIntensity = Math.Abs(ShipPosition.Z) * 0.000001f;
                    leftBattle = true;
                }

                if (leftBattle && !LeftBattle)
                {
                    LeftBattle = true;
                    LeftBattlefieldEvent();
                }
            }
            else
            {
                Camera.CameraShakeIntensity = 0;
                LeftBattle = false;
            }
        }

        public void LeftBattlefieldEvent()
        {
            if (LeftBattlefield != null)
                LeftBattlefield(this, EventArgs.Empty);
        }

        public static void PauseEvent()
        {
            if (Pause != null)
                Pause(null, EventArgs.Empty);
        }

        #endregion
    }
}
