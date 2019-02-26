using Desktop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desktop.Sprites
{
    public class Player:Sprite
    {
        public const int HealthMax = 20; // 100 points of health
        public int Health = HealthMax;
        public Player(Dictionary<string, Animation> animations, TheGame game):base(animations,game)
        { 
        }
        
    }
}
