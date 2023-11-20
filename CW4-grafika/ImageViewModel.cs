using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace CW4_grafika
{
    public class ImageViewModel : INotifyPropertyChanged
    {
        private ImageModel _imageModel;
        public WriteableBitmap Image
        {
            get { return _imageModel.Image; }
            set
            {
                _imageModel.Image = value;
                OnPropertyChanged(nameof(Image));
            }
        }
        public ImageViewModel()
        {
            _imageModel = new ImageModel();
        }
        public enum ImageOperation
        {
            Add,
            Subtract,
            Multiply,
            Divide,
            GrayScaleAverage,
            GrayScaleRed,
            GrayScaleGreen,
            GrayScaleBlue,
            GrayScaleMax,
            GrayScaleMin
        }

        private string _selectedOperation;
        public string SelectedOperation
        {
            get => _selectedOperation;
            set
            {
                if (_selectedOperation != value)
                {
                    _selectedOperation = value;
                    OnPropertyChanged(nameof(SelectedOperation));
                    IsOperationSelected = value != "Brightness";
                }
            }
        }

        private string _operationMode;
        public string OperationMode
        {
            get => _operationMode;
            set
            {
                if (_operationMode != value)
                {
                    _operationMode = value;
                    OnPropertyChanged(nameof(OperationMode));
                }
            }
        }
        
        private bool _isBrightnessSelected;
        public bool IsBrightnessSelected
        {
            get => _isBrightnessSelected;
            set
            {
                if (_isBrightnessSelected != value)
                {
                    _isBrightnessSelected = value;
                    OnPropertyChanged(nameof(IsBrightnessSelected));
                    // Ustawienie przeciwnej wartości dla IsOperationSelected
                    IsOperationSelected = !value;
                    IsGrayScaleSelected = !value;
                }
            }
        }

        private bool _isGrayScaleSelected;
        public bool IsGrayScaleSelected
        {
            get => _isGrayScaleSelected;
            set
            {
                if (_isGrayScaleSelected != value)
                {
                    _isGrayScaleSelected = value;
                    OnPropertyChanged(nameof(IsGrayScaleSelected));
                }
            }
        }

        private bool _isOperationSelected;
        public bool IsOperationSelected
        {
            get => _isOperationSelected;
            set
            {
                if (_isOperationSelected != value)
                {
                    _isOperationSelected = value;
                    OnPropertyChanged(nameof(IsOperationSelected));
                }
            }
        }

        private bool _isFiltersSelected;
        public bool IsFiltersSelected
        {
            get => _isFiltersSelected;
            set
            {
                if (_isFiltersSelected != value)
                {
                    _isFiltersSelected = value;
                    OnPropertyChanged(nameof(IsFiltersSelected));
                }
            }
        }

        private float _colorR;
        public float ColorR
        {
            get => _colorR;
            set
            {
                if (_colorR != value)
                {
                    _colorR = value;
                    OnPropertyChanged(nameof(ColorR));
                    if (_colorR == 0 && _colorG == 0 && _colorB == 0)
                    {
                        ResetToOriginalImage();
                    }
                    else
                    {
                        UpdateImage(); // Możesz zdecydować, czy chcesz to wykonać za każdym razem
                    }
                }
            }
        }

        private float _colorG;
        public float ColorG
        {
            get => _colorG;
            set
            {
                if (_colorG != value)
                {
                    _colorG = value;
                    OnPropertyChanged(nameof(ColorG));
                    if (_colorR == 0 && _colorG == 0 && _colorB == 0)
                    {
                        ResetToOriginalImage();
                    }
                    else
                    {
                        UpdateImage(); // Możesz zdecydować, czy chcesz to wykonać za każdym razem
                    }
                }
            }
        }
        private float _colorB;
        public float ColorB
        {
            get => _colorB;
            set
            {
                if (_colorB != value)
                {
                    _colorB = value;
                    OnPropertyChanged(nameof(ColorB));
                    if (_colorR == 0 && _colorG == 0 && _colorB == 0)
                    {
                        ResetToOriginalImage();
                    }
                    else
                    {
                        UpdateImage(); // Możesz zdecydować, czy chcesz to wykonać za każdym razem
                    }
                }
            }
        }

        private float _brightnessLevel;
        public float BrightnessLevel
        {
            get => _brightnessLevel;
            set
            {
                if (_brightnessLevel != value)
                {
                    _brightnessLevel = value;
                    OnPropertyChanged(nameof(BrightnessLevel));

                    if (_brightnessLevel == 0)
                    {
                        ResetBrightness();
                    }
                    else
                    {
                        UpdateBrightness();
                    }
                }
            }
        }

        private WriteableBitmap _originalImage;
        private WriteableBitmap _currentImage;

        public void UpdateBrightness()
        {
            if (Image == null) return;
            // Tworzenie kopii oryginalnego obrazu do modyfikacji
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
                    for (int color = 0; color < 3; color++) // Przejdź przez R, G i B
                    {
                        // Wczytaj obecny kolor
                        int colorValue = pixels[index + color];
                        // Dodaj poziom jasności, nie przekraczając zakresu 0-255
                        colorValue = ClampColorValue(colorValue + (int)_brightnessLevel);
                        pixels[index + color] = (byte)colorValue;
                    }
                    // Alpha channel (index + 3) pozostaje bez zmian
                }
            }

            writableImage.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);
            // Zapisz zmodyfikowany obraz jako aktualny
            _currentImage = writableImage;

            // Uaktualnij obraz w modelu
            Image = _currentImage;
        }
        public void ResetBrightness()
        {
            if (_originalImage != null)
            {
                _currentImage = _originalImage.Clone();
                Image = _currentImage;
            }
        }

        public void UpdateImage()
        {
            // Jeśli nie ma obrazu, nie rób nic
            if (Image == null) return;

            // Pobierz aktualne operacje i wartości RGB
            ImageOperation operation = DetermineOperation(OperationMode);
            float rValue = ColorR; // Załóżmy, że masz właściwości ColorR, ColorG, ColorB
            float gValue = ColorG;
            float bValue = ColorB;

            // Aplikuj operację na obrazie
            ApplyRgbOperation(operation, rValue, gValue, bValue);
        }

        // Ta metoda musi zostać zaimplementowana w zależności od tego, jak chcesz mapować stringi na enumy
        private ImageOperation DetermineOperation(string operationMode)
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



        public void UpdateOperationMode(string mode)
        {
            if (mode == "Brightness")
            {
                IsBrightnessSelected = true;
                IsOperationSelected = false;
                OperationMode = null;
            }
            else
            {
                IsOperationSelected = true;
                IsBrightnessSelected = false;
                OperationMode = mode;
            }
        }
        public void LoadImage(string path)
        {
            var bitmapImage = new BitmapImage(new Uri(path));
            _originalImage = new WriteableBitmap(bitmapImage);
            Image = _originalImage;
            _currentImage = _originalImage.Clone();
        }


        public void ApplyRgbOperation(ImageOperation operation, float rValue, float gValue, float bValue)
        {
            if (_originalImage == null) return;

            WriteableBitmap writableImage = _originalImage.Clone();


            int width = writableImage.PixelWidth;
            int height = writableImage.PixelHeight;
            int stride = width * ((writableImage.Format.BitsPerPixel + 7) / 8); // bytes per row

            byte[] pixels = new byte[height * stride];
            writableImage.CopyPixels(pixels, stride, 0);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * stride + x * 4;
                    pixels[index + 0] = ApplyOperation(pixels[index + 0], bValue, operation); // Blue
                    pixels[index + 1] = ApplyOperation(pixels[index + 1], gValue, operation); // Green
                    pixels[index + 2] = ApplyOperation(pixels[index + 2], rValue, operation); // Red
                                                                                              // Alpha channel (index + 3) is not changed
                }
            }

            writableImage.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);
            _currentImage = writableImage;
            Image = _currentImage;
        }
        public void ResetToOriginalImage()
        {
            if (_originalImage != null)
            {
                _currentImage = _originalImage.Clone();
                Image = _currentImage;
            }
        }
        public void SaveCurrentStateAsOriginal()
        {
            if (_currentImage != null)
            {
                _originalImage = _currentImage.Clone();
            }
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
                    // Sprawdzenie, czy wartość operationValue nie jest równa zero
                    if (operationValue != 0)
                    {
                        result /= operationValue;
                    }
                    else
                    {
                        // Możesz tutaj zdecydować co zrobić, jeśli operationValue == 0
                        // Na przykład, możesz zwrócić oryginalną wartość pixelValue
                        // lub ustawić result na maksymalną wartość, jeśli dzielisz przez zero
                        result = 255; // To może być traktowane jako "nieskończoność" w kontekście kolorów
                    }
                    break;
            }
            return ClampColorValue((int)result);
        }

        private byte ClampColorValue(int value)
        {
            return (byte)Math.Min(Math.Max(value, 0), 255);
        }

        public void ConvertToGrayScale(ImageOperation grayScaleType)
        {
            if (Image == null) return;

            WriteableBitmap writableImage = new WriteableBitmap(_imageModel.Image);
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
            _currentImage = writableImage;
            Image = _currentImage;
        }







        public void ApplySmoothingFilter()
        {
            if (_originalImage == null) return;

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
            _currentImage = writableImage;
            Image = _currentImage;
        }
        public void ApplyMedianFilter()
        {
            if (_originalImage == null) return;

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
            _currentImage = writableImage;
            Image = _currentImage;
        }
        public void ApplySobelFilter()
        {
            if (_originalImage == null) return;

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
            _currentImage = writableImage;
            Image = _currentImage;
        }
        public void ApplySharpeningFilter()
        {
            if (_originalImage == null) return;

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
            _currentImage = writableImage;
            Image = _currentImage;
        }
        public void ApplyGaussianBlur()
        {
            if (_originalImage == null) return;

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
            _currentImage = writableImage;
            Image = _currentImage;
        }

        private byte ClampColorValue1(double value)
        {
            return (byte)Math.Max(Math.Min(value, 255), 0);
        }

        public void ApplyFilter(int filterIndex)
        {
            switch (filterIndex)
            {
                case 0:
                    ResetToOriginalImage();
                    break;
                case 1:
                    ApplySmoothingFilter();
                    break;
                case 2:
                    ApplyMedianFilter();
                    break;
                case 3:
                    ApplySobelFilter();
                    break;
                case 4:
                    ApplySharpeningFilter();
                    break;
                case 5:
                    ApplyGaussianBlur();
                    break;
                case 6:
                    // Tutaj możesz dodać logikę dla splotu maski dowolnego rozmiaru
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Nieznany filtr");
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}