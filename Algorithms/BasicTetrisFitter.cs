using System.Collections.Generic;
using System.Drawing;
using Tetris.Shapes;

namespace Tetris.Algorithms
{
    public class BasicTetrisFitter : TetrisFitter
    {
        public override int[,] Fit(List<Shape> shapes, int shapeSize)
        {
            var result = CreateEmptyBoard(shapes.Count * shapeSize);

            int width = result.GetLength(0);
            int height = result.GetLength(1);
            foreach (var shape in shapes)
            {
                for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    foreach (var rotation in shape.OneSidedShape.Rotations)
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