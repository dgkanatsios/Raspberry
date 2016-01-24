using SenseHatGames.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenseHatGames.TetrisGameLibrary
{
    public static class ShapeFactory
    {
        private static List<List<RowColumn>> shapeAModifiers = new List<List<RowColumn>>();
        private static List<List<RowColumn>> shapeBModifiers = new List<List<RowColumn>>()
        {
            new List<RowColumn>(){new RowColumn(0,0),new RowColumn(-1,1), new RowColumn(-2,2)},
            new List<RowColumn>(){ new RowColumn(0, 0), new RowColumn(1, -1), new RowColumn(2, -2) }
        };
        private static List<List<RowColumn>> shapeCModifiers = new List<List<RowColumn>>()
        {
            new List<RowColumn>(){new RowColumn(0,-1),new RowColumn(1,0), new RowColumn(0,1)},
            new List<RowColumn>(){ new RowColumn(1, 0), new RowColumn(0, 1), new RowColumn(-1, 0) },
             new List<RowColumn>(){new RowColumn(0,1),new RowColumn(-1,0), new RowColumn(0,-1)},
            new List<RowColumn>(){ new RowColumn(-1, 0), new RowColumn(0, -1), new RowColumn(1, 0) }
        };

        private static Random random = new Random();
        public static Shape CreateRandomShape()
        {
            int x = random.Next(0, 3);
            if (x == 0) return CreateShape(ShapeType.ShapeA);
            else if (x == 1) return CreateShape(ShapeType.ShapeB);
            else return CreateShape(ShapeType.ShapeC);
        }

        private static Shape CreateShape(ShapeType type)
        {
            switch (type)
            {
                case ShapeType.ShapeA:
                    Shape sA = new Shape(shapeAModifiers);
                    sA.Add(new Piece(0, 3));
                    sA.Add(new Piece(0, 4));
                    sA.Add(new Piece(1, 3));
                    sA.Add(new Piece(1, 4));
                    return sA;
                case ShapeType.ShapeB:
                    Shape sB = new Shape(shapeBModifiers);
                    sB.Add(new Piece(0, 3));
                    sB.Add(new Piece(1, 3));
                    sB.Add(new Piece(2, 3));
                    return sB;
                case ShapeType.ShapeC:
                    Shape sC = new Shape(shapeCModifiers);
                    sC.Add(new Piece(0, 4));
                    sC.Add(new Piece(0, 3));
                    sC.Add(new Piece(1, 3));
                    return sC;
                default:
                    throw new Exception(type.ToString());
            }
        }
    }

    public enum ShapeType
    {
        ShapeA,
        ShapeB,
        ShapeC
    }
}
