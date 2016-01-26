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
    /// Result of snake attempted movement
    /// </summary>
    public enum MovementResult
    {
        MoveAllowed,
        FruitEaten,
        GameOver
    }

    /// <summary>
    /// Helpful class to encapsulate access to the game's internal array
    /// </summary>
    public class SnakeGameArray
    {
        private Snake snake;

        /// <summary>
        /// Does fruit exist in the array
        /// </summary>
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

        /// <summary>
        /// Indexer for the array
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
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
            matrix = new PieceBase[Constants.Rows, Constants.Columns];
            snake = new Snake();
        }

        /// <summary>
        /// Attempts to move the snake head to the desired direction
        /// </summary>
        /// <param name="direction"></param>
        /// <returns>Movement result</returns>
        public MovementResult TryMove(SnakeMovement direction)
        {
            //get a reference to the head
            SnakePiece head = snake[0];
            RowColumn newHeadLocation = new RowColumn();
            //check if the movement is valid, depending on
            //1. out of bounds check
            //2. new place to move is not already occupied by a snake piece
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
                    if (head.Column == Constants.Columns - 1 ||
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
                    if (head.Row == Constants.Rows - 1 ||
                        (matrix[head.Row + 1, head.Column] != null && matrix[head.Row + 1, head.Column] is SnakePiece))
                        return MovementResult.GameOver;
                    else
                        newHeadLocation = new RowColumn(head.Row + 1, head.Column);
                    break;
            }

            //if we have reached here, the move is allowed. We also check if there is a fruit piece in the new space
            MovementResult result = MovementResult.MoveAllowed;
            if (this[newHeadLocation.Row, newHeadLocation.Column] is FruitPiece)
                result = MovementResult.FruitEaten;

            //modify the Piece properties to reflect the new location
            var pieceLocation = head.RowColumn;
            MovePiece(head, newHeadLocation.Row, newHeadLocation.Column);

            //for all the rest snake pieces, move them
            //easy to do, the [i] piece is moved to the location of the [i-1] one
            for (int i = 1; i < snake.Count; i++)
            {
                //cache the item to be moved
                var cache = snake[i].RowColumn;
                //move it to the i-1 location
                MovePiece(snake[i], pieceLocation.Row, pieceLocation.Column);
                //cache the i location
                pieceLocation = cache;
            }
            //pieceLocation now has the previous location of the last snake item
            //no fruit eaten, so nullify it
            if (result == MovementResult.MoveAllowed)
                this[pieceLocation.Row, pieceLocation.Column] = null;
            //fruit eaten, so add another piece to the snake
            else if (result == MovementResult.FruitEaten)
            {
                this.AddSnakePiece(new SnakePiece(pieceLocation.Row, pieceLocation.Column, Colors.Navy));
                FruitExists = false;
                TimeFruitWasCreatedOrEaten = DateTime.Now;
            }

            return result;
        }

        /// <summary>
        /// Adds a new fruit to the game
        /// </summary>
        public void AddFruit()
        {
            FruitPiece fruitPiece = new FruitPiece();
            fruitPiece.Color = ColorFactory.RandomColor;
            int fruitRow, fruitColumn;
            //find a random position for the fruit
            do
            {
                fruitRow = random.Next(0, Constants.Rows);
                fruitColumn = random.Next(0, Constants.Columns);
            } while (this[fruitRow, fruitColumn] != null);

            this[fruitRow, fruitColumn] = fruitPiece;
        }

        /// <summary>
        /// Set the relevant properties of the piece object
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="newRow"></param>
        /// <param name="newColumn"></param>
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


    }
}
