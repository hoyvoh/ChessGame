using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ChessLogic;

namespace ChessUserInterface
{
    public static class Images
    {
        private static readonly Dictionary<PieceType, ImageSource> whiteSources = new()
        {
            { PieceType.Pawn, LoadImage("Assets\\PawnW.png") },
            { PieceType.Queen, LoadImage("Assets\\QueenW.png") },
            { PieceType.Bishop, LoadImage("Assets\\BishopW.png") },
            { PieceType.Rook, LoadImage("Assets\\RookW.png") },
            { PieceType.King, LoadImage("Assets\\KingW.png") },
            { PieceType.Knight, LoadImage("Assets\\KnightW.png") }
        };

        private static readonly Dictionary<PieceType, ImageSource> blackSources = new()
        {
            { PieceType.Pawn, LoadImage("Assets\\PawnB.png") },
            { PieceType.Queen, LoadImage("Assets\\QueenB.png") },
            { PieceType.Bishop, LoadImage("Assets\\BishopB.png") },
            { PieceType.Rook, LoadImage("Assets\\RookB.png") },
            { PieceType.King, LoadImage("Assets\\KingB.png") },
            { PieceType.Knight, LoadImage("Assets\\KnightB.png") }
        };

        private static ImageSource LoadImage(string filePath)
        {
            return new BitmapImage(new Uri(filePath, UriKind.Relative));
        }

        public static ImageSource GetImage(Player color, PieceType type)
        {
            return color switch
            {
                Player.White => whiteSources[type],
                Player.Black => blackSources[type],
                _ => null
            };
        }

        public static ImageSource GetImage(Piece piece)
        {
            if(piece == null)
            {
                return null;
            }
            return GetImage(piece.Color, piece.Type);
        }
    }
}
