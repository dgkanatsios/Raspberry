using SenseHatGames.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenseHatGames.SnakeGameLibrary
{
    public class SnakePiece : PieceBase
    {

        public SnakePiece(int row, int column)
        { Row = row; Column = column;  }
    }

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
    }

    public class FruitPiece : PieceBase
    {
    }
}
