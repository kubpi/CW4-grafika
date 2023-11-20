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

            var comboBox = sender as ComboBox;
            var selectedFilter = comboBox.SelectedIndex;

            viewModel.ApplyFilter(selectedFilter);
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

    }
}
