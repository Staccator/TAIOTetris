using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Tetris.Shapes;

namespace Tetris.Algorithms
{
    public class BasicTetrisFitter : TetrisFitter
    {
        public override int[,] Fit(List<Shape> shapes)
        {
            int shapeSize = shapes.First().Size;
            var result = CreateEmptyBoard(shapes.Count * shapeSize);

            int width = result.GetLength(0);
            int height = result.GetLength(1);
            foreach (var shape in shapes)
            {
                for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++)
                    foreach (var rotation in shape.OneSidedShape.FixedShapes)
                    {
                        var fittingPoints = MatchShapeOnBoard(result, rotation, new Point(i, j));
                        if (fittingPoints.Count == shapeSize)
                        {
                            foreach (var fittingPoint in fittingPoints)
                            {
                                result[fittingPoint.X, fittingPoint.Y] = shape.Index;
                            }

                            goto foundFit;
                        }
                    }

                foundFit: ;
            }

            return result;
        }
    }
}