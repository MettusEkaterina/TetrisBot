namespace TetrisClient.Logic
{
    public class TetrisBot
    {
        //bool[][] coordinate_combs = new bool[LENGHT][All_COMBS];//?
        bool[,] FiguresCombinations = new bool[(int)Tetromino.All, (int)Combination.All];

        public TetrisBot() //hardcode
        {
            FiguresCombinations[(int)Tetromino.O, (int)Combination.O] = true;
            FiguresCombinations[(int)Tetromino.T, (int)Combination.DU] = true;
            FiguresCombinations[(int)Tetromino.T, (int)Combination.U] = true;
            FiguresCombinations[(int)Tetromino.T, (int)Combination.D] = true;
            FiguresCombinations[(int)Tetromino.T, (int)Combination.OO] = true;
            //...h

        }
    }
}
