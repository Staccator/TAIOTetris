using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Tetris.Algorithms;
using Tetris.Shapes;

namespace Tetris.Graphics
{
    public static class DisplayMethods
    {
        public static void ExecuteAlgorithm(TetrisFitter fitter, int shapeCount)
        {
            //TODO change to generating shape on UI
            var shapes = new Shape[shapeCount];
            var indexToColor = new Dictionary<int, Color>();
            for (int i = 0; i < shapes.Length; i++)
            {
                shapes[i] = new Shape(i);
                indexToColor[i] = shapes[i].Color;
                indexToColor[TetrisFitter.EmptyField] = Color.White;
            }

            var fitResult = fitter.Fit(shapes);
            DisplayBoard(fitResult, indexToColor);
        }
        
        private static void DisplayBoard(int[,] board, Dictionary<int, Color> indexToColor)
        {
            var width = board.GetLength(0);
            var height = board.GetLength(1);
            int boardArea = width * height;
            
            var texelLists = new List<Texel>[1 + boardArea];
            var grid = DisplayObjects.Grid(width, height);
            texelLists[boardArea] = grid;
            
            Parallel.For(0, boardArea, i =>
            {
                int x = i % width;
                int y = i / width;
                texelLists[i] = DisplayObjects.CreateField(new Texel(x, y, indexToColor[board[x,y]]));
            });
            
            var buffer = Display.CreateNewBuffer();
            Parallel.For(0, texelLists.Length, body: i => { Display.WriteToBitmap(texelLists[i], buffer); });
            Display.CommitDraw(buffer);
        }
    }
}