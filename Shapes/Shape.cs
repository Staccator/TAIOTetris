using System;
using System.Drawing;
using Tetris.Services;

namespace Tetris.Shapes
{
    public class Shape
    {
        private static readonly Random Random = new Random();
        public int Index { get; set; }
        public OneSidedShape OneSidedShape { get; set; }
        public Color Color { get; set; }

        public Shape(int index, OneSidedShape oneSidedShape)
        {
            Index = index;
            OneSidedShape = oneSidedShape;
            Color = GetRandomColor();
        }

        private Color GetRandomColor()
            => Color.FromArgb(
                Random.Next(255),
                Random.Next(255),
                Random.Next(255));
    }
}