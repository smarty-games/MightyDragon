using System;
namespace Desktop
{
    public static class General
    {
        public enum eMoveType
        {
            ToColission = 0,
            ToTile = 1
        }
        /// <summary>
        /// Map tiles legend
        /// </summary>
        public enum Legend  
        {
            Crater = 2,  // where the Dragon lives
            PlayerPath = 1,    // Player can walk on path only
            LeftMark = -1,  // put -1 in the left position of Player moving direction 
                            // CONDITIONS: Player has MoveInCrater action AND left Player matrix cell is not DangerPath

            RightMark = -2, // put -2 in the left position of Player moving direction CONDITION: Player has MoveInCrater action
                            // CONDITIONS: Player has MoveInCrater action AND right Player matrix cell is not DangerPath

            DragonPath = 3, // if PlayerMap[line,col] == 3 Player is vulnerable to Dragon attacks
            Mountain = 4,   // every time the Player surrounds a path in DragonPath that has one Eye(©), Player wins game points and disover the map underneath that shows him the way to the Princess
                            // one level leads to the next
            Dragon = 6,
            MountainPath = 44,
            AreaLeftMark = -100,
            AreaRightMark = -200
        }
        public enum eDirection
        {
            None = 0,
            Idle = 1,
            Left = 2,
            Down = 4,
            Right = 8,
            Up = 16,
            Attack = 32
        }
        public enum ePlayerAction
        {
            MoveInCrater = 0,
            MoveOnGround = 1
        }
        public static int GameSpeed = 2;
        public static int TileSize = 32;

        public const int TilesHorizontaly = 25;
        public const int TilesVertically = 15;
    }
}
