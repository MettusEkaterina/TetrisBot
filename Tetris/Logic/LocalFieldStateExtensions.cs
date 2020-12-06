using System;
using System.Collections.Generic;
using System.Linq;

namespace TetrisClient.Logic
{
	public static class LocalFieldStateExtensions
    {
        private const double MaxSurfaceHeightMetricСoefficient = 1;
		private const double SmoothnessMetricСoefficient = 1; //0.6
		private const double FutureCombinationsMetricСoefficient = 1;
        private const int WeightLineRemoved = 0; //20;
        private const int WeightHole = 0; //20;

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
        
        public static Command GetCommand(this LocalFieldState currentState, List<Tetromino> nextFigures, bool tooLongCalculation)
        {
            var tooManySameFigures = IsTooManySameFigures(nextFigures);
            var resultFieldState = currentState.ProcessNextTetromino(nextFigures, 0, tooManySameFigures, tooLongCalculation);
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

        private static LocalFieldState ProcessNextTetromino(this LocalFieldState currentState, List<Tetromino> nextFigures, int level, bool tooManySameFigures, bool tooLongCalculation) // level - уровень рекурсии, номер обрабатываемой фигуры
        {
            var fieldStateOptions = currentState.GetFieldStateOptions(nextFigures[level]);
            var result = new LocalFieldState();

			if (fieldStateOptions.Count == 1)
            {
                result = fieldStateOptions.First();
				result.UpdateWeight();

                return result;
			}
            
            if (!(tooManySameFigures || tooLongCalculation) && level != 3) 
            {
                foreach (var option in fieldStateOptions) // параллелить
                {
                    option.Weight = option.ProcessNextTetromino(nextFigures, level + 1, tooManySameFigures, tooLongCalculation).Weight;
                }
            }
            else if (tooLongCalculation && level != 1 || (tooManySameFigures && nextFigures.First() != Tetromino.O && level != 2))
            {
                foreach (var option in fieldStateOptions) // параллелить
                {
                    option.Weight = option.ProcessNextTetromino(nextFigures, level + 1, tooManySameFigures, tooLongCalculation).Weight;
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
            localFieldState.Weight += MaxSurfaceHeightMetricСoefficient * localFieldState.GetMaxSurfaceHeightMetric();
            localFieldState.Weight += SmoothnessMetricСoefficient * localFieldState.GetSmoothnessMetric();
            localFieldState.Weight += FutureCombinationsMetricСoefficient * localFieldState.GetFutureCombinationsMetric();
        }
        
        private static int Drop(this LocalFieldState localFieldState, Tetromino figure)
		{
            var lowestTetraminoCell = figure.GetLowestTetraminoCell(localFieldState);
            var tetrominoBottom = figure.GetBottom(localFieldState.FigureAngle);
            var tetrominoHeight = figure.GetHeight(localFieldState.FigureAngle);
            var maxProducedColumnHeight = -1;

			for (var i = 0; i < figure.GetLength(localFieldState.FigureAngle); i++)
			{
				while (localFieldState.ColumnsHeight[localFieldState.FigureCoordinate + i] < lowestTetraminoCell + tetrominoBottom[i])
                {
                    localFieldState.Holes.Add(new Point(localFieldState.FigureCoordinate + i,
                        localFieldState.ColumnsHeight[localFieldState.FigureCoordinate + i]));
                    localFieldState.ColumnsHeight[localFieldState.FigureCoordinate + i]++;
                    localFieldState.Weight -= WeightHole;
                }

				localFieldState.ColumnsHeight[localFieldState.FigureCoordinate + i] += tetrominoHeight[i];

                if (localFieldState.ColumnsHeight[localFieldState.FigureCoordinate + i] > maxProducedColumnHeight)
                {
                    maxProducedColumnHeight = localFieldState.ColumnsHeight[localFieldState.FigureCoordinate + i];
                }
			}

            return maxProducedColumnHeight;
        }
        
		private static List<LocalFieldState> GetOptions(this LocalFieldState currentState, Tetromino figure)
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
                    var maxProducedColumnHeight = localFieldState.Drop(figure);

                    for (var i = 0; i < 4; i++)
                    {
                        var line = maxProducedColumnHeight - i - 1;

                        if (localFieldState.Holes.Exists(hole => hole.Y == line) || line > localFieldState.ColumnsHeight.Min() - 1)
                        {
                            continue;
                        }

                        localFieldState.RemoveLine(line);
                        localFieldState.Weight += WeightLineRemoved;
                    }

                    var tetrominoLength = figure.GetLength(localFieldState.FigureAngle);

                    if (!localFieldState.IsITetrominoFound)
                    {
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
                    else if (currentState.ColumnsHeight.Max() > 12 || 
                             tetrominoLength + localFieldState.FigureCoordinate <= currentState.FieldWidth - 1 || 
                             options.Count == 0)
                    {
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
        
		private static List<LocalFieldState> GetFieldStateOptions(this LocalFieldState currentState, Tetromino figure)
		{ 
            var options = new List<LocalFieldState>();
            var minColumnHeightExceptLastRight= GetMinColumnHeightExceptLastRight(currentState.ColumnsHeight);

            if (figure == Tetromino.I && currentState.ColumnsHeight[currentState.FieldWidth - 1] < minColumnHeightExceptLastRight && minColumnHeightExceptLastRight >= 4)
            {
                var localFieldState = currentState.Clone();
                localFieldState.FigureCoordinate = currentState.FieldWidth - 1;
                localFieldState.FigureAngle = 0;

                for (var i = 3; i >= 0; i--)
                {
                    if (!localFieldState.Holes.Exists(hole => hole.Y == i))
                    {
                        localFieldState.RemoveLine(i);
                    }
                }

                options.Add(localFieldState);

                return options;
            }

            options = currentState.GetOptions(figure);
            
            return options;
		}

        private static double GetMaxSurfaceHeightMetric(this LocalFieldState localFieldState)
        {
            return localFieldState.FieldHeight - localFieldState.ColumnsHeight.Max();
        }

        private static double GetSmoothnessMetric(this LocalFieldState localFieldState)
		{
            double weight = 0;

			for (var i = 0; i < localFieldState.FieldWidth - 1; i++)
			{
				weight += Math.Abs(localFieldState.ColumnsHeight[i] - localFieldState.ColumnsHeight[i + 1]);
			}

			return 50 - weight; //нормирование
		}

		private static double GetFutureCombinationsMetric(this LocalFieldState localFieldState)
        {
            double weight = 0;
            var combinationsNumber = localFieldState.GetCombinationsNumber();
            
			for (var i = 0; i < (int)Tetromino.All; i++)
			{
				if (i != (int)Tetromino.I)
				{
					if (combinationsNumber[i] != 0)
					{
						weight += 20 - 10 / (Math.Pow(2, (double)combinationsNumber[i] - 1)); //арифметическая прогрессия b1 = 10 q = 1/2 n = CombinationsNumber[i]
					}

                    weight -= 13; // нормализация
                }
			}

			return weight;
		}

        private static int[] GetCombinationsNumber(this LocalFieldState localFieldState)
        {
            var combinationsNumber = new int[(int)Tetromino.All];

            for (var i = 0; i < localFieldState.FieldWidth - 1; i++)
            {
                var comb = GetOneCellCombination(localFieldState.ColumnsHeight[i] - localFieldState.ColumnsHeight[i + 1]); // maybe reverse i and i+1

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

            return combinationsNumber;
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
                            localFieldState.Weight += WeightHole;
                        }
                    }
                }
            }
        }

        private static bool IsTooManySameFigures(List<Tetromino> nextFigures)
        {
            var tooManySameFigures = true;

            for (var i = 1; i < nextFigures.Count; i++)
            {
                if (nextFigures[i] != nextFigures[0])
                {
                    tooManySameFigures = false;
                    break;
                }
            }

            return tooManySameFigures;
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
    }
}