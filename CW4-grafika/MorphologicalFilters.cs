using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;

namespace CW4_grafika
{
    public class MorphologicalFilters
    {
        public WriteableBitmap Dilation(WriteableBitmap image)
        {
            int width = image.PixelWidth;
            int height = image.PixelHeight;
            int bytesPerPixel = (image.Format.BitsPerPixel + 7) / 8;
            // Ensure stride is a multiple of 4
            int stride = width * bytesPerPixel;
            stride = ((stride + 3) / 4) * 4;  // The stride is rounded up to the nearest multiple of 4

            byte[] pixels = new byte[height * stride];
            byte[] outputPixels = new byte[height * stride];

            image.CopyPixels(pixels, stride, 0);

            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    int index = y * stride + x;
                    if (pixels[index] == 255) // If the current pixel is white
                    {
                        // Set the current pixel and all its 8 neighbours to white
                        for (int ty = -1; ty <= 1; ty++)
                        {
                            for (int tx = -1; tx <= 1; tx++)
                            {
                                outputPixels[(y + ty) * stride + (x + tx)] = 255;
                            }
                        }
                    }
                }
            }

            WriteableBitmap outputImage = new WriteableBitmap(width, height, image.DpiX, image.DpiY, image.Format, null);
            outputImage.WritePixels(new Int32Rect(0, 0, width, height), outputPixels, stride, 0);
            return outputImage;
        }

        public WriteableBitmap Erosion(WriteableBitmap image)
        {
            int width = image.PixelWidth;
            int height = image.PixelHeight;
            int bytesPerPixel = (image.Format.BitsPerPixel + 7) / 8;
            // Ensure stride is a multiple of 4
            int stride = width * bytesPerPixel;
            stride = ((stride + 3) / 4) * 4;  // The stride is rounded up to the nearest multiple of 4

            byte[] pixels = new byte[height * stride];
            byte[] outputPixels = new byte[height * stride];

            image.CopyPixels(pixels, stride, 0);
            Array.Copy(pixels, outputPixels, pixels.Length);

            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    int index = y * stride + x;
                    bool erodePixel = false;
                    if (pixels[index] == 255) // If the current pixel is white
                    {
                        // Check all its 8 neighbours to see if they are all white
                        for (int ty = -1; ty <= 1; ty++)
                        {
                            for (int tx = -1; tx <= 1; tx++)
                            {
                                if (pixels[(y + ty) * stride + (x + tx)] == 0)
                                {
                                    erodePixel = true;
                                    break;
                                }
                            }
                            if (erodePixel) break;
                        }
                        if (erodePixel)
                        {
                            outputPixels[index] = 0; // Erode pixel
                        }
                    }
                }
            }

            WriteableBitmap outputImage = new WriteableBitmap(width, height, image.DpiX, image.DpiY, image.Format, null);
            outputImage.WritePixels(new Int32Rect(0, 0, width, height), outputPixels, stride, 0);
            return outputImage;
        }

        public WriteableBitmap Opening(WriteableBitmap image)
        {
            return Dilation(Erosion(image));
        }

        public WriteableBitmap Closing(WriteableBitmap image)
        {
            return Erosion(Dilation(image));
        }
        public WriteableBitmap HitOrMiss(WriteableBitmap image, int[,] foregroundKernel, int[,] backgroundKernel)
        {
            int width = image.PixelWidth;
            int height = image.PixelHeight;
            int bytesPerPixel = (image.Format.BitsPerPixel + 7) / 8;
            int stride = (width * bytesPerPixel + 3) & ~3;
            byte[] pixels = new byte[height * stride];
            byte[] outputPixels = new byte[height * stride];

            image.CopyPixels(pixels, stride, 0);

            int kernelWidth = foregroundKernel.GetLength(1);
            int kernelHeight = foregroundKernel.GetLength(0);
            int kernelOriginX = kernelWidth / 2;
            int kernelOriginY = kernelHeight / 2;

            // Assume the image is already binarized
            for (int y = kernelOriginY; y < height - kernelOriginY; y++)
            {
                for (int x = kernelOriginX; x < width - kernelOriginX; x++)
                {
                    bool matchForeground = true;
                    bool matchBackground = true;

                    for (int ky = -kernelOriginY; ky <= kernelOriginY; ky++)
                    {
                        for (int kx = -kernelOriginX; kx <= kernelOriginX; kx++)
                        {
                            int pixelValue = pixels[(y + ky) * stride + (x + kx) * bytesPerPixel];
                            if (foregroundKernel[ky + kernelOriginY, kx + kernelOriginX] == 1 && pixelValue != 255 ||
                                backgroundKernel[ky + kernelOriginY, kx + kernelOriginX] == -1 && pixelValue != 0)
                            {
                                matchForeground = false;
                                matchBackground = false;
                                break;
                            }
                        }

                        if (!matchForeground || !matchBackground)
                            break;
                    }

                    if (matchForeground && matchBackground)
                    {
                        outputPixels[y * stride + x * bytesPerPixel] = 255;
                    }
                }
            }

            WriteableBitmap outputImage = new WriteableBitmap(width, height, image.DpiX, image.DpiY, image.Format, null);
            outputImage.WritePixels(new Int32Rect(0, 0, width, height), outputPixels, stride, 0);
            return outputImage;
        }
        
        



    }
}
