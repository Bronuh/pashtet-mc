#region

using System.IO.Compression;

#endregion

namespace Common.IO.Zip;

public class UnpackTask
{
    private readonly string _zipFilePath;
    private readonly string _destinationPath;
    private long _totalBytes;
    private long _extractedBytes;
    private bool _isFinished;

    public UnpackTask(string zipFilePath, string destinationPath)
    {
        _zipFilePath = zipFilePath;
        _destinationPath = destinationPath;
        _totalBytes = 0;
        _extractedBytes = 0;
        _isFinished = false;
    }

    // Метод для проверки завершена ли распаковка
    public bool IsFinished()
    {
        return _isFinished;
    }

    // Метод для получения прогресса (от 0 до 1)
    public double GetProgress()
    {
        if (_totalBytes == 0) return 0;
        return (double)_extractedBytes / _totalBytes;
    }

    // Основной метод для запуска распаковки
    public async Task RunAsync()
    {
        await Task.Run(ExtractArchive);
    }

    // Метод для извлечения архива
    private void ExtractArchive()
    {
        // Подсчёт общего количества байтов в архиве
        using (var zipArchive = ZipFile.OpenRead(_zipFilePath))
        {
            foreach (var entry in zipArchive.Entries)
            {
                _totalBytes += entry.Length;
            }
        }

        // Распаковка архива с отслеживанием прогресса
        using (var zipArchive = ZipFile.OpenRead(_zipFilePath))
        {
            foreach (var entry in zipArchive.Entries)
            {
                var destinationFileName = Path.Combine(_destinationPath, entry.FullName);

                // Создаём директорию для вложенных файлов
                if (string.IsNullOrEmpty(entry.Name))
                {
                    Directory.CreateDirectory(destinationFileName);
                }
                else
                {
                    // Извлекаем файл и обновляем количество извлечённых байтов
                    Directory.CreateDirectory(Path.GetDirectoryName(destinationFileName)!);
                    
                    entry.ExtractToFile(destinationFileName, overwrite: true);

                    _extractedBytes += entry.Length;
                }
            }
        }

        _isFinished = true;
    }
}