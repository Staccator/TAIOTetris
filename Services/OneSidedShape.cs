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
        public readonly List<Point[]> Rotations = new List<Point[]>();

        public OneSidedShape(Point[] rotation)
        {
            Rotations.Add(rotation);
        }

        public void Add(Point[] rotation)
        {
            Rotations.Add(rotation);
        }


        public static bool operator ==(OneSidedShape left, Point[] right)
        {
            var points = left.Rotations.First();

            for (var index = 0; index < points.Length; index++)
            {
                var point1 = points[index];
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