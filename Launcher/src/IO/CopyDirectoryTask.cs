using System.IO;
using System.Threading.Tasks;

namespace IO;

public class DirectoryCopyTask
{
    private readonly string _sourcePath;
    private readonly string _destinationPath;
    private long _totalFilesAndDirectories;
    private long _processedFilesAndDirectories;

    // Конструктор, инициализирует пути исходной и целевой папок
    public DirectoryCopyTask(string sourcePath, string destinationPath)
    {
        _sourcePath = sourcePath ?? throw new ArgumentNullException(nameof(sourcePath));
        _destinationPath = destinationPath ?? throw new ArgumentNullException(nameof(destinationPath));
    }

    // Метод, проверяющий, завершено ли копирование
    public bool IsFinished()
    {
        return _processedFilesAndDirectories >= _totalFilesAndDirectories;
    }

    // Метод, возвращающий прогресс выполнения копирования от 0 до 1
    public double GetProgress()
    {
        if (_totalFilesAndDirectories == 0)
            return 0;
        
        return (double)_processedFilesAndDirectories / _totalFilesAndDirectories;
    }

    // Асинхронный метод для запуска процесса копирования
    public async Task Run()
    {
        // Подсчет всех файлов и папок для последующего расчета прогресса
        _totalFilesAndDirectories = Directory.GetFiles(_sourcePath, "*", SearchOption.AllDirectories).LongLength +
                                    Directory.GetDirectories(_sourcePath, "*", SearchOption.AllDirectories).LongLength;

        // Рекурсивное копирование папки
        await Task.Run(() => CopyDirectoryRecursive(_sourcePath, _destinationPath));
    }

    // Рекурсивный метод для копирования папок и файлов
    private void CopyDirectoryRecursive(string sourceDir, string destinationDir)
    {
        // Создание целевой папки, если она не существует
        if (!Directory.Exists(destinationDir))
        {
            Directory.CreateDirectory(destinationDir);
        }

        // Копирование всех файлов из текущей папки
        foreach (var file in Directory.GetFiles(sourceDir))
        {
            var fileName = Path.GetFileName(file);
            var destFile = Path.Combine(destinationDir, fileName);
            File.Copy(file, destFile, true);

            // Увеличение счетчика обработанных файлов/папок и расчет прогресса
            _processedFilesAndDirectories++;
        }

        // Рекурсивное копирование всех вложенных папок
        foreach (var directory in Directory.GetDirectories(sourceDir))
        {
            var directoryName = Path.GetFileName(directory);
            var destDirectory = Path.Combine(destinationDir, directoryName);
            CopyDirectoryRecursive(directory, destDirectory);

            // Увеличение счетчика обработанных файлов/папок
            _processedFilesAndDirectories++;
        }
    }
}