using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using Tetris.Algorithms;
using Tetris.Graphics;
using Tetris.Services;
using Tetris.Shapes;

namespace Tetris
{
    public partial class MainWindow
    {
        private (List<Shape> shapes, int shapeSize) _generatedShapes;
        private Stopwatch _sw;

        private readonly Dictionary<int, TetrisFitter> _tagToFitter = new Dictionary<int, TetrisFitter>
        {
            {0, new HeuristicTetrisFitter()},
            {1, new OptimalTetrisFitter()}
        };

        private CancellationTokenSource _tokenSource;

        public MainWindow()
        {
            InitializeComponent();
            ResolutionSurface = new PaintSurface(ResolutionImage);
            InputSurface = new PaintSurface(InputImage);
        }

        private PaintSurface ResolutionSurface { get; }
        private PaintSurface InputSurface { get; }

        public void Test()
        {
            var res = Enumerable.Range(1, 3).Select(n => (n,
                ShapeGenerator.GenerateOneSidedShapes(n, new CancellationTokenSource().Token)
                    .Select(shape => new Shape(0, shape, n))
                    .Select(s => s.GenerateAllCuts().Count).Max())).ToList();
        }

        private async void ExecuteAlgorithmClick(object sender, RoutedEventArgs e)
        {
            ShowOverlay();

            var tag = int.Parse((sender as Button)?.Tag.ToString()!);
            var tetrisFitter = _tagToFitter[tag];
            var shapes = _generatedShapes.shapes;
            var indexToColor = shapes.ToDictionary(s => s.Index, s => s.Color);
            indexToColor[TetrisFitter.EmptyField] = Color.White;

            try
            {
                var sw = Stopwatch.StartNew();
                var fitResult = await Task.Run(() => tetrisFitter.Fit(shapes.ToList(), _tokenSource.Token));
                DisplayMethods.DisplayBoard(fitResult.Item1, indexToColor, ResolutionSurface);
                Results.Content = $"Cięcia: {fitResult.Item2}, Czas: {sw.ElapsedMilliseconds}ms";
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

            var shapeCount = ShapeCount.Value.GetValueOrDefault();
            var shapeSize = ShapeSize.Value.GetValueOrDefault();

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

        private void CancelOperationClick(object sender, RoutedEventArgs e)
        {
            _tokenSource.Cancel();
        }

        private void LoadPentominoes(object sender, RoutedEventArgs e)
        {
            var dll = new FileInfo(Assembly.GetExecutingAssembly().Location);
            var filesToLoadDir = dll.Directory.Parent.Parent.Parent.FullName + "\\FilesToLoad";

            var ofd = new OpenFileDialog
            {
                InitialDirectory = filesToLoadDir,
                Title = "Load pentominoes",
                FilterIndex = 2,
                RestoreDirectory = false,
                CheckFileExists = true,
                CheckPathExists = true
            };

            var result = ofd.ShowDialog();

            if (result.HasValue && result.Value)
            {
                var file = File.ReadAllText(ofd.FileName);
                var shapeLists = Parser.Parse(file);
                LoadedSetups.Children.Clear();
                for (var i = 0; i < shapeLists.Count; i++)
                {
                    var shapes = shapeLists[i];
                    var button = new Button()
                    {
                        Content = i+1,
                        Width = 45
                    };
                    button.Click += (o, args) =>
                    {
                        _generatedShapes = shapes;
                        ResolutionSurface.Clear();
                        DisplayMethods.DisplayInputShapes(shapes.shapeSize, shapes.shapes, InputSurface);
                        EnableButtons();
                    };
                    LoadedSetups.Children.Add(button);
                }
            }
        }
    }
}