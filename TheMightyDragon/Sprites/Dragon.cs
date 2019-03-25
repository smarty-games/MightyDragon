using Desktop.Models;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desktop.Sprites
{
    public class PathToAttack
    {
        public List<Point> Path;
        public int Length;
        public int[][] Map;

        private Point player;

        public PathToAttack(Point dragon, Point player, int[][] groundMap)
        {
            this.player = player;
            InitMapWithGameMap(groundMap);
            Path = null; // no path to attack was found
            FindPathToAttackPoint(dragon);  // 100 init value for dragon path step to not override values for map Legend values

        }
        protected void InitMapWithGameMap(int[][] groundMap)
        {
            Map = new int[General.TilesVertically][];
            for (int i = 0; i < General.TilesVertically; i++)
            {
                Map[i] = new int[General.TilesHorizontaly];
                groundMap[i].CopyTo(Map[i], 0);
            }
        }
        private void FindPathToAttackPoint(Point dragon)
        {
            int marks = 1;
            int step = 100;
            Map[dragon.X][dragon.Y] = step;
            while (marks > 0)
            {
                step += 1;
                marks = 0;
                // TODO: if dragon trass pass player dragonpath than slow the player and substract health points
                for (int line = 1; line <= General.TilesVertically - 2; line++)
                    for (int col = 1; col <= General.TilesHorizontaly - 2; col++)
                    {
                        if (Map[line][col] == step - 1)

                        {
                            for (int i = -1; i <= 1; i++)
                                for (int j = -1; j <= 1; j++)
                                    if (Math.Abs(i) + Math.Abs(j) == 1)
                                    {
                                        Point next = new Point(line + i, col + j);
                                        if (Map[next.X][next.Y] == (int)General.Legend.Crater)
                                        {
                                            marks++;
                                            Map[next.X][next.Y] = step;
                                            if (IsCollision(next))
                                            {
                                                //ShowMatrix(Map);
                                                SetPathToAttackPoint(next, step);
                                                return;
                                            }
                                        }

                                    }
                        }
                    }
            }
        }

        private void SetPathToAttackPoint(Point dragon,int step)
        {
            if (Map[dragon.X][dragon.Y] == 100)
            {
                Path = new List<Point>() { dragon };
            }
            else
            {
                for (int i = -1; i <= 1; i++)
                    for (int j = -1; j <= 1; j++)
                    {
                        if (Math.Abs(i) + Math.Abs(j) == 1 && Path == null)
                        {
                            if (Map[dragon.X + i][dragon.Y + j] == step - 1)
                            {
                                SetPathToAttackPoint(new Point(dragon.X + i, dragon.Y + j), step - 1);
                                Path.Add(dragon);
                            }
                        }
                    }
            }
        }

        private bool IsCollision(Point dragon)
        {
            for (int i = -1; i <= 1; i++)
                for (int j = -1; j <= 1; j++)
                    if (player == new Point(dragon.X + j, dragon.Y + i))
                    {
                        return true;
                    }
            return false;
        }

        protected void ShowMatrix(int[][] map)
        {
            Console.Clear();
            for (int line = 0; line < General.TilesVertically; line++)
            {
                for (int col = 0; col < General.TilesHorizontaly; col++)
                {
                    string playerMatrixCell = "";
                        playerMatrixCell = map[line][col].ToString() + "|";
                    Console.Write(String.Format("{0,8}", playerMatrixCell ));

                }
                Console.WriteLine();
            }
            Console.WriteLine("--------------------------------------------------------------");
        }
    }

    public class Dragon:Sprite
    {
        // private fields
        double LastAttackTime = 0;  // in seconds
        Point current = new Point(0,0);
        // public fields
        public const int HealthMax = 100; // 100 points of health
        public int Health = HealthMax;

        public PathToAttack pathToAttack;

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
            StepX = 32;
            StepY = 32;
            Position = new Vector2(StepY, StepX);//new Vector2(2 * rnd.Next(5, 9) * General.TileSize, 2* rnd.Next(3, 6) * General.TileSize);
            Velocity = Vector2.Zero;
            Direction = General.eDirection.Idle;
            LastDirection = General.eDirection.Left;
        }
        public bool CanMove(Point next)
        {
            Direction = General.eDirection.Idle;
            if (next.X - current.X != 0) 
            Direction = next.X < current.X ? General.eDirection.Up:
                                             General.eDirection.Down;
            if (next.Y - current.Y != 0)
            Direction = next.Y < current.Y ? General.eDirection.Left:
                                             General.eDirection.Right;
            if (Direction == General.eDirection.Idle)
            {
                return false;
            }

            int line = Direction == General.eDirection.Down ?
                                      (int)((Position.Y + (StepY + Velocity.Y - 1)) / StepX)
                                      : (int)((Position.Y + Velocity.Y) / StepY);
            int col = Direction == General.eDirection.Right ?
                                        (int)((Position.X + (StepX + Velocity.X - 1)) / StepX)
                                        : (int)((Position.X + Velocity.X) / StepX);

            if (pathToAttack.Map[line][col] >= 100)
            {
                return true;
            }
            else return false;
        }
        public bool Collides()
        {
            foreach (var destination in TMD.Sprites.Values)
            {
                if (destination is Player)
                {
                    if (this.CollisionWith(destination))
                    {
                        this.Stop();
                        return true;
                    }
                }
            }
            return false;
        }
        public override void Move()
        {
            base.Move();

            Velocity = Vector2.Zero;
            if (Direction == General.eDirection.Up)
            {
                Speed = General.GameSpeed;
                Velocity.Y = -Speed;
            }
            if (Direction == General.eDirection.Down)
            {
                Speed = General.GameSpeed;
                Velocity.Y = Speed;
            }
            if (Direction == General.eDirection.Left)
            {
                Speed = General.GameSpeed;
                Velocity.X = -Speed;
            }
            if (Direction == General.eDirection.Right)
            {
                Speed = General.GameSpeed;
                Velocity.X = Speed;
            }
        }

        public override void UpdatePosition()
        {
            base.UpdatePosition();
            int dragonLine = (int)(Position.Y / General.TileSize);
            int dragonCol = (int)(Position.X / General.TileSize);
            int playerLine = (int)(TMD.Sprites["player"].Position.Y / General.TileSize);
            int playerCol = (int)(TMD.Sprites["player"].Position.X / General.TileSize);
            current = new Point(dragonLine, dragonCol);

            if (Direction == General.eDirection.Idle)
            {
                // find next path
                if (_gameTime.TotalGameTime.Seconds % 3 == 1)
                {
                    pathToAttack = new PathToAttack(current, new Point(playerLine, playerCol), TMD.GroundMap);
                    ShowMatrix(pathToAttack.Map);
                }
                  
            }
            if (pathToAttack != null) {
                if (pathToAttack.Path == null)
                {
                    // dragon is free of player area
                }
                else if (!Collides() &&
                    pathToAttack.Path.Count > 1 &&
                    CanMove(pathToAttack.Path[1]))
                {
                        Move();
                        Position += Velocity;
                }
            }

        }


        // if path of eliberation was hit = dragon stands on moutain than release the dragon to the mountain animation

    }
}
