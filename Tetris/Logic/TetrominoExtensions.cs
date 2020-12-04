using System;

namespace TetrisClient.Logic
{
	public static class TetrominoExtensions
	{
        //TODO: hardcode
        public static int GetLength(this Tetromino figure, int angle)
        {
            throw new NotImplementedException();
        }

        //TODO: hardcode
        public static int[] GetBottom(this Tetromino figure, int angle)
        {
            throw new NotImplementedException();
        }

        //TODO: hardcode
        public static int[] GetHeight(this Tetromino figure, int angle)
        {
            throw new NotImplementedException();
        }

        //TODO: hardcode
        public static int GetRotationsNumber(this Tetromino figure, Combination combination)
        {
            //var type = 10 * (int)figure + (int)combination;

            //switch (type) 
            //{
            //    10 * (int)Tetromino.O + (int)Combination.O => 0,
            //    10 * (int)Tetromino.T + (int)Combination.DU => 0,
            //    10 * (int)Tetromino.T + (int)Combination.U => 1,
            //    10 * (int)Tetromino.T + (int)Combination.OO => 2,
            //    10 * (int)Tetromino.T + (int)Combination.D => 3,
            //    //...
            //    _ => 5
            //}

            throw new NotImplementedException();
        }
    }
}