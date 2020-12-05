/*-
 * #%L
 * Codenjoy - it's a dojo-like platform from developers to developers.
 * %%
 * Copyright (C) 2018 Codenjoy
 * %%
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as
 * published by the Free Software Foundation, either version 3 of the
 * License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public
 * License along with this program.  If not, see
 * <http://www.gnu.org/licenses/gpl-3.0.html>.
 * #L%
 */

using System;
using TetrisClient.Logic;

namespace TetrisClient
{
	internal class YourSolver : AbstractSolver
	{
        private bool IsITetrominoFound = false;
        private int TicksWithoutITetromino = 0;

		public YourSolver(string server)
			: base(server)
		{
		}

		protected internal override Command Get(Board board)
        {
            System.Diagnostics.Stopwatch myStopwatch = new System.Diagnostics.Stopwatch();
            myStopwatch.Start();

            var tetromino = board.GetCurrentTetromino();
            var figurePoint = board.GetCurrentFigurePoint();
            var futureFigures = board.GetFutureFigures();
			futureFigures.Insert(0, tetromino);
			var (columnsHeight, holes) = board.GetFieldCharacteristics();

            if (futureFigures.Contains(Tetromino.I))
            {
                TicksWithoutITetromino = 0;
                IsITetrominoFound = true;
            }
            else
            {
                TicksWithoutITetromino++;
            }

            if (TicksWithoutITetromino > 12) //10?
            {
                IsITetrominoFound = false;
            }

            var currentFieldState = new LocalFieldState
            {
				FigureCoordinate = figurePoint.X,
				FigureAngle = 0,
                ColumnsHeight = columnsHeight,
				Holes = holes,
				Weight = 0,
                FieldHeight = board.Size,
                FieldWidth = board.Size,
                IsITetrominoFound = this.IsITetrominoFound
			};

            var command = currentFieldState.GetCommand(futureFigures);

            myStopwatch.Stop();
            var ts = myStopwatch.Elapsed;

            var elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);

            return command;
        }
	}
}
