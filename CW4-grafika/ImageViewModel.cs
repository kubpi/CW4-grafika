using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
        private MorphologicalFilters _morphologicalFilters;
        private BinarizationAlgorithms _binarizationAlgorithms;
        private PointTransformations _pointTransformations;
        private bool _isImageLoaded;
        private string _selectedOperation;
        private string _operationMode;
        private float _colorR, _colorG, _colorB, _binarizationThreshold, _procentBlackThreshold;
        private int _windowSize;
        private float _valueD;
        private float _brightnessLevel;
        private int _selectedFilterIndex;
        private int _selectedHistogramIndex;
        private int _selectedBinarizationAlgorithmsIndex;
        private int _selectedMorphologicalFiltersIndex;
        private bool _isBrightnessSelected;
        private bool _isGrayScaleSelected;
        private bool _isOperationSelected;
        private bool _isFiltersSelected;
        private bool _isHistogramSelected;
        private bool _isBinarizationAlgorithmsSelected;
        private bool _isMorphologicalFiltersSelected;
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
                    OnPropertyChanged(nameof(ImagePlaceholderText));
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
        public bool IsBinarizationAlgorithmsSelected
        {
            get => _isBinarizationAlgorithmsSelected;
            set
            {
                if (_isBinarizationAlgorithmsSelected != value)
                {
                    _isBinarizationAlgorithmsSelected = value;
                    OnPropertyChanged(nameof(IsBinarizationAlgorithmsSelected));
                }
            }
        }
        public bool IsMorphologicalFiltersSelected
        {
            get => _isMorphologicalFiltersSelected;
            set
            {
                if (_isMorphologicalFiltersSelected != value)
                {
                    _isMorphologicalFiltersSelected = value;
                    OnPropertyChanged(nameof(IsMorphologicalFiltersSelected));
                }
            }
        }
        public int SelectedBinarizationAlgorithmsIndex
        {
            get => _selectedBinarizationAlgorithmsIndex;
            set
            {
                if (_selectedBinarizationAlgorithmsIndex != value)
                {
                    _selectedBinarizationAlgorithmsIndex = value;
                    OnPropertyChanged(nameof(SelectedBinarizationAlgorithmsIndex));

                    OnPropertyChanged(nameof(IsCustomBinarizationSelected));
                    OnPropertyChanged(nameof(IsProcentBlackSelectionSelected));
                    OnPropertyChanged(nameof(IsNiblackSelected));
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
                }
            }
        }
        public int SelectedMorphologicalFiltersIndex
        {
            get => _selectedMorphologicalFiltersIndex;
            set
            {
                if (_selectedMorphologicalFiltersIndex != value)
                {
                    _selectedMorphologicalFiltersIndex = value;
                    OnPropertyChanged(nameof(SelectedMorphologicalFiltersIndex));
                }
            }
        }
        public bool IsConvolutionFilterSelected => SelectedFilterIndex == 6;
        public bool IsCustomBinarizationSelected => SelectedBinarizationAlgorithmsIndex == 1;
        public bool IsProcentBlackSelectionSelected => SelectedBinarizationAlgorithmsIndex == 2;
        public bool IsNiblackSelected => SelectedBinarizationAlgorithmsIndex == 6;
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
                        UpdateImage();
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
                        UpdateImage();
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
                    if (_binarizationThreshold != 0)
                    {
                        ApplyBinarizeImage();
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
                    if (_procentBlackThreshold != 0)
                    {
                        ApplyPercentBlackSelection();
                    }
                }
            }
        }
        public int WindowSize
        {
            get => _windowSize;
            set
            {
                if (_windowSize != value)
                {
                    _windowSize = value;
                    OnPropertyChanged(nameof(WindowSize));
                    if (_windowSize != 0)
                    {
                        ApplyNiblackThresholding();
                    }
                }
            }
        }
        public float ValueD
        {
            get => _valueD;
            set
            {
                if (_valueD != value)
                {
                    _valueD = value;
                    OnPropertyChanged(nameof(ValueD));
                    if (_valueD != 0)
                    {
                        ApplyNiblackThresholding(); // Możesz zdecydować, czy chcesz to wykonać za każdym razem
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
        public ICommand SaveImgCommand { get; private set; }
        public ImageViewModel()
        {
            _imageModel = new ImageModel();
            _filters = new Filters();
            _histogram = new Histograms();
            _binarizationAlgorithms = new BinarizationAlgorithms();
            _pointTransformations = new PointTransformations();
            _morphologicalFilters = new MorphologicalFilters();
            LoadImageCommand = new RelayCommand(ExecuteLoadImage);
            SaveCommand = new RelayCommand(ExecuteSave, CanExecuteSave);
            SaveImgCommand = new RelayCommand(ExecuteSaveImage);
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
            IsImageLoaded = true;
        }
        private bool CanExecuteSave(object parameter)
        {
            return _currentImage != null;
        }
        private void ExecuteSave(object parameter)
        {
            SaveCurrentStateAsOriginal();
        }
        private void ExecuteSaveImage(object parameter)
        {
            SaveImage();
        }
        public void SaveImage()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Pliki PNG (*.png)|*.png|Pliki JPEG (*.jpeg;*.jpg)|*.jpeg;*.jpg";
            if (saveFileDialog.ShowDialog() == true)
            {
                using (var fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                {
                    BitmapEncoder encoder = null;
                    switch (Path.GetExtension(saveFileDialog.FileName).ToLower())
                    {
                        case ".jpg":
                        case ".jpeg":
                            encoder = new JpegBitmapEncoder();
                            break;
                        case ".png":
                            encoder = new PngBitmapEncoder();
                            break;
                        default:
                            throw new InvalidOperationException("Nieobsługiwany format pliku");
                    }
                    encoder.Frames.Add(BitmapFrame.Create(_currentImage));
                    encoder.Save(fileStream);
                }
            }
        }
        public void UpdateBrightness()
        {
            _currentImage = _pointTransformations.UpdateBrightness(_originalImage, Image, _brightnessLevel);
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
            if (Image == null) return;
            ImageOperation operation = _pointTransformations.DetermineOperation(OperationMode);
            float rValue = ColorR;
            float gValue = ColorG;
            float bValue = ColorB;
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
            _currentImage = _pointTransformations.RgbOperation(_originalImage, operation, rValue, gValue, bValue);
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
                _originalImage = Image.Clone();
            }
        }
        public void ConvertToGrayScale(ImageOperation grayScaleType)
        {
            _currentImage = _pointTransformations.GrayScale(_originalImage, grayScaleType);
            Image = _currentImage;
        }
        public void ApplySmoothingFilter()
        {
            _currentImage = _filters.ApplySmoothingFilter(_originalImage);
            Image = _currentImage;
        }
        public void ApplyMedianFilter()
        {
            _currentImage = _filters.ApplyMedianFilter(_originalImage);
            Image = _currentImage;
        }
        public void ApplySobelFilter()
        {
            _currentImage = _filters.ApplySobelFilter(_originalImage);
            Image = _currentImage;
        }
        public void ApplySharpeningFilter()
        {
            _currentImage = _filters.ApplySharpeningFilter(_originalImage);
            Image = _currentImage;
        }
        public void ApplyGaussianBlur()
        {
            _currentImage = _filters.ApplyGaussianBlur(_originalImage);
            Image = _currentImage;
        }
        public void ApplyConvolutionFilter(double[,] mask)
        {
            _currentImage = _filters.ApplyConvolutionFilter(_originalImage, mask);
            Image = _currentImage;
        }
        public void ApplyStretchHistogram()
        {
            _currentImage = _histogram.StretchHistogram(_originalImage);
            Image = _currentImage;
        }
        public void ApplyEqualizeHistogram()
        {
            _currentImage = _histogram.EqualizeHistogram(_originalImage);
            Image = _currentImage;
        }
        public void ApplyBinarizeImage()
        {
            if (Image == null) return;
            float threshold = BinarizationThreshold;
            _currentImage = _binarizationAlgorithms.BinarizeImage(_originalImage, threshold);
            Image = _currentImage;
        }
        public void ApplyPercentBlackSelection()
        {
            if (Image == null) return;
            double threshold = ProcentBlackThreshold;
            _currentImage = _binarizationAlgorithms.PercentBlackSelection(_originalImage, threshold);
            Image = _currentImage;
        }
        public void ApplyMeanIterativeSelection()
        {
            _currentImage = _binarizationAlgorithms.MeanIterativeSelection(_originalImage);
            Image = _currentImage;
        }
        public void ApplyEntropySelection()
        {
            _currentImage = _binarizationAlgorithms.EntropySelection(_originalImage);
            Image = _currentImage;
        }
        public void ApplyOtsuThresholding()
        {
            _currentImage = _binarizationAlgorithms.OtsuThresholding(_originalImage);
            Image = _currentImage;
        }
        public void ApplyNiblackThresholding()
        {
            if (Image == null) return;
            float valueD = ValueD;
            int windowSize = WindowSize;
            _currentImage = _binarizationAlgorithms.NiblackThresholding(_originalImage, windowSize, valueD);
            Image = _currentImage;
        }
        public void ApplyKapurThresholding()
        {
            _currentImage = _binarizationAlgorithms.KapurThresholding(_originalImage);
            Image = _currentImage;
        }
        public void ApplyLuWuThresholding()
        {
            _currentImage = _binarizationAlgorithms.LuWuThresholding(_originalImage);
            Image = _currentImage;
        }
        public void ApplyDilation()
        {
            _currentImage = _morphologicalFilters.Dilation(_originalImage);
            Image = _currentImage;
        }
        public void ApplyErosion()
        {
            _currentImage = _morphologicalFilters.Erosion(_originalImage);
            Image = _currentImage;
        }
        public void ApplyOpening()
        {
            _currentImage = _morphologicalFilters.Opening(_originalImage);
            Image = _currentImage;
        }
        public void ApplyClosing()
        {
            _currentImage = _morphologicalFilters.Closing(_originalImage);
            Image = _currentImage;
        }
        public void ApplyHitOrMiss()
        {
            int[,] foregroundKernel = { { 0, 1, 0 },
                            { 1, 1, 1},
                            { 0, 1, 0 } };

            int[,] backgroundKernel = { { 1, 0, 1 },
                            { 0, 0, 0 },
                            { 1, 0, 1 } };
            _currentImage = _morphologicalFilters.HitOrMiss(_originalImage, foregroundKernel, backgroundKernel);
            Image = _currentImage;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}