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
        protected IEnumerable<Position> MovePositionInDir(Position pos, Board board, Direction dir)
        {

        }
    }
}
