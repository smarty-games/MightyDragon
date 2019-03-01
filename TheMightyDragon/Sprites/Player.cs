using Desktop.Models;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Desktop;
namespace Desktop.Sprites
{
    public class Player:Sprite
    {
        public const int HealthMax = 20; // 100 points of health
        public int Health = HealthMax;
        public General.eGroundType GroundType;
        public Player(Dictionary<string, Animation> animations, TheGame game) : base(animations, game)
        {
        }
        public void Init()
        {
            var rnd = new Random(31);
            StepX = 32;
            StepY = 32;
            Position = Vector2.Zero;
            Velocity = Vector2.Zero;
            Direction = General.eDirection.Idle;
            LastDirection = General.eDirection.Down;
        }
        /// <summary>
        /// check for matrix values (1) accepted for player next position
        /// </summary>
        /// <returns></returns>
        public bool CanMove()
        {
            int matrixX = Direction == General.eDirection.Down ?
                                         (int)((Position.Y + (StepY + Velocity.Y - 1)) / StepY) :
                                         (int)((Position.Y + Velocity.Y) / StepY);
            int matrixY = Direction == General.eDirection.Right ?
                                         (int)((Position.X + (StepX + Velocity.X - 1)) / StepX) :
                                         (int)((Position.X + Velocity.X) / StepX);
            if ((_game.GroundMap[matrixX][matrixY] == (int)General.Legend.Path ||
                _game.GroundMap[matrixX][matrixY] ==(int) General.Legend.Crater))
            {
                return true;
            }
            else return false;
        }
        public bool Collides()
        {
            foreach (var destination in _game.Sprites.Values)
            {
                if (destination is Dragon)
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
                  
    }
}
