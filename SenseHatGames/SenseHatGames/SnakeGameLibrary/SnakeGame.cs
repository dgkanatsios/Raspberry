using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;

namespace SenseHatGames.SnakeGameLibrary
{
    public class SnakeGame
    {
        //our game array
        public SnakeGameArray GameArray
        {
            get; private set;
        }
        //timer to coordinate snake movement
        DispatcherTimer timer;
        private SnakeMovement snakeMovement = SnakeMovement.Left;

        public SnakeMovement SnakeMovement
        {
            get
            {
                return snakeMovement;
            }
            set //do not allow to set the movement in the opposite direction, i.e. the snake can't move backwards!
            {
                switch (value)
                {
                    case SnakeMovement.Left:
                        if (snakeMovement == SnakeMovement.Right)
                            return;
                        break;
                    case SnakeMovement.Right:
                        if (snakeMovement == SnakeMovement.Left)
                            return;
                        break;
                    case SnakeMovement.Top:
                        if (snakeMovement == SnakeMovement.Bottom)
                            return;
                        break;
                    case SnakeMovement.Bottom:
                        if (snakeMovement == SnakeMovement.Top)
                            return;
                        break;
                    default:
                        break;
                }
                snakeMovement = value;
            }
        }
        //launch one fruit every three seconds, starting from the time fruit was eaten
        private TimeSpan DurationTillNextFruit = TimeSpan.FromSeconds(3);

        //event launching when game changes
        public event EventHandler Updated;
        //event launching when player loses
        public event EventHandler PlayerLost;

        public SnakeGame()
        {
            GameArray = new SnakeGameArray();
            //add three initial pieces to the snake
            //snake's head has a different color
            GameArray.AddSnakePiece(new SnakePiece(2, 4, Colors.Gray));
            GameArray.AddSnakePiece(new SnakePiece(2, 5, Colors.Navy));
            GameArray.AddSnakePiece(new SnakePiece(2, 6, Colors.Navy));
        }

        /// <summary>
        /// Move the snake and check if game over
        /// </summary>
        private void TryMove()
        {
            if (GameArray.TryMove(SnakeMovement) == MovementResult.GameOver)
            {
                PlayerLost?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Initializes the timer
        /// </summary>
        public void Start()
        {
            this.Stop();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += Timer_Tick;
            GameArray.TimeFruitWasCreatedOrEaten = DateTime.Now;
            timer.Start();
        }

        public void Stop()
        {
            timer?.Stop();
            timer = null;
        }

        /// <summary>
        /// Raised on each timer interval
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, object e)
        {
            //check if a new fruit must be added
            if (!GameArray.FruitExists &&
                GameArray.TimeFruitWasCreatedOrEaten + DurationTillNextFruit > DateTime.Now)
            {
                //create new fruit
                GameArray.AddFruit();
                GameArray.FruitExists = true;
                GameArray.TimeFruitWasCreatedOrEaten = DateTime.Now;
            }
            //move the snake
            TryMove();
            //inform the UI that the game state has changed
            Updated?.Invoke(this, EventArgs.Empty);
        }

    }


}
