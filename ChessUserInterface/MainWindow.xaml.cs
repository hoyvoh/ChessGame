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
            //Game Menu layer
            GameMenu();

            //Chess game layer
            InitializeBoard();
            gamesState = new GameState(Player.White, Board.Initial());
            DrawBoard(gamesState.Board);
            SetCursor(gamesState.CurrentPlayer);
            //System.Media.SoundPlayer player = new System.Media.SoundPlayer();
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
            //If there is a menu on the screen, nothing happpen on the board
            if (IsMenuOnScreen())
            {
                return;
            }
            
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
                if(move.Type == MoveType.PawnPromotion)
                {
                    HandlePromotion(move.FromPos, move.ToPos);
                }
                 else
                {
                    HandleMove(move);
                }
            }
        }

        private void HandlePromotion(Position from, Position to)
        {
            pieceImages[to.Row, to.Column].Source = Images.GetImage(gamesState.CurrentPlayer, PieceType.Pawn);
            pieceImages[from.Row, from.Column].Source = null;

            PromotionMenu promMenu = new PromotionMenu(gamesState.CurrentPlayer);
            MenuContainer.Content = promMenu;

            promMenu.PieceSelected += type => {
                MenuContainer.Content = null;
                Move promMove = new PawnPromotion(from, to, type);
                HandleMove(promMove);
            };
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

            //After the last move, if end game, show the GameOver menu
            if (gamesState.IsGameOver())
            {
                GameOverMenu();
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

        private bool IsMenuOnScreen()
        {
            //true: yes 
            return MenuContainer.Content != null;
        }

        private void GameOverMenu()
        {
            GameOverMenu gameOverMenu = new GameOverMenu(gamesState);
            MenuContainer.Content = gameOverMenu;

            gameOverMenu.OptionSelected += option =>
            {
                //If option == restart, go restart, else back to game menu
                if (option == Options.Restart)
                {
                    MenuContainer.Content = null;
                    RestartGame();
                }
                
                else if (option == Options.Exit)
                {
                    MenuContainer.Content = null;
                    GameMenu();
                }
            };
        }

        private void RestartGame()
        {
            selectedPos = null;
            GameState.TurnCounter = 0;
            HideHighlights();
            HideCheckHighlight();
            moveCache.Clear();
            gamesState = new GameState(Player.White, Board.Initial());
            DrawBoard(gamesState.Board);
            SetCursor(gamesState.CurrentPlayer);
        }

        private void GameMenu()
        {
            GameMenu gameMenu = new GameMenu();
            MenuContainer.Content = gameMenu;

            gameMenu.OptionSelected += option =>
            {
                if (option == Options.Start)
                {
                    MenuContainer.Content = null;
                    RestartGame();
                    return;
                }

                else if (option == Options.Exit)
                {
                    MenuContainer.Content = null;
                    Application.Current.Shutdown();
                }
            };
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (!IsMenuOnScreen() && e.Key == Key.Escape)
            {
                ShowPausedMenu();
            }
        }

        private void ShowPausedMenu()
        {
            PauseMenu pauseMenu = new PauseMenu();
            MenuContainer.Content = pauseMenu;

            pauseMenu.OptionSelected += option =>
            {
                MenuContainer.Content = null;
                if (option == Options.Restart)
                {
                    RestartGame();
                }
            };
        }
    }
}
