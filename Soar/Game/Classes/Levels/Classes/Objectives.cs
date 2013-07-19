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
using Paradox.Game.Classes.Cameras;

namespace Paradox.Game.Classes.Levels
{
    public abstract class Objective : Collidable
    {
        //The main components of what an objective truely is.
        public string Title { get; protected set; }
        public bool IsDone { get; protected set; } //This objective does not need to call anymore events.
        public bool IsFailed { get; protected set; } //This is true when the objective is failed.
        public bool IsDrawable { get; protected set; }
        public Vector3 Position { get; protected set; }

        public Objective()
        {
            Title = "";
            IsDone = false;
            this.BoundingSphereRadius = 0;
            this.BoundingSphereCenter = Vector3.Zero;
            this.CollidableType = Game.Classes.CollidableType.None;
            Position = Vector3.Zero;
        }

        //The Game can subscribe to these events and take appropriate actions.
        public event EventHandler Completed;
        public event EventHandler Failed;


        public abstract void Update(GameTime gameTime, EnemySquadron enemySquadron, FriendSquadron friendSquadron);
        public virtual void Draw(GraphicsDevice device, Camera camera) { }
        protected void FailedEvent()
        {
            if (Failed != null)
                Failed(this, EventArgs.Empty);
            IsDone = true;
            IsFailed = true;
        }

        protected void CompletedEvent()
        {
            if (Completed != null)
                Completed(this, EventArgs.Empty);
            IsDone = true;
        }
    }

    public class ObjectiveDrawable : Objective
    {
        private BasicModel _model;
        private Vector3 _position;
        private float _rotationSpeed;
        private Quaternion _rotation;
        public bool _drawModel { get; protected set; }

        public ObjectiveDrawable(CollidableType type, float radius, float rotationSpeed, BasicModel model, Vector3 position, Quaternion rotation)
        {
            _model = model;
            this.CollidableType = type;
            this.BoundingSphereRadius = radius;
            _position = position;
            Position = _position; //Update the main branched Objective position.
            _rotation = rotation;
            _rotationSpeed = rotationSpeed;
            IsDrawable = true;
        }

        public override void Update(GameTime gameTime, EnemySquadron enemySquadron, FriendSquadron friendSquadron)
        {
            //Gets how many seconds occured after the last update.
            float second = (float)gameTime.ElapsedGameTime.TotalSeconds;
            //Updates the rotation on one of the axis.
            _rotation *= Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), second * _rotationSpeed);
            //Updates the model while creating a world.
            _model.UpdateModel(Matrix.CreateFromQuaternion(_rotation) * Matrix.CreateTranslation(_position));

            this.BoundingSphereCenter = _position;
        }
        public override void Draw(GraphicsDevice device, Camera camera)
        {
            _model.DrawModel(camera);
        }
    }

    public class ObjectiveDestroyable : ObjectiveDrawable
    {
        private int Life;
        private TimeSpan _totalTime;
        private TimeSpan _maxTime;
        

        public ObjectiveDestroyable(CollidableType type, float radius, float rotationSpeed, BasicModel model, Vector3 position, Quaternion rotation, string description, int life, TimeSpan maxTime)
            : base(type, radius, rotationSpeed, model, position, rotation)
        {
            this.Title = description;
            this._drawModel = true;
            _maxTime = maxTime;
            Life = life;
        }

        public override void Update(GameTime gameTime, EnemySquadron enemySquadron, FriendSquadron friendSquadron)
        {
            if(_totalTime < _maxTime)
                _totalTime += gameTime.ElapsedGameTime;

            if (Life <= 0 && _drawModel && _totalTime < _maxTime)
            {
                 _drawModel = false;
                FailedEvent();
            }

            if (_totalTime > _maxTime && !IsDone)
            {
                CompletedEvent();

            }
            base.Update(gameTime, enemySquadron, friendSquadron);
        }

        public override void Draw(GraphicsDevice device, Camera camera)
        {
            base.Draw(device, camera);
        }

        public override void Collision(CollidableType objectCollidedWithType)
        {
            switch (objectCollidedWithType)
            {
                case CollidableType.EnemyBullet:
                    Life--;
                    break;
                case CollidableType.FriendlyShip:
                    Life--;
                    break;
                case Game.Classes.CollidableType.PlayerBullet:
                    Life--;
                    break;
            }
        }
    }

    public class ObjectiveEnemy : Objective
    {
        private string _titleStart;
        private int _amountEnemyTotal;
        private int _amountEnemy;

        public ObjectiveEnemy(string titlestart, int amountEnemyTotal)
        {
            _titleStart = titlestart;
            _amountEnemyTotal = amountEnemyTotal;
        }

        public override void Update(GameTime gameTime, EnemySquadron enemySquadron, FriendSquadron friendSquadron)
        {
            if(!IsDone)
            {
                _amountEnemy = enemySquadron.ShipsDestroyedPlayer;
                Title = _titleStart + _amountEnemy + " / " + _amountEnemyTotal;

                if (_amountEnemy >= _amountEnemyTotal)
                {
                    CompletedEvent();
                }
            }
        }
    }

    public class ObjectiveEnemyFrigate : Objective
    {
        private string _titleStart;
        private int _amountEnemyTotal;
        private int _amountEnemy;

        public ObjectiveEnemyFrigate(string titlestart, int amountEnemyTotal)
        {
            _titleStart = titlestart;
            _amountEnemyTotal = amountEnemyTotal;
        }

        public override void Update(GameTime gameTime, EnemySquadron enemySquadron, FriendSquadron friendSquadron)
        {
            if (!IsDone)
            {
                _amountEnemy = enemySquadron.FrigatesDestroyedPlayer;
                Title = _titleStart + _amountEnemy + " / " + _amountEnemyTotal;

                if (_amountEnemy >= _amountEnemyTotal)
                {
                    CompletedEvent();
                }
            }
        }
    }
}
