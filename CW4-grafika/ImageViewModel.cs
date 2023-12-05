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
            Image = _pointTransformations.RgbOperation(_originalImage, operation, rValue, gValue, bValue);
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
            Image = _filters.ApplyConvolutionFilter(_originalImage, mask);
        }
        public void ApplyStretchHistogram()
        {
            Image = _histogram.StretchHistogram(_originalImage);
        }
        public void ApplyEqualizeHistogram()
        {
            Image = _histogram.EqualizeHistogram(_originalImage);
        }
        public void ApplyBinarizeImage()
        {
            if (Image == null) return;
            float threshold = BinarizationThreshold;
            Image = _binarizationAlgorithms.BinarizeImage(_originalImage, threshold);
        }
        public void ApplyPercentBlackSelection()
        {
            if (Image == null) return;
            double threshold = ProcentBlackThreshold;
            Image = _binarizationAlgorithms.PercentBlackSelection(_originalImage, threshold);
        }
        public void ApplyMeanIterativeSelection()
        {
            Image = _binarizationAlgorithms.MeanIterativeSelection(_originalImage);
        }
        public void ApplyEntropySelection()
        {
            Image = _binarizationAlgorithms.EntropySelection(_originalImage);
        }
        public void ApplyOtsuThresholding()
        {
            Image = _binarizationAlgorithms.OtsuThresholding(_originalImage);
        }
        public void ApplyNiblackThresholding()
        {
            if (Image == null) return;
            float valueD = ValueD;
            int windowSize = WindowSize;
            Image = _binarizationAlgorithms.NiblackThresholding(_originalImage, windowSize, valueD);
        }
        public void ApplyKapurThresholding()
        {
            Image = _binarizationAlgorithms.KapurThresholding(_originalImage);
        }
        public void ApplyLuWuThresholding()
        {
            Image = _binarizationAlgorithms.LuWuThresholding(_originalImage);
        }
        public void ApplyDilation()
        {
            Image = _morphologicalFilters.Dilation(_originalImage);
        }
        public void ApplyErosion()
        {
            Image = _morphologicalFilters.Erosion(_originalImage);
        }
        public void ApplyOpening()
        {
            Image = _morphologicalFilters.Opening(_originalImage);
        }
        public void ApplyClosing()
        {
            Image = _morphologicalFilters.Closing(_originalImage);
        }
        public void ApplyHitOrMiss()
        {
            int[,] foregroundKernel = { { 0, 1, 0 },
                            { 1, 1, 1},
                            { 0, 1, 0 } };

            int[,] backgroundKernel = { { 1, 0, 1 },
                            { 0, 0, 0 },
                            { 1, 0, 1 } };
            Image = _morphologicalFilters.HitOrMiss(_originalImage, foregroundKernel, backgroundKernel);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}