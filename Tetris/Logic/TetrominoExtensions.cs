using System.Collections.Generic;
using System.Linq;

namespace TetrisClient.Logic
{
	public static class TetrominoExtensions
	{
		public static void UpdateWeight(this LocalFieldState localFieldState)
		{
			localFieldState.Weight += localFieldState.GetMetric1();
			localFieldState.Weight += localFieldState.GetMetric2();
			localFieldState.Weight += localFieldState.GetMetric3();
        }

	    public static LocalFieldState ProcessNextTetromino(this LocalFieldState currentState, Tetromino[] nextFigures, int level) // level - уровень рекурсии, номер обрабатываемой фигуры
	    {
            var fieldStateOptions = nextFigures[level].GetFieldStateOptions(currentState);

            //TODO: Refactor
		    if (level != nextFigures.Length - 1)
		    {
			    foreach (var option in fieldStateOptions) // параллелить
			    {
                    option.Weight = option.ProcessNextTetromino(nextFigures, level + 1).Weight;
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

		public static Command GetCommand(this LocalFieldState currentState, Tetromino[] nextFigures)
        {
			var resultFieldState = currentState.ProcessNextTetromino(nextFigures, 0);
            var rotationsNumber = resultFieldState.FigureAngle;
            var stepsNumber = resultFieldState.FigureCoordinate - currentState.FigureCoordinate;
            var commands = new List<Command>();

            for (var i = 0; i < rotationsNumber; i++)
            {
                commands.Add(Command.ROTATE_CLOCKWISE_90);
            }

            if (stepsNumber > 0)
            {
                for (var i = 0; i < stepsNumber; i++)
                {
                    commands.Add(Command.RIGHT);
                }
            }

            if (stepsNumber < 0)
            {
                stepsNumber = -stepsNumber;

                for (var i = 0; i < stepsNumber; i++)
                {
                    commands.Add(Command.LEFT);
                }
            }

            commands.Add(Command.DOWN);
            
            return commands.Aggregate((x, y) => x.Then(y));
        }

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
    }
}