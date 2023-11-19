using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLogic
{
    public class Counting
    {
        private readonly Dictionary<PieceType, int> WhiteCount = new();
        private readonly Dictionary<PieceType, int> BlackCount = new();
        public int TotalCount { get; private set; }

        //Initializer
        public Counting()
        {
            foreach(PieceType type in Enum.GetValues(typeof(PieceType)))
            {
                WhiteCount[type] = 0;
                BlackCount[type] = 0;
            }
        }

        //Setter
        public void Increment(Player color, PieceType type)
        {
            if(color == Player.White)
            {
                WhiteCount[type]++;
            }
            else if(color == Player.Black)
            {
                BlackCount[type]++;
            }
            TotalCount++;
        }

        //Getter
        public int White(PieceType type)
        {
            return WhiteCount[type];
        }

        public int Black(PieceType type)
        {
            return BlackCount[type];
        }
    }
}
