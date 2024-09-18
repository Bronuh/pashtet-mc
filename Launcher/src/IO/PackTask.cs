using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using FileAccess = System.IO.FileAccess;

namespace IO;

public class PackTask
{
    private string sourcePath;
    private string zipFilePath;
    private long totalSize = 0;
    private long processedSize = 0;
    private bool isFinished = false;

    public PackTask(string sourcePath, string zipFilePath)
    {
        this.sourcePath = sourcePath;
        this.zipFilePath = zipFilePath;
    }

    // Метод для проверки завершения упаковки
    public bool IsFinished()
    {
        return isFinished;
    }

    // Метод для получения прогресса упаковки (в диапазоне от 0 до 1)
    public double GetProgress()
    {
        if (totalSize == 0)
            return 0;

        return Math.Min(1.0, (double)processedSize / totalSize);
    }

    // Асинхронный метод для запуска упаковки
    public async Task RunAsync()
    {
        // Считаем общий размер файлов для упаковки
        CalculateTotalSize(sourcePath);

        // Создаем zip-архив
        using (var zipToOpen = new FileStream(zipFilePath, FileMode.Create))
        using (var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
        {
            // Упаковываем все файлы из папки
            await PackDirectoryAsync(sourcePath, archive, sourcePath);
        }

        // Отмечаем завершение упаковки
        isFinished = true;
    }

    // Метод для расчета общего размера файлов
    private void CalculateTotalSize(string directory)
    {
        foreach (var file in Directory.GetFiles(directory, "*", SearchOption.AllDirectories))
        {
            FileInfo fileInfo = new FileInfo(file);
            totalSize += fileInfo.Length;
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
                    processedSize += bytesRead;
                }
            }
        }
    }
}