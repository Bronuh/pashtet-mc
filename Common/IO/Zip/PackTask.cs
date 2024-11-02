#region

using System.IO.Compression;
using FileAccess = System.IO.FileAccess;

#endregion

namespace Common.IO.Zip;

public class PackTask
{
    private string _sourcePath;
    private string _zipFilePath;
    private long _totalSize = 0;
    private long _processedSize = 0;
    private bool _isFinished = false;

    public PackTask(string sourcePath, string zipFilePath)
    {
        _sourcePath = sourcePath;
        _zipFilePath = zipFilePath;
    }

    // Метод для проверки завершения упаковки
    public bool IsFinished()
    {
        return _isFinished;
    }

    // Метод для получения прогресса упаковки (в диапазоне от 0 до 1)
    public double GetProgress()
    {
        if (_totalSize == 0)
            return 0;

        return Math.Min(1.0, (double)_processedSize / _totalSize);
    }

    // Асинхронный метод для запуска упаковки
    public async Task RunAsync()
    {
        // Считаем общий размер файлов для упаковки
        CalculateTotalSize(_sourcePath);

        // Создаем zip-архив
        await using (var zipToOpen = new FileStream(_zipFilePath, FileMode.Create))
        using (var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
        {
            // Упаковываем все файлы из папки
            await PackDirectoryAsync(_sourcePath, archive, _sourcePath);
        }

        // Отмечаем завершение упаковки
        _isFinished = true;
    }

    // Метод для расчета общего размера файлов
    private void CalculateTotalSize(string directory)
    {
        foreach (var file in Directory.GetFiles(directory, "*", SearchOption.AllDirectories))
        {
            FileInfo fileInfo = new FileInfo(file);
            _totalSize += fileInfo.Length;
        }
    }

    // Асинхронный метод для упаковки папки
    private async Task PackDirectoryAsync(string directory, ZipArchive archive, string basePath)
    {
        foreach (var file in Directory.GetFiles(directory, "*", SearchOption.AllDirectories))
        {
            string relativePath = file.Substring(basePath.Length + 1);

            // Добавляем файл в архив
            var zipEntry = archive.CreateEntry(relativePath, CompressionLevel.Optimal);

            using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
            using (var zipEntryStream = zipEntry.Open())
            {
                byte[] buffer = new byte[81920];
                int bytesRead;
                while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await zipEntryStream.WriteAsync(buffer, 0, bytesRead);
                    _processedSize += bytesRead;
                }
            }
        }
    }
}