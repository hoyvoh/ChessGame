using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLogic.Pieces
{
    public class King : Piece
    {
        public override PieceType Type => PieceType.King;
        public override Player Color { get; }
        public King(Player color)
        {
            Color = color;
        }

        private static bool IsUnmovedRook(Position pos, Board board)
        {
            if (board[pos] == null)
            {
                return false;
            }
            Piece piece = board[pos];
            return piece.Type == PieceType.Rook && !piece.HasMoved;
        }

        private static bool IsAllEmpty(IEnumerable<Position> position, Board board)
        {
            return position.All(pos => board.IsEmpty(pos));
        }

        private bool CanCastleKingSide(Position from, Board board)
        {
            //If king has moved, castle denied
            if (HasMoved)
            {
                return false;
            }

            //If not, check if rook is moved and any pieces between them
            Position rookPos = new Position(from.Row, 7);
            Position[] betweenPosition = new Position[] { new(from.Row, 5), new(from.Row, 6), };
            return IsUnmovedRook(rookPos, board) && IsAllEmpty(betweenPosition, board);
        }

        private bool CanCastleQueenSide(Position from, Board board)
        {
            //If king moved, castle denined
            if(HasMoved)
            {
                return false;
            }

            //If not, do the same to king side but other squares checked
            Position rookPos = new Position(from.Row, 0);
            Position[] betweenPosition = new Position[] { new(from.Row, 1), new(from.Row, 2), new(from.Row, 3) };
            return IsUnmovedRook(rookPos,board) && IsAllEmpty(betweenPosition,board);
        }

        private static readonly Direction[] dirs = new Direction[]
        {
            Direction.Up,
            Direction.Down, 
            Direction.Left,
            Direction.Right,
            Direction.UpLeft,
            Direction.UpRight,
            Direction.DownLeft,
            Direction.DownRight
        };

        public override Piece Copy()
        {
            King copy = new King(Color);
            copy.HasMoved = HasMoved;
            return copy;
        }

        private IEnumerable<Position> MovePositions(Position from, Board board)
        {
            foreach(Direction dir in dirs)
            {
                Position to = from + dir;
                if (!Board.IsInside(to))
                {
                    continue;
                }
                if (board.IsEmpty(to) || board[to].Color != Color)
                {
                    yield return to;
                }
            }
        }

        public override IEnumerable<Move> GetMoves(Position from, Board board)
        {
            foreach (Position to in MovePositions(from, board)) {
                yield return new NormalMove(from, to);
            }

            if(CanCastleKingSide(from, board))
            {
                yield return new Castle(MoveType.CastleKingSide, from);
            }

            if (CanCastleQueenSide(from, board))
            {
                yield return new Castle(MoveType.CastleQueenSide, from);
            }
        }

        //King cannot capture opponent king
        //Castling do not capture anything
        //=> override method here
        public override bool CanCaptureOpponentKing(Position from, Board board)
        {
            //Check if all the legal moves are the opponent's king
            return MovePositions(from, board).Any(to =>
            {
                Piece piece = board[to];
                return piece != null && piece.Type == PieceType.King;
            });
        }


    }
}
