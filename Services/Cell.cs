using System.Drawing;

namespace Tetris.Services
{
    public class Cell
    {
        public int X;
        public int Y;
        public int Number;

        public Cell(int x, int y, int number)
        {
            X = x;
            Y = y;
            Number = number;
        }

        public static Point operator +(Cell a, Point b)
        {
            return new Point(a.X + b.X, a.Y + b.Y);
        }
    }
}