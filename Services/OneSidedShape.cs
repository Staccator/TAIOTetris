using System.Collections.Generic;
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

        public static OneSidedShape FromListOfPoints(List<Point> points, int n)
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

                bool newShapeAdded = false;
                for (var index = 0; index < firstShape.Length; index++)
                {
                    if (firstShape[index] != shapeRotation[index])
                    {
                        result.Add(shapeRotation);
                        newShapeAdded = true;
                        break;
                    }
                }

                if (!newShapeAdded) break;
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
    }
}