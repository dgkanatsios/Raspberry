using SenseHatGames.SnakeGameLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
            CoreWindow.GetForCurrentThread().KeyUp += KeyboardInput;
        }

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

        
        SnakeGame game = new SnakeGame();

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {

            for (int i = 0; i < 8; i++)
            {
                mainGrid.RowDefinitions.Add(new RowDefinition());
                mainGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            
            Draw(game.GameArray);
            game.Start();
            game.Updated += Game_Updated;
            game.PlayerLost += Game_PlayerLost;
        }

        private void Game_PlayerLost(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Game_Updated(object sender, EventArgs e)
        {
            Draw(game.GameArray);
        }

        

        private void Draw(SnakeGameArray snakeGameArray)
        {
            mainGrid.Children.Clear();
            for (int row = 0; row < 8; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    if (snakeGameArray[row, column] != null)
                    {
                        mainGrid.Children.Add(GetNewEllipse(row, column));
                    }
                }
            }
        }

        private Ellipse GetNewEllipse(int row, int column)
        {
            Ellipse el = new Ellipse();
            el.Width = el.Height = 40;
            el.Fill = new SolidColorBrush(Colors.Black);
            Grid.SetColumn(el, column);
            Grid.SetRow(el, row);
            return el;
        }

    }
}
