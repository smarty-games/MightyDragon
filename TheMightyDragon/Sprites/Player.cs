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
            InitMapWithGameMap();

        }
        /// <summary>
        /// check for matrix values (1) accepted for player next position
        /// </summary>
        /// <returns></returns>
        public bool CanMove(out Point next)
        {
            int line = Direction == General.eDirection.Down ?
                                        (int)((Position.Y + (StepY + Velocity.Y - 1)) / StepX)
                                        : (int)((Position.Y + Velocity.Y) / StepY);
            int col = Direction == General.eDirection.Right ?
                                        (int)((Position.X + (StepX + Velocity.X - 1)) / StepX)
                                        : (int)((Position.X + Velocity.X) / StepX);
            next = new Point(col, line);
            if (TMD.GroundMap[line][col] != (int)General.Legend.Mountain)
            {
                return true;
            }
            else return false;
        }
        public bool Collides()
        {
            foreach (var destination in TMD.Sprites.Values)
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

        public override void Stop()
        {

            if ((Position.Y % StepY) == 0 && (Position.X % StepX) == 0)
            {
                UpdateMap((int)(Position.Y / StepY), (int)(Position.X / StepX));

            }
            base.Stop();

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
                    if (TMD.GroundMap[next.Y][next.X] != (int)General.Legend.PlayerPath // means that player moves in danger zone (Crater)
                        && TheAction == General.ePlayerAction.MoveOnGround) // our player gets bravely into the Crater
                    {
                        TheAction = General.ePlayerAction.MoveInCrater;
                    }
                }
            }
        }
        private void UpdateMap(int line, int col)
        {

            if (Map[line][col] != (int)General.Legend.PlayerPath)
            {
                Map[line][col] = (int)General.Legend.DragonPath;
            }
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
                            SetMapSideMark(line +1 , col, General.Legend.LeftMark);
                            SetMapSideMark(line -1, col + 1, General.Legend.RightMark);
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
                if (Map[line][col] == (int)General.Legend.PlayerPath && this.TheAction == General.ePlayerAction.MoveInCrater) 
                // it reached the Path
                {
                // check if a Dragon Eye was made (loop in crater)
                if (DragonEyeCompleted())
                {
//                    ShowMatrix();

                    // update Player points
                }
            
                this.TheAction = General.ePlayerAction.MoveOnGround;
                InitMapWithGameMap();
                }
            
        }

        public void SetMapSideMark(int line, int col, General.Legend mark)
        {
          if (line > 0 && line < General.TilesVertically && col>0 && col<General.TilesHorizontaly && Map[line][col] != (int)General.Legend.PlayerPath) 
            {
                int pathSide = Map[line][col];
                Map[line][col] =   (pathSide != (int)General.Legend.PlayerPath && pathSide != (int)General.Legend.DragonPath) ?
                                    (int)mark 
                                    : pathSide;
            }

        }

        private bool DragonEyeCompleted()
        {

            General.Legend winZone = GetSmallerMapMark(); // LeftMark or RightMark
            if (winZone == General.Legend.LeftMark || winZone == General.Legend.RightMark)
            {
                CreateMountain(winZone);

                return true;
            }
            ActivePath = null;
            return false;
        }

        protected  void InitMapWithGameMap()
        {
            Map = new int[General.TilesVertically][];
            for (int i = 0; i < General.TilesVertically; i++)
            {
                Map[i] = new int[General.TilesHorizontaly];
                TMD.GroundMap[i].CopyTo(Map[i], 0);
            }
            ActivePath = null;
            
        }

        /// <summary>
        /// Gets the smaller map mark between LeftMark & RightMark
        /// </summary>
        /// <returns>The smaller map mark.</returns>
        private General.Legend GetSmallerMapMark()
        {
            int loopSizeLeftMark = 0;
            int loopSizeRightMark = 0;
                        loopSizeLeftMark = FillMapForMark(General.Legend.LeftMark);
                        loopSizeRightMark= FillMapForMark(General.Legend.RightMark);
           
            if (loopSizeLeftMark > 0 && loopSizeRightMark > 0)
            {
                if (loopSizeLeftMark < loopSizeRightMark)
                    return General.Legend.LeftMark;
                else return General.Legend.RightMark;
            }
            else return General.Legend.NoDragonPath; // no smaller map mark found
        }




        private void CreateMountain(General.Legend mark)
        {
            for (int line = 0; line < General.TilesVertically; line++)
            {
                for (int col = 0; col < General.TilesHorizontaly; col++)
                {
                    if (Map[line][col] == (int)mark * 100)
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

        private int FillMapForMark(int line, int col, General.Legend mark)
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


        private int FillMapForMark(General.Legend mark)
        {
            bool exists = false;
            int marks = 0; // number of cells to be completed with mark value
            do
            {
                exists = false;
                for (int line = 0; line < General.TilesVertically; line++)
                    for (int col = 0; col < General.TilesHorizontaly; col++)
                    {
                        if (Map[line][col] == (int)mark)
                        {
                            exists = true;
                            ++marks;
                            Map[line][col] = (int)mark * 100; // cancel mark counting for next iteration
                            // fill neighbour marks 
                            for (int i = -1; i <= 1; i++)
                                for (int j = -1; j <= 1; j++)
                                    if (Map[line + i][col + j] == (int)General.Legend.Crater)
                                    {
                                        Map[line + i][col + j] = (int)mark;  // mark map with mark int value
                                    }
                        }
                    }
               
            } while (exists);
            return marks;
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
