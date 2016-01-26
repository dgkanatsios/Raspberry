using SenseHatGames.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace SenseHatGames.SnakeGameLibrary
{
    /// <summary>
    /// Abstract class for all the pieces in the Snake game
    /// Contains information abour Row, Column and Color
    /// </summary>
    public abstract class PieceBase
    {
        public RowColumn RowColumn
        {
            get
            {
                return new RowColumn(Row, Column);
            }
        }
        public int Row { get; set; }
        public int Column { get; set; }
        public Color Color { get; set; }
    }

    public class SnakePiece : PieceBase
    {
        public SnakePiece(int row, int column, Color color)
        {
            Row = row;
            Column = column;
            Color = color;
        }
    }



    public class FruitPiece : PieceBase
    {
    }
}
