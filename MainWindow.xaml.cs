using System.Linq;
using System.Windows;
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
            ImageSurface = new PaintSurface(InputImage);
            // var result = ShapeGenerator.GenerateOneSidedShapes(4);
        }

        public PaintSurface ResolutionSurface { get; }
        public PaintSurface ImageSurface { get; }

        private void UpdatePaintSurfaceSize()
        {
            // ResolutionSurface.Width = DisplayObjects.PaintSurfaceWidth;
            // ResolutionSurface.Height = DisplayObjects.PaintSurfaceHeight;
        }

        private void UpdateWindowSize()
        {
             int margins = 48;
             // int windowWidth = 300 + margins + DisplayObjects.PaintSurfaceWidth;
             // int windowHeight = margins + 21 + DisplayObjects.PaintSurfaceHeight;
             // Width = windowWidth;
             // Height = windowHeight;
        }
        
        private void OptimalAlgorithmClick(object sender, RoutedEventArgs e)
        {
            int shapeCount = ShapeCount.Value.GetValueOrDefault();
            int shapeSize = ShapeSize.Value.GetValueOrDefault();
            var tetrisFitter = new BasicTetrisFitter();
            var shapes = ShapeGenerator.GenerateShapes(shapeCount, shapeSize);
            DisplayMethods.ExecuteAlgorithm(tetrisFitter, shapes, ResolutionSurface, shapeSize);
        }
    }
}
