using SenseHatGames.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenseHatGames.TetrisGameLibrary
{
    /// <summary>
    /// Represents each shape that falls in the game
    /// </summary>
    public class Shape : List<Piece>
    {
        //will hold the additive values that will help the shape rotate
        //each item holds row and column additive information to help rotate the shape
        //from each state to another
        private List<List<RowColumn>> RotationModifiers;

        public Shape(List<List<RowColumn>> rotationModifiers)
        {
            RotationModifiers = rotationModifiers;
        }

        //private constructor to be used in the Clone() method
        private Shape()
        { }

        //index of the next rotationModifier to be applied
        private int rotationModifierIndex = -1;

        /// <summary>
        /// Rotates the shape item
        /// </summary>
        public void Rotate()
        {
            if (RotationModifiers.Count == 0) return;
            else
            {
                //reset the index
                if (rotationModifierIndex == RotationModifiers.Count - 1)
                {
                    rotationModifierIndex = -1;
                }
                rotationModifierIndex += 1;
                //add each additive information for each row and column
                //to each piece in the shape
                for (int pieceIndex = 0; pieceIndex < this.Count; pieceIndex++)
                {
                    this[pieceIndex].Row +=
                        RotationModifiers[rotationModifierIndex][pieceIndex].Row;
                    this[pieceIndex].Column +=
                        RotationModifiers[rotationModifierIndex][pieceIndex].Column;
                }
            }
        }

        /// <summary>
        /// Clones a shape
        /// Gets current pieces' information from the shape along with the set rotation modifier
        /// </summary>
        /// <returns>A copy of the original shape</returns>
        public Shape Clone()
        {
            Shape copy = new Shape();
            for (int i = 0; i < this.Count; i++)
            {
                Piece p = new Piece(this[i].Row, this[i].Column); 
                copy.Add(p);
            }
            copy.RotationModifiers = this.RotationModifiers;
            copy.rotationModifierIndex = this.rotationModifierIndex;
            return copy;
        }

    }
}
