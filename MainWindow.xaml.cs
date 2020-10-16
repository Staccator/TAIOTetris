using System.Collections.Generic;
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
            {2, new BasicTetrisFitter()},
            {3, new BasicTetrisFitter()},
        };
        private void ExecuteAlgorithmClick(object sender, RoutedEventArgs e)
        {
            int tag = int.Parse((sender as Button)?.Tag.ToString()!);
            var tetrisFitter = _tagToFitter[tag];

            DisplayMethods.ExecuteAlgorithm(tetrisFitter, _generatedShapes.shapes, ResolutionSurface, _generatedShapes.shapeSize);
        }

        private void GenerateShapesClick(object sender, RoutedEventArgs e)
        {
            int shapeCount = ShapeCount.Value.GetValueOrDefault();
            int shapeSize = ShapeSize.Value.GetValueOrDefault();
            var shapes = ShapeGenerator.GenerateShapes(shapeCount, shapeSize);
            _generatedShapes = (shapes, shapeSize);
            DisplayMethods.DisplayInputShapes(shapeSize, shapes, InputSurface);
        }

        private (List<Shape> shapes, int shapeSize) _generatedShapes;
    }
}
