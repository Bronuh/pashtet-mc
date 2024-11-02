#region

using Common.IO.Http;

#endregion

namespace Common.Api;

/// <summary>
/// Record representing a remote read-accessible file.
/// </summary>
/// <param name="Name">The name of the file.</param>
/// <param name="Url">The URL to download the file from.</param>
/// <param name="Checksum">The checksum of the file for verification purposes.</param>
public record RemoteFile(string Name, string Url, string Checksum)
{
    /// <summary>
    /// Downloads the file to the specified directory.
    /// </summary>
    /// <param name="directoryPath">The path of the directory where the file will be saved.</param>
    /// <param name="fileName">
    /// Optional. The name of the file to be saved. If not provided, the file's original name will be used.
    /// </param>
    public void DownloadToDir(string directoryPath, string fileName = null)
    {
        var path = Path.Combine(directoryPath, String.IsNullOrWhiteSpace(fileName) ? Name : fileName);
        HttpHelper.DownloadFile(Url, path);
    }

    /// <summary>
    /// Downloads the file to the specified file path.
    /// </summary>
    /// <param name="filePath">The full path (including file name) where the file will be saved.</param>
    public void DownloadToFile(string filePath)
    {
        HttpHelper.DownloadFile(Url, filePath);
    }

    /// <summary>
    /// Asynchronously downloads the file to the specified directory.
    /// </summary>
    /// <param name="directoryPath">The path of the directory where the file will be saved.</param>
    /// <param name="fileName">
    /// Optional. The name of the file to be saved. If not provided, the file's original name will be used.
    /// </param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task DownloadToDirAsync(string directoryPath, string fileName = null)
    {
        var path = Path.Combine(directoryPath, String.IsNullOrWhiteSpace(fileName) ? Name : fileName);
        await HttpHelper.DownloadFileAsync(Url, path);
    }

    /// <summary>
    /// Asynchronously downloads the file to the specified file path.
    /// </summary>
    /// <param name="filePath">The full path (including file name) where the file will be saved.</param>
    /// <param name="fileName">
    /// Optional. The name of the file to be saved. If not provided, the file's original name will be used.
    /// </param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task DownloadToFileAsync(string filePath, string fileName = null)
    {
        await HttpHelper.DownloadFileAsync(Url, filePath);
    }
}