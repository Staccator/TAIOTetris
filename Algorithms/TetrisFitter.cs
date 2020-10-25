using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using Tetris.Services;
using Tetris.Shapes;

namespace Tetris.Algorithms
{
    public abstract class TetrisFitter
    {
        public const int EmptyField = -1;
        
        public abstract (int[,],int) Fit(List<Shape> shapes, CancellationToken tokenSourceToken);
        
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

        protected List<Point> MatchShapeOnBoard(int[,] board, FixedShape shape, Point location)
        {
            var shapePoints = shape.Points;
            int width = board.GetLength(0);
            int height = board.GetLength(1);
            var boardPositions = shapePoints.Select(p => location.Add(p)).ToList();

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