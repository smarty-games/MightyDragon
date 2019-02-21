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
        public int FrameWidth;
        public int FrameHeight;
        public General.MoveType MoveType;

        public float Speed = 2f;
        public Vector2 Velocity;
        protected General.Direction Direction = General.Direction.Down;


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
                Direction = General.Direction.Up;
            }
            else if (Keyboard.GetState().IsKeyDown(Input.Down))
            {
                Speed = General.GameSpeed;
                Velocity.Y = Speed;
                Direction = General.Direction.Down;
            }
            else if (Keyboard.GetState().IsKeyDown(Input.Left))
            {
                Speed = General.GameSpeed;
                Velocity.X = -Speed;
                Direction = General.Direction.Left;
            }
            else if (Keyboard.GetState().IsKeyDown(Input.Right))
            {
                Speed = General.GameSpeed;
                Velocity.X = Speed;
                Direction = General.Direction.Right;
            }

        }
    
    protected virtual void SetAnimations()
    {
      if (Velocity.X > 0)
        _animationManager.Play(_animations["WalkRight"]);
      else if (Velocity.X < 0)
        _animationManager.Play(_animations["WalkLeft"]);
      else if (Velocity.Y > 0)
        _animationManager.Play(_animations["WalkDown"]);
      else if (Velocity.Y < 0)
        _animationManager.Play(_animations["WalkUp"]);
      else _animationManager.Stop();
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

    public virtual void Update(GameTime gameTime, Sprite sprite)
    {

            if (_animations != null)
            {
                Move();
                Position += Velocity;

                if ((int)Position.X % FrameWidth == 0   &&
                    (int)Position.Y % FrameHeight == 0) 
                
                {
                    Direction = General.Direction.Idle;
                    Speed = 0f;
                    Velocity = Vector2.Zero;
                }

                SetAnimations();

                _animationManager.Update(gameTime);



            }
    }

        internal void SetStartPosition()
        {
            var rnd = new Random(3);
            Position = new Vector2(rnd.Next(1,10) * FrameWidth, rnd.Next(1, 10) * FrameHeight);
        }

        #endregion


    }
}
