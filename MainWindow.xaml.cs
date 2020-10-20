﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Tetris.Algorithms;
using Tetris.Graphics;
using Tetris.Services;
using Tetris.Shapes;

namespace Tetris
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            ResolutionSurface = new PaintSurface(ResolutionImage);
            InputSurface = new PaintSurface(InputImage);
        }

        public void Test()
        {
            var res = Enumerable.Range(1, 3).Select(n => (n,
                ShapeGenerator.GenerateOneSidedShapes(n, new CancellationTokenSource().Token)
                    .Select(shape => new Shape(0, shape, n))
                    .Select(s => s.GenerateAllCuts().Count).Max())).ToList();
        }

        private PaintSurface ResolutionSurface { get; }
        private PaintSurface InputSurface { get; }

        private Dictionary<int, TetrisFitter> _tagToFitter = new Dictionary<int, TetrisFitter>()
        {
            {0, new HeuristicTetrisFitter()},
            {1, new OptimalTetrisFitter()},
            {2, new BasicTetrisFitter()},
        };

        private async void ExecuteAlgorithmClick(object sender, RoutedEventArgs e)
        {
            ShowOverlay();

            int tag = int.Parse((sender as Button)?.Tag.ToString()!);
            var tetrisFitter = _tagToFitter[tag];
            var shapes = _generatedShapes.shapes;
            var indexToColor = shapes.ToDictionary(s => s.Index, s => s.Color);
            indexToColor[TetrisFitter.EmptyField] = Color.White;

            try
            {
                var fitResult = await Task.Run(() => tetrisFitter.Fit(shapes.ToList(), _tokenSource.Token));
                DisplayMethods.DisplayBoard(fitResult, indexToColor, ResolutionSurface);
            }
            catch (OperationCanceledException oce)
            {
                Console.WriteLine($"{nameof(OperationCanceledException)} thrown with message: {oce.Message}");
            }
            finally
            {
                _tokenSource.Dispose();
            }

            HideOverlay();
        }

        private async void GenerateShapesClick(object sender, RoutedEventArgs e)
        {
            ShowOverlay();

            int shapeCount = ShapeCount.Value.GetValueOrDefault();
            int shapeSize = ShapeSize.Value.GetValueOrDefault();

            try
            {
                var shapes = await Task.Run(() =>
                    ShapeGenerator.GenerateShapes(shapeCount, shapeSize, _tokenSource.Token));
                _generatedShapes = (shapes, shapeSize);
                ResolutionSurface.Clear();
                DisplayMethods.DisplayInputShapes(shapeSize, shapes, InputSurface);
                EnableButtons();
            }
            catch (OperationCanceledException oce)
            {
                Console.WriteLine($"{nameof(OperationCanceledException)} thrown with message: {oce.Message}");
            }
            finally
            {
                _tokenSource.Dispose();
            }

            HideOverlay();
        }

        private void EnableButtons()
        {
            Button1.IsEnabled = true;
            Button2.IsEnabled = true;
            Button3.IsEnabled = true;
        }

        private void HideOverlay()
        {
            Overlay.Visibility = Visibility.Hidden;
            MainDisplay.IsEnabled = true;
            Console.WriteLine($"Time executing: {_sw.ElapsedMilliseconds}ms.");
        }

        private void ShowOverlay()
        {
            _sw = Stopwatch.StartNew();
            _tokenSource = new CancellationTokenSource();
            Overlay.Visibility = Visibility.Visible;
            MainDisplay.IsEnabled = false;
        }

        private (List<Shape> shapes, int shapeSize) _generatedShapes;
        private CancellationTokenSource _tokenSource;
        private Stopwatch _sw;

        private void CancelOperationClick(object sender, RoutedEventArgs e)
        {
            _tokenSource.Cancel();
        }
    }
}