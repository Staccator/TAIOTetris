using System.Drawing;

namespace DesktopApp.Models
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

        public Texel Shifted(int x, int y)
        {
            return new Texel(Position.X + x, Position.Y + y, Color);
        }
    }
}