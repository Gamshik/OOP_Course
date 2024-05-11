using Dicom;
using Dicom.Imaging;
using Dicom.IO.Buffer;
using System;
using System.IO;

namespace ThresholdFilter
{
    public class DicomThresholdFilter
    {
        /// <summary>
        /// Очистка dicom срезов при помощи порогового фильтра
        /// </summary>
        /// <param name="pathToSourceDirectory">Путь к папке с исходными dicom срезами</param>
        /// <param name="pathToDestinationDirectory">Путь к папке для сохранения dicom срезов</param>
        /// <param name="thresholdValue">Пороговое значение</param>
        /// <exception cref="DirectoryNotFoundException">Если папки с dicom срезами или для сохранения срезов не существует</exception>
        public void CleanTheDicomKit(string pathToSourceDirectory, string pathToDestinationDirectory, short thresholdValue)
        {
            if (!Directory.Exists(pathToSourceDirectory))
            {
                throw new DirectoryNotFoundException($"Directory '{pathToSourceDirectory}' not found.");
            }

            if (!Directory.Exists(pathToDestinationDirectory))
            {
                throw new DirectoryNotFoundException($"Directory '{pathToDestinationDirectory}' not found.");
            }

            foreach (string pathToDicomFile in Directory.EnumerateFiles(pathToSourceDirectory))
            {
                var dicomFile = DicomFile.Open(pathToDicomFile);

                var pixelData = DicomPixelData.Create(dicomFile.Dataset);

                var originalPixelBytes = pixelData.GetFrame(0).Data;

                var originalPixelShorts = new short[originalPixelBytes.Length / sizeof(short)];

                Buffer.BlockCopy(originalPixelBytes, 0, originalPixelShorts, 0, originalPixelBytes.Length);

                var modifiedPixelBytes = CleanPixels(originalPixelShorts, thresholdValue);

                var modifiedPixelBytesBuffer = new MemoryByteBuffer(modifiedPixelBytes);

                dicomFile.Dataset.AddOrUpdatePixelData(DicomVR.OB, modifiedPixelBytesBuffer);

                string fileName = Path.GetFileName(pathToDicomFile);

                dicomFile.Save($"{pathToDestinationDirectory}\\{fileName}");
            }
        }
        /// <summary>
        /// Очистка исходных пикселей
        /// </summary>
        /// <param name="originalPixelShorts">Исходные пиксели</param>
        /// <param name="thresholdValue">Пороговое значение</param>
        /// <returns>Очищенные пиксели</returns>
        private byte[] CleanPixels(short[] originalPixelShorts, short thresholdValue)
        {
            var modifiedPixelBytes = new byte[originalPixelShorts.Length * sizeof(short)];

            for (int i = 0; i < originalPixelShorts.Length; i++)
            {
                var newPixelValueShort = originalPixelShorts[i] >= thresholdValue ? (short)2000 : (short)0;
                var newPixelValueBytes = BitConverter.GetBytes(newPixelValueShort);
                Buffer.BlockCopy(newPixelValueBytes, 0, modifiedPixelBytes, i * sizeof(short), sizeof(short));
            }

            return modifiedPixelBytes;
        }
    }
}