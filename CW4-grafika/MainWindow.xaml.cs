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
                }
                else
                {
                    viewModel.IsGrayScaleSelected = false; // Ukryj panel skali szarości dla innych przycisków

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


        private void ConvertToGrayScaleButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ImageViewModel;
            var selectedMethod = GrayScaleMethodComboBox.SelectedIndex;

            ImageOperation operation = selectedMethod switch
            {
                0 => ImageOperation.GrayScaleAverage,
                1 => ImageOperation.GrayScaleRed,
                2 => ImageOperation.GrayScaleGreen,
                3 => ImageOperation.GrayScaleBlue,
                4 => ImageOperation.GrayScaleMax,
                5 => ImageOperation.GrayScaleMin,
                _ => throw new InvalidOperationException("Nieznany wybór metody skali szarości"),
            };

            viewModel?.ConvertToGrayScale(operation);
        }

        // Metoda obsługująca kliknięcie przycisku resetującego skalę szarości
        private void ResetGrayScaleButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ImageViewModel;
            if (viewModel != null)
            {
                viewModel.ResetToOriginalImage();
            }
        }


    }
}
