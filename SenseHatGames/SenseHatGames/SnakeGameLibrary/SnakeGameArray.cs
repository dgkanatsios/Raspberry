using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenseHatGames.SnakeGameLibrary
{
    public enum MovementResult
    {
        MoveAllowed,
        FruitEaten,
        GameOver
    }

    public class SnakeGameArray
    {
        public bool FruitExists
        {
            get; set;
        } = false;
        public DateTime TimeFruitWasCreatedOrEaten
        {
            get; set;
        }

        private Random random = new Random();

        public PieceBase[,] matrix;
        public PieceBase this[int row, int column]
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
            matrix = new PieceBase[SnakeGame.Rows, SnakeGame.Columns];
            snake = new Snake();
        }
        public MovementResult TryMove(SnakeMovement direction)
        {
            SnakePiece head = snake[0];
            RowColumn newHeadLocation = new RowColumn();
            switch (direction)
            {
                case SnakeMovement.Left:
                    if (head.Column == 0 ||
                        (matrix[head.Row, head.Column - 1] != null && matrix[head.Row, head.Column - 1] is SnakePiece))
                        return MovementResult.GameOver;
                    else
                        newHeadLocation = new RowColumn(head.Row, head.Column - 1);
                    break;
                case SnakeMovement.Right:
                    if (head.Column == SnakeGame.Columns - 1 ||
                        (matrix[head.Row, head.Column + 1] != null && matrix[head.Row, head.Column + 1] is SnakePiece))
                        return MovementResult.GameOver;
                    else
                        newHeadLocation = new RowColumn(head.Row, head.Column + 1);
                    break;
                case SnakeMovement.Top:
                    if (head.Row == 0 ||
                        (matrix[head.Row - 1, head.Column] != null && matrix[head.Row - 1, head.Column] is SnakePiece))
                        return MovementResult.GameOver;
                    else
                        newHeadLocation = new RowColumn(head.Row - 1, head.Column);
                    break;
                case SnakeMovement.Bottom:
                    if (head.Row == SnakeGame.Rows - 1 ||
                        (matrix[head.Row + 1, head.Column] != null && matrix[head.Row + 1, head.Column] is SnakePiece))
                        return MovementResult.GameOver;
                    else
                        newHeadLocation = new RowColumn(head.Row + 1, head.Column);
                    break;
            }

            MovementResult result = MovementResult.MoveAllowed;
            if (this[newHeadLocation.Row, newHeadLocation.Column] is FruitPiece)
                result = MovementResult.FruitEaten;

            var pieceLocation = head.RowColumn;
            MovePiece(head, newHeadLocation.Row, newHeadLocation.Column);


            for (int i = 1; i < snake.Count; i++)
            {
                var cache = snake[i].RowColumn;
                MovePiece(snake[i], pieceLocation.Row, pieceLocation.Column);
                pieceLocation = cache;
            }

            if (result == MovementResult.MoveAllowed)
                this[pieceLocation.Row, pieceLocation.Column] = null;
            else if (result == MovementResult.FruitEaten)
            {
                this.AddSnakePiece(new SnakePiece(pieceLocation.Row, pieceLocation.Column));
                FruitExists = false;
                TimeFruitWasCreatedOrEaten = DateTime.Now;
            }

            return result;
        }

        public void AddFruit()
        {
            FruitPiece fruitPiece = new FruitPiece();
            int fruitRow, fruitColumn;
            do
            {
                fruitRow = random.Next(0, SnakeGame.Rows);
                fruitColumn = random.Next(0, SnakeGame.Columns);
            } while
            (this[fruitRow, fruitColumn] == null &&
            snake.All((x => Math.Abs(x.Column - fruitColumn) < 2
            && Math.Abs(x.Row - fruitRow) < 2)));

            this[fruitRow, fruitColumn] = fruitPiece;
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
