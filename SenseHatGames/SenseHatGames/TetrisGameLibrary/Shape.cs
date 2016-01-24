using SenseHatGames.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenseHatGames.TetrisGameLibrary
{
    public class Shape : List<Piece>
    {

        private int rotationModifierIndex = -1;

        public void Rotate()
        {
            if (RotationModifiers.Count == 0) return;
            else
            {
                if (rotationModifierIndex == RotationModifiers.Count - 1)
                {
                    rotationModifierIndex = -1;
                }
                rotationModifierIndex += 1;
                for (int pieceIndex = 0; pieceIndex < this.Count; pieceIndex++)
                {
                    this[pieceIndex].Row +=
                        RotationModifiers[rotationModifierIndex][pieceIndex].Row;
                    this[pieceIndex].Column +=
                        RotationModifiers[rotationModifierIndex][pieceIndex].Column;

                }
            }
        }

        public bool CanRotate
        {
            get
            {
                return true;
            }
        }

        public bool CanMoveDown
        {
            get
            {
                return true;
            }
        }

        private List<List<RowColumn>> RotationModifiers;

        public Shape(List<List<RowColumn>> rotationModifiers)
        {
            RotationModifiers = rotationModifiers;
        }

        private Shape()
        { }


        public Shape Clone()
        {
            Shape copy = new Shape();
            for (int i = 0; i < this.Count; i++)
            {
                Piece p = new Piece(this[i].Row, this[i].Column); 
                copy.Add(p);
            }
            return copy;
        }

    }
}
