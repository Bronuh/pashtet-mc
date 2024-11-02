#region

using Common.IO.Checksum;

#endregion

namespace Common.Api;

public class RemoteFilesList
{
    public RemoteFilesList()
    {
    }

    public RemoteFilesList(RemoteFile[] files)
    {
        Files = files;
    }

    public RemoteFilesList(IEnumerable<LocalFile> files, string apiBase, IChecksumProvider checksumProvider)
    {
        var baseUri = new Uri(apiBase.EndsWith('/') ? apiBase : apiBase + "/");
        var filesList = files.ToList();
        var remoteFiles = new RemoteFile[filesList.Count];
        Console.WriteLine($"baseUri: {baseUri}");
        int i = 0;
        foreach (var file in filesList)
        {
            var uri = new Uri(baseUri, file.FileName).ToString();
            var remoteFile = file.AsRemote(uri, checksumProvider);
            remoteFiles[i] = remoteFile;
            i++;
        }

        Files = remoteFiles;
    }

    public RemoteFile[] Files { get; set; }
}