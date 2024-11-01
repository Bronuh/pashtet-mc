﻿using Common.IO.Checksum;

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
        var baseUri = new Uri(apiBase);
        var filesList = files.ToList();
        var remoteFiles = new RemoteFile[filesList.Count];

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