using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media;
using System.Windows; // For MessageBox

namespace CW4_grafika
{
    public class Histograms: INotifyPropertyChanged
    {
        public WriteableBitmap StretchHistogram(WriteableBitmap image)
        {
            // Convert indexed image to a non-indexed format if necessary
            if (image.Format == PixelFormats.Indexed8 || image.Format == PixelFormats.Indexed4 || image.Format == PixelFormats.Indexed1)
            {
                // Create a non-indexed image (e.g., Bgr24)
                image = new WriteableBitmap(new FormatConvertedBitmap(image, PixelFormats.Bgr24, null, 0));
            }
            int width = image.PixelWidth;
            int height = image.PixelHeight;
            int stride = width * ((image.Format.BitsPerPixel + 7) / 8);

            byte[] pixels = new byte[height * stride];
            image.CopyPixels(pixels, stride, 0);

            byte min = 255, max = 0;
            for (int i = 0; i < pixels.Length; i += 4)
            {
                min = Math.Min(min, pixels[i]);
                max = Math.Max(max, pixels[i]);
            }

            for (int i = 0; i < pixels.Length; i += 4)
            {
                pixels[i] = (byte)((pixels[i] - min) * 255 / (max - min));
                pixels[i + 1] = (byte)((pixels[i + 1] - min) * 255 / (max - min));
                pixels[i + 2] = (byte)((pixels[i + 2] - min) * 255 / (max - min));
            }

            WriteableBitmap stretchedImage = new WriteableBitmap(width, height, image.DpiX, image.DpiY, image.Format, null);

            stretchedImage.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);
            return stretchedImage;
        }

        public WriteableBitmap EqualizeHistogram(WriteableBitmap image)
        {
            // Convert indexed image to a non-indexed format if necessary
            if (image.Format == PixelFormats.Indexed8 || image.Format == PixelFormats.Indexed4 || image.Format == PixelFormats.Indexed1)
            {
                // Create a non-indexed image (e.g., Bgr24)
                image = new WriteableBitmap(new FormatConvertedBitmap(image, PixelFormats.Bgr24, null, 0));
            }
            int width = image.PixelWidth;
            int height = image.PixelHeight;
            int stride = width * ((image.Format.BitsPerPixel + 7) / 8);

            byte[] pixels = new byte[height * stride];
            image.CopyPixels(pixels, stride, 0);

            int[] histogram = new int[256];
            for (int i = 0; i < pixels.Length; i += 4)
            {
                histogram[pixels[i]]++;
            }

            int total = width * height;
            float scale = 255.0f / total;
            int sum = 0;
            int[] lut = new int[256];
            for (int i = 0; i < histogram.Length; i++)
            {
                sum += histogram[i];
                lut[i] = (int)(sum * scale);
            }

            for (int i = 0; i < pixels.Length; i += 4)
            {
                pixels[i] = (byte)lut[pixels[i]];
                pixels[i + 1] = (byte)lut[pixels[i + 1]];
                pixels[i + 2] = (byte)lut[pixels[i + 2]];
            }

            WriteableBitmap equalizedImage = new WriteableBitmap(width, height, image.DpiX, image.DpiY, image.Format, null);
            equalizedImage.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);
            return equalizedImage;
        }

        


        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }
       

    }

}
