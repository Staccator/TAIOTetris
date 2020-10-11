using System;
using Tetris.Shapes;

namespace Tetris.Algorithms
{
    public abstract class TetrisFitter
    {
        private const int EmptyField = -1;
        
        protected int[,] CreateEmptyBoard(int area)
        {
            var sqrt = (int) Math.Sqrt(area);
            int height = 0;
            for (int i = sqrt; i >= 0; i--)
            {
                if (area % i == 0)
                {
                    height = i;
                    break;
                }
            }

            int width = area / height;
            var result = new int[width, height];
            for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
            {
                result[i, j] = EmptyField;
            }

            return result;
        }

        public abstract int[,] Fit(Shape[] shapes);
    }
}