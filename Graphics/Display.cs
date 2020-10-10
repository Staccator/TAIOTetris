using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Tetris.Graphics
{
    public static class Display
    {
        public static int Width;
        public static int Height;
        private static Int32Rect _sourceRect;
        private static WriteableBitmap _wb;
        
        public static void SetupBitmap(Image paintSurface)
        {
            Width = (int) paintSurface.Width;
            Height = (int) paintSurface.Height;
            
            _wb = new WriteableBitmap( Width, Height, 96, 96, PixelFormats.Bgra32, null);
            _sourceRect = new Int32Rect(0, 0, Width, Height);
            
            paintSurface.Source = _wb;
        }
        
        public static byte[] CreateNewBuffer()
        {
            var result =  new byte[Width * Height * (_wb.Format.BitsPerPixel / 8)];
            ClearBuffer(result);
            return result;
        }

        private static void ClearBuffer(byte[] buffer)
        {
            Parallel.For(0, Width * Height * 4, (i) => { buffer[i] = 255; });
        }

        public static void WriteToBitmap(List<Texel> texels, byte[] buffer)
        {
            for (var i = 0; i < texels.Count; i++)
            {
                var texel = texels[i];
                var x = texel.Position.X;
                var y = texel.Position.Y;
                if (x < 0 || x >= Width || y < 0 || y >= Height)
                    continue;

                var color = texel.Color;

                var pixelOffset = (x + Width * y) * 4;
                buffer[pixelOffset] = color.B;
                buffer[pixelOffset + 1] = color.G;
                buffer[pixelOffset + 2] = color.R;
                buffer[pixelOffset + 3] = color.A;
            }
        }
        
        public static void CommitDraw(byte[] buffer)
        {
            var stride = _wb.PixelWidth * (_wb.Format.BitsPerPixel / 8);
            _wb.WritePixels(_sourceRect, buffer, stride, 0);
        }
    }
}