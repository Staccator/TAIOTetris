using System.Linq;
using System.Windows;
 using Tetris.Algorithms;
 using Tetris.Graphics;
using Tetris.Shapes;

namespace Tetris
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            UpdatePaintSurfaceSize();
            Display.SetupBitmap(PaintSurface);
            UpdateWindowSize();
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
        private void OptimalAlgorithmClick(object sender, RoutedEventArgs e)
        {
            int shapeCount = ShapeCount.Value.GetValueOrDefault();
            var tetrisFitter = new BasicTetrisFitter();
            var shapes = Enumerable.Range(0, shapeCount).Select(i => new Shape(i)).ToArray();
            DisplayMethods.ExecuteAlgorithm(tetrisFitter, shapes);
        }
    }
}
