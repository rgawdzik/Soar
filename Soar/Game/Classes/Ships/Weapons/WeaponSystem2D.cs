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

using Paradox.Game.Classes;
using Paradox.Game.Classes.Ships.Weapons;
using Paradox.Game.Classes.Cameras;
using Paradox.Game.Classes.Engines;

namespace Paradox.Game.Classes.Ships
{
    public class WeaponSystem2D
    {
        #region Properties and Variables
        protected List<Collidable> CollidableReference;


        protected CollidableType _type;
        protected float _boundingRadius;

        protected List<Projectile> _projectileList;
        protected Vector3 _weaponPositionRelative; //The position of the weapon never changes

        public Vector3 WeaponPosition;

        protected float _weaponSpeed;
        protected float _coolDownMS; //How many milliseconds does it take for the weapon to cool down.
        protected float _timer;

        float _scale;
        protected Texture2D _weaponTexture;
        protected TextureQuad _textureQuad; //Helper Class for Point sprites.

        protected GraphicsDevice _device;
        protected SpriteBatch _spriteBatch;
        protected BasicEffect _basicEffect;

        protected float _bulletSpreadIntensity;

        protected int _maxBullets;

        protected Random _randomVar;

        protected List<Sound> _soundDump;

        protected SoundType _soundType;

        #endregion

        #region Constructor
        public WeaponSystem2D(List<Collidable> collidableRef, CollidableType type, float radius, SoundType soundType, Texture2D weaponTexture, float scale, GraphicsDevice device, int maxBullets,  Vector3 weaponPositionRelative, float coolDownMS, float weaponSpeed, float bulletSpreadIntensity)
        {
            CollidableReference = collidableRef;
            _projectileList = new List<Projectile>();
            _weaponPositionRelative = weaponPositionRelative;
            _coolDownMS = coolDownMS;
            _weaponSpeed = weaponSpeed;
            _timer = 0;
            _randomVar = new Random();
            _device = device;
            _textureQuad = new TextureQuad(_device, weaponTexture, scale, scale);
            
            _weaponTexture = weaponTexture;
            _scale = scale;

            _spriteBatch = new SpriteBatch(_device);
            _basicEffect = new BasicEffect(_device);

            _bulletSpreadIntensity = bulletSpreadIntensity;

            _maxBullets = maxBullets;

            _type = type;
            _boundingRadius = radius;

            _soundDump = new List<Sound>();
            _soundType = soundType;
        }
        #endregion

        #region Update
        /// <summary>
        /// A weapon system update without a ship position update.
        /// </summary>
        /// <param name="soundDump"></param>
        /// <param name="gameTime"></param>
        /// <returns></returns>
        public virtual void UpdateWeapons(List<Sound> soundDump, List<Explosion> explosionList, GameTime gameTime)
        {
            soundDump.AddRange(_soundDump);
            _soundDump.Clear();
            
            if (_timer > 0)
            {
                _timer -= gameTime.ElapsedGameTime.Milliseconds;
            }

            //Updates all the projectiles.
            for (int i = 0; i < _projectileList.Count; i++)
            {
                _projectileList[i].UpdateProjectile(gameTime);

                if (_projectileList[i].Destroyed)
                {
                    _soundDump.Add(new Sound(SoundType.Explosion, _projectileList[i].Position, false));
                    explosionList.Add(new Explosion(_projectileList[i].Position, 0, ExplosionType.Small));

                    CollidableReference.Remove(_projectileList[i]);

                    _projectileList.Remove(_projectileList[i--]);
                    
                }
            }

            if (_projectileList.Count() > _maxBullets)
            {
                int i = 0;
                while (_projectileList.Count() > _maxBullets)
                {
                    CollidableReference.Remove(_projectileList[i]);
                    _projectileList.Remove(_projectileList[i++]);
                }
            }
        }
        /// <summary>
        /// A weapon system update with the ship Position.
        /// </summary>
        /// <param name="shipPosition"></param>
        /// <param name="soundDump"></param>
        /// <param name="gameTime"></param>
        /// <returns></returns>
        public virtual void UpdateWeapons(Vector3 shipPosition, List<Sound> soundDump, List<Explosion> explosionList, GameTime gameTime)
        {
            soundDump.AddRange(_soundDump);
            _soundDump.Clear();
            WeaponPosition = _weaponPositionRelative + shipPosition;
            
            if (_timer > 0)
            {
                _timer -= gameTime.ElapsedGameTime.Milliseconds;
            }
            
            //Updates all the projectiles.
            for (int i = 0; i < _projectileList.Count; i++)
            {
                _projectileList[i].UpdateProjectile(gameTime);

                if (_projectileList[i].Destroyed)
                {
                    explosionList.Add(new Explosion(_projectileList[i].Position, 0, ExplosionType.Small));
                    CollidableReference.Remove(_projectileList[i]);
                    _projectileList.Remove(_projectileList[i--]);
                }
            }

            if (_projectileList.Count() > _maxBullets)
            {
                int i = 0;
                while (_projectileList.Count() > _maxBullets)
                {
                    CollidableReference.Remove(_projectileList[i]);
                    _projectileList.Remove(_projectileList[i++]);
                }
            }
        }
        #endregion

        #region Draw
        public void DrawWeapons(Camera camera)
        {
            if (_projectileList.Count() > 0)
            {
                //This sets the BlendState to Additive, which allows blending to occur between backgrounds.
                RasterizerState rs = new RasterizerState();
                rs.CullMode = CullMode.None;
                _device.RasterizerState = rs;
                _device.BlendState = BlendState.Additive;

                
                foreach (Projectile projectile in _projectileList)
                {
                    _textureQuad.Draw(camera, projectile.Position);
                }

                RasterizerState rs2 = new RasterizerState();
                rs2.CullMode = CullMode.CullCounterClockwiseFace;
                _device.RasterizerState = rs2;
                _device.BlendState = BlendState.NonPremultiplied;
            }
        }
        #endregion

        #region Helper Methods
        public virtual void RequestFire(Vector3 FiringPosition, Quaternion shipRotation)
        {
            if (_timer <= 0 || _timer == _coolDownMS) //This means that a squadron wants to fire.
            {
                Projectile projectile = new Projectile(_type, _boundingRadius, FiringPosition + _weaponPositionRelative, BulletSpread(shipRotation, _bulletSpreadIntensity), _weaponSpeed);
                _projectileList.Add(projectile);
                CollidableReference.Add(projectile);
                //Shoot Logic
                _timer = _coolDownMS;
                _soundDump.Add(new Sound(_soundType, FiringPosition, false)); //When shooting a bullet, the initial sound should not be looped.
            }
        }

        /// <summary>
        /// This Request Fire uses the Weapon Position as the firing position and fires at a location.
        /// </summary>
        /// <param name="PositionToFire"></param>
        public virtual void RequestFire(Vector3 PositionToFire)
        {
            if (_timer <= 0 || _timer == _coolDownMS) //This means that a squadron wants to fire.
            {
                Vector3 desiredDirection = Vector3.Normalize(WeaponPosition - PositionToFire);
                Quaternion desiredRotation = Quaternion.CreateFromRotationMatrix(Matrix.CreateWorld(WeaponPosition, desiredDirection, Vector3.Up)); //The up direction is irrelevant, a billboard is created anyways.

                Projectile proj = new Projectile(_type, _boundingRadius, WeaponPosition + _weaponPositionRelative, BulletSpread(desiredRotation, _bulletSpreadIntensity), _weaponSpeed);
                _projectileList.Add(proj);
                CollidableReference.Add(proj);
                //Shoot Logic
                _timer = _coolDownMS;
                _soundDump.Add(new Sound(_soundType, WeaponPosition, false)); //When shooting a bullet, the initial sound should not be looped.
            }
        }

        public Quaternion BulletSpread(Quaternion shipRotation, float intensity)
        {
            return shipRotation * Quaternion.CreateFromYawPitchRoll((float)Math.Sin(_randomVar.Next(-10, 10) * intensity),
                (float)Math.Sin(_randomVar.Next(-10, 10) * intensity), (float)Math.Sin(_randomVar.Next(-10, 10) * intensity));
        }

        #endregion
    }
}
