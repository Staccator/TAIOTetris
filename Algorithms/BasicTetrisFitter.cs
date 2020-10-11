using System;
using System.Linq;
using Tetris.Shapes;

namespace Tetris.Algorithms
{
    public class BasicTetrisFitter : TetrisFitter
    {
        private static readonly Random Random = new Random();
        public override int[,] Fit(Shape[] shapes)
        {
            var result = CreateEmptyBoard(shapes.Length * 4);
            var shapesIndexes = shapes.Select(s => s.Index).ToList();

            int width = result.GetLength(0);
            int height = result.GetLength(1);
            for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
            {
                result[i, j] = shapesIndexes[Random.Next(shapesIndexes.Count)];
            }

            return result;
        }
    }
}