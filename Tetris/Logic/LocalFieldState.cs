using System.Collections.Generic;

namespace TetrisClient.Logic
{
    public class LocalFieldState
    {
        public int FigureCoordinate; //расстояние от фигуры до левого края 
		public int FigureAngle; //угол, на который повернута падающая заданная фигура (количество поворотов по часовой стрелке)
        public int[] ColumnsHeight;
        public List<Point> Holes;
        public float Weight; // приоритет состояния игрового поля
    }
}