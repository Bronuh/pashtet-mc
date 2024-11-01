namespace Common.IO;

public class DownloadTask
{
    private readonly string _destinationPath;
    private double _progress;
    private bool _isFinished;
    private string _url;

    public DownloadTask(string url, string destinationPath)
    {
        _url = url;
        _destinationPath = destinationPath;
        _progress = 0;
        _isFinished = false;
    }

    // Проверка, завершена ли загрузка
    public bool IsFinished()
    {
        return _isFinished;
    }

    // Прогресс скачивания от 0 до 1
    public double GetProgress()
    {
        return _progress;
    }

    // Метод для запуска загрузки файла
    public async Task RunAsync()
    {
        using (HttpClient client = new HttpClient())
        {
            using (HttpResponseMessage response = await client.GetAsync(_url, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();

                var contentLength = response.Content.Headers.ContentLength;

                using (var contentStream = await response.Content.ReadAsStreamAsync())
                {
                    await ProcessContentStream(contentLength, contentStream);
                }
            }
        }

        _isFinished = true;
    }

    // Метод для обработки потока данных
    private async Task ProcessContentStream(long? totalDownloadSize, Stream contentStream)
    {
        long totalBytesRead = 0;
        var buffer = new byte[8192];
        bool isMoreToRead = true;

        using (FileStream fileStream = new FileStream(_destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
        {
            do
            {
                int bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length);

                if (bytesRead == 0)
                {
                    isMoreToRead = false;
                    continue;
                }

                await fileStream.WriteAsync(buffer, 0, bytesRead);

                totalBytesRead += bytesRead;

                // Обновляем прогресс
                if (totalDownloadSize.HasValue)
                {
                    _progress = (double)totalBytesRead / totalDownloadSize.Value;
                }
            }
            while (isMoreToRead);
        }
    }
}