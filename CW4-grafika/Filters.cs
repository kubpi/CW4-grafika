using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Windows;
using System.ComponentModel;

namespace CW4_grafika
{
    public class Filters : INotifyPropertyChanged
    {     
        public WriteableBitmap ApplySmoothingFilter(WriteableBitmap _originalImage)
        {
            if (_originalImage == null) return null;

            WriteableBitmap writableImage = _originalImage.Clone();



            int width = writableImage.PixelWidth;
            int height = writableImage.PixelHeight;
            int stride = width * ((writableImage.Format.BitsPerPixel + 7) / 8);

            byte[] pixels = new byte[height * stride];
            byte[] newPixels = new byte[height * stride];
            writableImage.CopyPixels(pixels, stride, 0);

            int filterSize = 3; // Rozmiar maski filtru 3x3
            int filterOffset = filterSize / 2;
            int calculatedStride = stride / 4;

            for (int y = filterOffset; y < height - filterOffset; y++)
            {
                for (int x = filterOffset; x < width - filterOffset; x++)
                {
                    int byteIndex = y * stride + x * 4;

                    float sumB = 0, sumG = 0, sumR = 0;
                    int count = 0;

                    // Pętla po masce filtru
                    for (int fy = -filterOffset; fy <= filterOffset; fy++)
                    {
                        for (int fx = -filterOffset; fx <= filterOffset; fx++)
                        {
                            int index = (y + fy) * stride + (x + fx) * 4;

                            sumB += pixels[index];
                            sumG += pixels[index + 1];
                            sumR += pixels[index + 2];
                            count++;
                        }
                    }

                    // Obliczanie średniej wartości piksela
                    newPixels[byteIndex] = ClampColorValue((int)(sumB / count));
                    newPixels[byteIndex + 1] = ClampColorValue((int)(sumG / count));
                    newPixels[byteIndex + 2] = ClampColorValue((int)(sumR / count));
                    newPixels[byteIndex + 3] = 255; // Alfa niezmieniona
                }
            }

            writableImage.WritePixels(new Int32Rect(0, 0, width, height), newPixels, stride, 0);        
            return writableImage;
           
        }
        public WriteableBitmap ApplyMedianFilter(WriteableBitmap _originalImage)
        {
            if (_originalImage == null) return null;

            WriteableBitmap writableImage = _originalImage.Clone();

            int width = writableImage.PixelWidth;
            int height = writableImage.PixelHeight;
            int stride = width * ((writableImage.Format.BitsPerPixel + 7) / 8);

            byte[] pixels = new byte[height * stride];
            byte[] newPixels = new byte[height * stride];
            writableImage.CopyPixels(pixels, stride, 0);

            int filterSize = 3; // Rozmiar maski filtru 3x3
            int filterOffset = filterSize / 2;
            List<byte> neighbourPixels = new List<byte>();

            for (int y = filterOffset; y < height - filterOffset; y++)
            {
                for (int x = filterOffset; x < width - filterOffset; x++)
                {
                    int byteIndex = y * stride + x * 4;

                    // Wyczyszczenie listy sąsiednich pikseli
                    neighbourPixels.Clear();

                    // Pętla po masce filtru
                    for (int fy = -filterOffset; fy <= filterOffset; fy++)
                    {
                        for (int fx = -filterOffset; fx <= filterOffset; fx++)
                        {
                            int index = (y + fy) * stride + (x + fx) * 4;

                            // Dodaj wartości RGB do listy
                            neighbourPixels.Add(pixels[index]);     // B
                            neighbourPixels.Add(pixels[index + 1]); // G
                            neighbourPixels.Add(pixels[index + 2]); // R
                        }
                    }

                    // Sortowanie listy
                    neighbourPixels.Sort();

                    // Wyznaczenie mediany dla każdego kanału
                    byte medianB = neighbourPixels[neighbourPixels.Count / 2];
                    byte medianG = neighbourPixels[neighbourPixels.Count / 2 + 1];
                    byte medianR = neighbourPixels[neighbourPixels.Count / 2 + 2];

                    newPixels[byteIndex] = medianB;
                    newPixels[byteIndex + 1] = medianG;
                    newPixels[byteIndex + 2] = medianR;
                    newPixels[byteIndex + 3] = 255; // Alfa niezmieniona
                }
            }

            writableImage.WritePixels(new Int32Rect(0, 0, width, height), newPixels, stride, 0);
            return writableImage;
        }
        public WriteableBitmap ApplySobelFilter(WriteableBitmap _originalImage)
        {
            if (_originalImage == null) return null;

            WriteableBitmap writableImage = _originalImage.Clone();

            int width = writableImage.PixelWidth;
            int height = writableImage.PixelHeight;
            int stride = width * ((writableImage.Format.BitsPerPixel + 7) / 8);

            byte[] pixels = new byte[height * stride];
            byte[] newPixels = new byte[height * stride];
            writableImage.CopyPixels(pixels, stride, 0);

            // Maski filtru Sobela
            int[] gx = new int[]
            {
        -1, 0, 1,
        -2, 0, 2,
        -1, 0, 1
            };

            int[] gy = new int[]
            {
        -1, -2, -1,
         0,  0,  0,
         1,  2,  1
            };

            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    float gradientX = 0;
                    float gradientY = 0;

                    // Oblicz gradient dla piksela (x, y)
                    for (int filterY = 0; filterY < 3; filterY++)
                    {
                        for (int filterX = 0; filterX < 3; filterX++)
                        {
                            int imageX = (x - 1) + filterX;
                            int imageY = (y - 1) + filterY;
                            byte gray = (byte)(0.299 * pixels[imageY * stride + imageX * 4 + 2] +
                                               0.587 * pixels[imageY * stride + imageX * 4 + 1] +
                                               0.114 * pixels[imageY * stride + imageX * 4]);

                            gradientX += gray * gx[filterY * 3 + filterX];
                            gradientY += gray * gy[filterY * 3 + filterX];
                        }
                    }

                    // Oblicz ostateczną wartość gradientu (magnitudę)
                    float gradientMagnitude = (float)Math.Sqrt((gradientX * gradientX) + (gradientY * gradientY));
                    byte gradientValue = (byte)ClampColorValue((int)gradientMagnitude);

                    // Ustaw piksel (x, y) na nową wartość gradientu
                    int byteIndex = y * stride + x * 4;
                    newPixels[byteIndex] = gradientValue; // B
                    newPixels[byteIndex + 1] = gradientValue; // G
                    newPixels[byteIndex + 2] = gradientValue; // R
                    newPixels[byteIndex + 3] = 255; // A
                }
            }

            writableImage.WritePixels(new Int32Rect(0, 0, width, height), newPixels, stride, 0);
            return writableImage;
        }
        public WriteableBitmap ApplySharpeningFilter(WriteableBitmap _originalImage)
        {
            if (_originalImage == null) return null;

            WriteableBitmap writableImage = _originalImage.Clone();

            int width = writableImage.PixelWidth;
            int height = writableImage.PixelHeight;
            int stride = width * ((writableImage.Format.BitsPerPixel + 7) / 8);

            byte[] pixels = new byte[height * stride];
            byte[] newPixels = new byte[height * stride];
            writableImage.CopyPixels(pixels, stride, 0);

            // Maska filtra wyostrzającego
            int[] mask = new int[]
            {
        -1, -1, -1,
        -1,  9, -1,
        -1, -1, -1
            };

            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    int pixelR = 0, pixelG = 0, pixelB = 0;

                    // Aplikacja maski na pikselach
                    for (int filterY = 0; filterY < 3; filterY++)
                    {
                        for (int filterX = 0; filterX < 3; filterX++)
                        {
                            int imageX = (x - 1) + filterX;
                            int imageY = (y - 1) + filterY;
                            int filterValue = mask[filterY * 3 + filterX];

                            pixelB += filterValue * pixels[imageY * stride + imageX * 4];
                            pixelG += filterValue * pixels[imageY * stride + imageX * 4 + 1];
                            pixelR += filterValue * pixels[imageY * stride + imageX * 4 + 2];
                        }
                    }

                    // Ustawienie piksela
                    int index = y * stride + x * 4;
                    newPixels[index] = (byte)ClampColorValue(pixelB);
                    newPixels[index + 1] = (byte)ClampColorValue(pixelG);
                    newPixels[index + 2] = (byte)ClampColorValue(pixelR);
                    newPixels[index + 3] = 255; // A
                }
            }

            writableImage.WritePixels(new Int32Rect(0, 0, width, height), newPixels, stride, 0);
            return writableImage;
        }
        public WriteableBitmap ApplyGaussianBlur(WriteableBitmap _originalImage)
        {
            if (_originalImage == null) return null;

            WriteableBitmap writableImage = _originalImage.Clone();

            int width = writableImage.PixelWidth;
            int height = writableImage.PixelHeight;
            int stride = width * ((writableImage.Format.BitsPerPixel + 7) / 8);

            byte[] pixels = new byte[height * stride];
            byte[] newPixels = new byte[height * stride];
            writableImage.CopyPixels(pixels, stride, 0);

            // Maska filtra gaussowskiego 3x3 z sigma = 1.0
            double[] mask = new double[]
            {
        1, 2, 1,
        2, 4, 2,
        1, 2, 1
            };
            double maskSum = 16; // Suma wartości w masce

            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    double pixelR = 0, pixelG = 0, pixelB = 0;

                    // Aplikacja maski na pikselach
                    for (int filterY = 0; filterY < 3; filterY++)
                    {
                        for (int filterX = 0; filterX < 3; filterX++)
                        {
                            int imageX = (x - 1) + filterX;
                            int imageY = (y - 1) + filterY;
                            double filterValue = mask[filterY * 3 + filterX];

                            pixelB += filterValue * pixels[imageY * stride + imageX * 4];
                            pixelG += filterValue * pixels[imageY * stride + imageX * 4 + 1];
                            pixelR += filterValue * pixels[imageY * stride + imageX * 4 + 2];
                        }
                    }

                    // Ustawienie piksela
                    int index = y * stride + x * 4;
                    newPixels[index] = (byte)ClampColorValue1(pixelB / maskSum);
                    newPixels[index + 1] = (byte)ClampColorValue1(pixelG / maskSum);
                    newPixels[index + 2] = (byte)ClampColorValue1(pixelR / maskSum);
                    newPixels[index + 3] = 255; // Alpha wartość niezmieniona
                }
            }

            writableImage.WritePixels(new Int32Rect(0, 0, width, height), newPixels, stride, 0);
            return writableImage;
        }
        private byte ClampColorValue1(double value)
        {
            return (byte)Math.Max(Math.Min(value, 255), 0);
        }
        public WriteableBitmap ApplyConvolutionFilter(WriteableBitmap _originalImage, double[,] mask)
        {
            if (_originalImage == null) return null;

            WriteableBitmap writableImage = _originalImage.Clone();

            int width = writableImage.PixelWidth;
            int height = writableImage.PixelHeight;
            int stride = width * ((writableImage.Format.BitsPerPixel + 7) / 8);

            byte[] pixels = new byte[height * stride];
            byte[] newPixels = new byte[height * stride];
            writableImage.CopyPixels(pixels, stride, 0);

            int maskSize = mask.GetLength(0);
            int margin = maskSize / 2;

            for (int y = margin; y < height - margin; y++)
            {
                for (int x = margin; x < width - margin; x++)
                {
                    double pixelR = 0, pixelG = 0, pixelB = 0;

                    for (int filterY = 0; filterY < maskSize; filterY++)
                    {
                        for (int filterX = 0; filterX < maskSize; filterX++)
                        {
                            int imageX = (x - margin) + filterX;
                            int imageY = (y - margin) + filterY;
                            double filterValue = mask[filterY, filterX];

                            pixelB += filterValue * pixels[imageY * stride + imageX * 4];
                            pixelG += filterValue * pixels[imageY * stride + imageX * 4 + 1];
                            pixelR += filterValue * pixels[imageY * stride + imageX * 4 + 2];
                        }
                    }

                    int index = y * stride + x * 4;
                    newPixels[index] = (byte)ClampColorValue1(pixelB);
                    newPixels[index + 1] = (byte)ClampColorValue1(pixelG);
                    newPixels[index + 2] = (byte)ClampColorValue1(pixelR);
                    newPixels[index + 3] = 255; // Alpha wartość niezmieniona
                }
            }

            writableImage.WritePixels(new Int32Rect(0, 0, width, height), newPixels, stride, 0);
            return writableImage;
        }
        public byte ClampColorValue(int value)
        {
            return (byte)Math.Min(Math.Max(value, 0), 255);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
