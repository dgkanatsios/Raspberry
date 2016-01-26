using Emmellsoft.IoT.Rpi.SenseHat;
using SenseHatGames.Common;
using SenseHatGames.SnakeGameLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SenseHatGames
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SnakeGamePage : Page
    {
        public SnakeGamePage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
            CoreWindow.GetForCurrentThread().KeyUp += KeyboardInput;
        }

        SnakeGame game = new SnakeGame();
        ISenseHat SenseHat;

        //keyboard handling 
        private void KeyboardInput(CoreWindow sender, KeyEventArgs args)
        {
            if (args.VirtualKey == Windows.System.VirtualKey.Up)
                game.SnakeMovement = SnakeMovement.Top;
            else if (args.VirtualKey == Windows.System.VirtualKey.Down)
                game.SnakeMovement = SnakeMovement.Bottom;
            else if (args.VirtualKey == Windows.System.VirtualKey.Left)
                game.SnakeMovement = SnakeMovement.Left;
            else if (args.VirtualKey == Windows.System.VirtualKey.Right)
                game.SnakeMovement = SnakeMovement.Right;
        }




        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            //initialize rows and columns for the grid
            for (int i = 0; i < Constants.Rows; i++)
            {
                mainGrid.RowDefinitions.Add(new RowDefinition());
                mainGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            Draw(game.GameArray);

            game.Updated += Game_Updated;
            game.PlayerLost += Game_PlayerLost;
            game.Start();

            //if running on Raspberry
#if ARM
            await InitializeSenseHatAsync();
            new Task(async () => await RunAsync()).Start();
#endif
        }

        private void Game_PlayerLost(object sender, EventArgs e)
        {
            game.Stop();
        }

        //redraw the array
        private async void Game_Updated(object sender, EventArgs e)
        {
            await Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, () => Draw(game.GameArray));
        }

        public async Task RunAsync()
        {
            //clear the display
            SenseHat.Display.Clear();
            while (true)
            {
                if (SenseHat.Joystick.Update()) // Has any of the buttons on the joystick changed?
                {
                    if (SenseHat.Joystick.LeftKey == KeyState.Pressed)
                    {
                        game.SnakeMovement = SnakeMovement.Left;
                    }
                    else if (SenseHat.Joystick.RightKey == KeyState.Pressed)
                    {
                        game.SnakeMovement = SnakeMovement.Right;
                    }
                    else if (SenseHat.Joystick.DownKey == KeyState.Pressed)
                    {
                        game.SnakeMovement = SnakeMovement.Bottom;
                    }
                    else if (SenseHat.Joystick.UpKey == KeyState.Pressed)
                    {
                        game.SnakeMovement = SnakeMovement.Top;
                    }
                }
                SenseHat.Display.Clear();
                FillDisplay();
                SenseHat.Display.Update();
                await Task.Delay(50); //sleep for 50 miliseconds
            }
        }
        
        /// <summary>
        /// Draw all pieces on the display
        /// </summary>
        private void FillDisplay()
        {
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    PieceBase piece = game.GameArray[x, y];
                    if (piece != null)
                    {
                        SenseHat.Display.Screen[y, x] = piece.Color;
                    }
                }
            }
        }


        private async Task InitializeSenseHatAsync()
        {
            SenseHat = await SenseHatFactory.Singleton.GetSenseHat().ConfigureAwait(false);
        }

        /// <summary>
        /// Draw all pieces on the XAML grid
        /// </summary>
        /// <param name="snakeGameArray"></param>
        private void Draw(SnakeGameArray snakeGameArray)
        {
            ClearEllipsesFromGrid();
            for (int row = 0; row < 8; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    PieceBase piece = snakeGameArray[row, column];
                    if (piece != null && piece is SnakePiece)
                    {
                        mainGrid.Children.Add(GetNewEllipse(row, column, piece.Color));
                    }
                    else if (piece != null && piece is FruitPiece)
                    {
                        mainGrid.Children.Add(GetNewEllipse(row, column, piece.Color));
                    }
                }
            }
        }

        /// <summary>
        /// Get a new ellipse from the object pooler
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        private Ellipse GetNewEllipse(int row, int column, Color color)
        {
            Ellipse el = ObjectPooler.GetEllipse();
            el.Fill = new SolidColorBrush(color);
            Grid.SetColumn(el, column);
            Grid.SetRow(el, row);
            return el;
        }

        /// <summary>
        /// Removes all ellipses from Grid and marks them as unused in the object pooler
        /// </summary>
        private void ClearEllipsesFromGrid()
        {
            for (int i = 0; i < mainGrid.Children.Count; i++)
            {
                var ellipse = mainGrid.Children[i] as Ellipse;
                if (ellipse != null)
                {
                    ObjectPooler.RemoveEllipse(ellipse);
                }
            }
            mainGrid.Children.Clear();
        }

    }
}
