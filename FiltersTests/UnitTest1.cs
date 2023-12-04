using CW4_grafika;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AForge.Imaging.Filters;
using System.Drawing;
namespace FiltersTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void CompareAlgorithms_ShouldProduceSameResult()
        {
            // Arrange
            var testImage = CreateTestImage();
            var filters = new Filters();

            // Convert WriteableBitmap to Bitmap for AForge
            var testImageBitmap = ConvertWriteableBitmapToBitmap(testImage);

            // Act
            var resultImageYourAlgorithm = filters.ApplySobelFilter(testImage);
            var resultImageLibraryAlgorithmBitmap = ApplyAForgeSmoothingFilter(testImageBitmap);

            // Convert the result from Bitmap back to WriteableBitmap
            var resultImageLibraryAlgorithm = ConvertBitmapToWriteableBitmap(resultImageLibraryAlgorithmBitmap);

            // Assert
            /*Assert.IsTrue(ImagesAreEqual(resultImageYourAlgorithm, resultImageLibraryAlgorithm));*/

            // ... [Omitted code for brevity]

            // Assert - Instead of just asserting, save the results if they are not equal
            if (!ImagesAreEqual(resultImageYourAlgorithm, resultImageLibraryAlgorithm))
            {
                SaveImageToFile(resultImageYourAlgorithm, "YourAlgorithmResult.png");
                SaveImageToFile(resultImageLibraryAlgorithm, "LibraryAlgorithmResult.png");
                Assert.Fail("The images are not equal.");
            }
            else
            {
                Assert.IsTrue(true, "The images are equal.");
            }

            // ... [Omitted code for brevity]

        }


        private void SaveImageToFile(WriteableBitmap image, string fileName)
        {
            // Define the path where the image will be saved
            string folderPath = @"C:\Users\kubpi\Desktop\CW4-grafika\CW4-grafika";
            string fullPath = Path.Combine(folderPath, fileName);

            // Ensure the directory exists
            Directory.CreateDirectory(folderPath);

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(stream);
            }
        }


        public Bitmap ApplyAForgeSmoothingFilter(Bitmap originalImage)
        {
            var gaussianBlurFilter = new GaussianBlur(2.0, 11);
            return gaussianBlurFilter.Apply(originalImage);
        }


        /* public WriteableBitmap ApplySmoothingFilter(WriteableBitmap originalImage)
         {
             if (originalImage == null) return null;

             int width = originalImage.PixelWidth;
             int height = originalImage.PixelHeight;
             int stride = width * ((originalImage.Format.BitsPerPixel + 7) / 8);

             byte[] pixels = new byte[height * stride];
             byte[] newPixels = new byte[height * stride];
             originalImage.CopyPixels(pixels, stride, 0);

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

                     // Pêtla po masce filtru
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

                     // Obliczanie œredniej wartoœci piksela
                     newPixels[byteIndex] = ClampColorValue((int)(sumB / count));
                     newPixels[byteIndex + 1] = ClampColorValue((int)(sumG / count));
                     newPixels[byteIndex + 2] = ClampColorValue((int)(sumR / count));
                     newPixels[byteIndex + 3] = 255; // Alfa niezmieniona
                 }
             }

             WriteableBitmap resultImage = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
             resultImage.WritePixels(new Int32Rect(0, 0, width, height), newPixels, stride, 0);
             return resultImage;
         }*/

        /*private byte ClampColorValue(int value)
        {
            return (byte)Math.Min(255, Math.Max(0, value));
        }*/
        private WriteableBitmap CreateTestImage()
        {
            int width = 10;
            int height = 10;
            WriteableBitmap bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool isBlack = (x + y) % 2 == 0;
                    SetPixel(bitmap, x, y, isBlack ? Colors.Black : Colors.White);
                }
            }

            return bitmap;
        }

        private void SetPixel(WriteableBitmap bitmap, int x, int y, System.Windows.Media.Color color)
        {
            // Calculate the index of the pixel
            int column = x;
            int row = y;
            int pixelWidth = bitmap.PixelWidth;
            int bitsPerPixel = bitmap.Format.BitsPerPixel;
            int bytesPerPixel = (bitsPerPixel + 7) / 8;
            int stride = pixelWidth * bytesPerPixel;
            int index = row * stride + column * bytesPerPixel;

            // Create an array to hold the pixel's color data
            byte[] pixelData = new byte[bytesPerPixel];
            pixelData[0] = color.B;
            pixelData[1] = color.G;
            pixelData[2] = color.R;
            pixelData[3] = color.A;

            // Use WriteableBitmap's WritePixels method to set the pixel
            Int32Rect rect = new Int32Rect(x, y, 1, 1);
            bitmap.WritePixels(rect, pixelData, stride, 0);
        }


        /*private WriteableBitmap CreateExpectedResultForSmoothing()
        {
            int width = 10;
            int height = 10;
            WriteableBitmap expectedBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Replace this with the actual expected result of the smoothing algorithm
                    SetPixel(expectedBitmap, x, y, Colors.Gray);
                }
            }

            return expectedBitmap;
        }*/

        private bool ImagesAreEqual(WriteableBitmap image1, WriteableBitmap image2)
        {
            if (image1.PixelWidth != image2.PixelWidth || image1.PixelHeight != image2.PixelHeight)
            {
                return false;
            }

            for (int y = 0; y < image1.PixelHeight; y++)
            {
                for (int x = 0; x < image1.PixelWidth; x++)
                {
                    if (GetPixel(image1, x, y) != GetPixel(image2, x, y))
                    {
                        return false;
                    }
                }
            }

            return true;
        }




        private System.Drawing.Color GetPixel(WriteableBitmap bitmap, int x, int y)
        {
            int stride = bitmap.PixelWidth * ((bitmap.Format.BitsPerPixel + 7) / 8);
            byte[] pixelByteArray = new byte[stride * bitmap.PixelHeight];
            bitmap.CopyPixels(pixelByteArray, stride, 0);

            int index = y * stride + x * 4;
            return System.Drawing.Color.FromArgb(pixelByteArray[index + 3], pixelByteArray[index + 2], pixelByteArray[index + 1], pixelByteArray[index]);
        }

        public static Bitmap ConvertWriteableBitmapToBitmap(WriteableBitmap writeableBitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(writeableBitmap));
                encoder.Save(stream);
                using (var result = new Bitmap(stream))
                {
                    return new Bitmap(result);
                }
            }
        }
        public static WriteableBitmap ConvertBitmapToWriteableBitmap(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                stream.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = stream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return new WriteableBitmap(bitmapImage);
            }
        }

    }
}
