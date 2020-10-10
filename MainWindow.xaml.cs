﻿using System;
 using System.Diagnostics;
using System.IO;
 using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
 using Tetris.Graphics;

 namespace Tetris
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            try
            {
                InitializeComponent();

                int columns = 12;
                int rows = 12;
                _grid = DisplayObjects.CreateGrid(columns, rows);
                UpdatePaintSurfaceSize();
                Display.SetupBitmap(PaintSurface);
                CompositionTarget.Rendering += UpdateScene;
                UpdateWindowSize();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void UpdatePaintSurfaceSize()
        {
            PaintSurface.Width = DisplayObjects.PaintSurfaceWidth;
            PaintSurface.Height = DisplayObjects.PaintSurfaceHeight;
        }

        private void UpdateWindowSize()
        {
             int margins = 48;
             int windowWidth = 300 + margins + DisplayObjects.PaintSurfaceWidth;
             int windowHeight = margins + 21 + DisplayObjects.PaintSurfaceHeight;
             Width = windowWidth;
             Height = windowHeight;
        }
    }
}
