using System;
using System.Collections.Generic;
using System.Drawing;

namespace Tetris.Shapes
{
    public class ShapeOnMatrix
    {
        private const int MaxTypes = 7;
        private readonly int _type;
        private bool[,] _matrix = new bool[4, 4];

        public ShapeOnMatrix(int type)
        {
            _type = type;
            if (type >= MaxTypes)
            {
                throw new Exception($"type not valid, choose a value between 0 and {MaxTypes - 1}");
            }

            SetupMatrix();
        }

        private void SetupMatrix()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    _matrix[i, j] = false;
                }
            }

            switch (_type)
            {
                case 0:
                    _matrix[0, 0] = _matrix[1, 0] = _matrix[2, 0] = _matrix[3, 0] = true;
                    break;
                case 1:
                    _matrix[1, 0] = _matrix[2, 0] = _matrix[1, 1] = _matrix[2, 1] = true;
                    break;
                case 2:
                    _matrix[2, 0] = _matrix[1, 1] = _matrix[2, 1] = _matrix[1, 2] = true;
                    break;
                case 3:
                    _matrix[1, 0] = _matrix[1, 1] = _matrix[2, 1] = _matrix[2, 2] = true;
                    break;
                case 4:
                    _matrix[1, 0] = _matrix[1, 1] = _matrix[1, 2] = _matrix[2, 1] = true;
                    break;
                case 5:
                    _matrix[1, 1] = _matrix[2, 1] = _matrix[2, 2] = _matrix[2, 3] = true;
                    break;
                case 6:
                    _matrix[1, 1] = _matrix[1, 2] = _matrix[1, 3] = _matrix[2, 1] = true;
                    break;
            }
        }

        public void RotateRight()
        {
            bool[,] rotatedMatrix = new bool[4, 4];

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    rotatedMatrix[3 - j, i] = _matrix[i, j];
                }
            }

            _matrix = rotatedMatrix;
        }

        public void RotateLeft()
        {
            bool[,] rotatedMatrix = new bool[4, 4];

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    rotatedMatrix[j, 3 - i] = _matrix[i, j];
                }
            }

            _matrix = rotatedMatrix;
        }

        public List<bool> GetListForPreview()
        {
            List<bool> result = new List<bool>();
            for (int j = 0; j < 4; j++)
            {
                for (int i = 1; i < 3; i++)
                {
                    result.Add(_matrix[i, j]);
                }
            }

            return result;
        }

        public List<Point> GetMatrixPositions()
        {
            var result = new List<Point>();
            
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (_matrix[i,j])
                        result.Add(new Point(i, j));
                }
            }

            return result;
        }
    }
}