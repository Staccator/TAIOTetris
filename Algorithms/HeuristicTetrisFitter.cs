﻿using System.Collections.Generic;
using System.Drawing;
using Tetris.Shapes;

namespace Tetris.Algorithms
{
    public class HeuristicTetrisFitter : TetrisFitter
    {
        public override int[,] Fit(List<Shape> shapes, int shapeSize)
        {
            int shapeCount = shapes.Count;
            var result = CreateEmptyBoard(shapeCount * shapeSize);

            int width = result.GetLength(0);
            int height = result.GetLength(1);
            for (int k = 0; k < shapeCount; k++)
            {
                int bestResultNumber = int.MaxValue;
                (List<Point> points, Shape shape) bestResult = (null, null);
                foreach (var shape in shapes)
                {
                    for (int j = 0; j < height; j++)
                    for (int i = 0; i < width; i++)
                        foreach (var rotation in shape.OneSidedShape.Rotations)
                        {
                            var fittingPoints = MatchShapeOnBoard(result, rotation, new Point(i, j));
                            if (fittingPoints.Count == shapeSize)
                            {
                                int resultNumber = j * width + i + rotation[0].X;
                                if (resultNumber < bestResultNumber)
                                {
                                    bestResultNumber = resultNumber;
                                    bestResult = (fittingPoints, shape);
                                }

                            }
                            // TODO check if its needed // goto foundFit;
                        }

                    // foundFit: ;
                }

                if (bestResult.points == null)
                {
                    break;
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