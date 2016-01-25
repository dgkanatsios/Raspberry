using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace SenseHatGames.TetrisGameLibrary
{
    public class Piece
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public Color Color { get; set; }
        public Piece(int row, int column, Color? color = null)
        {
            Row = row;
            Column = column;
            if (color != null)
                Color = color.Value;
        }

     
    }
}
