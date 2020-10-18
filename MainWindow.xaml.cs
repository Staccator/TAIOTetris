using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
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

        private PaintSurface ResolutionSurface { get; }
        private PaintSurface InputSurface { get; }

        private Dictionary<int, TetrisFitter> _tagToFitter = new Dictionary<int, TetrisFitter>()
        {
            {0, new BasicTetrisFitter()},
            {1, new HeuristicTetrisFitter()},
            {2, new TestTetrisFitter()},
            {3, new BasicTetrisFitter()},
        };

        private async void ExecuteAlgorithmClick(object sender, RoutedEventArgs e)
        {
            ShowOverlay();
            Stopwatch sw = Stopwatch.StartNew();

            int tag = int.Parse((sender as Button)?.Tag.ToString()!);
            var tetrisFitter = _tagToFitter[tag];
            var shapes = _generatedShapes.shapes;
            var indexToColor = shapes.ToDictionary(s => s.Index, s => s.Color);
            indexToColor[TetrisFitter.EmptyField] = Color.White;

            var fitResult = await Task.Run(() => tetrisFitter.Fit(shapes.ToList()));
            DisplayMethods.DisplayBoard(fitResult, indexToColor, ResolutionSurface);

            Console.WriteLine($"Time executing: {sw.ElapsedMilliseconds}ms.");
            HideOverlay();
        }

        private async void GenerateShapesClick(object sender, RoutedEventArgs e)
        {
            ShowOverlay();

            int shapeCount = ShapeCount.Value.GetValueOrDefault();
            int shapeSize = ShapeSize.Value.GetValueOrDefault();

            var shapes = await Task.Run(() => ShapeGenerator.GenerateShapes(shapeCount, shapeSize));
            _generatedShapes = (shapes, shapeSize);
            ResolutionSurface.Clear();
            DisplayMethods.DisplayInputShapes(shapeSize, shapes, InputSurface);

            // shapes[0].GenerateCuts();

            EnableButtons();
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
        }

        private void ShowOverlay()
        {
            Overlay.Visibility = Visibility.Visible;
            MainDisplay.IsEnabled = false;
        }

        private (List<Shape> shapes, int shapeSize) _generatedShapes;
    }
}