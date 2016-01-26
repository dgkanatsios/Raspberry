using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SenseHatGames.TetrisGameLibrary;
using Windows.UI;
using Windows.UI.Xaml.Shapes;
using Emmellsoft.IoT.Rpi.SenseHat;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using SenseHatGames.Common;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SenseHatGames
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TetrisGamePage : Page
    {
        public TetrisGamePage()
        {
            this.InitializeComponent();
            this.Loaded += TetrisGamePage_Loaded;
            CoreWindow.GetForCurrentThread().KeyUp += KeyboardInput;

        }

        TetrisGame game;
        ISenseHat SenseHat;


        private async void TetrisGamePage_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 8; i++)
            {
                mainGrid.RowDefinitions.Add(new RowDefinition());
                mainGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            game = new TetrisGame();
            game.GameUpdated += Game_GameUpdated;
            game.PlayerLost += Game_PlayerLost;
            game.Start();

#if ARM
            await InitializeSenseHatAsync();
            new Task(async () => await RunAsync()).Start();
#endif
        }

        private void Game_PlayerLost(object sender, EventArgs e)
        {
            game.Stop();
        }

        private async void Game_GameUpdated(object sender, EventArgs e)
        {
            await Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, () => Draw(game.GameArray));
        }

        //keyboard input
        private void KeyboardInput(CoreWindow sender, KeyEventArgs args)
        {
            CorePhysicalKeyStatus cpk = args.KeyStatus;
            if (cpk.IsKeyReleased)
            {
                if (args.VirtualKey == Windows.System.VirtualKey.Up)
                    game.TryRotate();
                else if (args.VirtualKey == Windows.System.VirtualKey.Down)
                    game.Move(Movement.Bottom);
                else if (args.VirtualKey == Windows.System.VirtualKey.Left)
                    game.Move(Movement.Left);
                else if (args.VirtualKey == Windows.System.VirtualKey.Right)
                    game.Move(Movement.Right);
            }
        }

        public async Task RunAsync()
        {
            SenseHat.Display.Clear();
            while (true)
            {
                if (SenseHat.Joystick.Update()) // Has any of the buttons on the joystick changed?
                {
                    if (SenseHat.Joystick.LeftKey == KeyState.Pressed)
                    {
                        game.Move(Movement.Left);
                    }
                    else if (SenseHat.Joystick.RightKey == KeyState.Pressed)
                    {
                        game.Move(Movement.Right);
                    }
                    else if (SenseHat.Joystick.DownKey == KeyState.Pressed)
                    {
                        game.Move(Movement.Bottom);
                    }
                    else if (SenseHat.Joystick.UpKey == KeyState.Pressed)
                    {
                        game.TryRotate();
                    }
                }
                SenseHat.Display.Clear();
                FillDisplay();
                SenseHat.Display.Update();
                await Task.Delay(50);
            }
        }


        private void FillDisplay()
        {
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    Piece piece = game.GameArray[x, y];
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


        private void Draw(TetrisGameArray tetrisGameArray)
        {
            ClearEllipsesFromGrid();
            for (int row = 0; row < 8; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    Piece piece = tetrisGameArray[row, column];
                    if (piece != null)
                    {
                        mainGrid.Children.Add(GetNewEllipse(row, column, piece.Color));
                    }

                }
            }
        }



        private Ellipse GetNewEllipse(int row, int column, Color color)
        {
            Ellipse el = ObjectPooler.GetEllipse();
            el.Width = el.Height = 40;
            el.Fill = new SolidColorBrush(color);
            Grid.SetColumn(el, column);
            Grid.SetRow(el, row);
            return el;
        }

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
