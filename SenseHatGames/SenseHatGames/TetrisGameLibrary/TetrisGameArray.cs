using SenseHatGames.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenseHatGames.TetrisGameLibrary
{
    /// <summary>
    /// Two dimensional array for the tetris-like game pieces
    /// </summary>
    public class TetrisGameArray
    {
        private Random random = new Random();

        public Piece[,] matrix;
        /// <summary>
        /// Indexer to facilitate access to the array
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
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
            matrix = new Piece[Constants.Rows, Constants.Columns];
        }
    }
}
