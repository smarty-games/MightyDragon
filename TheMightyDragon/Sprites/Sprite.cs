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

        protected Texture2D _texture;
        protected GameTime _gameTime;
        protected string _playerName;
        public TheGame TMD;
        public int[][] Map; // this is copied from GroundMap in the moment the Player moves into the dangerous area (2) in other means, the player steps on dragon path

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
            TMD = game;

            
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
        public virtual void UpdatePosition()
        {
            // position of characters on map are handled individually by UpdatePoisition() of character class
        }

        public bool OutOfScreen()
        {
            return (Position.X + Velocity.X < 0) 
            || (Position.X + General.TileSize + Velocity.X > (ScreenManager.Width / General.TileSize) * General.TileSize)
            || (Position.Y + Velocity.Y < 0)
            || (Position.Y + General.TileSize + Velocity.Y > (ScreenManager.Height / General.TileSize) * General.TileSize);
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

        protected void ShowMatrix(int[][] map)
        {
            for (int line = 0; line < General.TilesVertically; line++)
            {
                for (int col = 0; col < General.TilesHorizontaly; col++)
                {
                    string playerMatrixCell = "";
                    if (map[line][col] != TMD.GroundMap[line][col])
                        playerMatrixCell = map[line][col].ToString() + "|";
                    Console.Write(String.Format("{0,8}", playerMatrixCell + TMD.GroundMap[line][col].ToString()));

                }
                Console.WriteLine();
            }
            Console.WriteLine("--------------------------------------------------------------");
        }


        #endregion


    }
}
