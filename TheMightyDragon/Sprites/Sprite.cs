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

        protected Texture2D _texture;
        protected GameTime _gameTime;
        protected string _name;
        #endregion

        #region Properties

        public Input Input;
        public General.ePlayerType PlayerType;
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
        public int FrameWidth;
        public int FrameHeight;
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

        protected virtual void SetAnimations()
        {
            
            if ((_gameTime.TotalGameTime.TotalSeconds - LastAttackTime > 5) && _name == "dragon") // attack once at 5 secs
            {
                Direction = General.eDirection.Idle;
                LastAttackTime = _gameTime.TotalGameTime.TotalSeconds;
                _animationManager.Play(_animations["Idle" + LastDirection.ToString()]);
            }
            else if ((_gameTime.TotalGameTime.TotalSeconds - LastAttackTime > 4) && _name == "dragon") // attack once at 5 secs
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

        public Sprite(Dictionary<string, Animation> animations)
        {
            _animations = animations;
            var animation = _animations.First().Value;
            FrameWidth = animation.FrameWidth;
            FrameHeight = animation.FrameHeight;
            _animationManager = new AnimationManager(animation);
         
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


                if ((int)Position.X % FrameWidth == 0 && (int)Position.Y % FrameHeight == 0)
                {
                    Direction = General.eDirection.Idle;
                    Speed = 0f;
                    Velocity = Vector2.Zero;
                }

                SetAnimations();

                _animationManager.Update(gameTime);
            }
        }

        private void UpdatePosition()
        {

            if (PlayerType == General.ePlayerType.Player)
            {
                bool ok = true;
                if (Position.X + Velocity.X <= 0) ok = false;
                if (Position.X + FrameWidth + Velocity.X >=  ScreenManager.Width)
                {
                    ok = false;
                }
                if (Position.Y + Velocity.Y <= 0) ok = false;
                if (Position.Y + FrameHeight + Velocity.Y >= ScreenManager.Height)
                {
                    ok = false;
                }
                if (ok) Position += Velocity;
            }
            else if (PlayerType == General.ePlayerType.Dragon)
            {
                Position += Velocity;
            }
        }

        internal void SetStartPosition()
        {
            var rnd = new Random(31);
            Position = new Vector2(rnd.Next(1, 4) * FrameWidth, rnd.Next(1, 4) * FrameHeight);
            Velocity = Vector2.Zero;
            Direction = General.eDirection.Idle;
            LastDirection = General.eDirection.Down;

        }

        #endregion


    }
}
