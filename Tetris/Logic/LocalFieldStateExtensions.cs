using System;
using System.Collections.Generic;
using System.Linq;

namespace TetrisClient.Logic
{
	public static class LocalFieldStateExtensions
	{
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

        //TODO: hardcode
        private static bool[,] FiguresCombinations =
        {
            {true, false},
            {false, true}
        };

        //FiguresCombinations[(int)Tetromino.O, (int)Combination.O] = true;
        //FiguresCombinations[(int)Tetromino.T, (int)Combination.DU] = true;
        //FiguresCombinations[(int)Tetromino.T, (int)Combination.U] = true;
        //FiguresCombinations[(int)Tetromino.T, (int)Combination.D] = true;
        //FiguresCombinations[(int)Tetromino.T, (int)Combination.OO] = true;

        //TODO: hardcode
        private static Combination GetOneCellCombination(int relativeHeight)
		{
			throw new NotImplementedException();
		}

        //TODO: hardcode
        private static Combination GetTwoCellCombination(int relativeHeight1, int relativeHeight2)
		{
			throw new NotImplementedException();
		}

        private static LocalFieldState ProcessNextTetromino(this LocalFieldState currentState, Tetromino[] nextFigures, int level) // level - уровень рекурсии, номер обрабатываемой фигуры
        {
            var fieldStateOptions = currentState.GetFieldStateOptions(nextFigures[level]);

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

        private static void UpdateWeight(this LocalFieldState localFieldState)
        {
            localFieldState.Weight += localFieldState.GetMetric1();
            localFieldState.Weight += localFieldState.GetMetric2();
            localFieldState.Weight += localFieldState.GetMetric3();
        }

        private static void Drop(this LocalFieldState localFieldState, Tetromino figure)
		{
			var tetrominoBottom = figure.GetBottom(localFieldState.FigureAngle);
			var tetraminoFloor = localFieldState.ColumnsHeight[localFieldState.FigureCoordinate] + 1;

			for (var i = 1; i < figure.GetLength(localFieldState.FigureAngle); i++)
			{
				if (localFieldState.ColumnsHeight[localFieldState.FigureCoordinate + i] + 1 > tetraminoFloor + tetrominoBottom[i])
				{
					tetraminoFloor = localFieldState.ColumnsHeight[localFieldState.FigureCoordinate + i] + 1 - tetrominoBottom[i];
				}
			}

			var tetrominoHeight = figure.GetHeight(localFieldState.FigureAngle);

			for (var i = 0; i < figure.GetLength(localFieldState.FigureAngle); i++)
			{
				while (localFieldState.ColumnsHeight[localFieldState.FigureCoordinate + i] + 1 < tetraminoFloor + tetrominoBottom[i])
                {
                    localFieldState.Holes.Add(new Point(localFieldState.FigureCoordinate + i,
                        localFieldState.ColumnsHeight[localFieldState.FigureCoordinate + i] + 1));
                    localFieldState.ColumnsHeight[localFieldState.FigureCoordinate + i]++;
				}

				localFieldState.ColumnsHeight[localFieldState.FigureCoordinate + i] += tetrominoHeight[i];
			}
        }

		private static List<LocalFieldState> GetOptionsLine(this LocalFieldState currentState, Tetromino figure)
		{
			var options = new List<LocalFieldState>();
			var holes = 1000;

			for (var line = currentState.ColumnsHeight.Max(); line >= currentState.ColumnsHeight.Min(); line--)
			{
				var length = 0;
				var stop = false;
				var space = false;
				var distance = -1;

				for (var j = currentState.FieldWidth - 1; j >= 0; j--)
				{
					if (!space && length == 0 && currentState.ColumnsHeight[j] < line)
					{
						distance = j;
						length++;
						space = true;
					}
					else if (space && currentState.ColumnsHeight[j] < line)
					{
						length++;
					}
					else if (space && currentState.ColumnsHeight[j] >= line)
					{
						space = false;
					}
					else if (!space && currentState.ColumnsHeight[j] < line && length != 0)
					{
						stop = true;
						break;
					}
				}

				if (stop)
				{
					break;
				}

				distance -= length;

				for (var angle = 0; angle < 4; angle++)
                {
                    var localFieldState = currentState.Clone();

					if (figure.GetLength(localFieldState.FigureAngle) == length)
					{
                        localFieldState.FigureCoordinate = distance;

                        localFieldState.Drop(figure);

						var lineRemoved = false;

						for (var i = 0; i < 4; i++)
                        {
                            stop = false;

							if (localFieldState.Holes.Exists(hole => hole.Y == line - i))
							{
								continue;
							}

							for (var k = 0; k < length; k++)
							{
								if (localFieldState.ColumnsHeight[localFieldState.FigureCoordinate + k] < line - i)
								{
									stop = true;
									break;
								}
							}

							if (!stop)
							{
                                localFieldState.RemoveLine(line - i);
                                lineRemoved = true;
                            }
						}

						if (!lineRemoved)
						{
							break;
						}

                        if (localFieldState.Holes.Count < holes)
						{
							options.Clear();
							holes = localFieldState.Holes.Count;
							options.Add(localFieldState);
						}
						else if (localFieldState.Holes.Count == holes)
						{
							options.Add(localFieldState);
						}
					}
				}
			}

			return options;
		}

        private static List<LocalFieldState> GetOptionsCombs(this LocalFieldState currentState, Tetromino figure)
		{
			var options = new List<LocalFieldState>();

			if (figure == Tetromino.I)
			{
				return options;
			}

			for (var i = 0; i < currentState.FieldWidth - 1; i++)
            {
                var combinations = new List<Combination>
                {
                    GetOneCellCombination(currentState.ColumnsHeight[i + 1] - currentState.ColumnsHeight[i])
                };

                if (i < currentState.FieldWidth - 2)
                {
                    combinations.Add(GetTwoCellCombination(
                        currentState.ColumnsHeight[i + 1] - currentState.ColumnsHeight[i],
                        currentState.ColumnsHeight[i + 2] - currentState.ColumnsHeight[i + 1]));
                }

                foreach (var comb in combinations)
                {
					if (FiguresCombinations[(int)figure, (int)comb])
					{
                        var localFieldState = currentState.Clone();
                        localFieldState.FigureAngle = figure.GetRotationsNumber(comb);

                        var tetrominoHeight = figure.GetHeight(localFieldState.FigureAngle);

						for (var j = 0; i < tetrominoHeight.Length; i++)
						{
							localFieldState.ColumnsHeight[i + j] += tetrominoHeight[j];
						}

						localFieldState.FigureCoordinate = i;

						if (tetrominoHeight.Length + localFieldState.FigureCoordinate < currentState.FieldWidth - 1 || options.Count == 0)
						{
							options.Add(localFieldState);
						}
					}
				}
			}

			return options;
		}

		private static List<LocalFieldState> GetOptionsHoles(this LocalFieldState currentState, Tetromino figure)
		{
			var options = new List<LocalFieldState>();
			var holes = 1000;

			for (var angle = 0; angle < 4; angle++)
			{
				if (figure.GetLength(angle) == 0)
				{
					continue;
				}

                var localFieldState = currentState.Clone();
                localFieldState.FigureAngle = angle;

				for (var distance = 0; distance < currentState.FieldWidth - figure.GetLength(localFieldState.FigureAngle); distance++)
				{
                    localFieldState.FigureCoordinate = distance;
                    localFieldState.Drop(figure);

					if (localFieldState.Holes.Count < holes)
					{
						options.Clear();
						holes = localFieldState.Holes.Count;
						options.Add(localFieldState);
					}
					else if (localFieldState.Holes.Count == holes)
					{
						options.Add(localFieldState);
					}
				}
			}

			return options;
		}

		private static List<LocalFieldState> GetFieldStateOptions(this LocalFieldState currentState, Tetromino figure)
		{
			var options = new List<LocalFieldState>();

			if (figure == Tetromino.I && currentState.ColumnsHeight[currentState.FieldWidth - 1] == 0 && currentState.ColumnsHeight.Min() >= 3)
            {
                var localFieldState = currentState.Clone(); // проверить, что корректно клонируется
                localFieldState.FigureCoordinate = currentState.FieldWidth - 1;
                localFieldState.FigureAngle = 0;

				for (var i = 3; i >= 0; i--)
				{
					if (!localFieldState.Holes.Exists(hole => hole.Y == i)) //в строке i нет дырки
					{
                        localFieldState.RemoveLine(i);
					}
				}

				options.Add(localFieldState);

				return options;
			}

            bool searchedCombinations = false;

			if (currentState.ColumnsHeight[currentState.FieldWidth - 1] == 0 && currentState.ColumnsHeight.Max() <= 10)
            {
                searchedCombinations = true;
                options = currentState.GetOptionsCombs(figure);
            }

			if (options.Count == 0)
			{
				options = currentState.GetOptionsLine(figure);
			}

			if (options.Count == 0 && !searchedCombinations)
			{
				options = currentState.GetOptionsCombs(figure);
            }

			if (options.Count == 0)
			{
				options = currentState.GetOptionsHoles(figure);
			}

			return options;
		}

        private static double GetMetric1(this LocalFieldState localFieldState)
        {
            return localFieldState.FieldHeight / 2 - localFieldState.ColumnsHeight.Max();
        }

        private static double GetMetric2(this LocalFieldState localFieldState)
		{
            double weight = 0;

			for (var i = 0; i < localFieldState.FieldWidth - 1; i++)
			{
				weight = Math.Abs(localFieldState.ColumnsHeight[i] - localFieldState.ColumnsHeight[i + 1]);
			}

			return weight - 12; //нормирование
		}

		private static double GetMetric3(this LocalFieldState localFieldState)
		{
			var combinationsNumber = new int[(int)Tetromino.All];

			for (var i = 0; i < localFieldState.FieldWidth - 1; i++)
			{
				var comb = GetOneCellCombination(localFieldState.ColumnsHeight[i] - localFieldState.ColumnsHeight[i + 1]);

				for (var j = 0; j < (int)Tetromino.All; j++)
				{
					if (FiguresCombinations[j, (int)comb])
					{
						combinationsNumber[j]++;
					}
				}

				if (i < localFieldState.FieldWidth - 2)
				{
					comb = GetTwoCellCombination(localFieldState.ColumnsHeight[i + 1] - localFieldState.ColumnsHeight[i],
						localFieldState.ColumnsHeight[i + 2] - localFieldState.ColumnsHeight[i + 1]);

					for (var j = 0; j < (int)Tetromino.All; j++)
					{
						if (FiguresCombinations[j, (int)comb])
						{
							combinationsNumber[j]++;
						}
					}
				}
			}

            double weight = 0;

			for (var i = 0; i < (int)Tetromino.All; i++)
			{
				weight += (30 * Math.Pow(2.0, (double)combinationsNumber[i])) /
					(Math.Pow(3, (double)combinationsNumber[i])) - 30; //арифметическая прогрессия b1 = 10 q = 2/3 n = CombinationsNumber[i]
				weight -= 15; // нормирование 
			}

			return weight;
		}

        private static void RemoveLine(this LocalFieldState localFieldState, int line)
        {
            localFieldState.Holes = localFieldState.Holes.Select(hole => hole.Y > line ? new Point(hole.X, hole.Y - 1) : hole).ToList();

            for (var i = 0; i < localFieldState.FieldWidth; i++) // параллелить
            {
                if (localFieldState.ColumnsHeight[i] >= line)
                {
                    localFieldState.ColumnsHeight[i]--;

                    if (localFieldState.ColumnsHeight[i] == line - 1)
                    {
                        while (localFieldState.Holes.RemoveAll(hole =>
                            hole.X == i && hole.Y == localFieldState.ColumnsHeight[i]) > 0)
                        {
                            localFieldState.ColumnsHeight[i]--;
                        }
                    }
                }
            }
        }
    }
}