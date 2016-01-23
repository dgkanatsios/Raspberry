using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenseHatGames
{
    public class SnakeGame
    {
        private Random Randomizer = new Random();
        public static int Rows = 8;
        public static int Columns = 8;

        public SnakePiece[,] matrix;

        public Snake snake;

        public SnakeGame()
        {
            matrix = new SnakePiece[Rows, Columns];
            snake = new Snake(matrix);
            snake.Add(new SnakePiece(2, 2,matrix));
            snake.Add(new SnakePiece(2, 3,matrix));
            snake.Add(new SnakePiece(2, 4,matrix));
        }
    }

    public class Snake : List<SnakePiece>
    {
        private SnakePiece[,] matrix;

        public Snake(SnakePiece[,] array)
        {
            if (array == null)
                throw new ArgumentException("array");
            matrix = array;
        }

        public new void Add(SnakePiece piece)
        {
            base.Add(piece);
            matrix[piece.Row, piece.Column] = piece;
        }

        public bool TryMove(Movement direction)
        {
            SnakePiece head = this[0];
            var pieceLocation = head.RowColumn;
            switch (direction)
            {
                case Movement.Left:
                    if (head.Column == 0 || matrix[ head.Row, head.Column - 1] != null)
                        return false;
                    else
                        head.Move( head.Row, head.Column - 1);
                    break;
                case Movement.Right:
                    if (head.Column == SnakeGame.Columns - 1 || matrix[head.Row, head.Column + 1] != null)
                        return false;
                    else
                        head.Move( head.Row, head.Column + 1);
                    break;
                case Movement.Top:
                    if (head.Row == 0 || matrix[ head.Row - 1, head.Column] != null)
                        return false;
                    else
                        head.Move( head.Row - 1, head.Column);
                    break;
                case Movement.Bottom:
                    if (head.Row == SnakeGame.Rows - 1 || matrix[ head.Row + 1, head.Column] != null)
                        return false;
                    else
                        head.Move( head.Row + 1, head.Column);
                    break;
                default:
                    return false;
            }

            for (int i = 1; i < this.Count; i++)
            {
                var cache = this[i].RowColumn;
                this[i].Move(pieceLocation.Item1, pieceLocation.Item2);
                pieceLocation = cache;
            }
            //last one will be null
            matrix[pieceLocation.Item1, pieceLocation.Item2] = null;
            return true;
        }

        public enum Movement { Left, Right, Top, Bottom }
    }

    public class SnakePiece
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
        private SnakePiece[,] matrix;

        public SnakePiece(int row, int column, SnakePiece[,] _matrix)
        { Row = row; Column = column; matrix = _matrix; }

        public void Move(int newRow, int newColumn)
        {
            Row = newRow;
            Column = newColumn;
            matrix[Row, Column] = this;
        }
    }


}
