using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLogic
{
    public class DoublePawns : Move
    {
        public override MoveType Type => MoveType.DoubleStepPawn;
        public override Position FromPos { get; }
        public override Position ToPos {  get; }

        private readonly Position skippedPos;

        public DoublePawns(Position from, Position to)
        {
            FromPos = from;
            ToPos = to;
            skippedPos = new Position((from.Row + to.Row) / 2, from.Column);
        }

        public override void Execute(Board board)
        {
            Player player = board[FromPos].Color;
            board.SetPawnSkipPosition(player, skippedPos);
            new NormalMove(FromPos, ToPos).Execute(board);
        }
    }
}
