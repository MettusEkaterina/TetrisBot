using System;
using System.Collections.Generic;

namespace TetrisClient.Logic
{
	public static class LocalFieldStateExtensions
	{

		
		private const int AreaHeight = 18;
		private const int AreaLength = 18;

		public static LocalFieldState CreateLocalFieldStateNext(LocalFieldState CurState)
		{
			LocalFieldState State = new LocalFieldState;
			State = CurState;
			return State;
		}

		public static int GetLenghtTetromino(Tetromino figure, int angle)
		{
			throw new NotImplementedException();
		}

		public static int[] GetFigureBottom(Tetromino figure, int angle)
		{
			throw new NotImplementedException();
		}

		public static int[] GetFiureHeight(Tetromino figure, int angle)
		{
			throw new NotImplementedException();
		}

		public static int GetOneCellCombination(int RelativeHeight)
		{
			throw new NotImplementedException();
		}

		public static int GetTowCellCombination(int RelativeHeight1, int RelativeHeight2)
		{
			throw new NotImplementedException();
		}

		
		public static void Drop(this LocalFieldState localFieldState, Tetromino figure)
		{
			int[] LoclBottom = GetFigureBottom(figure, localFieldState.FigureAngle);
			int Height = localFieldState.ColumnsHeight[localFieldState.FigureXCoordinate] + 1;

			for (int i = 1; i < GetLenghtTetromino(figure, localFieldState.FigureAngle); i++)
			{
				if (localFieldState.ColumnsHeight[localFieldState.FigureXCoordinate + i] + 1 > Height + LoclBottom[i])
				{
					Height = localFieldState.ColumnsHeight[localFieldState.FigureXCoordinate + i] + 1 - LoclBottom[i];
				}
			}

			int[] LoclHeight = GetFiureHeight(figure, localFieldState.FigureAngle);

			for (int i = 0; i < GetLenghtTetromino(figure, localFieldState.FigureAngle); i++)
			{
				while (localFieldState.ColumnsHeight[localFieldState.FigureXCoordinate + i] + 1 < Height + LoclBottom[i])
				{
					// Добавить дыку по кординатам (option.FigureXCoordinate + i, option.ColumnsHeight[option.FigureXCoordinate + i] + 1)
					localFieldState.ColumnsHeight[localFieldState.FigureXCoordinate + i]++;
				}
				localFieldState.ColumnsHeight[localFieldState.FigureXCoordinate + i] += LoclHeight[i];
			}

		}

		private static HashSet<LocalFieldState> GetOptionsLine(Tetromino figure, this LocalFieldState CurState)
		{
			HashSet<LocalFieldState> options = new HashSet<LocalFieldState>();
			int hols = 100;
			for (int line = CurState.ColumnsHeight.Max(); line >= CurState.ColumnsHeight.Min(); line--)
			{
				int lenght = 0;
				bool stop = false;
				bool spase = false;
				int distanse = -1;

				for (int j = AreaLength - 1; j >= 0; j--)
				{
					if (!spase && lenght == 0 && CurState.ColumnsHeight[j] < line)
					{
						distanse = line;
						lenght++;
						spase = true;
					}
					else if (spase && CurState.ColumnsHeight[j] < line)
					{
						lenght++;
					}
					else if (spase && CurState.ColumnsHeight[j] >= line)
					{
						spase = false;
					}
					else if (!spase && CurState.ColumnsHeight[j] < line && lenght != 0)
					{
						stop = true;
						break;
					}
				}

				if (stop)
				{
					break;
				}

				distanse -= lenght;

				for (int angle = 0; angle < 4; angle++)
				{
					LocalFieldState localFieldState = CreateLocalFieldStateNext(CurState)

					if (GetLenghtTetromino(figure, angle) != 0 && GetLenghtTetromino(figure, localFieldState.FigureAngle) == lenght)
					{
						CengeLoclColumnsHeight(localFieldState, CurState);
						localFieldState.FigureXCoordinate = distanse;
						Drop(localFieldState, figure);
						int p = 0;

						for (int i = 0; i < 4; i++)
						{
							if (LinesWithHoles[line - i] != 0)//есть ли дыка в строке line - i
							{
								continue;
							}
							for (int k = 0; k < lenght; k++)
							{
								if (localFieldState.ColumnsHeight[localFieldState.FigureXCoordinate + k] < line - i)
								{
									stop = true;
									break;
								}
							}
							if (stop)
							{
								stop = false;
							}
							else
							{
								localFieldState.RemoveLine(line - i);
								p++;
							}
						}
						if (p == 0)
						{
							break;
						}


						if (localFieldState.Holes.Count() < hols)
						{
							options.Cear();
							hols = localFieldState.Holes.Count();
							options.Add(localFieldState);
						}
						else if (localFieldState.Holes.Count() == hols)
						{
							options.Add(localFieldState);
						}
					}
				}
			}
			return options;
		}


		private static HashSet<LocalFieldState> GetOptionsCombs(this Tetromino figure, LocalFieldState CurState)
		{
			HashSet<LocalFieldState> options = new HashSet<LocalFieldState>();
			if (figure == I)
			{
				return options;
			}
			for (int i = 0; i < AreaLength - 1; i++)
			{
				if (i < AreaLength - 2)
				{
					int combs = new int[2];
					combs[0] = GetOneCellCombination(CurState.ColumnsHeight[i + 1] - CurState.ColumnsHeight[i]);
					combs[1] = GetTwoCellCombination(CurState.ColumnsHeight[i + 1] - CurState.ColumnsHeight[i],
						CurState.ColumnsHeight[i + 2] - CurState.ColumnsHeight[i + 1]);
				}
				else
				{
					int combs = new int[1];
					combs[0] = GetOneCellCombination(CurState.ColumnsHeight[i + 1] - CurState.ColumnsHeight[i]);
				}
				for (int j = 0; j < combs.Lenght; j++)
				{
					if (FiguresCombinations[(int)figure][combs[i]])
					{
						int agel = TetrominoExtensions.GetRotationsNumber(figure, combs[i]);
						LocalFieldState localFieldState = CreateLocalFieldStateNext(CurState)
						int[] LoclHeight = GetFiureHeight(figure, localFieldState.FigureAngle);
						for (int j = 0; i < LoclHeight.Lenght; i++)
						{
							localFieldState.ColumnsHeight[i + j] += LoclHeight[j];
						}
						localFieldState.FigureXCoordinate = i;
						if (LoclHeight.Lenght + localFieldState.FigureXCoordinate < AreaLength - 1 ||
							(options.Count == 0 && LoclHeight.Lenght + localFieldState.FigureXCoordinate < AreaLength - 1))
						{
							options.Add(option);
						}
					}
				}
			}
			return options;
		}

		private static HashSet<LocalFieldState> GetOptionsHols(this Tetromino figure, LocalFieldState CurState)
		{
			HashSet<LocalFieldState> options = new HashSet<LocalFieldState>();
			int hols = 100;
			for (int angle = 0; angle < 4; angle++)
			{
				if (GetLenghtTetromino(figure, angle) == 0)
				{
					continue;
				}

				LocalFieldState localFieldState = CreateLocalFieldStateNext(CurState)
				localFieldState.FigureAngle = angle

				for (int distanse = 0; distanse < AreaLength - GetLenghtTetromino(figure, localFieldState.FigureAngle) ; distanse++)
				{

					localFieldState.FigureXCoordinate = distanse;
					Drop(localFieldState, figure);

					if (localFieldState.Holes.Count() < hols)
					{
						options.Cear();
						hols = localFieldState.Holes.Count();
						options.Add(localFieldState);
					}
					else if (localFieldState.Holes.Count() == hols)
					{
						options.Add(localFieldState);
					}
				}
			}
			return options;
		}

		public static HashSet<LocalFieldState> GetFieldStateOptions(this Tetromino figure, LocalFieldState CurState)
		{
			//throw new NotImplementedException();
			HashSet<LocalFieldState> options = new HashSet<LocalFieldState>();
			if (figure == Tetromino.I && CurState.ColumnsHeight[AreaLength - 1] != 0 &&
				 CurState.ColumnsHeight.Min() >= 3)
			{
				LocalFieldState localFieldState = CreateLocalFieldStateNext(CurState)
				option.FigureXCoordinate = AreaLength - 1;
				option.FigureAngle = 0;
				int j = 0;
				for (int i = 3; i >= 0; i--)
				{
					if (LinesWithHoles[i] == 0)//в строке i нет дырки
					{
						option.RemoveLine(i);
					}
				}
				options.Add(option);
				return options;
			}

			if (CurState.CombinationsNumber[figure] != 0 || CurState.CombinationsNumber.Max() <= 10)
			{
				options = GetOptionsCombs(figure, CurState);
				return options;
			}

			if (options.Count == 0 && (ColumnsHeight[AreaLength - 1] != 0 || CurState.CombinationsNumber.Max() > 10))
			{
				options = GetOptionsLine(figure, CurState);
			}

			if (options.Count == 0)
			{
				options = GetOptionsCombs(figure, CurState);
				return options;
			}

			if (options.Count == 0)
			{
				options = GerOptionsHols(figure, CurState);
			}
			return options;
		}

		private static void RemoveLine(this LocalFieldState localFieldState, int line)
		{
			for (var i = 0; i < AreaLength; i++) // параллелить
			{
				if (localFieldState.ColumnsHeight[i] >= line)
				{
					localFieldState.ColumnsHeight[i]--;

					if (localFieldState.ColumnsHeight[i] == line - 1)
					{

						/*  while есть дыка по кординатам (i, option.ColumnsHeight[i])
						{
							убрать её
							localFieldState.ColumnsHeight[i]--;
						}*/
					}
				}
			}
		}

		public static float GetMetric2(this LocalFieldState localFieldState)
		{
			float weight = 0;

			for (var i = 0; i < AreaLength - 1; i++)
			{
				weight = Math.Abs(localFieldState.ColumnsHeight[i] - localFieldState.ColumnsHeight[i + 1]);
			}

			return weight - 12;
		}

		public static float GetMetric1(this LocalFieldState localFieldState)
		{
			int Max = 0;

			for (var i = 0; i < AreaLength; i++)
			{
				if (localFieldState.ColumnsHeight[i] > Max)
				{
					Max = localHeight[localFieldState.FigureXCoordinate + i];
				}
			}
			return AreaHeight / 2 - max;
		}

		public static float GetMetric3(this LocalFieldState localFieldState)
		{
			var CombinationsNumber = new int[(int)Tetromino.All];

			for (var i = 0; i < AreaLength - 1; i++)
			{
				int combs = GetOneCellCombination(localFieldState.ColumnsHeight[i] - localFieldState.ColumnsHeight[i + 1]);
				for (var j = 0; j < (int)Tetromino.All; j++)
				{
					if (FiguresCombinations[j, combs])
					{
						CombinationsNumber[j]++;
					}
				}
				if (i < AreaLength - 2)
				{
					combs = GetTowCellCombination(localFieldState.ColumnsHeight[i + 1] - localFieldState.ColumnsHeight[i],
						localFieldState.ColumnsHeight[i + 2] - localFieldState.ColumnsHeight[i + 1]);
					for (var j = 0; j < (int)Tetromino.All; j++)
					{
						if (FiguresCombinations[j, combs])
						{
							CombinationsNumber[j]++;
						}
					}
				}
			}


			float weight = 0.0;
			for (var j = 0; j < (int)Tetromino.All; j++)
			{
				weight += (30 * Math.Pow(2.0, (double)CombinationsNumber[i])) /
					(Math.Pow(3, (double)CombinationsNumber[i])) - 30; //арефметическая прогресия b1 = 10 q = 2/3 n = CombinationsNumber[i]
				weight -= 15; // нормирование 
			}

			return weight;
		}

		/*public static void CombinationAdd(Combination combination, bool add)
		{
			throw new NotImplementedException();
		}

		public static void combs_cange_update(int coordinate, int[] Relative)
		{
			if (coordinate > 0)
			{
				CombinationAdd(GetTwoCellsCombination(RelativeHeight[coordinate - 1], RelativeHeight[coordinate]), false);
				CombinationAdd(GetTwoCellsCombination(Relative[coordinate - 1], Relative[coordinate]), true);
			}

			CombinationAdd(GetOneCellCombination(RelativeHeight[coordinate]), false);
			CombinationAdd(GetOneCellCombination(Relative[coordinate]), true);
		}

		void update(this LocalFieldState localFieldState)
		{
			var localRelative = new int[AreaLength - 1];

			canedHeight(localFieldState, ColumnsHeight, true, true);//??????????

			for (var i = 0; i < AreaLength - 1; i++)
			{
				localRelative[i] = ColumnsHeight[i + 1] - ColumnsHeight[i + 1];
			}

			for (var i = 0; i < AreaLength - 1; i++)
			{
				if (localRelative[i] != RelativeHeight[i])
				{
					combs_cange(i, localRelative);
				}
			}
		}

		private static Combination GetOneCellCombination(int relativeCombination)
		{
			return relativeCombination switch
			{
				0 => Combination.O,
				1 => Combination.U,
				-1 => Combination.D,
				2 => Combination.UU,
				-2 => Combination.DD,
				_ => Combination.All
			};
		}

		private static Combination GetTwoCellsCombination(int relativeCombination1, int relativeCombination2)
		{
			if (relativeCombination1 >= 9 || relativeCombination2 >= 9)
			{
				return Combination.All;
			}

			var type = 10 * relativeCombination1 + relativeCombination2;

			return type switch
			{
				0 => Combination.OO,
				10 => Combination.UO,
				-10 => Combination.DO,
				1 => Combination.OU,
				-1 => Combination.OD,
				9 => Combination.DU,
				_ => Combination.All
			};
		}

		private static float GetCombinationWeight(Combination combination)
		{
			throw new NotImplementedException();
		}

		private static void Change()
		{
			throw new NotImplementedException();
		}*/
	}
}