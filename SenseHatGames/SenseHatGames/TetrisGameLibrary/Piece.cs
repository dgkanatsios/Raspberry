using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenseHatGames.TetrisGameLibrary
{
    public class Piece
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public Piece(int row, int column)
        {
            Row = row;
            Column = column;
        }

     
    }
}
