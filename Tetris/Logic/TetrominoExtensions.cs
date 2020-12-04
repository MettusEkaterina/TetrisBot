using System;
using System.Linq;

namespace TetrisClient.Logic
{
	public static class TetrominoExtensions
	{
		private const int AreaLength = 18;

		public static void UpdateWeight(this LocalFieldState localFieldState)
		{
			localFieldState.Weight += localFieldState.GetMetric1();
			localFieldState.Weight += localFieldState.GetMetric2();
			localFieldState.Weight += localFieldState.GetMetric3();
        }

	    public static LocalFieldState ProcessNextTetromino(Tetromino[] nextFigures, LocalFieldState currentState, int level) // level - уровень рекурсии, номер обрабатываемой фигуры
	    {
            var fieldStateOptions = nextFigures[level].GetFieldStateOptions(currentState);

            //TODO: Refactor
		    if (level != nextFigures.Length - 1)
		    {
			    foreach (var option in fieldStateOptions) // параллелить
			    {
                    option.Weight = ProcessNextTetromino(nextFigures, option, level + 1).Weight;
			    }
		    }
		    else
		    {
			    foreach (var option in fieldStateOptions) // параллелить
			    {
				    option.UpdateWeight();
			    }
		    }

		    var result = new LocalFieldState();

		    foreach (var option in fieldStateOptions.Where(option => option.Weight > result.Weight))
		    {
                result = option;
		    }

		    return result;
	    }

		public static Step GetResultStep(this Tetromino[] nextFigures, LocalFieldState currentState)
        {
			var result = ProcessNextTetromino(nextFigures, currentState, 0);

            return result.GetResultStep();
        }

        public static Tetromino GetTetromino(this Element element)
        {
            switch (element)
            {
                case Element.BLUE:
                    return Tetromino.I;
                case Element.CYAN:
                    return Tetromino.J;
                case Element.ORANGE:
                    return Tetromino.L;
                case Element.YELLOW:
                    return Tetromino.O;
                case Element.GREEN:
                    return Tetromino.S;
                case Element.PURPLE:
                    return Tetromino.T;
                case Element.RED:
                    return Tetromino.Z;
                case Element.NONE:
                    return Tetromino.All;
                default:
                    throw new ArgumentOutOfRangeException(nameof(element), element, null);
            }
        }

        //public static int[] GetCoordinate(this Tetromino[] nextFigures, int angle)
        //{
        //    var result = figure.GetLocalCoordinate();

        //    result[0] = result[0] - AreaLength; //примерно
        //    result[1] = (result[1] + angle) % 4;

        //    return result;
        //}

        //TODO: Refactor and hardcode
        public static int GetRotationsNumber(this Tetromino figure, Combination combination)
        {
            var type = 10 * (int)figure + (int)combination;

            switch (type) //совсем не нравится расчет, придумаю, как лучше сделать
            {
                case 10 * (int)Tetromino.O + (int)Combination.O => 0,
                10 * (int)Tetromino.T + (int)Combination.DU => 0,
                10 * (int)Tetromino.T + (int)Combination.U => 1,
                10 * (int)Tetromino.T + (int)Combination.OO => 2,
                10 * (int)Tetromino.T + (int)Combination.D => 3,
                //...
                _ => 5
            };
        }

        //public static LocalFieldState CreateOption(this Tetromino figure, int angle) //hardcode
        //{
        //    var option = new LocalFieldState();

        //    switch (figure)
        //    {
        //        case Tetromino.O:
        //            if (angle == 0)
        //            {
        //                option.Length = 2;
        //                option.Height = new[] { 2, 2 };
        //                break;
        //            }
        //            else
        //            {
        //                return null;
        //            }

        //        case Tetromino.I:
        //            if (angle == 0)
        //            {
        //                option.Length = 1;
        //                option.Height = new[] { 4 };
        //                break;
        //            }
        //            else if (angle == 1)
        //            {
        //                option.Length = 4;
        //                option.Height = new[] { 1, 1, 1, 1 };
        //                break;
        //            }
        //            else
        //            {
        //                return null;
        //            }
        //        //...
        //        default:
        //            return null;
        //    }

        //    return option;
        //}
    }
}