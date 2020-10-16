using System;
using System.Collections.Generic;
using System.Linq;
using Tetris.Services;
using Color = System.Drawing.Color;

namespace Tetris.Graphics
{
    public static class DisplayObjects
    {
        private const int LineThickness = 3;
        private const int SplittingLineThickness = 10;
        private const int ResolutionFieldSize = 40;

        private static int PaintSurfaceWidth(int columns) =>
            columns * ResolutionFieldSize + (columns + 1) * LineThickness;

        private static int PaintSurfaceHeight(int rows) => rows * ResolutionFieldSize + (rows + 1) * LineThickness;

        public static List<Texel> Grid(int columns, int rows, out int paintSurfaceWidth, out int paintSurfaceHeight)
        {
            List<Texel> grid = new List<Texel>();
            paintSurfaceWidth = PaintSurfaceWidth(columns);
            paintSurfaceHeight = PaintSurfaceHeight(rows);
            var gridColor = Color.Black;

            for (int i = 0; i < columns + 1; i++)
            for (int j = 0; j < paintSurfaceHeight; j++)
            for (int k = 0; k < LineThickness; k++)
                grid.Add(new Texel(i * (LineThickness + ResolutionFieldSize) + k, j, gridColor));

            for (int i = 0; i < rows + 1; i++)
            for (int j = 0; j < paintSurfaceWidth; j++)
            for (int k = 0; k < LineThickness; k++)
                grid.Add(new Texel(j, i * (LineThickness + ResolutionFieldSize) + k, gridColor));

            return grid;
        }

        public static List<Texel> FieldDisplay(Texel texel)
        {
            int gridX = texel.Position.X;
            int gridY = texel.Position.Y;
            int gap = ResolutionFieldSize + LineThickness;
            int xShift = LineThickness + gridX * gap;
            int yShift = LineThickness + gridY * gap;

            var result = new List<Texel>();
            for (int i = 0; i < ResolutionFieldSize; i++)
            for (int j = 0; j < ResolutionFieldSize; j++)
                result.Add(new Texel(xShift + i, yShift + j, texel.Color));

            return result;
        }

        public static List<Texel> ShapesFrame(int inputSurfaceWidth, int shapeSize, List<int> shapeHeights,
            out int paintSurfaceWidth, out int paintSurfaceHeight, out int fieldSize)
        {
            fieldSize = inputSurfaceWidth / shapeSize;
            fieldSize = Math.Min(fieldSize, ResolutionFieldSize);
            paintSurfaceWidth = fieldSize * shapeSize;
            var shapeCount = shapeHeights.Count;
            paintSurfaceHeight = (shapeCount - 1) * SplittingLineThickness + shapeHeights.Sum() * fieldSize;

            var result = new List<Texel>();
            var aggregatedShapesHeight = 0;
            for (int i = 0; i < shapeCount - 1; i++)
            {
                aggregatedShapesHeight += shapeHeights[i];
                for (int k = 0; k < paintSurfaceWidth; k++)
                for (int j = 0; j < SplittingLineThickness; j++)
                    result.Add(new Texel(k, aggregatedShapesHeight * fieldSize + SplittingLineThickness * i + j, Color.Black));
            }

            return result;
        }

        public static List<Texel> ShapeDisplay(int index, int fieldSize, Color color, FixedShape fixedShape,
            List<int> shapeHeights)
        {
            int yShift = index * SplittingLineThickness + shapeHeights.Take(index).Sum() * fieldSize;

            var result = new List<Texel>();
            foreach (var point in fixedShape.Points)
            {
                for (int i = 0; i < fieldSize; i++)
                for (int j = 0; j < fieldSize; j++)
                    result.Add(new Texel(point.X * fieldSize + i, yShift + point.Y * fieldSize + j, color));
            }

            return result;
        }
    }
}