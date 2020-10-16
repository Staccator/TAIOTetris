using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Tetris.Graphics
{
    public class PaintSurface
    {
        private readonly Image _image;
        private int _width;
        private int _height;
        private Int32Rect _sourceRect;
        private WriteableBitmap _wb;

        public PaintSurface(Image image)
        {
            _image = image;
            _width = (int) image.Width;
        }

        public int Width => _width;

        public void SetupBitmap(int width, int height)
        {
            _width = width;
            _height = height;

            _wb = new WriteableBitmap(Width, _height, 96, 96, PixelFormats.Bgra32, null);
            _sourceRect = new Int32Rect(0, 0, Width, _height);

            _image.Dispatcher.Invoke(() =>
            {
                _image.Source = _wb;
                _image.Width = Width;
                _image.Height = _height;
            });
        }

        public byte[] CreateNewBuffer()
        {
            var result = new byte[Width * _height * (_wb.Format.BitsPerPixel / 8)];
            ClearBuffer(result);
            return result;
        }

        private void ClearBuffer(byte[] buffer)
        {
            Parallel.For(0, Width * _height * 4, (i) => { buffer[i] = 255; });
        }

        public void WriteToBitmap(List<Texel> texels, byte[] buffer)
        {
            for (var i = 0; i < texels.Count; i++)
            {
                var texel = texels[i];
                var x = texel.Position.X;
                var y = texel.Position.Y;
                if (x < 0 || x >= Width || y < 0 || y >= _height)
                    continue;

                var color = texel.Color;

                var pixelOffset = (x + Width * y) * 4;
                buffer[pixelOffset] = color.B;
                buffer[pixelOffset + 1] = color.G;
                buffer[pixelOffset + 2] = color.R;
                buffer[pixelOffset + 3] = color.A;
            }
        }

        public void CommitDraw(byte[] buffer)
        {
            var stride = _wb.PixelWidth * (_wb.Format.BitsPerPixel / 8);
            _wb.WritePixels(_sourceRect, buffer, stride, 0);
        }

        public void Clear()
        {
            SetupBitmap(1, 1);
            var buffer = CreateNewBuffer();
            CommitDraw(buffer);
        }
    }
}