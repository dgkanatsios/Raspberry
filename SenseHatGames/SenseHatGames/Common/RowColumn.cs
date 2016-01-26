using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenseHatGames.Common
{
    /// <summary>
    /// Helpful struct to encapsulate Row and Column information
    /// </summary>
    public struct RowColumn
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public RowColumn(int row, int column)
        {
            Row = row; Column = column;
        }
    }
}
