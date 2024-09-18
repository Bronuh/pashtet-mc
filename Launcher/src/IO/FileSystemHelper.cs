using System.IO;

namespace IO;

public static class FileSystemHelper
{
    public static void CopyDirectory(string sourceDir, string destDir)
    {
        // Create the destination directory if it doesn't exist.
        Directory.CreateDirectory(destDir);

        // Get the files in the source directory and copy to the destination directory.
        foreach (var file in Directory.GetFiles(sourceDir))
        {
            string destFile = Path.Combine(destDir, Path.GetFileName(file));
            File.Copy(file, destFile, true);  // true to overwrite existing files
        }

        // Recursively copy subdirectories.
        foreach (var directory in Directory.GetDirectories(sourceDir))
        {
            string destSubDir = Path.Combine(destDir, Path.GetFileName(directory));
            CopyDirectory(directory, destSubDir);  // Recursive call
        }
    }
}