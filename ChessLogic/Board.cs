using ChessLogic.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ChessLogic
{
    public class Board
    {
        //Rectangular array for 8x8 board
        private readonly Piece[,] pieces = new Piece[8, 8];

        //When a player move a pawn position, we store that position by color
        private readonly Dictionary<Player, Position> pawnSkipPosition = new Dictionary<Player, Position>
        {
            { Player.White, null},
            { Player.Black, null}
        };

        //Indexer
        public Piece this[int row, int col]
        {
            get
            {
                return pieces[row, col];
            }
            set
            {
                pieces[row, col] = value;
            }
        }

        public Piece this[Position pos]
        {
            get { return this[pos.Row, pos.Column]; }
            set { this[pos.Row, pos.Column] = value; }
        }

        //For double pawn move
        //Helper for EnPassant method
        public Position GetPawnSkipPosition(Player player)
        {
            return pawnSkipPosition[player];
        }

        public void SetPawnSkipPosition(Player player, Position pos)
        {
            pawnSkipPosition[player] = pos;
        }

        //Initialize the board
        public static Board Initial()
        {
            Board board = new Board();
            board.AddStartPieces();
            return board;
        }

        //Add standard starting Pieces
        private void AddStartPieces()
        {
            //Upper row for black pieces
            this[0, 0] = new Rook(Player.Black);
            this[0, 1] = new Knight(Player.Black);
            this[0, 2] = new Bishop(Player.Black);
            this[0, 3] = new Queen(Player.Black);
            this[0, 4] = new King(Player.Black);
            this[0, 5] = new Bishop(Player.Black);
            this[0, 6] = new Knight(Player.Black);
            this[0, 7] = new Rook(Player.Black);
            //Bottom row for white pieces
            this[7, 0] = new Rook(Player.White);
            this[7, 1] = new Knight(Player.White);
            this[7, 2] = new Bishop(Player.White);
            this[7, 3] = new Queen(Player.White);
            this[7, 4] = new King(Player.White);
            this[7, 5] = new Bishop(Player.White);
            this[7, 6] = new Knight(Player.White);
            this[7, 7] = new Rook(Player.White);

            //Setup pawns for both players
            for(int i = 0; i<8; i++)
            {
                this[1, i] = new Pawn(Player.Black);
                this[6, i] = new Pawn(Player.White);
            }
        }

        //Check if a given position is still inside of the board
        public static bool IsInside(Position pos)
        {
            return pos.Row >= 0 && pos.Row < 8 && pos.Column >= 0 && pos.Column < 8;
        }

        //Check if a given position is empty
        public bool IsEmpty(Position pos)
        {
            return this[pos] == null;
        }

        //Loop through positions
        //=> return non-empty positions
        public IEnumerable<Position> PiecePositions()
        {
            for(int r = 0; r<8; r++)
            {
                for(int c= 0; c<8; c++)
                {
                    Position pos = new Position(r, c);
                    if (!IsEmpty(pos))
                    {
                        yield return pos;
                    }
                }
            }
        }
        //=> return positions containing pieces of a certain color
        public IEnumerable<Position> PiecePositionsFor(Player player)
        {
            return PiecePositions().Where(pos => this[pos].Color == player);
        }

        public bool IsCheck(Player player)
        {
            //Get position of all opponent's pieces
            return PiecePositionsFor(player.Opponent()).Any(pos =>
            {
                //Check if each of them is able to capture
                //this player's king in the next move
                Piece piece = this[pos];
                return piece.CanCaptureOpponentKing(pos, this);
            });
        }

        public Board Copy()
        {
            Board copy = new Board();
            foreach(Position pos in PiecePositions())
            {
                copy[pos] = this[pos].Copy();
            }
            return copy;
        }

        public Counting CountPieces()
        {
            Counting counting = new Counting();
            foreach(Position pos in PiecePositions())
            {
                Piece piece = this[pos];
                counting.Increment(piece.Color, piece.Type);
            }
            return counting;
        }

        public bool isInsufficientMaterials()
        {
            Counting counting = CountPieces();

            return IsKingVsKing(counting) ||
                    IsKingVsKingBishop(counting) ||
                    IsKingVsKingKnight(counting) ||
                    IsKBVsKB(counting);
        }

        //Case of King vs King
        private static bool IsKingVsKing(Counting counting)
        {
            return counting.TotalCount == 2;
        }

        //Case of KingVKing and Knight
        private static bool IsKingVsKingKnight(Counting counting)
        {
            return counting.TotalCount == 3 && 
                  (counting.Black(PieceType.Knight) == 1 || counting.White(PieceType.Knight) == 1);
        }
        //Case of KingVKing and Bishop
        private static bool IsKingVsKingBishop(Counting counting)
        {
            return counting.TotalCount == 3 &&
                  (counting.Black(PieceType.Bishop) == 1 || counting.White(PieceType.Knight) == 1);
        }

        //Case of King&Bishop vs King&Bishop
        private bool IsKBVsKB(Counting counting)
        {
            //Check if remaining pieces are 4
            //And if both of them are bishops
            if (counting.TotalCount !=4)
            {
                return false;
            }
            if(counting.White(PieceType.Bishop)!=1 || counting.Black(PieceType.Bishop) != 1)
            {
                return false;
            }

            //Locate black and white bishops
            Position wBishop = FindPiece(Player.White, PieceType.Bishop);
            Position bBishop = FindPiece(Player.Black, PieceType.Bishop);
            return wBishop.SquareColor() == bBishop.SquareColor();
        }
        //Locate piece 
        private Position FindPiece(Player color, PieceType type)
        {
            return PiecePositionsFor(color).First(pos => this[pos].Type == type);
        }
    }
}
