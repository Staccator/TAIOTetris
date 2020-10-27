using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Tetris.Services;

namespace Tetris.Shapes
{
    public class Shape
    {
        private readonly int _size;
        private static readonly Random Random = new Random();
        public int Index { get; set; }
        public OneSidedShape OneSidedShape { get; set; }
        public Color Color { get; set; }

        public int Size => _size;

        public Shape(int index, OneSidedShape oneSidedShape, int size)
        {
            _size = size;
            Index = index;
            OneSidedShape = oneSidedShape;
            Color = GetRandomColor();
        }

        private Color GetRandomColor()
            => Color.FromArgb(DiscreteInt(), DiscreteInt(), DiscreteInt());

        private int DiscreteInt()
        {
            return 20 + Random.Next(7) * 30;
        }

        public (Shape, Shape) SplitIntoTwoRandomShapes()
        {
            var shapes = GenerateCuts(OneSidedShape.FixedShapes[0]);

            if (OneSidedShape.FixedShapes.Count > 1)
                shapes.AddRange(GenerateCuts(OneSidedShape.FixedShapes[1]));

            var split = shapes.GetRandomElement();
            var first = new Shape(Index, split.Item1, split.Item1.FixedShapes.First().Points.Length);
            var second = new Shape(Index, split.Item2, split.Item2.FixedShapes.First().Points.Length);
            first.Color = Color;
            second.Color = Color;
            return (first, second);
        }

        public List<(Shape, Shape)> GenerateAllCuts()
        {
            var shapes = GenerateCuts(OneSidedShape.FixedShapes[0]);

            if (OneSidedShape.FixedShapes.Count > 1)
                shapes.AddRange(GenerateCuts(OneSidedShape.FixedShapes[1]));

            var result = new List<(Shape, Shape)>();

            foreach (var shape in shapes)
            {
                var first = new Shape(Index, shape.Item1, shape.Item1.FixedShapes.First().Points.Length);
                var second = new Shape(Index, shape.Item2, shape.Item2.FixedShapes.First().Points.Length);
                first.Color = Color;
                second.Color = Color;
                result.Add((first, second));
            }

            return result;
        }

        public List<(OneSidedShape, OneSidedShape)> GenerateCuts(FixedShape fixedShape)
        {
            int n = fixedShape.Points.Length;
            var result = new List<(OneSidedShape, OneSidedShape)>();
            var matrix = new bool[n, n];
            foreach (var point in fixedShape.Points)
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
                            var (canCut, resultShapes) = CutShape(matrix, n, i, j - gapHeight, gapHeight);
                            if (canCut) result.Add(resultShapes);
                        }
                    }
                    else
                    {
                        if (gapHeight > 0)
                        {
                            var (canCut, resultShapes) = CutShape(matrix, n, i, j - gapHeight, gapHeight);
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

            if (gapY > 0) // one below
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

            if (leftResult.Count + rightResult.Count != n) return (false, (null, null));

            return (true,
                (OneSidedShape.FromListOfPoints(leftResult, n), OneSidedShape.FromListOfPoints(rightResult, n)));
        }

        private static void ExpandShape(bool[,] matrix, int n, Queue<Point> pointsToCheck,
            List<Point> resultShape)
        {
            while (pointsToCheck.Any())
            {
                var point = pointsToCheck.Dequeue();
                var x = point.X;
                var y = point.Y;
                if (x > 0 && matrix[x - 1, y] && !resultShape.Contains(new Point(x - 1, y)) &&
                    !pointsToCheck.Contains(new Point(x - 1, y)))
                    pointsToCheck.Enqueue(new Point(x - 1, y));
                if (x < n - 1 && matrix[x + 1, y] && !resultShape.Contains(new Point(x + 1, y)) &&
                    !pointsToCheck.Contains(new Point(x + 1, y)))
                    pointsToCheck.Enqueue(new Point(x + 1, y));
                if (y > 0 && matrix[x, y - 1] && !resultShape.Contains(new Point(x, y - 1)) &&
                    !pointsToCheck.Contains(new Point(x, y - 1)))
                    pointsToCheck.Enqueue(new Point(x, y - 1));
                if (y < n - 1 && matrix[x, y + 1] && !resultShape.Contains(new Point(x, y + 1)) &&
                    !pointsToCheck.Contains(new Point(x, y + 1)))
                    pointsToCheck.Enqueue(new Point(x, y + 1));

                resultShape.Add(point);
            }
        }
    }
}