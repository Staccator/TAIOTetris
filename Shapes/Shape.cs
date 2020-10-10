using System;
using System.Collections.Generic;
using System.Drawing;
using Tetris.Graphics;

namespace Tetris.Shapes
{
    public class Shape
    {
        private static readonly Random Random = new Random();
        public int Index { get; set; }
        public ShapeOnMatrix ShapeOnMatrix { get; set; }
        public Color Color { get; set; }

        public Shape(int index)
        {
            Index = index;
            ShapeOnMatrix = new ShapeOnMatrix(Random.Next(7));
            Color = GetRandomColor();
        }

        private Color GetRandomColor()
            => Color.FromArgb(
                Random.Next(255),
                Random.Next(255),
                Random.Next(255));

        public List<Texel> GetShapeTexels(Point shift = new Point())
        {
            shift = new Point(Random.Next(7), Random.Next(5));
            var result = new List<Texel>();
            foreach (var point in ShapeOnMatrix.GetMatrixPositions())
            {
                var shiftedPoint = new Point(point.X + shift.X, point.Y + shift.Y);
                result.AddRange(DisplayObjects.CreateField(new Texel(shiftedPoint, Color)));
            }

            return result;
        }
    }
}