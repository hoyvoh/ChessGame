namespace ChessLogic
{
    public class Direction
    {
        public readonly static Direction Up = new Direction(-1, 0);
        public readonly static Direction Down = new Direction(1, 0);
        public readonly static Direction Left = new Direction(0, -1);
        public readonly static Direction Right = new Direction(0, 1);
        public readonly static Direction UpLeft = Up + Left;
        public readonly static Direction UpRight = Up + Right;
        public readonly static Direction DownLeft = Down + Left;
        public readonly static Direction DownRight = Down + Right;

        
        //Row == Oy
        public int RowDelta { get; }
        //Column == Ox
        public int ColDelta { get; }

        public Direction(int rowDelta, int colDelta)
        {
            this.RowDelta = rowDelta;
            this.ColDelta = colDelta;
        }

        //Define how to combine 2 direction vectors
        public static Direction operator +(Direction d1, Direction d2)
        {
            return new Direction(d1.RowDelta + d2.RowDelta, d1.ColDelta + d2.ColDelta);
        }
        //Define how to scale direction vector
        public static Direction operator *(Direction d1, int scalar)
        {
            return new Direction(d1.RowDelta*scalar, d1.ColDelta*scalar);
        }
    }
}
