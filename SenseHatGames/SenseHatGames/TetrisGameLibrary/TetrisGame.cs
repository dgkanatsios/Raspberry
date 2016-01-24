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
    public enum Movement { Left, Bottom, Right };

    public class TetrisGame
    {
        private object lockerObject = new object();
        public static int Rows = 8;
        public static int Columns = 8;
        public Shape CurrentShape { get; set; }
        DispatcherTimer timer;

        public event EventHandler GameUpdated;

        public TetrisGameArray GameArray
        {
            get; private set;
        }
        public TetrisGame()
        {
            GameArray = new TetrisGameArray();
        }

        public void Start()
        {
            this.Stop();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
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
            ClearPreviousCurrentShapePosition();
            CurrentShape.Rotate();
            PlaceCurrentShape();
            GameUpdated?.Invoke(this, EventArgs.Empty);
        }

        private void Timer_Tick(object sender, object e)
        {
            this.Move(Movement.Bottom);
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

            //if any column is larger than 8
            //or there exists another item in the new positions
            if (pieces.Any(x => x.Row >= Rows) || pieces.Any(x => x.Column >= Columns)
                || pieces.Any(x => x.Column < 0) ||
                pieces.Any(x => GameArray[x.Row, x.Column] != null))
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

        public void Move(Movement movement)
        {
            if (CurrentShape == null) return;

            lock (lockerObject)
            {
                if (CanMovementCanBeMade(movement))
                {
                    ClearPreviousCurrentShapePosition();
                    switch (movement)
                    {
                        case Movement.Left:
                            for (int i = 0; i < CurrentShape.Count; i++)
                            {
                                CurrentShape[i].Column--;
                            }
                            break;
                        case Movement.Bottom:
                            //update all the shape rows - movement can be done
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

                    PlaceCurrentShape();
                    GameUpdated?.Invoke(this, EventArgs.Empty);
                }
                else//movement cannot be made
                {
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

        private void ClearLinesAndMovePiecesBelow()
        {
            for (int row = Rows - 1; row >= 0; row--)
            {
                bool ShouldClearRow = true;
                for (int column = 0; column < Columns; column++)
                {   //if we have at least one empty item, break
                    if (GameArray[row, column] == null)
                    {
                        ShouldClearRow = false;
                        break;
                    }
                }
                if (ShouldClearRow)
                {   //empty current row
                    for (int column = 0; column < Columns; column++)
                    {
                        GameArray[row, column] = null;
                    }
                    if (row == 0) continue;
                    //move all rows above the deleted one, one row below
                    for (int row2 = row - 1; row2 >= 0; row2--)
                    {
                        for (int column = 0; column < Columns; column++)
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

        public void Stop()
        {
            timer?.Stop();
            timer = null;
        }



        private void PlaceCurrentShape()
        {
            for (int i = 0; i < CurrentShape.Count; i++)
            {
                GameArray[CurrentShape[i].Row, CurrentShape[i].Column] =
                    CurrentShape[i];
            }
        }

        private void ClearPreviousCurrentShapePosition()
        {
            if (CurrentShape != null)
                for (int i = 0; i < CurrentShape.Count; i++)
                {
                    GameArray[CurrentShape[i].Row, CurrentShape[i].Column] = null;
                }
        }
    }

    public class TetrisGameArray
    {
        private Random random = new Random();

        public Piece[,] matrix;
        public Piece this[int row, int column]
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
        public TetrisGameArray()
        {
            matrix = new Piece[TetrisGame.Rows, TetrisGame.Columns];
        }




    }
}
