using System;
using System.Collections.Generic;

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

        public static LocalFieldState Clone(this LocalFieldState source)
        {
            var holes = new List<Point>();
            holes.AddRange(source.Holes);

            var columnsHeight = new List<int>();
            columnsHeight.AddRange(source.ColumnsHeight);

            return new LocalFieldState
            {
                FigureCoordinate = source.FigureCoordinate,
                FigureAngle = source.FigureAngle,
                ColumnsHeight = columnsHeight,
                Holes = holes,
                Weight = source.Weight,
                FieldHeight = source.FieldHeight,
                FieldWidth = source.FieldWidth,
                IsITetrominoFound = source.IsITetrominoFound
            };
        }
    }
}