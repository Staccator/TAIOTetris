using System.Collections.Generic;
using Tetris.Shapes;
using Color = System.Drawing.Color;

namespace Tetris.Graphics
{
    public static class DisplayObjects
    {
        private const int LineThickness = 3;
        private const int FieldSize = 50;
        private static int PaintSurfaceWidth(int columns) => columns * FieldSize + (columns + 1) * LineThickness;
        private static int PaintSurfaceHeight(int rows) => rows * FieldSize + (rows + 1) * LineThickness; 
        
        public static List<Texel> Grid(int columns, int rows, out int paintSurfaceWidth, out int paintSurfaceHeight)
        {
            List<Texel> grid = new List<Texel>();
            paintSurfaceWidth = PaintSurfaceWidth(columns);
            paintSurfaceHeight = PaintSurfaceHeight(rows);
            var gridColor = Color.Black;

            for (int i = 0; i < columns + 1; i++)
            for (int j = 0; j < paintSurfaceHeight; j++)
            for (int k = 0; k < LineThickness; k++)
                grid.Add(new Texel(i * (LineThickness + FieldSize) + k, j, gridColor));

            for (int i = 0; i < rows + 1; i++)
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

        public static List<Texel> CreateShapeDisplay(int i, Shape shape)
        {
            int yShift = 2 * FieldSize * i;
            throw new System.NotImplementedException();
        }
    }
}