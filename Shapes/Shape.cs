using System;
using System.Drawing;

namespace Tetris.Shapes
{
    public class Shape
    {
        private static readonly Random Random = new Random();
        public int Index { get; set; }
        public ShapeMatrix ShapeMatrix { get; set; }
        public Color Color { get; set; }

        public Shape(int index)
        {
            Index = index;
            ShapeMatrix = new ShapeMatrix(Random.Next(7));
            Color = GetRandomColor();
        }

        private Color GetRandomColor()
            => Color.FromArgb(
                Random.Next(255),
                Random.Next(255),
                Random.Next(255));
    }
}