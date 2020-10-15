using System;
using System.Collections.Generic;
using System.Drawing;
using Tetris.Shapes;

namespace Tetris.Algorithms
{
    public abstract class TetrisFitter
    {
        public const int EmptyField = -1;
        
        public abstract int[,] Fit(Shape[] shapes);
        
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

        protected List<Point> MatchShapeOnBoard(int[,] board, ShapeMatrix matrix, Point location)
        {
            int width = board.GetLength(0);
            int height = board.GetLength(1);
            var boardPositions = matrix.GetBoardPositions(location.X, location.Y);

            var fittingPositions = new List<Point>();
            foreach (var position in boardPositions)
            {
                if (position.X < 0 || position.X >= width || position.Y < 0 || position.Y >= height)
                    continue;

                if (board[position.X, position.Y] == EmptyField)
                    fittingPositions.Add(position);
            }

            return fittingPositions;
        }
    }
}