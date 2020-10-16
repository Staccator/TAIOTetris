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