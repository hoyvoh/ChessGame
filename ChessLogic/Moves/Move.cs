using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLogic
{
    public abstract class Move
    {
        public abstract MoveType Type { get; }
        public abstract Position FromPos { get; }
        public abstract Position ToPos { get; }

        //This is to execute different moves of pieces
        public abstract void Execute(Board board); 
        
        //This is used to check if after one player
        //execute the next move, he is checked or not.
        //If that move cause him to be checked => illegal
        public virtual bool IsLegal(Board board)
        {
            Player player = board[FromPos].Color;
            Board boardCopy = board.Copy();
            Execute(boardCopy);
            return !boardCopy.IsCheck(player);
        }
    }
}
