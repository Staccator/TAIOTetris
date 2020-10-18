using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace Tetris.Services
{
    // ReSharper disable PossibleNullReferenceException
#pragma warning disable 660,661
    public class OneSidedShape
#pragma warning restore 660,661
    {
        public readonly List<FixedShape> FixedShapes = new List<FixedShape>();

        public FixedShape ShortestFixedShape => FixedShapes.OrderBy(f => f.Height).First();

        private static OneSidedShape FromListOfPoints(List<Point> points, int n)
        {
            int xShift = points.Min(p => p.X);
            int yShift = points.Min(p => p.Y);
            var firstShape = points.Select(p => new Point(p.X - xShift, p.Y - yShift))
                .OrderBy(p => p.Y).ThenBy(p => p.X).ToArray();

            var result = new OneSidedShape(firstShape);

            var rotatedPoints = firstShape.ToArray();
            for (int i = 0; i < 3; i++)
            {
                rotatedPoints = rotatedPoints.Select(p => new Point(p.Y, n - 1 - p.X)).ToArray();
                var rotatedPointsShiftX = rotatedPoints.Min(p => p.X);
                var rotatedPointsShiftY = rotatedPoints.Min(p => p.Y);

                var shapeRotation = rotatedPoints
                    .Select(p => new Point(p.X - rotatedPointsShiftX, p.Y - rotatedPointsShiftY))
                    .OrderBy(p => p.Y).ThenBy(p => p.X).ToArray();

                for (var index = 0; index < firstShape.Length; index++)
                {
                    if (firstShape[index] != shapeRotation[index])
                    {
                        result.Add(shapeRotation);
                        break;
                    }
                }
            }

            return result;
        }

        public OneSidedShape(Point[] rotation)
        {
            FixedShapes.Add(new FixedShape(rotation));
        }

        public void Add(Point[] rotation)
        {
            FixedShapes.Add(new FixedShape(rotation));
        }

        public static bool operator ==(OneSidedShape left, Point[] right)
        {
            var firstFixedShape = left.FixedShapes.First();

            var firstFixedShapePoints = firstFixedShape.Points;
            for (var index = 0; index < firstFixedShapePoints.Length; index++)
            {
                var point1 = firstFixedShapePoints[index];
                var point2 = right[index];
                if (point1 != point2)
                    return false;
            }

            return true;
        }

        public static bool operator !=(OneSidedShape left, Point[] right)
        {
            return !(left == right);
        }

        public List<(OneSidedShape, OneSidedShape)> GenerateCuts(int n)
        {
            var result = new List<(OneSidedShape, OneSidedShape)>();
            var matrix = new bool[n, n];
            foreach (var point in FixedShapes.First().Points)
            {
                matrix[point.X, point.Y] = true;
            }

            for (int i = 0; i < n - 1; i++)
            {
                int gapHeight = 0;
                for (int j = 0; j < n; j++)
                {
                    if (matrix[i, j] && matrix[i + 1, j])
                    {
                        gapHeight += 1;
                        if (j == n - 1)
                        {
                            (bool canCut, (OneSidedShape, OneSidedShape) resultShapes) =
                                CutShape(matrix, n, i, j - gapHeight, gapHeight);
                            if (canCut) result.Add(resultShapes);
                        }
                    }
                    else
                    {
                        if (gapHeight > 0)
                        {
                            (bool canCut, (OneSidedShape, OneSidedShape) resultShapes) =
                                CutShape(matrix, n, i, j - gapHeight, gapHeight);
                            if (canCut) result.Add(resultShapes);
                            gapHeight = 0;
                        }
                    }
                }
            }

            return result;
        }

        private (bool canCut, (OneSidedShape, OneSidedShape) resultShapes) CutShape(bool[,] matrix, int n, int gapX,
            int gapY,
            int gapHeight)
        {
            var leftResult = new List<Point>();
            var rightResult = new List<Point>();
            var leftPointsToCheck = new Queue<Point>();
            var rightPointsToCheck = new Queue<Point>();

            if (gapY > 1) // one below
            {
                if (matrix[gapX, gapY - 1]) leftPointsToCheck.Enqueue(new Point(gapX, gapY - 1));
                if (matrix[gapX + 1, gapY - 1]) rightPointsToCheck.Enqueue(new Point(gapX + 1, gapY - 1));
            }

            if (gapY + gapHeight < n) // one above
            {
                if (matrix[gapX, gapY + gapHeight]) leftPointsToCheck.Enqueue(new Point(gapX, gapY + gapHeight));
                if (matrix[gapX + 1, gapY + gapHeight])
                    rightPointsToCheck.Enqueue(new Point(gapX + 1, gapY + gapHeight));
            }

            for (int i = gapY; i < gapY + gapHeight; i++) // next to
            {
                if (gapX > 0 && matrix[gapX - 1, i]) leftPointsToCheck.Enqueue(new Point(gapX - 1, i));
                if (gapX < n - 2 && matrix[gapX + 2, i]) rightPointsToCheck.Enqueue(new Point(gapX + 2, i));

                leftResult.Add(new Point(gapX, i));
                rightResult.Add(new Point(gapX + 1, i));
            }

            ExpandShape(matrix, n, leftPointsToCheck, leftResult);
            ExpandShape(matrix, n, rightPointsToCheck, rightResult);

            Debug.Assert(leftResult.Count + rightResult.Count == n);
            return (true, (FromListOfPoints(leftResult, n), FromListOfPoints(rightResult, n)));
        }

        private static void ExpandShape(bool[,] matrix, int n, Queue<Point> pointsToCheck,
            List<Point> resultShape)
        {
            while (pointsToCheck.Any())
            {
                var point = pointsToCheck.Dequeue();
                var x = point.X;
                var y = point.Y;
                if (x > 0 && matrix[x - 1, y] && !resultShape.Contains(new Point(x - 1, y)))
                    pointsToCheck.Enqueue(new Point(x - 1, y));
                if (x < n - 1 && matrix[x + 1, y] && !resultShape.Contains(new Point(x + 1, y)))
                    pointsToCheck.Enqueue(new Point(x + 1, y));
                if (y > 0 && matrix[x, y - 1] && !resultShape.Contains(new Point(x, y - 1)))
                    pointsToCheck.Enqueue(new Point(x, y - 1));
                if (y < n - 1 && matrix[x, y + 1] && !resultShape.Contains(new Point(x, y + 1)))
                    pointsToCheck.Enqueue(new Point(x, y + 1));

                resultShape.Add(point);
            }
        }
    }
}