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
    public class MissleSystem2D : WeaponSystem2D
    {
        public int AmountMissles;
        int _life;

        public MissleSystem2D(List<Collidable> collidableList, CollidableType type, float radius, SoundType soundType, Texture2D weaponTexture, float scale, GraphicsDevice device, int maxBullets,
            Vector3 weaponPositionRelative, float coolDownMS, float weaponSpeed, float bulletSpreadIntensity, int amountMissles, int life)
            : base(collidableList, type, radius, soundType, weaponTexture, scale, device, maxBullets, weaponPositionRelative, coolDownMS, weaponSpeed, bulletSpreadIntensity)
        {
            AmountMissles = amountMissles;
            _life = life;
        }

        public void UpdateWeapons(List<Enemy> enemyList, List<Sound> soundDump, List<Explosion> explosionList, GameTime gameTime)
        {
            soundDump.AddRange(_soundDump);
            _soundDump.Clear();

            if (_timer > 0)
            {
                _timer -= gameTime.ElapsedGameTime.Milliseconds;
            }

            for (int i = 0; i < _projectileList.Count; i++)
            {
                _projectileList[i].UpdateProjectile(gameTime, enemyList);

                if (_projectileList[i].Destroyed)
                {
                    _projectileList.Remove(_projectileList[i--]);
                }
                else if (_projectileList[i].Life <= 0)
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

        public void UpdateWeapons(Vector3 shipPosition, List<Enemy> enemyList, List<Sound> soundDump, List<Explosion> explosionList, GameTime gameTime)
        {
            soundDump.AddRange(_soundDump);
            _soundDump.Clear();
            WeaponPosition = _weaponPositionRelative + shipPosition;

            if (_timer > 0)
            {
                _timer -= gameTime.ElapsedGameTime.Milliseconds;
            }

            for(int i = 0; i < _projectileList.Count; i++)
            {
                _projectileList[i].UpdateProjectile(gameTime, enemyList);

                if (_projectileList[i].Destroyed)
                {
                    _projectileList.Remove(_projectileList[i--]);
                }
                else if (_projectileList[i].Life <= 0)
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

        public override void RequestFire(Vector3 PositionToFire, Quaternion shipRotation)
        {
            if ((_timer <= 0 || _timer == _coolDownMS) && AmountMissles > 0) //This means that a squadron wants to fire.
            {
                Vector3 desiredDirection = Vector3.Normalize(WeaponPosition - PositionToFire);
                Quaternion desiredRotation = Quaternion.CreateFromRotationMatrix(Matrix.CreateWorld(WeaponPosition, desiredDirection, Vector3.Up)); //The up direction is irrelevant, a billboard is created anyways.

                Projectile proj = new Projectile(_type, _boundingRadius, WeaponPosition + _weaponPositionRelative, BulletSpread(shipRotation, _bulletSpreadIntensity), _weaponSpeed, _life);

                _projectileList.Add(proj);
                CollidableReference.Add(proj);
                //Shoot Logic
                _timer = _coolDownMS;
                _soundDump.Add(new Sound(_soundType, WeaponPosition, false)); //When shooting a bullet, the initial sound should not be looped.
                AmountMissles--;
            }
        }
    }
}
