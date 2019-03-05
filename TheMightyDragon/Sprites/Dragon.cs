using Desktop.Models;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desktop.Sprites
{
    public class Dragon:Sprite
    {
        // public fields
        public const int HealthMax = 100; // 100 points of health
        public int Health = HealthMax;
        // private fields
        private double LastAttackTime = 0;  // in seconds
        public Dragon(Dictionary<string, Animation> animations, TheGame game):base(animations,game)
        { 
        }
        protected override void SetAnimations()
        {
            if (_gameTime.TotalGameTime.TotalSeconds - LastAttackTime > 5) // attack for 1 second
            {
                Direction = General.eDirection.Idle;
                LastAttackTime = _gameTime.TotalGameTime.TotalSeconds;
                _animationManager.Play(_animations["Idle" + LastDirection.ToString()]);
            }
            else if (_gameTime.TotalGameTime.TotalSeconds - LastAttackTime > 4) // attack once at 4 seconds
            {
                Direction = General.eDirection.Attack;
                _animationManager.Play(_animations["Attack" + LastDirection.ToString()]);

            }
            else
            {
                base.SetAnimations();
            }
            
        }
        public void Init()
        {
            var rnd = new Random(41);
            StepX = 64;
            StepY = 64;
            Position = new Vector2(2 * rnd.Next(5, 9) * General.TileSize, 2* rnd.Next(3, 6) * General.TileSize);
            Velocity = Vector2.Zero;
            Direction = General.eDirection.Idle;
            LastDirection = General.eDirection.Down;
        }
        public bool CanMove(out Point next)
        {
            int line = Direction == General.eDirection.Down ?
                                        (int)((Position.Y + (StepY + Velocity.Y - 1)) / StepX)
                                        : (int)((Position.Y + Velocity.Y) / StepY);
            int col = Direction == General.eDirection.Right ?
                                        (int)((Position.X + (StepX + Velocity.X - 1)) / StepX)
                                        : (int)((Position.X + Velocity.X) / StepX);
            next = new Point(col, line);
            if (_game.GroundMap[line][col] == (int)General.Legend.Crater)
            {
                return true;
            }
            else return false;
        }
        public bool Collides()
        {
            foreach (var destination in _game.Sprites.Values)
            {
                if (destination is Player)
                {
                    if (this.CollisionWith(destination))
                    {
                        this.IsCollision = true;
                        destination.IsCollision = true;
                        this.Stop();
                        destination.Stop();
                        return this.IsCollision;
                    }
                    else
                    {
                        this.IsCollision = false;
                        destination.IsCollision = false;
                    }
                }
            }
            return this.IsCollision;
        }
        public override void UpdatePosition()
        {
            if (!OutOfScreen())
            {

                base.UpdatePosition();
                Point current = new Point((int)(Position.Y / StepY), (int)(Position.X / StepX));
                Point next = new Point(0, 0);

                if (!Collides() && (CanMove(out next)))
                {
                    Position += Velocity;
                }
            }
        }

    }
}
