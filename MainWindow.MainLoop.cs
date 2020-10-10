using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using Tetris.Graphics;
using Tetris.Shapes;

namespace Tetris
{
    public partial class MainWindow
    {
        private readonly List<Texel> _grid;
        private int _shapeCount;
        private Stopwatch sw = Stopwatch.StartNew();

        private void UpdateScene(object sender, EventArgs e)
        {
            if (sw.ElapsedMilliseconds < 1000) return;
            sw.Restart();
            
            _shapeCount = ShapeCount.Value.GetValueOrDefault();
            var buffer = Display.CreateNewBuffer();
            var shapes = new Shape[_shapeCount];
            for (int i = 0; i < shapes.Length; i++)
            {
                shapes[i] = new Shape(i);
            }

            var texelLists = new List<Texel>[1 + shapes.Length];
            texelLists[^1] = _grid;
            Parallel.For(0, shapes.Length, i => { texelLists[i] = shapes[i].GetShapeTexels(); });
            Parallel.For(0, texelLists.Length, body: i => { Display.WriteToBitmap(texelLists[i], buffer); });
            Display.CommitDraw(buffer);
        }

        private void ShapeCountChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _shapeCount = (int) e.NewValue;
        }
    }
}