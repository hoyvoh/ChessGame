using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLogic
{
    public class EnPassant : Move
    {
        public override MoveType Type => MoveType.EnPassant;
        public override Position FromPos { get; }
        public override Position ToPos { get; }

        private readonly Position CapturePos;
        public EnPassant(Position from, Position to)
        {
            FromPos = from;
            ToPos = to;
            CapturePos = new Position(from.Row, to.Column);
        }

        public override void Execute(Board board)
        {
            new NormalMove(FromPos, ToPos).Execute(board);
            board[CapturePos] = null;
        }
    }
}
