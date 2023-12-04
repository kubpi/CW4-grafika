using System;
using System.Windows;
using System.Windows.Controls;

namespace CW4_grafika
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int maskSize;
        private TextBox[,] maskTextBoxes;
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new ImageViewModel();
        }
        private void OperationRadioButton_Click(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;
            var viewModel = DataContext as ImageViewModel;

            if (viewModel != null)
            {
                // Resetuj wartości RGB i jasności do 0
                viewModel.ColorR = 0;
                viewModel.ColorG = 0;
                viewModel.ColorB = 0;
                viewModel.BrightnessLevel = 0;

                
                // Sprawdź, czy wybrano GrayButton
                if (radioButton.Name == "GrayButton")
                {
                    viewModel.IsGrayScaleSelected = true;
                    viewModel.IsOperationSelected = false;
                    viewModel.IsBrightnessSelected = false;
                    viewModel.IsFiltersSelected = false;
                    viewModel.IsHistogramSelected = false;
                }
                else if (radioButton.Name == "FiltersButton")
                {
                    viewModel.IsFiltersSelected = true;
                    viewModel.IsGrayScaleSelected = false;
                    viewModel.IsBrightnessSelected = false;
                    viewModel.IsOperationSelected = false;
                    viewModel.IsHistogramSelected = false;
                }
                else if (radioButton.Name == "HistogramsButton")
                {
                    viewModel.IsFiltersSelected = false;
                    viewModel.IsGrayScaleSelected = false;
                    viewModel.IsBrightnessSelected = false;
                    viewModel.IsOperationSelected = false;
                    viewModel.IsHistogramSelected = true;
                }
                else
                {
                    viewModel.IsGrayScaleSelected = false; // Ukryj panel skali szarości dla innych przycisków
                    viewModel.IsFiltersSelected = false;
                    viewModel.IsHistogramSelected = false;
                    if (radioButton.Name != "BrightnessButton")
                    {
                        viewModel.UpdateOperationMode(radioButton.Content.ToString());
                    }
                    else
                    {
                        viewModel.UpdateOperationMode(null);
                    }
                }
            }
        }
             
        private void FiltersComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var viewModel = DataContext as ImageViewModel;
            if (viewModel == null) return;
            switch (viewModel.SelectedFilterIndex)
            {
                case 0:
                    viewModel.ResetToOriginalImage();
                    break;
                case 1:
                    viewModel.ApplySmoothingFilter();
                    break;
                case 2:
                    viewModel.ApplyMedianFilter();
                    break;
                case 3:
                    viewModel.ApplySobelFilter();
                    break;
                case 4:
                    viewModel.ApplySharpeningFilter();
                    break;
                case 5:
                    viewModel.ApplyGaussianBlur();
                    break;
                case 6:
                    viewModel.ResetToOriginalImage();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Nieznany filtr");
            }
        }        
        private void HistogramsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var viewModel = DataContext as ImageViewModel;
            if (viewModel == null) return;
            switch (viewModel.SelectedHistogramIndex)
            {
                case 0:
                    viewModel.ResetToOriginalImage();
                    break;
                case 1:
                    viewModel.ApplyStretchHistogram();
                    break;
                case 2:
                    viewModel.ApplyEqualizeHistogram();
                    break;
                case 3:
                    viewModel.ApplyBinarizeImage();
                    break;
                case 4:
                    viewModel.ApplyPercentBlackSelection();
                    break;
                case 5:
                    viewModel.ApplyMeanIterativeSelection();
                    break;
                case 6:
                    viewModel.ApplyEntropySelection();
                    break;
                case 7:
                    viewModel.ApplyOtsuThresholding();
                    break;
                case 8:
                    viewModel.ApplyNiblackThresholding();
                    break;
                case 9:
                    viewModel.ApplyKapurThresholding();
                    break;
                case 10:
                    viewModel.ApplyLuWuThresholding();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Nieznany filtr");
            }
        }

        private void GrayScaleMethodComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var viewModel = DataContext as ImageViewModel;
            if (viewModel == null) return;

            var comboBox = sender as ComboBox;
            var selectedMethod = comboBox.SelectedIndex;

            switch (selectedMethod)
            {
                case 0: // Brak skali szarości
                    viewModel.ResetToOriginalImage();
                    break;
                case 1: // Średnia RGB
                    viewModel.ConvertToGrayScale(ImageViewModel.ImageOperation.GrayScaleAverage);
                    break;
                case 2:
                    viewModel.ConvertToGrayScale(ImageViewModel.ImageOperation.GrayScaleRed);
                    break;
                case 3:
                    viewModel.ConvertToGrayScale(ImageViewModel.ImageOperation.GrayScaleGreen);
                    break;
                case 4:
                    viewModel.ConvertToGrayScale(ImageViewModel.ImageOperation.GrayScaleBlue);
                    break;
                case 5:
                    viewModel.ConvertToGrayScale(ImageViewModel.ImageOperation.GrayScaleMax);
                    break;
                case 6:
                    viewModel.ConvertToGrayScale(ImageViewModel.ImageOperation.GrayScaleMin);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Nieznana metoda skali szarości");
            }
        }

        private void CreateMaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(MaskSizeTextBox.Text, out maskSize) || maskSize <= 0)
            {
                MessageBox.Show("Rozmiar maski musi być dodatnią liczbą całkowitą.");
                return;
            }

            maskTextBoxes = new TextBox[maskSize, maskSize];
            MaskGrid.Items.Clear();
            for (int i = 0; i < maskSize; i++)
            {
                var rowPanel = new StackPanel { Orientation = Orientation.Horizontal };
                for (int j = 0; j < maskSize; j++)
                {
                    var textBox = new TextBox { Width = 40, Margin = new Thickness(2) };
                    maskTextBoxes[i, j] = textBox;
                    rowPanel.Children.Add(textBox);
                }
                MaskGrid.Items.Add(rowPanel);
            }
        }

        private void ApplyConvolutionButton_Click(object sender, RoutedEventArgs e)
        {
            var mask = new double[maskSize, maskSize];
            for (int i = 0; i < maskSize; i++)
            {
                for (int j = 0; j < maskSize; j++)
                {
                    if (!double.TryParse(maskTextBoxes[i, j].Text, out double value))
                    {
                        MessageBox.Show($"Nieprawidłowa wartość w masce: wiersz {i + 1}, kolumna {j + 1}");
                        return;
                    }
                    mask[i, j] = value;
                }
            }

            // Przykład wywołania metody z ImageViewModel z podaną maską
            var viewModel = DataContext as ImageViewModel;
            viewModel?.ApplyConvolutionFilter(mask);
        }
    }
}
