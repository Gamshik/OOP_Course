﻿using System.IO;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace UserUi
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void BrowseSourceButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SourceDirectoryTextBlock.Text = "Selected directory: " + dialog.SelectedPath;
            }
        }
        private void BrowseDestinationButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DestinationDirectoryTextBlock.Text = "Selected directory: " + dialog.SelectedPath;
            }
        }
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            string sourceDirectory = SourceDirectoryTextBlock.Text.Substring(SourceDirectoryTextBlock.Text.IndexOf(":") + 1).Trim();
            string destinationDirectory = DestinationDirectoryTextBlock.Text.Substring(SourceDirectoryTextBlock.Text.IndexOf(":") + 1).Trim();

            if (sourceDirectory == destinationDirectory)
            {
                MessageBox.Show("The paths to the directories cannot be the same.", "Invalid paths", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!Directory.Exists(sourceDirectory))
            {
                MessageBox.Show("Source directory does not exist.", "Invalid directory", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!Directory.Exists(destinationDirectory))
            {
                MessageBox.Show("Destination directory does not exist.", "Invalid directory", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var processingWindow = new ProcessingWindow();
            processingWindow.Show();
            processingWindow.StartProcessing(sourceDirectory, destinationDirectory);
            Close();
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
    }
}