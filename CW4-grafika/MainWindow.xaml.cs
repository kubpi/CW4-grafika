using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static CW4_grafika.ImageViewModel;

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

        private void LoadImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                var viewModel = DataContext as ImageViewModel;
                if (viewModel != null)
                {
                    viewModel.LoadImage(openFileDialog.FileName);
                }
            }
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
                    viewModel.IsOperationSelected = false; // Jeśli to potrzebne
                    viewModel.IsBrightnessSelected = false; // Jeśli to potrzebne
                    viewModel.IsFiltersSelected = false;
                }else if (radioButton.Name == "FiltersButton")
                {
                    viewModel.IsFiltersSelected = true;
                    viewModel.IsGrayScaleSelected = false;
                    viewModel.IsBrightnessSelected = false;
                    viewModel.IsOperationSelected = false;
                }
                else
                {
                    viewModel.IsGrayScaleSelected = false; // Ukryj panel skali szarości dla innych przycisków
                    viewModel.IsFiltersSelected = false;
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



        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ImageViewModel;
            if (viewModel != null)
            {
                viewModel.UpdateImage();
            }
        }
        private void ApplyBrightnessButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ImageViewModel;
            viewModel?.UpdateBrightness();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ImageViewModel;
            viewModel?.SaveCurrentStateAsOriginal();
        }

        private void FiltersComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var viewModel = DataContext as ImageViewModel;
            if (viewModel == null) return;

            viewModel.ApplyFilter(viewModel.SelectedFilterIndex);
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
