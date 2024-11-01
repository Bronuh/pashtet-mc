namespace Common.FileSystem.Deploy;

public sealed class WindowsHardLinkDeploymentChecker : IHardLinkDeploymentChecker
{
    public static string[] SupportedFormats = ["NTFS", "ntfs"];
    
    public bool IsHardLinkDeploymentAvailable(string path)
    {
        var fullPath = Path.GetFullPath(path);
        var pathRoot = Path.GetPathRoot(fullPath);
        var driveInfo = new DriveInfo(pathRoot);

        return CheckDriveFormat(driveInfo.DriveFormat);
    }

    public bool IsHardLinkDeploymentAvailable(string from, string to)
    {
        if (!IsTheSameDevice(from, to))
        {
            return false;
        }

        // If from and to are on the same drive, then we can just check any of them
        return IsHardLinkDeploymentAvailable(from);
    }

    private bool IsTheSameDevice(string from, string to)
    {
        var fullPathFrom = Path.GetFullPath(from);
        var fullPathTo = Path.GetFullPath(to);
        
        var rootFrom = Path.GetPathRoot(fullPathFrom);
        var rootTo = Path.GetPathRoot(fullPathTo);

        if (!rootFrom[0].Equals(rootTo[0]))
        {
            // The drives are not the same
            return false;
        }
        
        return true;
    }
    
    private bool CheckDriveFormat(string format)
    {
        return SupportedFormats.Contains(format);
    }
}