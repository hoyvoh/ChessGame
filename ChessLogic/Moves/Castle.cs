using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLogic
{
    public class Castle : Move
    {
        public override MoveType Type { get; }
        public override Position FromPos {  get; }
        public override Position ToPos { get; }

        private readonly Direction kingMoveDir;
        private readonly Position rookFromPos;
        private readonly Position rookToPos;

        public Castle(MoveType type, Position kingPos)
        {
            Type = type;
            FromPos = kingPos;
            
            if(type == MoveType.CastleKingSide)
            {
                kingMoveDir = Direction.Right;
                ToPos = new Position(kingPos.Row, 6);
                rookFromPos = new Position(kingPos.Row, 7);
                rookToPos = new Position(kingPos.Row, 5);
            }
            else if(type == MoveType.CastleQueenSide)
            {
                kingMoveDir = Direction.Left;
                ToPos = new Position(kingPos.Row, 2);
                rookFromPos = new Position(kingPos.Row, 0);
                rookToPos = new Position(kingPos.Row, 3);
            }
        }

        public override void Execute(Board board)
        {
            new NormalMove(FromPos, ToPos).Execute(board);
            new NormalMove(rookFromPos, rookToPos).Execute(board);
        }

        public override bool IsLegal(Board board)
        {
            Player player = board[FromPos].Color;
            //If player is in checked, no legal castle
            if (board.IsCheck(player))
            {
                return false;
            }

            //Make a copy of the board then move the king 2 steps
            //Check each iteration if that move makes king checked
            Board copyBoard = board.Copy();
            Position kingPosInCopy = FromPos;

            for(int i = 0; i<2; i++)
            {
                new NormalMove(kingPosInCopy, kingPosInCopy+kingMoveDir).Execute(copyBoard);
                kingPosInCopy = kingPosInCopy+kingMoveDir;

                if (copyBoard.IsCheck(player))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
