namespace ChessLogic
{
    //Player enum:
    //+ Who win the game
    //+ Turn management
    //+ Colors of chess pieces
    public enum Player
    {
        //None is set whenever there is a stalemate
        None,
        White,
        Black
    }

    public static class PlayerExtension
    {
        public static Player Opponent(this Player player)
        {
            return player switch
            {
                Player.White => Player.Black,
                Player.Black => Player.White,
                _ => Player.None,
            };
        }
    }

}
