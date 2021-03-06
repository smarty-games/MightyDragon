﻿using Desktop.Models;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Desktop;
namespace Desktop.Sprites
{
    public class Player : Sprite
    {
        public const int HealthMax = 20; // 100 points of health
        public int Health = HealthMax;
        public General.ePlayerAction TheAction;
        public List<Point> ActivePath; // last walked dragon path 
       
        public Player(Dictionary<string, Animation> animations, TheGame game) : base(animations, game)
        {
        }
        public void Init()
        {
            var rnd = new Random(31);
            StepX = 32;
            StepY = 32;
            Position = new Vector2(128,0);
            Velocity = Vector2.Zero;
            Direction = General.eDirection.Idle;
            LastDirection = General.eDirection.Down;
            TheAction = General.ePlayerAction.MoveOnGround;
            Map = InitMapWith(TMD.GroundMap);

        }
        /// <summary>
        /// check for matrix values (1) accepted for player next position
        /// </summary>
        /// <returns></returns>
        public bool CanMove(ref Point next)
        {
            switch (Direction)
            {
                case General.eDirection.Up:
                    next.Y -= 1;
                    break;
                case General.eDirection.Down:
                    next.Y += 1;
                    break;
                case General.eDirection.Left:
                    next.X -= 1;
                    break;
                case General.eDirection.Right:
                    next.X += 1;
                    break;
                case General.eDirection.Idle:
                    return false;

                default: break;
            }

            if (!OutOfScreen(next) && TMD.GroundMap[next.Y][next.X] != (int)General.Legend.Mountain)
            {
                return true;
            }
            return false;
        }
        public bool Collides()
        {
            foreach (var destination in TMD.Sprites.Values)
            {
                if (destination is Dragon)
                {
                    if (this.CollisionWith(destination))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public override void Stop(Point current)
        {
            ShowMatrix(Map);
            base.Stop(current);
        }
        public override void UpdatePosition()
        {
            base.UpdatePosition();
            // current player position before moving
            Point current = new Point((int)(Position.X / StepX), (int)(Position.Y / StepY));
            // next point on player map towards player will eventually set its Direction & Velocity in Move() function call
            Point next = current;

            if (OutOfScreen())
            {
                Stop(current);
                return;
            }
            
            if (((int)Position.Y % StepY) == 0 && ((int)Position.X % StepX) == 0)
            {
                if (TMD.GroundMap[current.Y][current.X] == (int)General.Legend.Crater)
                {
                    Map[current.Y][current.X] = (int)General.Legend.DragonPath;
                    UpdateMap(current.Y, current.X);
                    this.TheAction = General.ePlayerAction.MoveInCrater;

                }
                else if (TMD.GroundMap[current.Y][current.X] == (int)General.Legend.PlayerPath 
                        && this.TheAction == General.ePlayerAction.MoveInCrater)
                { // returns 
                    Stop(current);
                    this.TheAction = General.ePlayerAction.MoveOnGround;
                    if (DragonEyeCompleted())
                    {
                        ShowMatrix(Map);
                    }
                    return;
                }
                Move();
                if (CanMove(ref next))
                {
                    if (TMD.GroundMap[next.Y][next.X] == (int)General.Legend.Crater 
                     && TMD.GroundMap[current.Y][current.X] == (int)General.Legend.PlayerPath)
                    { // player leaves path
                        TheAction = General.ePlayerAction.MoveInCrater;
                    }
                }
            }
            Position += Velocity;
        }
        private void UpdateMap(int line, int col)
        {
            // set Crater (2) on Player path. Here, Action for player should be MoveOnCrater already
            switch (Direction)
            {
                case General.eDirection.Up:
                    {
                        SetMapSideMark(line, col - 1, General.Legend.LeftMark);
                        SetMapSideMark(line, col + 1, General.Legend.RightMark);
                        break;
                    }
                case General.eDirection.Down:
                    {
                        SetMapSideMark(line, col + 1, General.Legend.LeftMark);
                        SetMapSideMark(line, col - 1, General.Legend.RightMark);
                        break;
                    }
                case General.eDirection.Left:
                    {
                        SetMapSideMark(line + 1, col, General.Legend.LeftMark);
                        SetMapSideMark(line - 1, col, General.Legend.RightMark);
                        break;
                    }
                case General.eDirection.Right:
                    {
                        SetMapSideMark(line - 1, col, General.Legend.LeftMark);
                        SetMapSideMark(line + 1, col, General.Legend.RightMark);
                        break;
                    }
                case General.eDirection.Idle:
                    {
                        break;
                    }
            }
        }
        public void SetMapSideMark(int line, int col, General.Legend mark)
        {
            if (line > 0 && col > 0
                && line < General.TilesVertically
                && col < General.TilesHorizontaly
                && Map[line][col] != (int)General.Legend.PlayerPath
                && Map[line][col] != (int)General.Legend.DragonPath)
            {
                Map[line][col] = (int)mark;
            }

        }

        private bool DragonEyeCompleted()
        {

            Point winPoint = GetSmallerMapMark(); // LeftMark or RightMark
            if (winPoint != Point.Zero)
            {
                CreateMountain(winPoint);
                return true;
            }
            ActivePath = null;
            return false;
        }

        protected  int[][] InitMapWith(int[][] origin)
        {
            int[][] map = new int[General.TilesVertically][];
            for (int i = 0; i < General.TilesVertically; i++)
            {
                map[i] = new int[General.TilesHorizontaly];
                origin[i].CopyTo(map[i], 0);
            }
            ActivePath = null;
            return map;
        }

        /// <summary>
        /// Gets the smaller map mark between LeftMark & RightMark
        /// </summary>
        /// <returns>The smaller map mark.</returns>
        private Point GetSmallerMapMark()
        {
            Point mark;
            Point minMark = Point.Zero;

            int marks = 375;
            int minMarks = 375;

            do
            {
                mark = GetFirstMarkPoint(Map, General.Legend.LeftMark);
                marks = FillMapForMark(General.Legend.LeftMark, mark, 1);
                if (marks<minMarks)
                {
                    minMarks = marks;
                    minMark = new Point(mark.X, mark.Y);
                }
            }
            while (mark != Point.Zero);

            do
            {
                mark = GetFirstMarkPoint(Map, General.Legend.RightMark);
                marks = FillMapForMark(General.Legend.RightMark, mark, 1);
                if (marks < minMarks)
                {
                    minMarks = marks;
                    minMark = new Point(mark.X, mark.Y);
                }
            }
            while (mark != Point.Zero);
            return minMark;

        }

        private Point GetFirstMarkPoint(int[][] map, General.Legend mark)
        {
            for (int line = 0; line < General.TilesVertically; line++)
            {
                for (int col = 0; col < General.TilesHorizontaly; col++)
                {
                    if (Map[line][col] == (int)mark) return new Point(col,line);
                }
            }

            return Point.Zero;
        }

        private void CreateMountain(Point smallestAreaPoint)
        {
            FillMapForMark(General.Legend.MountainPath, smallestAreaPoint, 1);
            for (int line = 0; line < General.TilesVertically; line++)
            {
                for (int col = 0; col < General.TilesHorizontaly; col++)
                {
                    if (Map[line][col] == (int)General.Legend.MountainPath)
                    {
                        Map[line][col] = (int)General.Legend.Mountain;
                        TMD.GroundMap[line][col] = (int)General.Legend.Mountain;
                        for (int i = -1; i <= 1; i++)
                            for (int j = -1; j <= 1; j++)
                                if (Map[line + i][col + j] == (int)General.Legend.DragonPath)
                                {

                                    Map[line + i][col + j] = (int)General.Legend.PlayerPath;
                                    TMD.GroundMap[line + i][col + j] = (int)General.Legend.PlayerPath;
                                }
                    }
                }
            }

        }


        public int FillMapForMark(int line, int col, General.Legend mark)
        {
            if (Map[line][col] == (int)General.Legend.PlayerPath || Map[line][col] == (int)General.Legend.DragonPath)
            {
                return 0;
            }
            Map[line][col] = 100 * (int)mark;  // mark map with mark int value
           
            return 1 + FillMapForMark(line - 1, col, mark)   
                     + FillMapForMark(line, col + 1, mark)
                     + FillMapForMark(line + 1, col, mark)
                     + FillMapForMark(line, col - 1, mark);
        }


        private int FillMapForMark(General.Legend mark,Point toMark, int marks)
        {
            Map[toMark.Y][toMark.X] = (int)mark*100;  // transform in area mark
                for (int line = -1; line < 1; line++)
                    for (int col = -1; col < toMark.Y; col++)
                    {
                        if (Math.Abs( col+line)== 1)
                        if (!OutOfScreen(new Point(toMark.Y + line, toMark.X + col)))
                        if (Map[toMark.Y + line][toMark.X + col] == (int)mark)
                        {
                           return marks + FillMapForMark(mark,new Point(toMark.X+col,toMark.Y+line),marks);
                        }
                    }
               
            return marks;
        }

        private bool OutOfScreen(Point point)
        {
            if (point.X >=0 && point.X < General.TilesHorizontaly && point.Y >= 0 && point.Y < General.TilesVertically)
            {
                return false;
            }
            else return true;
        }

        public List<Point> GetActivePath()
        {
            var activePath = new List<Point>();
            for (int line = 0; line < General.TilesVertically; line++)
                for (int col = 0; col < General.TilesHorizontaly; col++)
                {
                    if (Map[line][col] == (int)General.Legend.DragonPath)
                    {
                        activePath.Add(new Point(col, line));
                    }
                }
            return activePath;
        }
    }
}
