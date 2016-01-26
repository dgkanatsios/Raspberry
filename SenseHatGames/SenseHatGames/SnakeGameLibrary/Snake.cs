using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenseHatGames.SnakeGameLibrary
{
    //make it a class as we may need to add other stuff in the future
    public class Snake : List<SnakePiece>
    {
        
    }

    public enum SnakeMovement { Left, Right, Top, Bottom }
}
