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
            var rnd = new Random(31);
            StepX = 64;
            StepY = 64;
            Position = new Vector2(rnd.Next(1, 14) * General.TileSize, rnd.Next(1, 24) * General.TileSize);
            Velocity = Vector2.Zero;
            Direction = General.eDirection.Idle;
            LastDirection = General.eDirection.Down;
        }

    }
}
