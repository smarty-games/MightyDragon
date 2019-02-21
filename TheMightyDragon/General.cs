using System;
namespace Desktop
{

    public static class General
    {
        public enum MoveType
        {
            ToColission = 0,
            ToTile = 1
        }
        public enum Direction
        {
            Idle = 0,
            Left = 1,
            Down = 2,
            Right = 3,
            Up = 4
        }
        public const MoveType CharacterMoveType = MoveType.ToColission;
        internal static float GameSpeed = 2f;
    }
}
