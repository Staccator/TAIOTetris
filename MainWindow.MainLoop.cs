using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using DesktopApp.Graphics;
using DesktopApp.Models;
using Tetris.Graphics;

namespace Tetris
{
    public partial class MainWindow
    {
        private static readonly Random Random = new Random();
        private readonly List<Texel> _grid;
        private int _shapeCount;
        private Stopwatch sw = Stopwatch.StartNew();

        private void UpdateScene(object sender, EventArgs e)
        {
            if (sw.ElapsedMilliseconds < 200) return;
            sw.Restart();
            
            _shapeCount = ShapeCount.Value.GetValueOrDefault();
            var buffer = Display.CreateNewBuffer();
            var displayObjects = new Texel[_shapeCount];
            for (int i = 0; i < displayObjects.Length; i++)
            {
                displayObjects[i] =
                    new Texel(i, i, Color.FromArgb(Random.Next(255), Random.Next(255), Random.Next(255)));
            }

            var texelLists = new List<Texel>[1 + displayObjects.Length];
            texelLists[^1] = _grid;
            Parallel.For(0, displayObjects.Length, i => { texelLists[i] = Visuals.CreateField(displayObjects[i]); });
            Parallel.For(0, texelLists.Length, body: i => { Display.WriteToBitmap(texelLists[i], buffer); });
            Display.CommitDraw(buffer);
        }

        private void ShapeCountChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _shapeCount = (int) e.NewValue;
        }
    }
}