using System;
using System.Collections.Generic;
using System.Linq;
using Tetris.Services;
using Xceed.Wpf.Toolkit;
using Color = System.Drawing.Color;

namespace Tetris.Graphics
{
    public static class DisplayObjects
    {
        private const int LineThickness = 2;
        private const int SplittingLineThickness = 10;
        private const int ResolutionFieldSize = 40;

        private static int PaintSurfaceWidth(int columns) =>
            columns * ResolutionFieldSize + (columns + 1) * LineThickness;

        private static int PaintSurfaceHeight(int rows) => rows * ResolutionFieldSize + (rows + 1) * LineThickness;

        private static int BitmapToBoardIndex(int idx) => (idx - LineThickness) / (ResolutionFieldSize + LineThickness);

        public static List<Texel> Grid(int columns, int rows, int[,] board, Dictionary<int, Color> indexToColor, out int paintSurfaceWidth, out int paintSurfaceHeight)
        {
            const int translation = LineThickness + ResolutionFieldSize;
            List<Texel> grid = new List<Texel>();
            paintSurfaceWidth = PaintSurfaceWidth(columns);
            paintSurfaceHeight = PaintSurfaceHeight(rows);
            var gridColor = Color.Black;

            for (int i = 0; i < columns + 1; i++)
                for (int j = 0; j < paintSurfaceHeight; j++)
                {
                    int row = BitmapToBoardIndex(j);
                    if (i > 0 && i < columns && indexToColor[board[i, row]] == indexToColor[board[i - 1, row]])
                        gridColor = indexToColor[board[i, row]];
                    for (int k = 0; k < LineThickness; k++)
                        grid.Add(new Texel(i * translation + k, j, gridColor));

                    gridColor = Color.Black;
                }

            for (int i = 0; i < rows + 1; i++)
                for (int j = 0; j < paintSurfaceWidth; j++)
                {
                    int column = BitmapToBoardIndex(j);
                    if (i > 0 && i < rows && indexToColor[board[column, i]] == indexToColor[board[column, i - 1]])
                        gridColor = indexToColor[board[column, i]];

                    for (int k = 0; k < LineThickness; k++)
                        grid.Add(new Texel(j, i * translation + k, gridColor));

                    gridColor = Color.Black;
                }

            for(int i=0; i<rows+1; i++)
                for(int j=0; j<columns+1; j++)
                {
                    //It looks poor, but I dont know better way to write it. At least it has cool name.
                    bool paintItBlack = i == 0 || j == 0 || i == rows || j == columns ||
                                        indexToColor[board[j, i - 1]] != indexToColor[board[j, i]] ||
                                        indexToColor[board[j, i]] != indexToColor[board[j - 1, i]] ||
                                        indexToColor[board[j - 1, i]] != indexToColor[board[j - 1, i - 1]] ||
                                        indexToColor[board[j - 1, i - 1]] != indexToColor[board[j, i - 1]];
                    if(paintItBlack)
                        for (int k = 0; k < LineThickness; k++)
                            for (int l = 0; l < LineThickness; l++)
                                grid.Add(new Texel(j * translation + l, i * translation + k, gridColor)); 

                }

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
                    result.Add(new Texel(k, aggregatedShapesHeight * fieldSize + SplittingLineThickness * i + j, Color.White));
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