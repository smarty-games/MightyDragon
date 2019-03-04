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
       
        private int[][] _map; // this is copied from GroundMap in the moment the Player moves into Danger area (2)

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
            if (_game.GroundMap[line][col] != (int)General.Legend.Mountain)
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

        public override void Stop()
        {

            if ((Position.Y % StepY) == 0 && (Position.X % StepX) == 0)
            {
                UpdateMap((int)(Position.Y / StepY), (int)(Position.X / StepX));

            }
            base.Stop();

        }
        private void UpdateMap(int line, int col)
        {

            if (_map[line][col] != (int)General.Legend.PlayerPath)
            {
                _map[line][col] = (int)General.Legend.DragonPath;
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
                if (_map[line][col] == (int)General.Legend.PlayerPath && this.TheAction == General.ePlayerAction.MoveInCrater) 
                // it reached the Path
                {
                // check if a Dragon Eye was made (loop in crater)
                if (DragonEyeCompleted())
                {
                    ShowMatrix();

                    // update Player points
                }
            
                this.TheAction = General.ePlayerAction.MoveOnGround;
                    InitMapWithGameMap();
                }
            
        }

        public void SetMapSideMark(int line, int col, General.Legend mark)
        {
            if (line > 0 && line < General.TilesVertically && col>0 && col<General.TilesHorizontaly)
            {
                int pathSide = _map[line][col];
                _map[line][col] =   (pathSide != (int)General.Legend.PlayerPath && pathSide != (int)General.Legend.DragonPath) ?
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

            return false;
        }

        internal void InitMapWithGameMap()
        {
            _map = new int[General.TilesVertically][];
            for (int i = 0; i < General.TilesVertically; i++)
            {
                _map[i] = new int[General.TilesHorizontaly];
                _game.GroundMap[i].CopyTo(_map[i],0);
            }
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
            else return General.Legend.PlayerPath; // no smaller map mark found
        }


        private void ShowMatrix()
        {
            Console.Clear();
            for (int line = 0; line < General.TilesVertically; line++)
            {
                for (int col = 0; col < General.TilesHorizontaly; col++)
                {
                    string playerMatrixCell = "";
                    if (_map[line][col] != _game.GroundMap[line][col])
                    playerMatrixCell = _map[line][col].ToString()+"|";
                    Console.Write(String.Format("{0,8}", playerMatrixCell + _game.GroundMap[line][col].ToString()));

                }
                Console.WriteLine();
            }
            Console.WriteLine("--------------------------------------------------------------");
        }

        private void CreateMountain(General.Legend mark)
        {
            for (int line = 0; line < General.TilesVertically; line++)
            {
                for (int col = 0; col < General.TilesHorizontaly; col++)
                {
                    if (_map[line][col] == (int)mark * 100)
                    {
                        _map[line][col] = (int)General.Legend.Mountain;
                        _game.GroundMap[line][col] = (int)General.Legend.Mountain;
                        for (int i = -1; i <= 1; i++)
                            for (int j = -1; j <= 1; j++)
                                if (_map[line + i][col + j] == (int)General.Legend.DragonPath)
                                {
                                    _map[line + i][col + j] = (int)General.Legend.PlayerPath;
                                    _game.GroundMap[line + i][col + j] = (int)General.Legend.PlayerPath;


                                }
                    }
                }
            }
        }

        private int FillMapForMark(int line, int col, General.Legend mark)
        {
            if (_map[line][col] == (int)General.Legend.PlayerPath || _map[line][col] == (int)General.Legend.DragonPath)
            {
                return 0;
            }
            _map[line][col] = 100 * (int)mark;  // mark map with mark int value
           
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
                        if (_map[line][col] == (int)mark)
                        {
                            exists = true;
                            ++marks;
                            _map[line][col] = (int)mark * 100; // cancel mark counting for next iteration
                            // fill neighbour marks 
                            for (int i = -1; i <= 1; i++)
                                for (int j = -1; j <= 1; j++)
                                    if (_map[line + i][col + j] == (int)General.Legend.Crater)
                                    {
                                        _map[line + i][col + j] = (int)mark;  // mark map with mark int value
                                    }
                        }
                    }
               
            } while (exists);
            return marks;
        }
    }
}
