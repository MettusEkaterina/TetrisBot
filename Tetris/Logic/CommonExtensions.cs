using System;

namespace TetrisClient.Logic
{
    public static class CommonExtensions
    {
        public static Tetromino ToTetromino(this Element element)
        {
            switch (element)
            {
                case Element.I:
                    return Tetromino.I;
                case Element.J:
                    return Tetromino.J;
                case Element.L:
                    return Tetromino.L;
                case Element.O:
                    return Tetromino.O;
                case Element.S:
                    return Tetromino.S;
                case Element.T:
                    return Tetromino.T;
                case Element.Z:
                    return Tetromino.Z;
                default:
                    return Tetromino.All;
            }
        }

        public static T Clone<T>(this T source)
        {
            return Activator.CreateInstance<T>();
        }
    }
}