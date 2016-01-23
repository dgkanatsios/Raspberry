using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenseHatGames.SnakeGameLibrary
{
    public class SnakeGameArray
    {
        public SnakePiece[,] matrix;
        public SnakePiece this[int row, int column]
        {
            get
            {
                return matrix[row, column];
            }
            set
            {
                matrix[row, column] = value;
            }
        }
        public SnakeGameArray()
        {
            matrix = new SnakePiece[SnakeGame.Rows, SnakeGame.Columns];
            snake = new Snake();
        }
        public bool TryMove(SnakeMovement direction)
        {
            SnakePiece head = snake[0];
            var pieceLocation = head.RowColumn;
            switch (direction)
            {
                case SnakeMovement.Left:
                    if (head.Column == 0 || matrix[head.Row, head.Column - 1] != null)
                        return false;
                    else
                        MovePiece(head,head.Row, head.Column - 1);
                    break;
                case SnakeMovement.Right:
                    if (head.Column == SnakeGame.Columns - 1 || matrix[head.Row, head.Column + 1] != null)
                        return false;
                    else
                        MovePiece(head,head.Row, head.Column + 1);
                    break;
                case SnakeMovement.Top:
                    if (head.Row == 0 || matrix[head.Row - 1, head.Column] != null)
                        return false;
                    else
                        MovePiece(head, head.Row - 1, head.Column);
                    break;
                case SnakeMovement.Bottom:
                    if (head.Row == SnakeGame.Rows - 1 || matrix[head.Row + 1, head.Column] != null)
                        return false;
                    else
                        MovePiece(head, head.Row + 1, head.Column);
                    break;
                default:
                    return false;
            }

            for (int i = 1; i < snake.Count; i++)
            {
                var cache = snake[i].RowColumn;
                MovePiece(snake[i], pieceLocation.Item1, pieceLocation.Item2);
                pieceLocation = cache;
            }
            //last one will be null
            matrix[pieceLocation.Item1, pieceLocation.Item2] = null;
            return true;
        }

        private void MovePiece(SnakePiece piece, int newRow, int newColumn)
        {
            piece.Row = newRow;
            piece.Column = newColumn;
            matrix[newRow, newColumn] = piece;
        }

        public void AddSnakePiece(SnakePiece piece)
        {
            snake.Add(piece);
            matrix[piece.Row, piece.Column] = piece;
        }

        private Snake snake;
    }
}
