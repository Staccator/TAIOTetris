using System.Drawing;
using System.Linq;

namespace Tetris.Services
{
    public class FixedShape
    {
        public Point[] Points;

        public FixedShape(Point[] points)
        {
            Points = points;
        }

        public int Height => Points.Max(p => p.Y) + 1;
    }
}