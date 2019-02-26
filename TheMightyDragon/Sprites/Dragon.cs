﻿using Desktop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desktop.Sprites
{
    public class Dragon:Sprite
    {
        public const int HealthMax = 100; // 100 points of health
        public int Health = HealthMax;
        public Dragon(Dictionary<string, Animation> animations, TheGame game):base(animations,game)
        { 
        }
        
    }
}
