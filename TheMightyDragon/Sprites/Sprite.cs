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
        protected string _playerName;
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
        public string Name { get { return _playerName; } set { _playerName = value; } }
        public bool IsCollision = false;
        public int StepX;
        public int StepY;
        public General.eMoveType MoveType;
        public float Speed = 2f;
        public Vector2 Velocity;
        protected General.eDirection Direction = General.eDirection.Idle;
        protected General.eDirection LastDirection;
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
            if (Direction != General.eDirection.Idle && Direction != General.eDirection.Attack)
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
        public virtual void Update(GameTime gameTime, Sprite sprite)
        {
            _gameTime = gameTime;
            
            if (_animations != null)
            {
                if (Direction == General.eDirection.Idle)
                {
                    Move(); // set direction if directions key are pressed
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
        
        // update position only if collision and matrix positions are ok for the next move
        private void UpdatePosition()
        {
            if (!OutOfScreen())
            {
                if (this is Player)
                {
                    Player pl = (Player)this;
                    Point current = new Point((int)(Position.Y / StepY), (int)(Position.X / StepX));
                    Point next = new Point(0,0);

                    if (!pl.Collides() && (pl.CanMove(out next)))
                    {
                        Position += Velocity;
                        if (_game.GroundMap[next.Y][next.X] != (int)General.Legend.PlayerPath // means that player moves in danger zone (Crater)
                            && pl.TheAction == General.ePlayerAction.MoveOnGround) // our player gets bravely into the Crater
                        {
                            pl.TheAction = General.ePlayerAction.MoveInCrater;                     
                        }
                    }
                }
            }
        }

        private bool OutOfScreen()
        {
            return (Position.X + Velocity.X < 0) 
            || (Position.X + StepX + Velocity.X > (ScreenManager.Width / this.StepX) * this.StepX)
            || (Position.Y + Velocity.Y < 0)
            || (Position.Y + StepY + Velocity.Y > (ScreenManager.Height / this.StepY) * this.StepY);
        }

        /// <summary>
        /// check for colision with destination
        /// </summary>
        /// <param name="destinationType">the destination sprite the collision is check with</param>
        /// <returns></returns>
        protected bool CollisionWith(Sprite destination)
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
