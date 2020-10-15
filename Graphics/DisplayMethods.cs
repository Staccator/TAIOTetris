using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Tetris.Algorithms;
using Tetris.Shapes;

namespace Tetris.Graphics
{
    public static class DisplayMethods
    {
        public static void ExecuteAlgorithm(TetrisFitter fitter, List<Shape> shapes, PaintSurface resolutionSurface, int shapeSize)
        {
            var indexToColor = shapes.ToDictionary(s => s.Index, s => s.Color);
            indexToColor[TetrisFitter.EmptyField] = Color.White;

            var fitResult = fitter.Fit(shapes, shapeSize);
            DisplayBoard(fitResult, indexToColor, resolutionSurface);
        }
        
        private static void DisplayBoard(int[,] board, Dictionary<int, Color> indexToColor,
            PaintSurface resolutionSurface)
        {
            var width = board.GetLength(0);
            var height = board.GetLength(1);
            int boardArea = width * height;
            
            var texelListArray = new List<Texel>[1 + boardArea];
            var grid = DisplayObjects.Grid(width, height, out int paintSurfaceWidth, out int paintSurfaceHeight);
            texelListArray[boardArea] = grid;
            
            Parallel.For(0, boardArea, i =>
            {
                int x = i % width;
                int y = i / width;
                texelListArray[i] = DisplayObjects.CreateField(new Texel(x, y, indexToColor[board[x,y]]));
            });
            
            resolutionSurface.SetupBitmap(paintSurfaceWidth, paintSurfaceHeight);
            var buffer = resolutionSurface.CreateNewBuffer();
            Parallel.For(0, texelListArray.Length, body: i => { resolutionSurface.WriteToBitmap(texelListArray[i], buffer); });
            resolutionSurface.CommitDraw(buffer);
        }
        
        private static void DisplayInputShapes(List<Shape> shapes, PaintSurface inputSurface)
        {
            var texelListArray = new List<Texel>[shapes.Count];
            
            Parallel.For(0, shapes.Count, (int i) =>
            {
                texelListArray[i] = DisplayObjects.CreateShapeDisplay(i, shapes[i]);
            });
            
            // var buffer = PaintSurface.CreateNewBuffer();
            // Parallel.For(0, texelListArray.Length, body: i => { PaintSurface.WriteToBitmap(texelListArray[i], buffer); });
            // PaintSurface.CommitDraw(buffer);
        }
    }
}