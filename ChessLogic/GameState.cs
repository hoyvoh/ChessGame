namespace ChessLogic
{
    public class GameState
    {
        public Board Board { get; }
        public Player CurrentPlayer { get; private set; }
        public Result result { get; private set; } = null;
        public static int TurnCounter = 0;
        public GameState(Player player, Board board)
        {
            CurrentPlayer = player;
            Board = board;
        }
        public IEnumerable<Move> LegalMovesForPiece(Position pos)
        {
            if (Board.IsEmpty(pos) || Board[pos].Color != CurrentPlayer)
            {
                return Enumerable.Empty<Move>();
            }

            Piece piece = Board[pos];
            IEnumerable<Move> moveCandidates = piece.GetMoves(pos, Board);
            return moveCandidates.Where(move => move.IsLegal(Board));
        }

        public void MakeMove(Move move)
        {
            move.Execute(Board);
            CurrentPlayer = CurrentPlayer.Opponent();
            CheckForGameOver();
            TurnCounter++;
        }

        //Get all the legal move for a player
        public IEnumerable<Move> AllLegalMovesFor(Player player)
        {
            //Get all moves of all pieces from a player on the board
            IEnumerable<Move> moveCandidates = Board.PiecePositionsFor(player).SelectMany(pos => {
                Piece piece = Board[pos];
                return piece.GetMoves(pos, Board);
            });
            //Filter legal moves to return
            return moveCandidates.Where(move => move.IsLegal(Board));
        }

        private void CheckForGameOver()
        {
            //Check for Checkmate and Stalemate
            //if there is no legal moves for current player
            if (!AllLegalMovesFor(CurrentPlayer).Any())
            {
                //If the player is in check, set the opponent as the winner
                //and end the game
                if (Board.IsCheck(CurrentPlayer))
                {
                    result = Result.Win(CurrentPlayer.Opponent());
                }
                //Else, it is a stalemate
                else
                {
                    result = Result.Draw(EndReason.Stalemate);
                }
            }

            //Check for 50movesRule
            if (!Board.IsCheck(CurrentPlayer) && TurnCounter >= 99)
            {
                //Turn counter exceed 100,
                //White min 50
                //Black min 49
                //And that move was not a check
                //=> end game
                result = Result.Draw(EndReason.FiftyMovesRule);
                return;
            }

            //Check for 3fold repetition
            //Create a small cache for 6 closest game states and player's turn
            //Check if odd and even game states are the same
            

            //Check for Insufficient Materials
            //Get all the remaining pieces on the board of two players
            //Check if both of them have only king remaining
            
        }

        public bool IsGameOver()
        {
            return result != null;
        }
    }
}
