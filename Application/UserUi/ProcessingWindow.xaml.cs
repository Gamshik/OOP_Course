using Dicom;
using Dicom.Imaging;
using DualContouring;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using ThresholdFilter;
using Image = System.Windows.Controls.Image;
using MessageBox = System.Windows.MessageBox;

namespace UserUi
{
    /// <summary>
    /// Логика взаимодействия для ProcessingWindow.xaml
    /// </summary>
    public partial class ProcessingWindow : Window
    {
        private string _sourceDirectory;
        private string _destinationDirectory;

        public ProcessingWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Обработка dicom срезов
        /// </summary>
        /// <param name="sourceDirectory">Путь к папке с исходными dicom срезами</param>
        /// <param name="destinationDirectory">Путь к папке для сохранения dicom срезов</param>
        public void StartProcessing(string sourceDirectory, string destinationDirectory)
        {
            _sourceDirectory = sourceDirectory;
            _destinationDirectory = destinationDirectory;

            var thresholdFilter = new DicomThresholdFilter();

            try
            {
                thresholdFilter.CleanTheDicomKit(_sourceDirectory, _destinationDirectory, 1200);
            }
            catch (DirectoryNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Invalid directory", MessageBoxButton.OK, MessageBoxImage.Error);
                ShowMainWindow();
                Close();
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ShowMainWindow();
                Close();
                return;
            }

            DrawDicomSet(_sourceDirectory, OriginalImagesPanel);
            DrawDicomSet(_destinationDirectory, ProcessedImagesPanel);
        }
        /// <summary>
        /// Отображение dicom срезов
        /// </summary>
        /// <param name="pathToDicomSetDirectory">Путь к папке с dicom срезами</param>
        /// <param name="panel">Панель в которой будут содержаться изображения</param>
        private void DrawDicomSet(string pathToDicomSetDirectory, WrapPanel panel)
        {
            foreach (string pathToDicomFile in Directory.EnumerateFiles(pathToDicomSetDirectory))
            {
                var dicomFile = DicomFile.Open(pathToDicomFile);

                var dicomImage = new DicomImage(dicomFile.Dataset);

                using (Bitmap bitmap = dicomImage.RenderImage().AsSharedBitmap())
                {
                    var bitmapImage = ConvertBitmapToBitmapImage(bitmap);

                    var image = new Image
                    {
                        Margin = new Thickness(5),
                        Source = bitmapImage
                    };

                    panel.Children.Add(image);
                }
            }
        }
        /// <summary>
        /// Преобразование Bitmap в BitmapImage
        /// </summary>
        /// <param name="bitmap">Bitmap сущность</param>
        /// <returns></returns>
        private BitmapImage ConvertBitmapToBitmapImage(Bitmap bitmap)
        {
            var bitmapImage = new BitmapImage();
            using (var memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, ImageFormat.Bmp);
                memoryStream.Position = 0;
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.EndInit();
            }

            return bitmapImage;
        }
        private void Create3DModelButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "STL files (*.stl)|*.stl";
            saveFileDialog.DefaultExt = ".stl";
            saveFileDialog.AddExtension = true;

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;

                var dualContouringFilter = new DualContouringFilter();

                dualContouringFilter.ReconstructionDicomSlices(_destinationDirectory, filePath);

                MessageBox.Show("Model has been saved successfully.", "Reconstruction successful", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void ChangeDicomSetButton_Click(object sender, RoutedEventArgs e)
        {
            ShowMainWindow();
            Close();
        }
        private void CloseButton_Clock(object sender, RoutedEventArgs e) => Close();
        /// <summary>
        /// Показать главное окно
        /// </summary>
        private void ShowMainWindow()
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}