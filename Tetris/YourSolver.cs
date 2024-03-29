﻿/*-
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

using TetrisClient.Logic;

namespace TetrisClient
{
	internal class YourSolver : AbstractSolver
	{
        //private bool IsITetrominoFound = false;

		public YourSolver(string server)
			: base(server)
		{
		}

		protected internal override Command Get(Board board)
        {
            var tetromino = board.GetCurrentTetromino();
            var figurePoint = board.GetCurrentFigurePoint();
            var futureFigures = board.GetFutureFigures();
			futureFigures.Insert(0, tetromino);
			var (columnsHeight, holes) = board.GetFieldCharacteristics();

            //if (!IsITetrominoFound && tetromino == Tetromino.I)
            //{
            //    IsITetrominoFound = true;
            //}

            // for debug
            if (tetromino == Tetromino.I)
            {
                var a = 0;
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
                //IsITetrominoFound = this.IsITetrominoFound
			};

            return currentFieldState.GetCommand(futureFigures);
		}
	}
}
