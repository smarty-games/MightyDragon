using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Desktop.Managers;
using Desktop.Models;
using Desktop;
namespace Desktop.Sprites
{
    public class Sprite
    {
        #region Fields

        protected AnimationManager _animationManager;

        protected Dictionary<string, Animation> _animations;

        protected Vector2 _position;
        // keep reference to game for accesing properties
        protected TheGame _game;
        protected Texture2D _texture;
        protected GameTime _gameTime;
        protected string _name;
        #endregion

        #region Properties

        public Input Input;
        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;

                if (_animationManager != null)
                    _animationManager.Position = _position;
            }
        }
        public string Name { get { return _name; } set { _name = value; } }

        public bool IsCollision = false;

        public int StepX;
        public int StepY;
        public General.eMoveType MoveType;

        public float Speed = 2f;
        public Vector2 Velocity;
        protected General.eDirection Direction = General.eDirection.Idle;
        protected General.eDirection LastDirection;
        private double LastAttackTime = 0;  // in seconds

        #endregion

        #region Methods

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (_texture != null)
                spriteBatch.Draw(_texture, Position, Color.White);
            else if (_animationManager != null)
                _animationManager.Draw(spriteBatch);
            else throw new Exception("This ain't right..!");
        }
        
        protected virtual void SetAnimations()
        {
            
            if ((_gameTime.TotalGameTime.TotalSeconds - LastAttackTime > 5) && this is Dragon) // attack once at 5 secs
            {
                Direction = General.eDirection.Idle;
                LastAttackTime = _gameTime.TotalGameTime.TotalSeconds;
                _animationManager.Play(_animations["Idle" + LastDirection.ToString()]);
            }
            else if ((_gameTime.TotalGameTime.TotalSeconds - LastAttackTime > 4) && this is Dragon) // attack once at 5 secs
            {
                Direction = General.eDirection.Attack;
                _animationManager.Play(_animations["Attack" + LastDirection.ToString()]);

            }
            else if (Direction != General.eDirection.Idle && Direction != General.eDirection.Attack)
            {
                _animationManager.Play(_animations["Walk" + Direction.ToString()]);
            }
            else if (Direction == General.eDirection.Idle)
            {
                _animationManager.Play(_animations["Idle" + LastDirection.ToString()]);
            }
        }
        public Sprite(Dictionary<string, Animation> animations,TheGame game)
        {
            _animations = animations;
            var animation = _animations.First().Value;
            _animationManager = new AnimationManager(animation);
            _game = game;
        }
        public Sprite(Texture2D texture)
        {
            _texture = texture;
        }
        public virtual void Update(GameTime gameTime, Sprite sprite, bool isStartGame = false)
        {
            _gameTime = gameTime;
            if (isStartGame)
            {
                SetStartPosition();
            }
            if (_animations != null)
            {
                if (Direction == General.eDirection.Idle)
                {
                    Move(); // set direction if moving key is pressed
                }
                UpdatePosition();

                if ((int)Position.X % StepX == 0 && (int)Position.Y % StepY == 0)
                {
                    Stop();
                }
                SetAnimations();
                _animationManager.Update(gameTime);
            }
        }
        public virtual void Stop()
        {
            Direction = General.eDirection.Idle;
            Speed = 0f;
            Velocity = Vector2.Zero;
        }
        public virtual void Move()
        {
            if (Keyboard.GetState().IsKeyDown(Input.Up))
            {
                Speed = General.GameSpeed;
                Velocity.Y = -Speed;
                Direction = General.eDirection.Up;
                LastDirection = Direction;
            }
            else if (Keyboard.GetState().IsKeyDown(Input.Down))
            {
                Speed = General.GameSpeed;
                Velocity.Y = Speed;
                Direction = General.eDirection.Down;
                LastDirection = Direction;
            }
            else if (Keyboard.GetState().IsKeyDown(Input.Left))
            {
                Speed = General.GameSpeed;
                Velocity.X = -Speed;
                Direction = General.eDirection.Left;
                LastDirection = Direction;
            }
            else if (Keyboard.GetState().IsKeyDown(Input.Right))
            {
                Speed = General.GameSpeed;
                Velocity.X = Speed;
                Direction = General.eDirection.Right;
                LastDirection = Direction;
            }
        }
        internal void SetStartPosition()
        {
            var rnd = new Random(31);
            StepX = this is Dragon ? 64 : 32;
            StepY = this is Dragon ? 64 : 32;
            Position = new Vector2(rnd.Next(1, 4) * StepX, rnd.Next(1, 4) * StepY);
            if (this is Player) Position = Vector2.Zero;
            Velocity = Vector2.Zero;
            Direction = General.eDirection.Idle;
            LastDirection = General.eDirection.Down;
        }
        // update position only if collision and matrix positions are ok for the next move
        private void UpdatePosition()
        {

            bool ok = true;
            if (Position.X + Velocity.X < 0) ok = false;
            if (Position.X + StepX + Velocity.X > (ScreenManager.Width/this.StepX) * this.StepX)
            {
                ok = false;
            }
            if (Position.Y + Velocity.Y < 0) ok = false;
            if (Position.Y + StepY + Velocity.Y > (ScreenManager.Height/this.StepY) * this.StepY)
            {
                ok = false;
            }
            if (ok)

            {
                //check for collision of Player with Dragon
                foreach (var source in _game.Sprites)
                {
                    if (source is Player)
                    {

                        foreach (var destination in _game.Sprites)
                        {
                            if (destination is Dragon)
                            {
                                if (source.CollisionWith(destination))
                                {
                                    source.IsCollision = true;
                                    destination.IsCollision = true;
                                    source.Stop();
                                    destination.Stop();
                                }
                                else
                                {
                                    source.IsCollision = false;
                                    destination.IsCollision = false;
                                }

                            }
                        }

                    }
                    if (!source.IsCollision && CanMove())
                    {
                        Position += Velocity;
                        if (this is Player)
                        {
                            int mX = (int)(Position.Y / StepY);
                            int mY = (int)(Position.X / StepX);
                            _game.PlayerMatrix[mX][mY] = 1;
                        }
                    }
                }
            }
        }

        private bool CanMove()
        {
            int matrixX = Direction == General.eDirection.Down ?
                                         (int)((Position.Y + (StepY + Velocity.Y - 1)) / StepY):
                                         (int)((Position.Y + Velocity.Y) / StepY);
            int matrixY = Direction == General.eDirection.Right ?
                                         (int)((Position.X + (StepX + Velocity.X - 1)) / StepX) :
                                         (int)((Position.X + Velocity.X) / StepX);
            if (_game.PlayerMatrix[matrixX][matrixY] == 1)
            {
                return true;
            }
            else return false;
        }

        /// <summary>
        /// check for colision with destination
        /// </summary>
        /// <param name="destinationType">the destination sprite which the collision is computed for</param>
        /// <returns></returns>
        internal bool CollisionWith(Sprite destination)
        {
            Rectangle sourceRect = new Rectangle((int)(this.Position.X + this.Velocity.X),
                                                 (int)(this.Position.Y + this.Velocity.Y),
                                                 this.StepX,
                                                 this.StepY);
            Rectangle destinationRect = new Rectangle((int)(destination.Position.X + destination.Velocity.X),
                                                                     (int)(destination.Position.Y + destination.Velocity.Y),
                                                                     destination.StepX,
                                                                     destination.StepY);
            if (sourceRect.Intersects(destinationRect))
            {
                return true;
            }
            else return false;
        }


        #endregion


    }
}
