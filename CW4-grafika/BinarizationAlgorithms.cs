﻿using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace CW4_grafika
{
    public class BinarizationAlgorithms
    {
        public WriteableBitmap BinarizeImage(WriteableBitmap image, float threshold)
        {
            if (image.Format == PixelFormats.Indexed8 || image.Format == PixelFormats.Indexed4 || image.Format == PixelFormats.Indexed1)
            {
                image = new WriteableBitmap(new FormatConvertedBitmap(image, PixelFormats.Bgr24, null, 0));
            }
            int width = image.PixelWidth;
            int height = image.PixelHeight;
            int bytesPerPixel = (image.Format.BitsPerPixel + 7) / 8;
            int stride = (width * bytesPerPixel + 3) & ~3;

            byte[] pixels = new byte[height * stride];
            image.CopyPixels(pixels, stride, 0);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * stride + x * bytesPerPixel;
                    byte grayScaleValue = (byte)(0.3 * pixels[index + 2] + 0.59 * pixels[index + 1] + 0.11 * pixels[index]);
                    byte binarizedValue = grayScaleValue >= threshold ? (byte)255 : (byte)0;

                    pixels[index] = binarizedValue;      
                    pixels[index + 1] = binarizedValue; 
                    pixels[index + 2] = binarizedValue;
                }
            }

            WriteableBitmap binarizedImage = new WriteableBitmap(width, height, image.DpiX, image.DpiY, image.Format, null);
            binarizedImage.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);

            return binarizedImage;
        }

        public WriteableBitmap PercentBlackSelection(WriteableBitmap image, double blackPercent)
        {
            try
            {
                if (image.Format == PixelFormats.Indexed8 || image.Format == PixelFormats.Indexed4 || image.Format == PixelFormats.Indexed1)
                {
                    image = new WriteableBitmap(new FormatConvertedBitmap(image, PixelFormats.Bgr24, null, 0));
                }
                int width = image.PixelWidth;
                int height = image.PixelHeight;
                int bytesPerPixel = (image.Format.BitsPerPixel + 7) / 8;
                int stride = (width * bytesPerPixel + 3) & ~3;

                byte[] pixels = new byte[height * stride];
                image.CopyPixels(pixels, stride, 0);

                List<byte> grayScaleValues = new List<byte>();
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int index = y * stride + x * bytesPerPixel;
                        byte grayScaleValue = (byte)(0.3 * pixels[index + 2] + 0.59 * pixels[index + 1] + 0.11 * pixels[index]);
                        grayScaleValues.Add(grayScaleValue);
                    }
                }

                grayScaleValues.Sort();

                int thresholdIndex = (int)(blackPercent * grayScaleValues.Count / 100.0);
                thresholdIndex = Math.Max(0, Math.Min(thresholdIndex, grayScaleValues.Count - 1));
                byte threshold = grayScaleValues[thresholdIndex];

                for (int i = 0; i < pixels.Length; i += bytesPerPixel)
                {
                    byte grayScaleValue = (byte)(0.3 * pixels[i + 2] + 0.59 * pixels[i + 1] + 0.11 * pixels[i]);
                    byte binarizedValue = grayScaleValue < threshold ? (byte)0 : (byte)255;

                    pixels[i] = binarizedValue;    
                    pixels[i + 1] = binarizedValue; 
                    pixels[i + 2] = binarizedValue; 
                }

                WriteableBitmap binarizedImage = new WriteableBitmap(width, height, image.DpiX, image.DpiY, image.Format, null);
                binarizedImage.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);
                return binarizedImage;
            }

            catch (ArgumentOutOfRangeException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public WriteableBitmap MeanIterativeSelection(WriteableBitmap image)
        {
            try
            {
                if (image.Format == PixelFormats.Indexed8 || image.Format == PixelFormats.Indexed4 || image.Format == PixelFormats.Indexed1)
                {
                    image = new WriteableBitmap(new FormatConvertedBitmap(image, PixelFormats.Bgr24, null, 0));
                }
                int width = image.PixelWidth;
                int height = image.PixelHeight;
                int bytesPerPixel = (image.Format.BitsPerPixel + 7) / 8;
                int stride = (width * bytesPerPixel + 3) & ~3;

                byte[] pixels = new byte[height * stride];
                image.CopyPixels(pixels, stride, 0);

                double threshold = 0;
                for (int i = 0; i < pixels.Length; i += bytesPerPixel)
                {
                    threshold += 0.3 * pixels[i + 2] + 0.59 * pixels[i + 1] + 0.11 * pixels[i];
                }
                threshold /= (width * height);

                bool thresholdChanged;
                do
                {
                    double meanAbove = 0, meanBelow = 0;
                    int countAbove = 0, countBelow = 0;

                    for (int i = 0; i < pixels.Length; i += bytesPerPixel)
                    {
                        double gray = 0.3 * pixels[i + 2] + 0.59 * pixels[i + 1] + 0.11 * pixels[i];

                        if (gray < threshold)
                        {
                            meanBelow += gray;
                            countBelow++;
                        }
                        else
                        {
                            meanAbove += gray;
                            countAbove++;
                        }
                    }

                    if (countBelow > 0) meanBelow /= countBelow;
                    if (countAbove > 0) meanAbove /= countAbove;

                    double newThreshold = (meanAbove + meanBelow) / 2;
                    thresholdChanged = newThreshold != threshold;
                    threshold = newThreshold;
                } while (thresholdChanged);

                for (int i = 0; i < pixels.Length; i += bytesPerPixel)
                {
                    byte grayScaleValue = (byte)(0.3 * pixels[i + 2] + 0.59 * pixels[i + 1] + 0.11 * pixels[i]);
                    byte binarizedValue = grayScaleValue < threshold ? (byte)0 : (byte)255;

                    pixels[i] = binarizedValue;      
                    pixels[i + 1] = binarizedValue;  
                    pixels[i + 2] = binarizedValue;  
                }

                WriteableBitmap binarizedImage = new WriteableBitmap(width, height, image.DpiX, image.DpiY, image.Format, null);
                binarizedImage.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);

                return binarizedImage;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public WriteableBitmap EntropySelection(WriteableBitmap image)
        {
            try
            {
                if (image.Format == PixelFormats.Indexed8 || image.Format == PixelFormats.Indexed4 || image.Format == PixelFormats.Indexed1)
                {
                    image = new WriteableBitmap(new FormatConvertedBitmap(image, PixelFormats.Bgr24, null, 0));
                }
                int width = image.PixelWidth;
                int height = image.PixelHeight;
                int bytesPerPixel = (image.Format.BitsPerPixel + 7) / 8;
                int stride = (width * bytesPerPixel + 3) & ~3;

                byte[] pixels = new byte[height * stride];
                image.CopyPixels(pixels, stride, 0);

                int[] histogram = new int[256];
                for (int i = 0; i < pixels.Length; i += bytesPerPixel)
                {
                    int grayScaleValue = (int)(0.3 * pixels[i + 2] + 0.59 * pixels[i + 1] + 0.11 * pixels[i]);
                    histogram[grayScaleValue]++;
                }

                double maxEntropy = -1;
                int optimalThreshold = 0;

                for (int t = 0; t < 256; t++)
                {
                    double entropyBack = 0, entropyObj = 0;
                    int sumBack = 0, sumObj = 0;

                    for (int i = 0; i <= t; i++)
                    {
                        sumBack += histogram[i];
                    }
                    for (int i = t + 1; i < 256; i++)
                    {
                        sumObj += histogram[i];
                    }

                    if (sumBack == 0 || sumObj == 0) continue;

                    for (int i = 0; i <= t; i++)
                    {
                        if (histogram[i] == 0) continue;
                        double p = (double)histogram[i] / sumBack;
                        entropyBack -= p * Math.Log(p, 2);
                    }
                    for (int i = t + 1; i < 256; i++)
                    {
                        if (histogram[i] == 0) continue;
                        double p = (double)histogram[i] / sumObj;
                        entropyObj -= p * Math.Log(p, 2);
                    }

                    double totalEntropy = entropyBack + entropyObj;
                    if (totalEntropy > maxEntropy)
                    {
                        maxEntropy = totalEntropy;
                        optimalThreshold = t;
                    }
                }

                for (int i = 0; i < pixels.Length; i += bytesPerPixel)
                {
                    byte grayScaleValue = (byte)(0.3 * pixels[i + 2] + 0.59 * pixels[i + 1] + 0.11 * pixels[i]);
                    byte binarizedValue = grayScaleValue <= optimalThreshold ? (byte)0 : (byte)255;

                    pixels[i] = binarizedValue;      
                    pixels[i + 1] = binarizedValue;  
                    pixels[i + 2] = binarizedValue;  
                }

                WriteableBitmap binarizedImage = new WriteableBitmap(width, height, image.DpiX, image.DpiY, image.Format, null);
                binarizedImage.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);

                return binarizedImage;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public WriteableBitmap OtsuThresholding(WriteableBitmap image)
        {
            try
            {
                if (image.Format == PixelFormats.Indexed8 || image.Format == PixelFormats.Indexed4 || image.Format == PixelFormats.Indexed1)
                {
                    image = new WriteableBitmap(new FormatConvertedBitmap(image, PixelFormats.Bgr24, null, 0));
                }
                int width = image.PixelWidth;
                int height = image.PixelHeight;
                int bytesPerPixel = (image.Format.BitsPerPixel + 7) / 8;
                int stride = (width * bytesPerPixel + 3) & ~3;

                byte[] pixels = new byte[height * stride];
                image.CopyPixels(pixels, stride, 0);

                int[] histogram = new int[256];
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int i = y * stride + x * bytesPerPixel;
                        if (i + 2 < pixels.Length)
                        {
                            int grayScaleValue = (int)(0.3 * pixels[i + 2] + 0.59 * pixels[i + 1] + 0.11 * pixels[i]);
                            histogram[grayScaleValue]++;
                        }
                    }
                }


                int total = width * height;
                float sum = 0;
                for (int i = 0; i < 256; i++) sum += i * histogram[i];

                float sumB = 0;
                int wB = 0;
                int wF = 0;

                float varMax = 0;
                int threshold = 0;

                for (int i = 0; i < 256; i++)
                {
                    wB += histogram[i];           
                    if (wB == 0) continue;

                    wF = total - wB;               
                    if (wF == 0) break;

                    sumB += (float)(i * histogram[i]);

                    float mB = sumB / wB;          
                    float mF = (sum - sumB) / wF;     

                    float varBetween = (float)wB * (float)wF * (mB - mF) * (mB - mF);

                    if (varBetween > varMax)
                    {
                        varMax = varBetween;
                        threshold = i;
                    }
                }

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int i = y * stride + x * bytesPerPixel;
                        if (i + 2 < pixels.Length)
                        {
                            byte grayScaleValue = (byte)(0.3 * pixels[i + 2] + 0.59 * pixels[i + 1] + 0.11 * pixels[i]);
                            byte binarizedValue = grayScaleValue <= threshold ? (byte)0 : (byte)255;

                            pixels[i] = binarizedValue;      
                            pixels[i + 1] = binarizedValue;  
                            pixels[i + 2] = binarizedValue;  
                        }
                    }
                }


                WriteableBitmap binarizedImage = new WriteableBitmap(width, height, image.DpiX, image.DpiY, image.Format, null);
                binarizedImage.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);

                return binarizedImage;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public WriteableBitmap NiblackThresholding(WriteableBitmap image, int windowSize, float k)
        {
            try
            {
                if (image.Format == PixelFormats.Indexed8 || image.Format == PixelFormats.Indexed4 || image.Format == PixelFormats.Indexed1)
                {
                    image = new WriteableBitmap(new FormatConvertedBitmap(image, PixelFormats.Bgr24, null, 0));
                }
                int width = image.PixelWidth;
                int height = image.PixelHeight;
                int bytesPerPixel = (image.Format.BitsPerPixel + 7) / 8;
                int stride = (width * bytesPerPixel + 3) & ~3;

                byte[] pixels = new byte[height * stride];
                image.CopyPixels(pixels, stride, 0);

                WriteableBitmap thresholdedImage = new WriteableBitmap(width, height, image.DpiX, image.DpiY, image.Format, null);
                byte[] thresholdedPixels = new byte[height * stride];

                int halfWindowSize = windowSize / 2;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        double sum = 0;
                        double sumSquares = 0;
                        int count = 0;

                        for (int wy = -halfWindowSize; wy <= halfWindowSize; wy++)
                        {
                            int py = y + wy;
                            if (py < 0 || py >= height) continue;

                            for (int wx = -halfWindowSize; wx <= halfWindowSize; wx++)
                            {
                                int px = x + wx;
                                if (px < 0 || px >= width) continue;

                                int pixelIndex = py * stride + px * bytesPerPixel;
                                byte grayScaleValue = (byte)(0.3 * pixels[pixelIndex + 2] + 0.59 * pixels[pixelIndex + 1] + 0.11 * pixels[pixelIndex]);
                                sum += grayScaleValue;
                                sumSquares += grayScaleValue * grayScaleValue;
                                count++;
                            }
                        }

                        double mean = sum / count;
                        double variance = (sumSquares / count) - (mean * mean);
                        double stdDev = Math.Sqrt(variance);
                        double threshold = mean + k * stdDev;

                        int pixelOffset = y * stride + x * bytesPerPixel;
                        byte pixelValue = (byte)(0.3 * pixels[pixelOffset + 2] + 0.59 * pixels[pixelOffset + 1] + 0.11 * pixels[pixelOffset]);
                        byte binarizedValue = pixelValue <= threshold ? (byte)0 : (byte)255;

                        thresholdedPixels[pixelOffset] = binarizedValue;
                        thresholdedPixels[pixelOffset + 1] = binarizedValue;
                        thresholdedPixels[pixelOffset + 2] = binarizedValue;
                    }
                }

                thresholdedImage.WritePixels(new Int32Rect(0, 0, width, height), thresholdedPixels, stride, 0);

                return thresholdedImage;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public WriteableBitmap KapurThresholding(WriteableBitmap image)
        {
            try
            {
                if (image.Format == PixelFormats.Indexed8 || image.Format == PixelFormats.Indexed4 || image.Format == PixelFormats.Indexed1)
                {
                    image = new WriteableBitmap(new FormatConvertedBitmap(image, PixelFormats.Bgr24, null, 0));
                }
                int width = image.PixelWidth;
                int height = image.PixelHeight;
                int bytesPerPixel = (image.Format.BitsPerPixel + 7) / 8;
                int stride = (width * bytesPerPixel + 3) & ~3; 

                byte[] pixels = new byte[height * stride];
                image.CopyPixels(pixels, stride, 0);

                int[] histogram = new int[256];
                for (int i = 0; i < pixels.Length; i += bytesPerPixel)
                {
                    int grayScaleValue = (int)(0.3 * pixels[i + 2] + 0.59 * pixels[i + 1] + 0.11 * pixels[i]);
                    histogram[grayScaleValue]++;
                }

                double[] probability = new double[256];
                int totalPixels = width * height;

                for (int i = 0; i < 256; i++)
                {
                    probability[i] = (double)histogram[i] / totalPixels;
                }

                double maxEntropy = -1;
                int optimalThreshold = 0;

                for (int t = 0; t < 256; t++)
                {
                    double entropyBack = 0, entropyFore = 0;
                    double sumBack = 0, sumFore = 0;

                    for (int i = 0; i <= t; i++)
                    {
                        sumBack += probability[i];
                    }
                    for (int i = t + 1; i < 256; i++)
                    {
                        sumFore += probability[i];
                    }

                    if (sumBack == 0 || sumFore == 0) continue;

                    for (int i = 0; i <= t; i++)
                    {
                        if (probability[i] == 0) continue;
                        entropyBack -= (probability[i] / sumBack) * Math.Log(probability[i] / sumBack, 2);
                    }
                    for (int i = t + 1; i < 256; i++)
                    {
                        if (probability[i] == 0) continue;
                        entropyFore -= (probability[i] / sumFore) * Math.Log(probability[i] / sumFore, 2);
                    }

                    double totalEntropy = entropyBack + entropyFore;
                    if (totalEntropy > maxEntropy)
                    {
                        maxEntropy = totalEntropy;
                        optimalThreshold = t;
                    }
                }

                for (int i = 0; i < pixels.Length; i += bytesPerPixel)
                {
                    byte grayScaleValue = (byte)(0.3 * pixels[i + 2] + 0.59 * pixels[i + 1] + 0.11 * pixels[i]);
                    byte binarizedValue = grayScaleValue <= optimalThreshold ? (byte)0 : (byte)255;

                    pixels[i] = binarizedValue;     
                    pixels[i + 1] = binarizedValue;  
                    pixels[i + 2] = binarizedValue;  
                }

                WriteableBitmap binarizedImage = new WriteableBitmap(width, height, image.DpiX, image.DpiY, image.Format, null);
                binarizedImage.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);

                return binarizedImage;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public WriteableBitmap LuWuThresholding(WriteableBitmap image)
        {
            try
            {
                if (image.Format == PixelFormats.Indexed8 || image.Format == PixelFormats.Indexed4 || image.Format == PixelFormats.Indexed1)
                {
                    image = new WriteableBitmap(new FormatConvertedBitmap(image, PixelFormats.Bgr24, null, 0));
                }
                int width = image.PixelWidth;
                int height = image.PixelHeight;
                int bytesPerPixel = (image.Format.BitsPerPixel + 7) / 8;
                int stride = (width * bytesPerPixel + 3) & ~3; 

                byte[] pixels = new byte[height * stride];
                image.CopyPixels(pixels, stride, 0);

                int[] histogram = new int[256];
                for (int i = 0; i < pixels.Length; i += bytesPerPixel)
                {
                    int grayScaleValue = (int)(0.3 * pixels[i + 2] + 0.59 * pixels[i + 1] + 0.11 * pixels[i]);
                    histogram[grayScaleValue]++;
                }

                double[] probability = new double[256];
                int totalPixels = width * height;

                for (int i = 0; i < 256; i++)
                {
                    probability[i] = (double)histogram[i] / totalPixels;
                }

                double maxEntropy = -1;
                int optimalThreshold = 0;

                for (int t = 0; t < 256; t++)
                {
                    double entropyBack = 0, entropyFore = 0;
                    double sumBack = 0, sumFore = 0;

                    for (int i = 0; i <= t; i++)
                    {
                        sumBack += probability[i];
                    }
                    for (int i = t + 1; i < 256; i++)
                    {
                        sumFore += probability[i];
                    }

                    if (sumBack == 0 || sumFore == 0) continue;

                    for (int i = 0; i <= t; i++)
                    {
                        if (probability[i] == 0) continue;
                        entropyBack += Math.Sqrt(probability[i] / sumBack);
                    }

                    for (int i = t + 1; i < 256; i++)
                    {
                        if (probability[i] == 0) continue;
                        entropyFore += Math.Sqrt(probability[i] / sumFore);
                    }

                    double totalEntropy = entropyBack * entropyFore;
                    if (totalEntropy > maxEntropy)
                    {
                        maxEntropy = totalEntropy;
                        optimalThreshold = t;
                    }

                }

                for (int i = 0; i < pixels.Length; i += bytesPerPixel)
                {
                    byte grayScaleValue = (byte)(0.3 * pixels[i + 2] + 0.59 * pixels[i + 1] + 0.11 * pixels[i]);
                    byte binarizedValue = grayScaleValue <= optimalThreshold ? (byte)0 : (byte)255;

                    pixels[i] = binarizedValue;     
                    pixels[i + 1] = binarizedValue;  
                    pixels[i + 2] = binarizedValue;  
                }

                WriteableBitmap binarizedImage = new WriteableBitmap(width, height, image.DpiX, image.DpiY, image.Format, null);
                binarizedImage.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);

                return binarizedImage;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }
                
    }
}
