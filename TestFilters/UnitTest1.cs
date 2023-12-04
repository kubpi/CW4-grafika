using CW4_grafika; // Jeœli taka jest nazwa przestrzeni nazw dla projektu CW4-grafika
namespace TestFilters
{
    [TestClass]
    public class UnitTest1 : PageTest
    {
        [TestMethod]
        public void TestApplySmoothingFilter()
        {
            // Przygotowanie obrazu testowego
            WriteableBitmap testImage = CreateTestImage(10, 10);

            // Tworzenie oczekiwanego wyniku
            WriteableBitmap expectedImage = CreateExpectedImageForSmoothing();

            // Stosowanie filtru wyg³adzaj¹cego
            Filters filters = new Filters();
            WriteableBitmap resultImage = filters.ApplySmoothingFilter(testImage);

            // Porównanie wyniku z oczekiwanym obrazem
            Assert.AreEqual(expectedImage, resultImage, "Wynik filtru wyg³adzaj¹cego nie jest zgodny z oczekiwaniami.");
        }


        // Metody pomocnicze, np. do tworzenia testowego obrazu

        private WriteableBitmap CreateExpectedImageForSmoothing()
        {
            int width = 10, height = 10;
            WriteableBitmap expectedImage = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);

            byte[] pixels = new byte[width * height * 4]; // 4 bajty na piksel: BGRA

            // Przygotowanie obrazu testowego
            WriteableBitmap testImage = CreateTestImage(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = (y * width + x) * 4;
                    byte expectedPixelValue = CalculateExpectedPixelValue(x, y, testImage);
                    pixels[index] = expectedPixelValue;    // B
                    pixels[index + 1] = expectedPixelValue;// G
                    pixels[index + 2] = expectedPixelValue;// R
                    pixels[index + 3] = 255;               // A
                }
            }

            expectedImage.WritePixels(new Int32Rect(0, 0, width, height), pixels, width * 4, 0);

            return expectedImage;
        }



        private WriteableBitmap CreateTestImage(int width, int height)
        {
            WriteableBitmap testImage = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);

            byte[] pixels = new byte[width * height * 4]; // 4 bajty na piksel: BGRA

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = (y * width + x) * 4;
                    byte colorValue = (byte)((x + y) % 2 * 255); // Wzór szachownicy
                    pixels[index] = colorValue;        // B
                    pixels[index + 1] = colorValue;    // G
                    pixels[index + 2] = colorValue;    // R
                    pixels[index + 3] = 255;           // A
                }
            }

            testImage.WritePixels(new Int32Rect(0, 0, width, height), pixels, width * 4, 0);

            return testImage;
        }


        private byte CalculateExpectedPixelValue(int x, int y, WriteableBitmap inputImage)
        {
            int sum = 0;
            int count = 0;
            int width = inputImage.PixelWidth;
            int height = inputImage.PixelHeight;
            byte[] pixels = new byte[width * height * 4];
            inputImage.CopyPixels(pixels, width * 4, 0);

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int pixelX = x + i;
                    int pixelY = y + j;

                    if (pixelX >= 0 && pixelX < width && pixelY >= 0 && pixelY < height)
                    {
                        int index = (pixelY * width + pixelX) * 4;
                        sum += pixels[index]; // Zak³adaj¹c, ¿e pracujemy z wartoœciami szaroœci
                        count++;
                    }
                }
            }

            return (byte)(sum / count);
        }
    }
}
