using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Tetris.Services
{
    public static class ExtensionMethods
    {
        private static readonly Random Random = new Random();

        public static T GetRandomElement<T>(this IEnumerable<T> enumerable)
        {
            var list = enumerable.ToList();
            return list.ElementAt(Random.Next(list.Count));
        }

        public static Point Add(this Point point, Point b)
        {
            return new Point(point.X + b.X, point.Y + b.Y);
        }

        public static T[] Shuffle<T>(this T[] array)
        {
            for (int n = array.Length; n > 1;)
            {
                int k = Random.Next(n);
                --n;
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }

            return array;
        }
    }
}