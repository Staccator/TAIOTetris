using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using Tetris.Shapes;

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

        private static Dictionary<int, List<OneSidedShape>> _oneSidedShapesOfSize =
            new Dictionary<int, List<OneSidedShape>>();

        public static List<Shape> GenerateShapes(int shapeCount, int shapeSize, CancellationToken cancellationToken)
        {
            // if (!_oneSidedShapesOfSize.ContainsKey(shapeSize))
            // {
            //     _oneSidedShapesOfSize[shapeSize] = GenerateOneSidedShapes(shapeSize, cancellationToken);
            // }
            //
            // var oneSidedShapes = _oneSidedShapesOfSize[shapeSize];
            return Enumerable.Range(0, shapeCount)
                .Select(i => new Shape(i, GenerateRandomOneSidedShape(shapeSize, cancellationToken), shapeSize))
                .ToList();
        }

        public static List<OneSidedShape> GenerateOneSidedShapes(int maxSize, CancellationToken cancellationToken)
        {
            int startSize = 1;
            int lastAddedCellNumber = 1;
            var cellsToCheck = new List<Cell>();

            var startingCell = new Cell(maxSize - 1, 0, lastAddedCellNumber);
            var cellStack = new Stack<Cell>();
            cellStack.Push(startingCell);

            var board = new bool[2 * maxSize - 1, maxSize];
            board[startingCell.X, startingCell.Y] = true;
            var result = new List<OneSidedShape>();

            GenerateBiggerShapes(cellStack, cellsToCheck, startSize, maxSize, lastAddedCellNumber, board, result,
                cancellationToken);

            return result;
        }

        private static void GenerateBiggerShapes(Stack<Cell> cellStack,
            List<Cell> cellsToCheck,
            int currentSize,
            int maxSize,
            int lastAddedCellNumber,
            bool[,] board,
            List<OneSidedShape> resultShapes, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
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
                    board, resultShapes, cancellationToken);
                cellStack.Pop();
            }

            adjacentCells.ForEach(c => board[c.X, c.Y] = false);
        }

        public static OneSidedShape GenerateRandomOneSidedShape(int maxSize, CancellationToken cancellationToken)
        {
            OneSidedShape result = null;
            while (result is null)
            {
                int startSize = 1;
                int lastAddedCellNumber = 1;
                var cellsToCheck = new List<Cell>();

                var startingCell = new Cell(maxSize - 1, 0, lastAddedCellNumber);
                var cellStack = new Stack<Cell>();
                cellStack.Push(startingCell);

                var board = new bool[2 * maxSize - 1, maxSize];
                board[startingCell.X, startingCell.Y] = true;

                result = GenerateRandomBiggerShape(cellStack, cellsToCheck, startSize, maxSize, lastAddedCellNumber,
                    board, cancellationToken);
            }

            return result;
        }

        private static OneSidedShape GenerateRandomBiggerShape(Stack<Cell> cellStack,
            List<Cell> cellsToCheck,
            int currentSize,
            int maxSize,
            int lastAddedCellNumber,
            bool[,] board,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (currentSize == maxSize)
            {
                var fixedShape = cellStack.Select(c => new Point(c.X, c.Y)).ToList();
                return OneSidedShape.FromListOfPoints(fixedShape, maxSize);
            }

            var currentCell = cellStack.Peek();

            List<Cell> adjacentCells = GenerateAdjacentCells(board, currentCell, lastAddedCellNumber, maxSize);
            cellsToCheck.RemoveAll(c => c.Number < currentCell.Number);
            cellsToCheck.AddRange(adjacentCells);

            if (!cellsToCheck.Any()) return null;

            adjacentCells.ForEach(c => board[c.X, c.Y] = true);

            var randomCell = cellsToCheck.GetRandomElement();
            cellStack.Push(randomCell);
            return GenerateRandomBiggerShape(cellStack, cellsToCheck.Where(c => c != randomCell).ToList(),
                currentSize + 1, maxSize, lastAddedCellNumber + adjacentCells.Count,
                board, cancellationToken);
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
                .OrderBy(p => p.Y).ThenBy(p => p.X).ToArray();

            var rotatedPoints = newShape.ToArray();
            for (int i = 0; i < 3; i++)
            {
                rotatedPoints = rotatedPoints.Select(p => new Point(p.Y, maxSize - 1 - p.X)).ToArray();
                var rotatedPointsShiftX = rotatedPoints.Min(p => p.X);
                var rotatedPointsShiftY = rotatedPoints.Min(p => p.Y);

                var shapeRotation = rotatedPoints
                    .Select(p => new Point(p.X - rotatedPointsShiftX, p.Y - rotatedPointsShiftY))
                    .OrderBy(p => p.Y).ThenBy(p => p.X).ToArray();

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
}