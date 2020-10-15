using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Tetris.Services
{
    public static class ShapeGenerator
    {
        private static readonly List<Point> RelativeCellsToCheck = new List<Point>
        {
            new Point(-1, 0),
            new Point(0, -1),
            new Point(1, 0),
            new Point(0, 1),
        };

        public static List<OneSidedShape> GenerateShapeMatrices(int maxSize)
        {
            maxSize = 3;
            int startSize = 1;
            int lastAddedCellNumber = 1;
            var cellsToCheck = new List<Cell>();

            var startingCell = new Cell(maxSize - 1, 0, lastAddedCellNumber);
            var cellStack = new Stack<Cell>();
            cellStack.Push(startingCell);

            var board = new bool[2 * maxSize - 1, maxSize];
            board[startingCell.X, startingCell.Y] = true;
            var result = new List<OneSidedShape>();

            GenerateBiggerShapes(cellStack, cellsToCheck, startSize, maxSize, lastAddedCellNumber, board, result);

            return result;
        }

        private static void GenerateBiggerShapes(
            Stack<Cell> cellStack,
            List<Cell> cellsToCheck,
            int currentSize,
            int maxSize,
            int lastAddedCellNumber,
            bool[,] board,
            List<OneSidedShape> resultShapes
        )
        {
            if (currentSize == maxSize)
            {
                var fixedShape = cellStack.Select(c => new Point(c.X, c.Y)).ToList();
                AddNewShape(fixedShape, resultShapes, maxSize);
                return;
            }

            var currentCell = cellStack.Peek();

            List<Cell> adjacentCells = GenerateAdjacentCells(board, currentCell, lastAddedCellNumber, maxSize);
            cellsToCheck.RemoveAll(c => c.Number < currentCell.Number);
            cellsToCheck.AddRange(adjacentCells);

            adjacentCells.ForEach(c => board[c.X, c.Y] = true);

            foreach (var cell in cellsToCheck)
            {
                cellStack.Push(cell);
                GenerateBiggerShapes(cellStack, cellsToCheck.Where(c => c != cell).ToList(),
                    currentSize + 1, maxSize, lastAddedCellNumber + adjacentCells.Count,
                    board, resultShapes);
                cellStack.Pop();
            }

            adjacentCells.ForEach(c => board[c.X, c.Y] = false);
        }

        private static List<Cell> GenerateAdjacentCells(bool[,] board, Cell currentCell, int lastAddedCellNumber,
            int maxSize)
        {
            var result = new List<Cell>();
            foreach (var point in RelativeCellsToCheck)
            {
                var adjacentCell = currentCell + point;
                if (adjacentCell.Y < 0 ||
                    adjacentCell.Y == 0 && adjacentCell.X < maxSize - 1 ||
                    board[adjacentCell.X, adjacentCell.Y]
                )
                {
                    continue;
                }

                result.Add(new Cell(adjacentCell.X, adjacentCell.Y, ++lastAddedCellNumber));
            }

            return result;
        }

        private static void AddNewShape(List<Point> points, List<OneSidedShape> resultShapes, int maxSize)
        {
            int xShift = points.Min(p => p.X);
            var newShape = points.Select(p => new Point(p.X - xShift, p.Y))
                .OrderBy(p => p.X).ThenBy(p => p.Y).ToArray();

            var rotatedPoints = newShape;
            for (int i = 0; i < 3; i++)
            {
                rotatedPoints = rotatedPoints.Select(p => new Point(p.Y, maxSize - 1 - p.X)).ToArray();
                var rotatedPointsShiftX = rotatedPoints.Min(p => p.X);
                var rotatedPointsShiftY = rotatedPoints.Min(p => p.Y);
                
                var shapeRotation = rotatedPoints.Select(p => new Point(p.X - rotatedPointsShiftX, p.Y - rotatedPointsShiftY))
                    .OrderBy(p => p.X).ThenBy(p => p.Y).ToArray();

                foreach (var oneSidedShape in resultShapes)
                {
                    if (oneSidedShape == shapeRotation)
                    {
                        oneSidedShape.Add(newShape);
                        return;
                    }
                }
            }

            resultShapes.Add(new OneSidedShape(newShape));
        }
    }

    // ReSharper disable PossibleNullReferenceException
#pragma warning disable 660,661
    public class OneSidedShape
#pragma warning restore 660,661
    {
        public readonly List<Point[]> Rotations = new List<Point[]>();

        public OneSidedShape(Point[] rotation)
        {
            Rotations.Add(rotation);
        }

        public void Add(Point[] rotation)
        {
            Rotations.Add(rotation);
        }


        public static bool operator ==(OneSidedShape left, Point[] right)
        {
            var points = left.Rotations.First();

            for (var index = 0; index < points.Length; index++)
            {
                var point1 = points[index];
                var point2 = right[index];
                if (point1 != point2)
                    return false;
            }

            return true;
        }

        public static bool operator !=(OneSidedShape left, Point[] right)
        {
            return !(left == right);
        }
    }
}