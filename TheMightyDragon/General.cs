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
            None = 0,
            Idle = 1,
            Left = 2,
            Down = 4,
            Right = 8,
            Up = 16,
            Attack = 32
        }
        public const MoveType CharacterMoveType = MoveType.ToColission;
        internal static float GameSpeed = 2f;
        public static int TotalAttackLoops = 2;
    }
}
