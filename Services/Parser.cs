using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Tetris.Shapes;

namespace Tetris.Services
{
    public static class Parser
    {
        private static readonly Dictionary<int, int> pentominoNumberToPentominoGenerator = new Dictionary<int, int>
        {
            {6, 0}, {9, 1}, {12, 2}, {5, 3}, {11, 4}, {7, 5}, {2, 6}, {13, 7}, {17, 8}, {4, 9}, {15, 10},
            {10, 11}, {16, 12}, {1, 13}, {8, 14}, {3, 15}, {18, 16}, {14, 17}
        };

        private static readonly Dictionary<int, int> hexominoNumberToHexominoGenerator = new Dictionary<int, int>
        {
            {1, 46}, {2, 37}, {3, 45}, {4, 43}, {5, 47}, {6, 24}, {7, 35}, {8, 17}, {9, 20}, {10, 14}, {11, 44},
            {12, 42}, {13, 27}, {14, 33}, {15, 36}, {16, 59}, {17, 57}, {18, 48}, {19, 18}, {20, 49}, {21, 40}, {22, 3},
            {23, 51}, {24, 22}, {25, 53}, {26, 50}, {27, 31}, {28, 11}, {29, 41}, {30, 26}, {31, 34}, {32, 1}, {33, 19},
            {34, 21}, {35, 54}
        };

        public static List<(List<Shape> shapes, int shapeSize)> Parse(string file)
        {
            var result = new List<(List<Shape> shapes, int shapeSize)>();
            
            var lines = file.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);

            var shapeSize = -1;
            for (var k = 0; k < lines.Length; k++)
            {
                var modI = k % 3;
                var shapesOfSizeCount = shapeSize == 5 ? 18 : 35;
                var line = lines[k];
                switch (modI)
                {
                    case 0:
                        shapeSize = int.Parse(line);
                        break;
                    case 2:
                        var shapeCounts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(shapesOfSizeCount).Select(int.Parse).ToList();
                        var shapes = new List<Shape>();
                        var shapeIndex = 0;
                        for (var i = 0; i < shapeCounts.Count; i++)
                        {
                            var pentominoIndex = shapeSize == 5
                                ? pentominoNumberToPentominoGenerator[i + 1]
                                : hexominoNumberToHexominoGenerator[i + 1];
                            var polyominos = ShapeGenerator.GenerateOneSidedShapes(shapeSize, CancellationToken.None);
                            var oneSidedShape = polyominos[pentominoIndex];
                            for (var j = 0; j < shapeCounts[i]; j++)
                                shapes.Add(new Shape(shapeIndex++, oneSidedShape, shapeSize));
                        }
                        result.Add((shapes, shapeSize));
                        break;
                }
            }

            return result;
        }
    }
}