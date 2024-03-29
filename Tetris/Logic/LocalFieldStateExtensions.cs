﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace TetrisClient.Logic
{
	public static class LocalFieldStateExtensions
	{

		private const int WeightLineRemoved = 20;
		private const int WeightHole = 20;
		private const int СoefficientMetric1 = 1;
		private const int СoefficientMetric2 = 1;
		private const int СoefficientMetric3 = 2;


        private static bool[,] FiguresCombinations =
        { 
            //        OO     OU      OD     UO     DO     DU     UU     DD     U      D      O     All
            /*O*/   {false, false, false, false, false, false, false, false, false, false, true,  false},
            /*I*/   {false, false, false, false, false, false, false, false, false, false, false, false},
            /*S*/   {false, true,  false, false, false, false, false, false, false, true,  false, false},
            /*Z*/   {false, false, false, false, true,  false, false, false, true,  false, false, false},
            /*J*/   {true,  false, true,  false, false, false, true,  false, false, false, true,  false},
            /*L*/   {true,  false, false, true,  false, false, false, true,  false, false, true,  false},
            /*T*/   {true,  false, false, false, false, true,  false, false, true,  true,  false, false},
            /*All*/ {false, false, false, false, false, false, false, false, false, false, false, false}
        };

        public static Command GetCommand(this LocalFieldState currentState, List<Tetromino> nextFigures)
        {
            var resultFieldState = currentState.ProcessNextTetromino(nextFigures, 0);
            var rotationsNumber = resultFieldState.FigureAngle;
            var stepsNumber = resultFieldState.FigureCoordinate - currentState.FigureCoordinate + nextFigures.First().GetAdditionalStepsAfterRotations(resultFieldState.FigureAngle);
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

		private static Combination GetOneCellCombination(int relativeHeight)
        {
            switch (relativeHeight)
            {
				case 2:
                    return Combination.UU;
				case -2:
                    return Combination.DD;
				case 1:
                    return Combination.U;
				case -1:
                    return Combination.D;
				case 0:
                    return Combination.O;
				default:
                    return Combination.All;
			}
		}

        private static Combination GetTwoCellCombination(int relativeHeight1, int relativeHeight2)
		{
            switch (relativeHeight1)
            {
                case 0:
                {
                    switch (relativeHeight2)
                    {
						case 0:
                            return Combination.OO;
						case 1:
                            return Combination.OU;
						case -1:
                            return Combination.OD;
						default:
                            return Combination.All;
                    }
                }
                case 1:
                {
					switch (relativeHeight2)
                    {
                        case 0:
                            return Combination.UO;
                        default:
                            return Combination.All;
                    }
				}
                case -1:
                {
					switch (relativeHeight2)
                    {
                        case 0:
                            return Combination.DO;
                        case 1:
                            return Combination.DU;
						default:
                            return Combination.All;
                    }
				}
                default:
                    return Combination.All;
            }
		}

        private static LocalFieldState ProcessNextTetromino(this LocalFieldState currentState, List<Tetromino> nextFigures, int level) // level - уровень рекурсии, номер обрабатываемой фигуры
        {
            var fieldStateOptions = currentState.GetFieldStateOptions(nextFigures[level]);
            var result = new LocalFieldState();

			if (fieldStateOptions.Count == 1)
            {
                result = fieldStateOptions.First();
				result.UpdateWeight();

                return result;
			}

			//if (level != nextFigures.Count - 1)
			if (false)
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

            foreach (var option in fieldStateOptions)
            {
                if (option.Weight > result.Weight)
                {
                    result = option;
                }
            }

            return result;
        }

        private static void UpdateWeight(this LocalFieldState localFieldState)
        {
            // for debug

			localFieldState.Weight += СoefficientMetric1*localFieldState.GetMetric1();
            //localFieldState.Weight += СoefficientMetric2*localFieldState.GetMetric2();
            //localFieldState.Weight += СoefficientMetric3*localFieldState.GetMetric3();
        }

        private static void Drop(this LocalFieldState localFieldState, Tetromino figure)
		{
			var tetrominoBottom = figure.GetBottom(localFieldState.FigureAngle);
			var tetraminoFloor = localFieldState.ColumnsHeight[localFieldState.FigureCoordinate]; //IndexOutOfBoundException

			for (var i = 1; i < figure.GetLength(localFieldState.FigureAngle); i++)
			{
				if (localFieldState.ColumnsHeight[localFieldState.FigureCoordinate + i] > tetraminoFloor + tetrominoBottom[i])
				{
					tetraminoFloor = localFieldState.ColumnsHeight[localFieldState.FigureCoordinate + i] - tetrominoBottom[i];
				}
			}

			var tetrominoHeight = figure.GetHeight(localFieldState.FigureAngle);

			for (var i = 0; i < figure.GetLength(localFieldState.FigureAngle); i++)
			{
				while (localFieldState.ColumnsHeight[localFieldState.FigureCoordinate + i] < tetraminoFloor + tetrominoBottom[i])
                {
                    localFieldState.Holes.Add(new Point(localFieldState.FigureCoordinate + i,
                        localFieldState.ColumnsHeight[localFieldState.FigureCoordinate + i]));
                    localFieldState.ColumnsHeight[localFieldState.FigureCoordinate + i]++;
				}

				localFieldState.ColumnsHeight[localFieldState.FigureCoordinate + i] += tetrominoHeight[i];
			}
        }

		private static List<LocalFieldState> GetOptionsLine(this LocalFieldState currentState, Tetromino figure)
		{
			var options = new List<LocalFieldState>();
			var holes = 1000;

			for (var line = currentState.ColumnsHeight.Max() - 1; line >= currentState.ColumnsHeight.Min(); line--)
			{
				var length = 0;
				var stop = false;
				var space = false;
				var distance = -1;

				for (var j = currentState.FieldWidth - 1; j >= 0; j--)
				{
                    if (length > 4)
                    {
                        stop = true;
                        break;
                    }
                    else if (!space && length == 0 && currentState.ColumnsHeight[j] < line)
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

				if (stop || length == 0)
				{
					break;
				}

				distance -= length + 1;

				for (var angle = 0; angle < 4; angle++)
                {
                    var localFieldState = currentState.Clone();
                    localFieldState.FigureAngle = angle;

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

						if(localFieldState.FigureAngle == 4)
						{
							continue;
						}

                        var tetrominoHeight = figure.GetHeight(localFieldState.FigureAngle);

						for (var j = 0; j < tetrominoHeight.Length; j++)
						{
							localFieldState.ColumnsHeight[i + j] += tetrominoHeight[j];
						}

						localFieldState.FigureCoordinate = i;

						if (/*!currentState.IsITetrominoFound ||*/ tetrominoHeight.Length + localFieldState.FigureCoordinate <= currentState.FieldWidth - 1 || options.Count == 0)  // ATTENTION -1????
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
				
				for (var distance = 0; distance < currentState.FieldWidth - figure.GetLength(angle) + 1; distance++)
				{
                    var localFieldState = currentState.Clone();
                    localFieldState.FigureAngle = angle;
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

        private static int GetMinColumnHeightExceptLastRight(List<int> columnsHeight)
        {
            var min = columnsHeight.First();

            for (var i = 1; i < columnsHeight.Count - 1; i++)
            {
                if (columnsHeight[i] < min)
                {
                    min = columnsHeight[i];
                }
            }

            return min;
        }

		private static List<LocalFieldState> GetFieldStateOptions(this LocalFieldState currentState, Tetromino figure)
		{
			var options = new List<LocalFieldState>();

			//if (figure == Tetromino.I && currentState.ColumnsHeight[currentState.FieldWidth - 1] == 0 && GetMinColumnHeightExceptLastRight(currentState.ColumnsHeight) >= 4)
   //         {
   //             var localFieldState = currentState.Clone();
   //             localFieldState.FigureCoordinate = currentState.FieldWidth - 1;
   //             localFieldState.FigureAngle = 0;

			//	for (var i = 3; i >= 0; i--)
			//	{
			//		if (!localFieldState.Holes.Exists(hole => hole.Y == i)) //в строке i нет дырки
			//		{
   //                     localFieldState.RemoveLine(i);
			//		}
			//	}

			//	options.Add(localFieldState);

			//	return options;
			//}

   //         var searchedCombinations = false;

			//if (currentState.ColumnsHeight[currentState.FieldWidth - 1] == 0 && currentState.ColumnsHeight.Max() <= 8 /*|| !currentState.IsITetrominoFound*/)
   //         {
   //             searchedCombinations = true;
   //             options = currentState.GetOptionsCombs(figure);
   //         }

			//if (options.Count == 0)
			//{
			//	options = currentState.GetOptionsLine(figure);
			//}

			//if (options.Count == 0 && !searchedCombinations)
			//{
			//	options = currentState.GetOptionsCombs(figure);
   //         }

			if (options.Count == 0)
			{
				options = currentState.GetOptionsHoles(figure);
			}

			return options;
		}

        private static double GetMetric1(this LocalFieldState localFieldState)
        {
            return localFieldState.FieldHeight - localFieldState.ColumnsHeight.Max();
        }

        private static double GetMetric2(this LocalFieldState localFieldState)
		{
            double weight = 0;

			for (var i = 0; i < localFieldState.FieldWidth - 1; i++)
			{
				weight += Math.Abs(localFieldState.ColumnsHeight[i] - localFieldState.ColumnsHeight[i + 1]);
			}

			return 50 - weight; //нормирование
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
				if(i != (int)Tetromino.I)
				{
					if(combinationsNumber[i] != 0)
					{
						weight += 20 -  10 /(Math.Pow(2, (double)combinationsNumber[i]- 1)); //арифметическая прогрессия b1 = 10 q = 2/3 n = CombinationsNumber[i]
					}

                    weight -= 13;
                }
			}

			return weight;
		}

        private static void RemoveLine(this LocalFieldState localFieldState, int line)
        {
            localFieldState.Holes = localFieldState.Holes.Select(hole => hole.Y > line ? new Point(hole.X, hole.Y - 1) : hole).ToList();

            for (var i = 0; i < localFieldState.FieldWidth; i++) // параллелить
            {
                if (localFieldState.ColumnsHeight[i] - 1 >= line)
                {
                    localFieldState.ColumnsHeight[i]--;

                    if (localFieldState.ColumnsHeight[i] - 1 == line - 1)
                    {
                        while (localFieldState.Holes.RemoveAll(hole =>
                            hole.X == i && hole.Y == localFieldState.ColumnsHeight[i] - 1 ) > 0)
                        {
                            localFieldState.ColumnsHeight[i]--;
                        }
                    }
                }
            }
        }
    }
}