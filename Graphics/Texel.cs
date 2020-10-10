using System.Drawing;

namespace Tetris.Graphics
{
    public struct Texel
    {
        public Point Position;
        public Color Color;

        public Texel(Point position, Color color)
        {
            Position = position;
            Color = color;
        }
        public Texel(int x, int y, Color color)
        {
            Position = new Point(x, y);
            Color = color;
        }
    }
}