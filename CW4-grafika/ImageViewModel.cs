using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace CW4_grafika
{
    public class ImageViewModel : INotifyPropertyChanged
    {
       
        private ImageModel _imageModel;
        private WriteableBitmap _originalImage;
        private WriteableBitmap _currentImage;   
        private Filters _filters;
        private Histograms _histogram;
        private PointTransformations _pointTransformations;
        private bool _isImageLoaded;
        private string _selectedOperation;
        private string _operationMode;
        private float _colorR, _colorG, _colorB, _binarizationThreshold, _procentBlackThreshold;
        private float _brightnessLevel;
        private int _selectedFilterIndex;
        private int _selectedHistogramIndex;
        private bool _isBrightnessSelected;
        private bool _isGrayScaleSelected;
        private bool _isOperationSelected;
        private bool _isFiltersSelected;
        private bool _isHistogramSelected;
        public WriteableBitmap Image
        {
            get { return _imageModel.Image; }
            set
            {
                _imageModel.Image = value;
                OnPropertyChanged(nameof(Image));
            }
        }
        public bool IsImageLoaded
        {
            get => _isImageLoaded;
            set
            {
                if (_isImageLoaded != value)
                {
                    _isImageLoaded = value;
                    OnPropertyChanged(nameof(IsImageLoaded));
                    OnPropertyChanged(nameof(CanApplyOperations));
                    OnPropertyChanged(nameof(ImagePlaceholderText)); // dodaj to
                }
            }
        }
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
                    IsFiltersSelected = !value;
                }
            }
        }
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

        public bool IsHistogramSelected
        {
            get => _isHistogramSelected;
            set
            {
                if (_isHistogramSelected != value)
                {
                    _isHistogramSelected = value;
                    OnPropertyChanged(nameof(IsHistogramSelected));
                }
            }
        }
        public int SelectedFilterIndex
        {
            get => _selectedFilterIndex;
            set
            {
                if (_selectedFilterIndex != value)
                {
                    _selectedFilterIndex = value;
                    OnPropertyChanged(nameof(SelectedFilterIndex));
                    OnPropertyChanged(nameof(IsConvolutionFilterSelected));
                }
            }
        }

        public int SelectedHistogramIndex
        {
            get => _selectedHistogramIndex;
            set
            {
                if (_selectedHistogramIndex != value)
                {
                    _selectedHistogramIndex = value;
                    OnPropertyChanged(nameof(SelectedHistogramIndex));
                    OnPropertyChanged(nameof(IsCustomBinarizationSelected));
                    OnPropertyChanged(nameof(IsProcentBlackSelectionSelected));
                }
            }
        }
        public bool IsConvolutionFilterSelected => SelectedFilterIndex == 6;
        public bool IsCustomBinarizationSelected => SelectedHistogramIndex == 3;
        public bool IsProcentBlackSelectionSelected => SelectedHistogramIndex == 4;
        public string ImagePlaceholderText => IsImageLoaded ? string.Empty : "Wczytaj obraz, aby odblokować opcje";
        public bool CanApplyOperations => _isImageLoaded;
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
        
        public float BinarizationThreshold
        {
            get => _binarizationThreshold;
            set
            {
                if (_binarizationThreshold != value)
                {
                    _binarizationThreshold = value;
                    OnPropertyChanged(nameof(BinarizationThreshold));                  
                    if(_binarizationThreshold != 0)
                    {
                        ApplyBinarizeImage(); // Możesz zdecydować, czy chcesz to wykonać za każdym razem
                    }
                }
            }
        } 
        public float ProcentBlackThreshold
        {
            get => _procentBlackThreshold;
            set
            {
                if (_procentBlackThreshold != value)
                {
                    _procentBlackThreshold = value;
                    OnPropertyChanged(nameof(ProcentBlackThreshold));                  
                    if(_procentBlackThreshold != 0)
                    {
                        ApplyPercentBlackSelection(); // Możesz zdecydować, czy chcesz to wykonać za każdym razem
                    }
                }
            }
        }
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
        public ICommand LoadImageCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }            
        public ImageViewModel()
        {
            _imageModel = new ImageModel();
            _filters = new Filters();
            _histogram = new Histograms();
            _pointTransformations = new PointTransformations(); 
            LoadImageCommand = new RelayCommand(ExecuteLoadImage);
            SaveCommand = new RelayCommand(ExecuteSave, CanExecuteSave);
        }           
        private void ExecuteLoadImage(object parameter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                LoadImage(openFileDialog.FileName);
            }
        }
        public void LoadImage(string path)
        {
            var bitmapImage = new BitmapImage(new Uri(path));
            _originalImage = new WriteableBitmap(bitmapImage);
            Image = _originalImage;
            _currentImage = _originalImage.Clone();
            IsImageLoaded = true; // Ustawienie flagi po wczytaniu obrazu
        }
        private bool CanExecuteSave(object parameter)
        {
            return _currentImage != null; // Zapis możliwy, gdy obraz został wczytany
        }
        private void ExecuteSave(object parameter)
        {
            SaveCurrentStateAsOriginal();
        }   
        public void UpdateBrightness()
        {
            Image = _pointTransformations.UpdateBrightness(_originalImage, Image, _brightnessLevel);
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
            ImageOperation operation = _pointTransformations.DetermineOperation(OperationMode);
            float rValue = ColorR; // Załóżmy, że masz właściwości ColorR, ColorG, ColorB
            float gValue = ColorG;
            float bValue = ColorB;

            // Aplikuj operację na obrazie
            ApplyRgbOperation(operation, rValue, gValue, bValue);
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
        public void ApplyRgbOperation(ImageOperation operation, float rValue, float gValue, float bValue)
        {          
            Image = _pointTransformations.RgbOperation( _originalImage,  operation,  rValue,  gValue,  bValue);
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
        public void ConvertToGrayScale(ImageOperation grayScaleType)
        {
            Image = _pointTransformations.GrayScale(_originalImage, grayScaleType);
        }
        public void ApplySmoothingFilter()
        {
            Image = _filters.ApplySmoothingFilter(_originalImage);
        }
        public void ApplyMedianFilter()
        {
            Image = _filters.ApplyMedianFilter(_originalImage);
        }
        public void ApplySobelFilter()
        {
            Image = _filters.ApplySobelFilter(_originalImage);
        }
        public void ApplySharpeningFilter()
        {
            Image = _filters.ApplySharpeningFilter(_originalImage);
        }
        public void ApplyGaussianBlur()
        {
            Image = _filters.ApplyGaussianBlur(_originalImage);
        }    
        public void ApplyConvolutionFilter(double[,] mask)
        {
            Image = _filters.ApplyConvolutionFilter(_originalImage,mask);
        }

        public void ApplyStretchHistogram()
        {
            Image = _histogram.OtsuThresholding(_originalImage);
        }
        public void ApplyEqualizeHistogram()
        {
            Image = _histogram.EqualizeHistogram(_originalImage);
        }

        public void ApplyBinarizeImage()
        {
            // Jeśli nie ma obrazu, nie rób nic
            if (Image == null) return;

         
            float threshold = BinarizationThreshold;
            Image = _histogram.BinarizeImage(_originalImage, threshold);
        }

        public void ApplyPercentBlackSelection()
        {

            if (Image == null) return;
            double threshold = ProcentBlackThreshold;
            Image = _histogram.PercentBlackSelection(_originalImage, threshold);
        }
        public void ApplyMeanIterativeSelection()
        {
            Image = _histogram.MeanIterativeSelection(_originalImage);
        }

        public void ApplyEntropySelection()
        {
            Image = _histogram.EntropySelection(_originalImage);
        }

        public void ApplyOtsuThresholding()
        {
            Image = _histogram.OtsuThresholding(_originalImage);
        }

        public void ApplyNiblackThresholding()
        {
            //Image = _histogram.NiblackThresholding(_originalImage);
        }

        public void ApplyKapurThresholding()
        {
            Image = _histogram.KapurThresholding(_originalImage);
        } 
        public void ApplyLuWuThresholding()
        {
            Image = _histogram.LuWuThresholding(_originalImage);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}