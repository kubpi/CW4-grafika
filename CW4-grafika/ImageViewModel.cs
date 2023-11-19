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
            var writeableBitmap = new WriteableBitmap(bitmapImage);
            Image = writeableBitmap;
        }

        public void ApplyRgbOperation(ImageOperation operation, float rValue, float gValue, float bValue)
        {
            WriteableBitmap writableImage = new WriteableBitmap(_imageModel.Image);

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
            Image = writableImage;
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
                    result = operationValue != 0 ? result / operationValue : 0;
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
