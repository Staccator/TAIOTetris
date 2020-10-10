﻿using System;
 using System.Diagnostics;
using System.IO;
 using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
 using DesktopApp.Graphics;
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

                int columns = 6;
                int rows = 8;
                _grid = Visuals.CreateGrid(columns, rows);
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
            PaintSurface.Width = Visuals.PaintSurfaceWidth;
            PaintSurface.Height = Visuals.PaintSurfaceHeight;
        }

        private void UpdateWindowSize()
        {
             int margins = 48;
             int windowWidth = 300 + margins + Visuals.PaintSurfaceWidth;
             int windowHeight = margins + 21 + Visuals.PaintSurfaceHeight;
             Width = windowWidth;
             Height = windowHeight;
        }
    }
}
