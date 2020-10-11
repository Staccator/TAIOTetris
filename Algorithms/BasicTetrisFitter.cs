using System;
using System.Drawing;
using System.Linq;
using Tetris.Shapes;

namespace Tetris.Algorithms
{
    public class BasicTetrisFitter : TetrisFitter
    {
        private static readonly Random Random = new Random();

        public override int[,] Fit(Shape[] shapes)
        {
            var result = CreateEmptyBoard(shapes.Length * 4);
            var shapesIndexes = shapes.Select(s => s.Index).ToList();

            int width = result.GetLength(0);
            int height = result.GetLength(1);
            foreach (var shape in shapes)
            {
                for (int i = 0; i < width + 3; i++)
                for (int j = -3; j < height + 3; j++)
                for (int k = 0; k < 4; k++)
                {
                    shape.ShapeMatrix.RotateRight();
                    var fittingPoints = MatchMatrixOnBoard(result, shape.ShapeMatrix, new Point(i, j));
                    if (fittingPoints.Count == 4)
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