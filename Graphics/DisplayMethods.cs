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
        public static void ExecuteAlgorithm(TetrisFitter fitter, Shape[] shapes)
        {
            
            var indexToColor = shapes.ToDictionary(s => s.Index, s => s.Color);
            indexToColor[TetrisFitter.EmptyField] = Color.White;

            var fitResult = fitter.Fit(shapes);
            DisplayBoard(fitResult, indexToColor);
        }
        
        private static void DisplayBoard(int[,] board, Dictionary<int, Color> indexToColor)
        {
            var width = board.GetLength(0);
            var height = board.GetLength(1);
            int boardArea = width * height;
            
            var texelListArray = new List<Texel>[1 + boardArea];
            var grid = DisplayObjects.Grid(width, height);
            texelListArray[boardArea] = grid;
            
            Parallel.For(0, boardArea, i =>
            {
                int x = i % width;
                int y = i / width;
                texelListArray[i] = DisplayObjects.CreateField(new Texel(x, y, indexToColor[board[x,y]]));
            });
            
            var buffer = Display.CreateNewBuffer();
            Parallel.For(0, texelListArray.Length, body: i => { Display.WriteToBitmap(texelListArray[i], buffer); });
            Display.CommitDraw(buffer);
        }
        
        private static void DisplayShapes(Shape[] shapes)
        {
            var texelListArray = new List<Texel>[shapes.Length];
            
            Parallel.For(0, shapes.Length, (int i) =>
            {
                texelListArray[i] = DisplayObjects.CreateShapeDisplay(i, shapes[i]);
            });
            
            var buffer = Display.CreateNewBuffer();
            Parallel.For(0, texelListArray.Length, body: i => { Display.WriteToBitmap(texelListArray[i], buffer); });
            Display.CommitDraw(buffer);
        }
    }
}