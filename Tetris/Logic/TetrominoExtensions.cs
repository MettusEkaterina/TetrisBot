namespace TetrisClient.Logic
{
	public static class TetrominoExtensions
	{
        public static int GetLength(this Tetromino figure, int angle)
        {
            switch (figure)
            {
                case Tetromino.I:
                {
                    if (angle == 0)
                        return 1;
                    else if (angle == 1)
                        return 4;
                    else
                        return 0;
                }
                case Tetromino.J:
                case Tetromino.L:
                {
                    if (angle == 0 || angle == 2)
                        return 2;
                    else if (angle == 1 || angle == 3)
                        return 3;
                    else
                        return 0;
                }
                case Tetromino.O:
                {
                    if (angle == 0)
                        return 2;
                    else
                        return 0;
                }
                case Tetromino.S:
                case Tetromino.Z:
                {
                    if (angle == 0)
                        return 3;
                    else if (angle == 1)
                        return 2;
                    else
                        return 0;
                }
                case Tetromino.T:
                {
                    if (angle == 0 || angle == 2)
                        return 3;
                    else if (angle == 1 || angle == 3)
                        return 2;
                    else
                        return 0;
                }
                default:
                    return 0;
            }
        }

        public static int[] GetBottom(this Tetromino figure, int angle)
        {
            switch (figure)
            {
                case Tetromino.I:
                {
                    if (angle == 0)
                        return new int[] {0};
                    else if (angle == 1)
                        return new int[] {0, 0, 0, 0};
                    else
                        return null;
                }
                case Tetromino.J:
                {
                    if (angle == 0)
                        return new int[] {0, 0};
                    else if (angle == 1)
                        return new int[] {0, 0, 0};
                    else if (angle == 2)
                        return new int[] {0, 2};
                    else if (angle == 3)
                        return new int[] {0, 0, -1};
                    else
                        return null;
                }
                case Tetromino.L:
                {
                    if (angle == 0)
                        return new int[] {0, 0};
                    else if (angle == 1)
                        return new int[] {0, 1, 1};
                    else if (angle == 2)
                        return new int[] {0, -2};
                    else if (angle == 3)
                        return new int[] {0, 0, 0};
                    else
                        return null;
                }
                case Tetromino.O:
                {
                    if (angle == 0)
                        return new int[] {0, 0};
                    else
                        return null;
                }
                case Tetromino.S:
                {
                    if (angle == 0)
                        return new int[] {0, 0, 1};
                    else if (angle == 1)
                        return new int[] {0, -1};
                    else
                        return null;
                }
                case Tetromino.Z:
                {
                    if (angle == 0)
                        return new int[] {0, -1, -1};
                    else if (angle == 1)
                        return new int[] {0, 1};
                    else
                        return null;
                }
                case Tetromino.T:
                {
                    if (angle == 0)
                        return new int[] {0, 0, 0};
                    else if (angle == 1)
                        return new int[] {0, 1};
                    else if (angle == 2)
                        return new int[] {0, -1, 0};
                    else if (angle == 3)
                        return new int[] {0, -1};
                    else
                        return null;
                }
                default:
                    return null;
            }
        }

        public static int[] GetHeight(this Tetromino figure, int angle)
        {
            switch (figure)
            {
                case Tetromino.I:
                {
                    if (angle == 0)
                        return new int[] {4};
                    else if (angle == 1)
                        return new int[] {1, 1, 1, 1};
                    else
                        return null;
                }
                case Tetromino.J:
                {
                    if (angle == 0)
                        return new int[] {1, 3};
                    else if (angle == 1)
                        return new int[] {2, 1, 1};
                    else if (angle == 2)
                        return new int[] {3, 1};
                    else if (angle == 3)
                        return new int[] {1, 1, 2};
                    else
                        return null;
                }
                case Tetromino.L:
                {
                    if (angle == 0)
                        return new int[] {3, 1};
                    else if (angle == 1)
                        return new int[] {1, 1, 2};
                    else if (angle == 2)
                        return new int[] {1, 3};
                    else if (angle == 3)
                        return new int[] {2, 1, 1};
                    else
                        return null;
                }
                case Tetromino.O:
                {
                    if (angle == 0)
                        return new int[] {2, 2};
                    else
                        return null;
                }
                case Tetromino.S:
                case Tetromino.Z:
                {
                    if (angle == 0)
                        return new int[] {1, 2, 1};
                    else if (angle == 1)
                        return new int[] {2, 2};
                    else
                        return null;
                }
                case Tetromino.T:
                {
                    if (angle == 0 || angle == 2)
                        return new int[] {1, 2, 1};
                    else if (angle == 1)
                        return new int[] {3, 1};
                    else if (angle == 3)
                        return new int[] {1, 3};
                    else
                        return null;
                }
                default:
                    return null;
            }
        }

        public static int GetRotationsNumber(this Tetromino figure, Combination combination)
        {
            switch (combination)
            {
                case Combination.OO:
                {
                    if (figure == Tetromino.T)
                        return 0;
                    else if (figure == Tetromino.L)
                        return 3;
                    else if (figure == Tetromino.J)
                        return 1;
                    else
                        return 4;
                }
                case Combination.OU:
                {
                    if (figure == Tetromino.S)
                        return 0;
                    else
                        return 4;
                }
                case Combination.OD:
                {
                    if (figure == Tetromino.J)
                        return 3;
                    else
                        return 4;
                }
                case Combination.UO:
                {
                    if (figure == Tetromino.L)
                        return 1;
                    else
                        return 4;
                }
                case Combination.DO:
                {
                    if (figure == Tetromino.Z)
                        return 0;
                    else
                        return 4;
                }
                case Combination.DU:
                {
                    if (figure == Tetromino.T)
                        return 2;
                    else
                        return 4;
                }
                case Combination.UU:
                {
                    if (figure == Tetromino.J)
                        return 2;
                    else
                        return 4;
                }
                case Combination.DD:
                {
                    if (figure == Tetromino.L)
                        return 2;
                    else
                        return 4;
                }
                case Combination.U:
                {
                    if (figure == Tetromino.T)
                        return 1;
                    else if (figure == Tetromino.Z)
                        return 1;
                    else
                        return 4;
                }
                case Combination.D:
                {
                    if (figure == Tetromino.T)
                        return 3;
                    else if (figure == Tetromino.S)
                        return 1;
                    else
                        return 4;
                }
                case Combination.O:
                {
                    if (figure == Tetromino.O)
                        return 0;
                    else if (figure == Tetromino.L)
                        return 0;
                    else if (figure == Tetromino.J)
                        return 0;
                    else
                        return 4;
                }
                default:
                    return 4;
            }
        }

        public static int GetAdditionalStepsAfterRotations(this Tetromino figure, int angle)
        {
            switch (figure)
            {
                case Tetromino.I:
                {
                    if (angle == 1)
                        return 2;
                    else
                        return 0;
                }
                case Tetromino.J:
                {
                    if (angle == 2)
                        return 0;
                    else 
                        return 1;
                }
                case Tetromino.L:
                {
                    if (angle == 1 || angle == 2 || angle == 3)
                        return 1;
                    else
                        return 0;
                }
                case Tetromino.O:
                {
                    return 0;
                }
                case Tetromino.S:
                {
                    if (angle == 1)
                        return 0;
                    else
                        return 1;
                }
                case Tetromino.Z:
                {
                    if (angle == 1)
                        return 0;
                    else
                        return 1;
                }
                case Tetromino.T:
                {
                    if (angle == 1)
                        return 0;
                    else
                        return 1;
                }
                default:
                    return 0;
            }
        }
    }
}