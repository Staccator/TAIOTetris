using System.Collections.Generic;
using Color = System.Drawing.Color;

namespace Tetris.Graphics
{
    public static class DisplayObjects
    {
        private const int LineThickness = 3;
        private const int FieldSize = 50;

        private static int _columns = 12;
        private static int _rows = 12;

        public static int PaintSurfaceWidth => _columns * FieldSize + (_columns + 1) * LineThickness;
        public static int PaintSurfaceHeight => _rows * FieldSize + (_rows + 1) * LineThickness;
        
        public static List<Texel> Grid(int columns, int rows)
        {
            _columns = columns;
            _rows = rows;
            List<Texel> grid = new List<Texel>();
            int paintSurfaceWidth = PaintSurfaceWidth;
            int paintSurfaceHeight = PaintSurfaceHeight;
            var gridColor = Color.Black;

            for (int i = 0; i < _columns + 1; i++)
            for (int j = 0; j < paintSurfaceHeight; j++)
            for (int k = 0; k < LineThickness; k++)
                grid.Add(new Texel(i * (LineThickness + FieldSize) + k, j, gridColor));

            for (int i = 0; i < _rows + 1; i++)
            for (int j = 0; j < paintSurfaceWidth; j++)
            for (int k = 0; k < LineThickness; k++)
                grid.Add(new Texel(j, i * (LineThickness + FieldSize) + k, gridColor));

            return grid;
        }
        
        public static List<Texel> CreateField(Texel texel)
        {
            int gridX = texel.Position.X;
            int gridY = texel.Position.Y;
            int gap = FieldSize + LineThickness;
            int xShift = LineThickness + gridX * gap;
            int yShift = LineThickness + gridY * gap;

            var result = new List<Texel>();
            for (int i = 0; i < FieldSize; i++)
            for (int j = 0; j < FieldSize; j++)
                result.Add(new Texel(xShift + i, yShift + j, texel.Color));
            
            return result;
        }
    }
}