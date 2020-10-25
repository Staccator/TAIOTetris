using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Policy;
using System.Threading;
using Tetris.Services;
using Tetris.Shapes;

namespace Tetris.Algorithms
{
    class OptimalTetrisFitter : HeuristicTetrisFitter
    {
        public override (int[,],int) Fit(List<Shape> shapes, CancellationToken tokenSourceToken)
        {
            int shapeSize = shapes.First().Size;
            int shapeCount = shapes.Count;
            int[,] board = CreateEmptyBoard(shapeCount * shapeSize);
            List<Shape> fitted = new List<Shape>();
            List<List<Shape>> listsOfShapes = new List<List<Shape>>() { shapes };
            List <string> checkedLists = new List<string>();

            while (!FitList(listsOfShapes[0], board, fitted, tokenSourceToken))
            {
                // Here is a place for parallelization, we can check multiple lists at once. 
                // What is more, we should find a way to check whether or not certain list was already processed.

                var checkedList = listsOfShapes[0];
                listsOfShapes.RemoveAt(0);

                for (int i = 0; i < checkedList.Count; ++i)
                {
                    var shape = checkedList[i];
                    checkedList.RemoveAt(i);
                    foreach (var split in shape.GenerateAllCuts())
                    {
                        var listToAdd = new List<Shape>(checkedList);
                        listToAdd.Add(split.Item1);
                        listToAdd.Add(split.Item2);

                        if (!IsAlreadyChecked(checkedLists, listToAdd))
                            listsOfShapes.Add(listToAdd);
                    }
                    checkedList.Insert(i, shape);
                }
            }

            return (board, listsOfShapes[0].Count-shapes.Count);
        }

        private bool FitList(List<Shape> shapes, int[,] board, List<Shape> fitted, CancellationToken tokenSourceToken)
        {
            if (fitted.Count == shapes.Count)
                return true;

            tokenSourceToken.ThrowIfCancellationRequested();
            foreach (var shape in shapes)
            {
                if (fitted.Contains(shape))
                    continue;

                fitted.Add(shape);
                // If we failed to fit "shape", then we can be sure that the shape we put before is not placed correcly.
                // So we can make extra step up in the recursion.
                foreach (var fixedShape in shape.OneSidedShape.FixedShapes)
                {
                    var addedPoints = new List<Point>();

                    if (TryToFit(fixedShape, board, shape.Index, addedPoints))
                    {
                        if (FitList(shapes, board, fitted, tokenSourceToken))
                            return true;

                        RemoveShapeFromBoard(board, addedPoints);
                    }
                }
                fitted.Remove(shape);
            }

            return false;
        }

        private void RemoveShapeFromBoard(int[,] board, List<Point> addedPoints)
        {
            foreach (var point in addedPoints)
                board[point.X, point.Y] = -1;
        }

        private bool TryToFit(FixedShape fixedShape, int[,] board, int index, List<Point> addedPoints)
        {
            int width = board.GetLength(0);
            int height = board.GetLength(1);

            for (int i = 0; i < width; ++i)
                for (int j = 0; j < height; ++j)
                {
                    var result = MatchShapeOnBoard(board, fixedShape, new Point(i, j));
                    if (result.Count == fixedShape.Points.Length)
                    {
                        foreach (var point in result)
                        {
                            board[point.X, point.Y] = index;
                            addedPoints.Add(point);
                        }
                        return true;
                    }


                }

            return false;
        }

        private bool IsAlreadyChecked(List<string> checkedLists, List<Shape> shapesList)
        {
            string hash = ListToHash(shapesList);

            if (checkedLists.Contains(hash))
                return true;

            checkedLists.Add(hash);
            return false;
        }

        private string ListToHash(List<Shape> list)
        {
            List<string> listHash = new List<string>();

            foreach(var shape in list)
            {
                List<int> shapesCodes = new List<int>();
                foreach(var point in shape.OneSidedShape.FixedShapes[0].Points)
                {
                    shapesCodes.Add(point.X + point.Y * shape.Size);
                }
                shapesCodes.Sort();
                string hash = string.Join(".",shapesCodes.ToArray());
                listHash.Add(hash);
            }
            listHash.Sort();
            return string.Join("_",listHash.ToArray());
        }
    }
}

