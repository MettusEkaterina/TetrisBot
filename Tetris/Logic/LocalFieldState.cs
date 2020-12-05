using System.Collections.Generic;

namespace TetrisClient.Logic
{
    public class LocalFieldState
    {
        public int FigureCoordinate; //расстояние от фигуры до левого края 
		public int FigureAngle; //угол, на который повернута падающая заданная фигура (количество поворотов по часовой стрелке)
        public List<int> ColumnsHeight;
        public List<Point> Holes;
        public double Weight; // приоритет состояния игрового поля
        public int FieldHeight; // высота игрового поля
        public int FieldWidth; // ширина игрового поля
        //public bool IsITetrominoFound; // падала ли ранее палочка
    }
}