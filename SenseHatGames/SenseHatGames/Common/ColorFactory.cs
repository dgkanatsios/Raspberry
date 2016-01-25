using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace SenseHatGames.Common
{
    public static class ColorFactory
    {
        public static readonly Color[] ShapeColors = new Color[]
            {Colors.Red, Colors.Green, Colors.Orange,Colors.Blue, Colors.Brown,Colors.Pink,Colors.Purple};

        private static Random random = new Random();
        public static Color RandomColor
        {
            get
            {
                return ShapeColors[random.Next(0, ShapeColors.Length)];
            }
        }
    }
}
