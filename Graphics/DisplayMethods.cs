using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Tetris.Shapes;

namespace Tetris.Graphics
{
    public static class DisplayMethods
    {
        public static void DisplayBoard(int[,] board, Dictionary<int, Color> indexToColor,
            PaintSurface resolutionSurface)
        {
            var width = board.GetLength(0);
            var height = board.GetLength(1);
            int boardArea = width * height;
            
            var texelListArray = new List<Texel>[1 + boardArea];
            var grid = DisplayObjects.Grid(width, height, board, indexToColor, out int paintSurfaceWidth, out int paintSurfaceHeight);
            texelListArray[boardArea] = grid;
            
            Parallel.For(0, boardArea, i =>
            {
                int x = i % width;
                int y = i / width;
                texelListArray[i] = DisplayObjects.FieldDisplay(new Texel(x, y, indexToColor[board[x,y]]));
            });
            
            resolutionSurface.SetupBitmap(paintSurfaceWidth, paintSurfaceHeight);
            var buffer = resolutionSurface.CreateNewBuffer();
            Parallel.For(0, texelListArray.Length, body: i => { resolutionSurface.WriteToBitmap(texelListArray[i], buffer); });
            resolutionSurface.CommitDraw(buffer);
        }

        public static void DisplayInputShapes(int shapeSize, List<Shape> shapes, PaintSurface inputSurface)
        {
            int shapeCount = shapes.Count;
            var texelListArray = new List<Texel>[1 + shapeCount];

            var fixedShapes = shapes.Select(s => s.OneSidedShape.ShortestFixedShape).ToList();
            var fixedShapeHeights = fixedShapes.Select(f => f.Height).ToList();
            var frame = DisplayObjects.ShapesFrame(280, shapeSize, fixedShapeHeights, out int paintSurfaceWidth, out int paintSurfaceHeight, out int fieldSize);
            texelListArray[shapeCount] = frame;
            
            Parallel.For(0, fixedShapes.Count, i =>
            {
                texelListArray[i] = DisplayObjects.ShapeDisplay(i, fieldSize, shapes[i].Color, fixedShapes[i], fixedShapeHeights);
            });
            
            inputSurface.SetupBitmap(paintSurfaceWidth, paintSurfaceHeight);
            var buffer = inputSurface.CreateNewBuffer();
            Parallel.For(0, texelListArray.Length, body: i => { inputSurface.WriteToBitmap(texelListArray[i], buffer); });
            inputSurface.CommitDraw(buffer);
        }
    }
}