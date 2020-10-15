using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Tetris.Algorithms;
 using Tetris.Graphics;
using Tetris.Services;

namespace Tetris
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            ResolutionSurface = new PaintSurface(ResolutionImage);
            ImageSurface = new PaintSurface(InputImage);
        }

        private PaintSurface ResolutionSurface { get; }
        private PaintSurface ImageSurface { get; }

        private Dictionary<string, TetrisFitter> _tagToFitter = new Dictionary<string, TetrisFitter>()
        {
            {"Basic", new BasicTetrisFitter()},
            {"Heuristic", new HeuristicTetrisFitter()},
            {"Optimal", new BasicTetrisFitter()},
        };
        private void ExecuteAlgorithmClick(object sender, RoutedEventArgs e)
        {
            string tag = (sender as Button)?.Tag.ToString();
            var tetrisFitter = _tagToFitter[tag!];

            int shapeCount = ShapeCount.Value.GetValueOrDefault();
            int shapeSize = ShapeSize.Value.GetValueOrDefault();
            var shapes = ShapeGenerator.GenerateShapes(shapeCount, shapeSize);
            DisplayMethods.ExecuteAlgorithm(tetrisFitter, shapes, ResolutionSurface, shapeSize);
        }
    }
}
