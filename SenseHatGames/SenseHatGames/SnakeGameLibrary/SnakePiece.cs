using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenseHatGames.SnakeGameLibrary
{
    public class SnakePiece : PieceBase
    {
       
        private SnakeGameArray matrix;

        public SnakePiece(int row, int column, SnakeGameArray _matrix)
        { Row = row; Column = column; matrix = _matrix; }
    }

    public abstract class PieceBase
    {
        public Tuple<int, int> RowColumn
        {
            get
            {
                return new Tuple<int, int>(Row, Column);
            }
        }
        public int Row { get; set; }
        public int Column { get; set; }
    }

    public class FruitPiece : PieceBase
    {
    }
}
