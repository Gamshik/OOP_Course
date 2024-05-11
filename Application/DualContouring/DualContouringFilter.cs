using Kitware.VTK;
using System.IO;

namespace DualContouring
{
    public class DualContouringFilter
    {
        /// <summary>
        /// Реконструкция dicom срезов в 3D модель
        /// </summary>
        /// <param name="pathToDicomSlicesDirectory">Путь к папке с dicom срезами</param>
        /// <param name="stlSavePath">Путь для сохранения stl файла</param>
        /// <exception cref="DirectoryNotFoundException">Если папки с dicom срезами не существует</exception>
        public void ReconstructionDicomSlices(string pathToDicomSlicesDirectory, string stlSavePath)
        {
            if (!Directory.Exists(pathToDicomSlicesDirectory))
            {
                throw new DirectoryNotFoundException($"Directory '{pathToDicomSlicesDirectory}' not found.");
            }

            var reader = new vtkDICOMImageReader();

            reader.SetDirectoryName(pathToDicomSlicesDirectory);

            reader.Update();

            var imageData = reader.GetOutput();

            var contourFilter = new vtkContourFilter();

            contourFilter.SetInput(imageData);

            contourFilter.SetValue(0, 250);

            contourFilter.Update();

            var contourFilterOutput = contourFilter.GetOutputPort();

            WriteToStl(contourFilterOutput, stlSavePath);
        }
        /// <summary>
        /// Запись 3D модели в stl файл
        /// </summary>
        /// <param name="contourFilterOutput">Результат работы Dual Contouring алгоритма</param>
        /// <param name="stlSavePath">>Путь для сохранения stl файла</param>
        private void WriteToStl(vtkAlgorithmOutput contourFilterOutput, string stlSavePath)
        {
            if (File.Exists(stlSavePath))
            {
                File.Delete(stlSavePath);
            }

            var stlWriter = new vtkSTLWriter();

            stlWriter.SetInputConnection(contourFilterOutput);

            stlWriter.SetFileName(stlSavePath);

            stlWriter.Write();
        }
    }
}