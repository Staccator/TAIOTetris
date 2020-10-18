using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Tetris.Shapes;

namespace Tetris.Algorithms
{
    public class TestTetrisFitter : TetrisFitter
    {
        public override int[,] Fit(List<Shape> shapes)
        {
            int shapeCount = shapes.Count;
            var result = CreateEmptyBoard(shapeCount * shapes.First().Size);

            int width = result.GetLength(0);
            int height = result.GetLength(1);
            while (shapes.Any())
            {
                int bestResultNumber = int.MaxValue;
                (List<Point> points, Shape shape) bestResult = (null, null);
                foreach (var shape in shapes)
                {
                    for (int j = 0; j < height; j++)
                    for (int i = 0; i < width; i++)
                    {
                        bool foundFit = false;
                        foreach (var rotation in shape.OneSidedShape.FixedShapes)
                        {
                            var fittingPoints = MatchShapeOnBoard(result, rotation, new Point(i, j));
                            if (fittingPoints.Count == shape.Size)
                            {
                                foundFit = true;
                                int resultNumber = j * width + i + rotation.Points[0].X;
                                if (resultNumber < bestResultNumber)
                                {
                                    bestResultNumber = resultNumber;
                                    bestResult = (fittingPoints, shape);
                                }
                            }
                        }

                        if (foundFit) goto foundFit;
                    }

                    foundFit: ;
                }

                if (bestResult.points == null)
                {
                    List<Shape> shapeSplits = new List<Shape>();
                    foreach(var shape in shapes)
                    {
                        var split = shape.SplitIntoTwoRandomShapes();
                        shapeSplits.Add(split.Item1);
                        shapeSplits.Add(split.Item2);
                    }

                    shapes = shapeSplits;
                    continue;
                }

                foreach (var point in bestResult.points)
                {
                    result[point.X, point.Y] = bestResult.shape.Index;
                }

                shapes.Remove(bestResult.shape);
            }

            return result;
        }
    }
}