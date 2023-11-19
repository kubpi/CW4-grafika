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
            Divide
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

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
