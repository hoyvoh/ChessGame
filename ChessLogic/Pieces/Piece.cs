using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLogic
{
    //This class aim to represent all pieces generally
    //All other pieces will inherit from this class
    public abstract class Piece
    {
        public abstract PieceType Type { get; }
        public abstract Player Color { get; }
        public bool HasMoved { get; set; } = false;
        public abstract Piece Copy();
        public abstract IEnumerable<Move> GetMoves(Position from, Board board);
        
        // To check possible moves in a given direction, from a position on a board 
        protected IEnumerable<Position> MovePositionInDir(Position from, Board board, Direction dir)
        {
            //Iterate through every position in that direction while on the board
            for (Position pos = from+dir; Board.IsInside(pos); pos = pos+dir)
            {
                //Check if the position is empty
                //If empty -> reacheable
                //Else check color
                if (board.IsEmpty(pos)){
                    yield return pos;
                    continue;
                }
                //Check if same color
                //Not same color -> reacheable due to capture
                //Else -> unreacheable
                Piece piece = board[pos];
                if(piece.Color != Color)
                {
                    yield return pos;
                }
                //If encounter a piece -> stop iteration
                yield break;
            }
        }

        //For pieces that can go multiple directions, return all possible moves in each of them
        protected IEnumerable<Position> MovePositionInDirs(Position from, Board board, Direction[] dirs) 
        {
            return dirs.SelectMany(dir => MovePositionInDir(from, board, dir));
        }

        public virtual bool CanCaptureOpponentKing(Position from, Board board)
        {
            return GetMoves(from, board).Any(move =>
            {
                //If existing a position that a piece can capture the opponent's king, return true
                Piece piece = board[move.ToPos];
                //The destination position must not be null and contains a King
                return piece != null && piece.Type == PieceType.King;
            });
        }
    }
}
