using System.Collections.Generic;

namespace TetrisClient.Logic
{
    public class LocalFieldState
    {
        public int FigureXCoordinate; //расстояние от фигуры до левого края 
		public int FigureAngle; //угол, на который повернута падающая заданная фигура
		public List<Point> Holes;
		public int[] ColumnsHeight;
		public float Weight; // приоритет состояния игрового поля
    }
}