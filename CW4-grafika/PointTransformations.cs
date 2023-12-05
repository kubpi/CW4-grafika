using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;
using static CW4_grafika.ImageViewModel;
using System.Windows.Media;

namespace CW4_grafika
{
    public class PointTransformations : Filters
    {
        public ImageOperation DetermineOperation(string operationMode)
        {
            return operationMode switch
            {
                "Add" => ImageOperation.Add,
                "Subtract" => ImageOperation.Subtract,
                "Multiply" => ImageOperation.Multiply,
                "Divide" => ImageOperation.Divide,
                _ => throw new ArgumentException("Nieznany tryb operacji", nameof(operationMode)),
            };
        }
        public WriteableBitmap RgbOperation(WriteableBitmap _originalImage, ImageOperation operation, float rValue, float gValue, float bValue)
        {


            WriteableBitmap writableImage = _originalImage.Clone();


            int width = writableImage.PixelWidth;
            int height = writableImage.PixelHeight;
            int stride = width * ((writableImage.Format.BitsPerPixel + 7) / 8);

            byte[] pixels = new byte[height * stride];
            writableImage.CopyPixels(pixels, stride, 0);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * stride + x * 4;
                    pixels[index + 0] = ApplyOperation(pixels[index + 0], bValue, operation);
                    pixels[index + 1] = ApplyOperation(pixels[index + 1], gValue, operation);
                    pixels[index + 2] = ApplyOperation(pixels[index + 2], rValue, operation);

                }
            }

            writableImage.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);
            return writableImage;
        }
        private byte ApplyOperation(byte pixelValue, float operationValue, ImageOperation operation)
        {
            float result = pixelValue;
            switch (operation)
            {
                case ImageOperation.Add:
                    result += operationValue;
                    break;
                case ImageOperation.Subtract:
                    result -= operationValue;
                    break;
                case ImageOperation.Multiply:
                    result *= operationValue;
                    break;
                case ImageOperation.Divide:

                    if (operationValue != 0)
                    {
                        result /= operationValue;
                    }
                    else
                    {
                        result = 255;
                    }
                    break;
            }
            return ClampColorValue((int)result);
        }
        public WriteableBitmap UpdateBrightness(WriteableBitmap _originalImage, WriteableBitmap Image, float _brightnessLevel)
        {

            if (Image == null) return null;
            WriteableBitmap writableImage = _originalImage.Clone();


            int width = writableImage.PixelWidth;
            int height = writableImage.PixelHeight;
            int stride = width * ((writableImage.Format.BitsPerPixel + 7) / 8);

            byte[] pixels = new byte[height * stride];
            writableImage.CopyPixels(pixels, stride, 0);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * stride + x * 4;
                    for (int color = 0; color < 3; color++)
                    {

                        int colorValue = pixels[index + color];
                        colorValue = ClampColorValue(colorValue + (int)_brightnessLevel);
                        pixels[index + color] = (byte)colorValue;
                    }
                }
            }

            writableImage.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);
            return writableImage;
        }
        public WriteableBitmap GrayScale(WriteableBitmap _originalImage, ImageOperation grayScaleType)
        {
            if (_originalImage == null) return null;

            WriteableBitmap writableImage = _originalImage.Clone();
            int width = writableImage.PixelWidth;
            int height = writableImage.PixelHeight;
            int stride = width * ((writableImage.Format.BitsPerPixel + 7) / 8);
            byte[] pixels = new byte[height * stride];
            writableImage.CopyPixels(pixels, stride, 0);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * stride + x * 4;
                    byte red = pixels[index + 2];
                    byte green = pixels[index + 1];
                    byte blue = pixels[index + 0];

                    byte gray = grayScaleType switch
                    {
                        ImageOperation.GrayScaleAverage => (byte)((red + green + blue) / 3),
                        ImageOperation.GrayScaleRed => red,
                        ImageOperation.GrayScaleGreen => green,
                        ImageOperation.GrayScaleBlue => blue,
                        ImageOperation.GrayScaleMax => Math.Max(red, Math.Max(green, blue)),
                        ImageOperation.GrayScaleMin => Math.Min(red, Math.Min(green, blue)),
                        _ => throw new ArgumentException("Nieznany typ skali szarości", nameof(grayScaleType)),
                    };

                    pixels[index + 0] = gray;
                    pixels[index + 1] = gray;
                    pixels[index + 2] = gray;
                }
            }

            writableImage.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);
            return writableImage;
        }
    }
}
