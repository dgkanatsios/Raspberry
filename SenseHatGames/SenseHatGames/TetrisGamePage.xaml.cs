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

        private void TetrisGamePage_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 8; i++)
            {
                mainGrid.RowDefinitions.Add(new RowDefinition());
                mainGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            game = new TetrisGame();
            game.GameUpdated += Game_GameUpdated;
            game.Start();
        }

        private void Game_GameUpdated(object sender, EventArgs e)
        {
            Draw(game.GameArray);
        }

        private void KeyboardInput(CoreWindow sender, KeyEventArgs args)
        {
            if (args.VirtualKey == Windows.System.VirtualKey.Up)
                game.TryRotate();
            //else if (args.VirtualKey == Windows.System.VirtualKey.Down)
            //    //game.SnakeMovement = SnakeMovement.Bottom;
            else if (args.VirtualKey == Windows.System.VirtualKey.Left)
                game.Move(Movement.Left);
            else if (args.VirtualKey == Windows.System.VirtualKey.Right)
                game.Move(Movement.Right);
        }

        private void Draw(TetrisGameArray tetrisGameArray)
        {
            mainGrid.Children.Clear();
            for (int row = 0; row < 8; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    Piece piece = tetrisGameArray[row, column];
                    if (piece != null)
                    {
                        mainGrid.Children.Add(GetNewEllipse(row, column, Colors.Black));
                    }
                    
                }
            }
        }



        private Ellipse GetNewEllipse(int row, int column, Color color)
        {
            Ellipse el = new Ellipse();
            el.Width = el.Height = 40;
            el.Fill = new SolidColorBrush(color);
            Grid.SetColumn(el, column);
            Grid.SetRow(el, row);
            return el;
        }
    }
}
