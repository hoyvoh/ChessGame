using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ChessLogic;
namespace ChessUserInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Image[,] pieceImages = new Image[8, 8];
        private readonly Rectangle[,] highlights = new Rectangle[8,8];
        private readonly Dictionary<Position, Move> moveCache = new Dictionary<Position, Move>();
        private GameState gamesState;
        private Position selectedPos = null;

        public MainWindow()
        {
            InitializeComponent();
            InitializeBoard();
            gamesState = new GameState(Player.White, Board.Initial());
            DrawBoard(gamesState.Board);
            SetCursor(gamesState.CurrentPlayer);
        }

        private void InitializeBoard()
        {
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    Image image = new Image();
                    pieceImages[r, c] = image;
                    PieceGrid.Children.Add(image);
                    
                    Rectangle highlight = new Rectangle();
                    highlights[r, c] = highlight;
                    HighlightGrid.Children.Add(highlight);
                }
            }
        }

        private void DrawBoard(Board board)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Piece piece = board[i, j];
                    pieceImages[i, j].Source = Images.GetImage(piece);
                }
            }
        }

        private void BoardGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(BoardGrid);
            Position pos = ToSquarePosition(point);

            if (selectedPos == null)
            {
                OnFromPositionSelected(pos);
            }
            else
            {
                OnToPositionSelected(pos);
            }
        }

        private Position ToSquarePosition(Point point)
        {
            double squareSize = BoardGrid.ActualWidth / 8;
            int row = (int)(point.Y / squareSize);
            int col = (int)(point.X / squareSize);
            return new Position(row, col);
        }

        private void OnFromPositionSelected(Position pos)
        {
            //Get all the legal moves
            IEnumerable<Move> moves = gamesState.LegalMovesForPiece(pos);
            
            //If there are legal moves
            if (moves.Any())
            {
                //Consider that position is selected
                //Save that position to cache
                //Highlight that position
                selectedPos = pos;
                CacheMoves(moves);
                ShowHighlights();
            }
        }

        private void OnToPositionSelected(Position pos)
        {
            selectedPos = null;
            HideHighlights();

            if(moveCache.TryGetValue(pos,out Move move))
            {
                HandleMove(move);
            }
        }

        private void HandleMove(Move move)
        {
            HideCheckHighlight();
            gamesState.MakeMove(move);
            
            DrawBoard(gamesState.Board);
            
            SetCursor(gamesState.CurrentPlayer);
            //if after opponent's move, the player is in check, highlight
            if (gamesState.Board.IsCheck(gamesState.CurrentPlayer))
            {
                CheckHighlight();
            }
        }

        private void CacheMoves(IEnumerable<Move> moves)
        {
            moveCache.Clear();
            foreach(Move move in moves)
            {
                moveCache[move.ToPos] = move;
            }
        }

        private void ShowHighlights()
        {
            Color color = Color.FromArgb(150, 125, 255, 125);
            foreach (Position to in moveCache.Keys)
            {
                highlights[to.Row, to.Column].Fill = new SolidColorBrush(color);
            }
        }

        private void HideHighlights()
        {
            foreach(Position to in moveCache.Keys)
            {
                highlights[to.Row, to.Column].Fill = Brushes.Transparent;
            }
        }

        private void HideCheckHighlight()
        {
            foreach (Position pos in gamesState.Board.PiecePositions())
            {
                highlights[pos.Row, pos.Column].Fill = Brushes.Transparent;
            }
        }

        private void CheckHighlight()
        {            
            Color color = Color.FromArgb(150, 255, 0, 0);
            //get position of current player's king
            //Red highlight that square
            IEnumerable<Position> kingPos = gamesState.Board.PiecePositions().Where(pos => 
                        (gamesState.Board[pos].Type == PieceType.King) && 
                        (gamesState.Board[pos].Color == gamesState.CurrentPlayer));
            foreach (Position pos in kingPos)
            {
                highlights[pos.Row, pos.Column].Fill = new SolidColorBrush(color);
            }
        }

        private void SetCursor(Player player)
        {
            if (player == Player.White)
            {
                Cursor = ChessCursors.WhiteCursor;
            }
            else
            {
                Cursor = ChessCursors.BlackCursor;
            }
        }
    }
}
