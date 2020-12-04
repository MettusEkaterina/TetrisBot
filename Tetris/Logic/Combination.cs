namespace TetrisClient.Logic
{
	public enum Combination
    {
        OO = 0, //00 ---
        OU, // 01 _ _-
        OD, // 0-1 --_
        UO, // 10 _--
        DO, // -10 -_ _
        DU, // -11-_-
        UU, // 2
        DD, // -2
        U,  // 1
        D,  // -1
        O,  // 0
        All
    }
}