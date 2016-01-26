using SenseHatGames.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace SenseHatGames.TetrisGameLibrary
{
    //Allowed movement for the shape
    public enum Movement { Left, Bottom, Right };

    /// <summary>
    /// Our tetris-like game base class
    /// </summary>
    public class TetrisGame
    {
        //useful for the lock construct in the Move method
        private object lockerObject = new object();
        //a reference to the current moveable shape
        public Shape CurrentShape { get; set; }
        DispatcherTimer timer;
        //raised when the state is updated
        public event EventHandler GameUpdated;

        public TetrisGameArray GameArray
        {
            get; private set;
        }
        public TetrisGame()
        {
            GameArray = new TetrisGameArray();
        }

        /// <summary>
        /// Initializes the timer and creates the first shape
        /// </summary>
        public void Start()
        {
            this.Stop();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1000);
            timer.Tick += Timer_Tick;
            timer.Start();

            CreateNewShape();
        }

        private void CreateNewShape()
        {
            ClearPreviousCurrentShapePosition();
            CurrentShape = ShapeFactory.CreateRandomShape();
            PlaceCurrentShape();
            GameUpdated?.Invoke(this, EventArgs.Empty);
        }



        public void TryRotate()
        {
            if (CanRotationBeMade())
            {
                ClearPreviousCurrentShapePosition();
                CurrentShape.Rotate();
                PlaceCurrentShape();
                GameUpdated?.Invoke(this, EventArgs.Empty);
            }
        }

        private void Timer_Tick(object sender, object e)
        {
            this.Move(Movement.Bottom);
        }

        private bool CanRotationBeMade()
        {
            //we need to see if this can be rotated
            //get a local copy
            Shape localCopy = CurrentShape.Clone();
            localCopy.Rotate();
            Piece[] pieces = localCopy.ToArray();
            if (pieces.Any(x => x.Row >= Constants.Rows) || pieces.Any(x => x.Column >= Constants.Columns)
                || pieces.Any(x => x.Column < 0) ||
                //we get the new position of each piece
                //we check if already exists a piece in this position in the array
                //and this position is *not* in the current shape :)
                (pieces.Any(x => GameArray[x.Row, x.Column] != null
                && CurrentShape.Where(y => y.Row == x.Row && y.Column == x.Column).Count() == 0)))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool CanMovementCanBeMade(Movement movement)
        {
            //first we need to see if this can be moved
            //we get a copy of the item
            Shape localCopy = CurrentShape.Clone();
            //we get a reference to all its left,right or bottom pieces
            Piece[] pieces = localCopy.ToArray();
            switch (movement)
            {
                case Movement.Left:
                    for (int i = 0; i < pieces.Count(); i++)
                    {
                        pieces[i].Column--;
                    }
                    break;
                case Movement.Bottom:
                    //we increase their row property by one
                    for (int i = 0; i < pieces.Count(); i++)
                    {
                        pieces[i].Row++;
                    }
                    break;
                case Movement.Right:
                    for (int i = 0; i < pieces.Count(); i++)
                    {
                        pieces[i].Column++;
                    }
                    break;
                default:
                    break;
            }

            //if any column is greater than 7, row less than 0 or greater than 7
            //or there exists another item in the new positions
            if (pieces.Any(x => x.Row >= Constants.Rows) || pieces.Any(x => x.Column >= Constants.Columns)
                || pieces.Any(x => x.Column < 0) ||
                //we get the new position of each piece
                //we check if already exists a piece in this position in the array
                //and this position is *not* in the current shape :)
                (pieces.Any(x => GameArray[x.Row, x.Column] != null
                && CurrentShape.Where(y => y.Row == x.Row && y.Column == x.Column).Count() == 0)))
            {
                //movement cannot be done
                return false;
            }
            else
            {
                //movement OK
                return true;
            }
        }

        /// <summary>
        /// Try and move the shape
        /// </summary>
        /// <param name="movement"></param>
        public void Move(Movement movement)
        {
            if (CurrentShape == null) return;

            //"sensitive" area
            lock (lockerObject)
            {
                //check if shape can be moved
                if (CanMovementCanBeMade(movement))
                {
                    //nullify its previous position in the array
                    ClearPreviousCurrentShapePosition();
                    //update all pieces' row or column information according to the movement requested
                    switch (movement)
                    {
                        case Movement.Left:
                            for (int i = 0; i < CurrentShape.Count; i++)
                            {
                                CurrentShape[i].Column--;
                            }
                            break;
                        case Movement.Bottom:
                            for (int i = 0; i < CurrentShape.Count; i++)
                            {
                                CurrentShape[i].Row++;
                            }
                            break;
                        case Movement.Right:
                            for (int i = 0; i < CurrentShape.Count; i++)
                            {
                                CurrentShape[i].Column++;
                            }
                            break;
                        default:
                            break;
                    }
                    //move the current shape in the array
                    PlaceCurrentShape();
                    GameUpdated?.Invoke(this, EventArgs.Empty);
                }
                else//movement cannot be made
                {
                    //item cannot be moved
                    //if the requested movement is bottom, this means that the shape cannot move even further
                    //so we need to 1. check if any row(s) are full of pieces, i.e. there exists a horizontal line
                    //2. remove these lines
                    //3. move all the rest lines towards the bottom of the array
                    //4. request another shape
                    if (movement == Movement.Bottom)
                    {
                        CurrentShape = null;
                        //check and clear lines
                        //move pieces below
                        ClearLinesAndMovePiecesBelow();
                        //create new shape
                        CreateNewShape();
                    }
                }
            }
        }

        /// <summary>
        /// Clears lines that have no null items
        /// Moves the rest of the lines below
        /// </summary>
        private void ClearLinesAndMovePiecesBelow()
        {
            //we check all rows
            for (int row = Constants.Rows - 1; row >= 0; row--)
            {
                bool ShouldClearRow = true;
                for (int column = 0; column < Constants.Columns; column++)
                {   //if we have at least one empty item, check the next row
                    if (GameArray[row, column] == null)
                    {
                        ShouldClearRow = false;
                        break;
                    }
                }
                //current row is to be removed
                if (ShouldClearRow)
                {   //empty current row
                    for (int column = 0; column < Constants.Columns; column++)
                    {
                        GameArray[row, column] = null;
                    }
                    //top row, so nothing more to do here
                    if (row == 0) continue;
                    //move all rows above the deleted one, one row below
                    for (int row2 = row - 1; row2 >= 0; row2--)
                    {
                        for (int column = 0; column < Constants.Columns; column++)
                        {
                            //move row2 to row
                            GameArray[row2 + 1, column] = GameArray[row2, column];
                            //clear row2, column item
                            GameArray[row2, column] = null;
                            //change row property on item
                            if (GameArray[row2 + 1, column] != null)
                                GameArray[row2 + 1, column].Row = row2 + 1;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Stops the timer
        /// </summary>
        public void Stop()
        {
            timer?.Stop();
            timer = null;
        }


        /// <summary>
        /// Sets the shape's position in the array
        /// </summary>
        private void PlaceCurrentShape()
        {
            for (int i = 0; i < CurrentShape.Count; i++)
            {
                GameArray[CurrentShape[i].Row, CurrentShape[i].Column] =
                    CurrentShape[i];
            }
        }

        /// <summary>
        /// The shape is to be moved, so we nullify its previous position
        /// </summary>
        private void ClearPreviousCurrentShapePosition()
        {
            if (CurrentShape != null)
                for (int i = 0; i < CurrentShape.Count; i++)
                {
                    GameArray[CurrentShape[i].Row, CurrentShape[i].Column] = null;
                }
        }
    }

    
}
