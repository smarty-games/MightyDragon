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
        public enum ePlayerType
        {
            Player = 0,
            Dragon = 1
        }
        public const eMoveType CharacterMoveType = eMoveType.ToColission;
        internal static float GameSpeed = 2f;
        public static int TotalAttackLoops = 2;
    }
}
