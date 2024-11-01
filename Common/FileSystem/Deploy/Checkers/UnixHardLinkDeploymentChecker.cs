#region

using System.Runtime.InteropServices;

#endregion

namespace Common.FileSystem.Deploy;

public class UnixHardLinkDeploymentChecker : IHardLinkDeploymentChecker
{
    public static string[] SupportedFormats = ["ntfs", "ext2", "ext3", "ext4", "btrfs"];
    
    public bool IsHardLinkDeploymentAvailable(string path)
    {
        var fullPath = Path.GetFullPath(path);
        var driveInfo = new DriveInfo(fullPath);

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
        from = Path.GetFullPath(from);
        to = Path.GetFullPath(to);
        
        StatBuf fromStat, toStat;
        if (Stat(from, out fromStat) == 0 && Stat(to, out toStat) == 0)
        {
            if (fromStat.st_dev == toStat.st_dev)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            throw new Exception($"Exception while checking files device for {from} and {to}");
        }
    }
    
    private bool CheckDriveFormat(string format)
    {
        return SupportedFormats.Contains(format);
    }
    
    // Define the stat structure (not all fields)
    [StructLayout(LayoutKind.Sequential)]
    public struct StatBuf
    {
        public ulong st_dev; // Device ID
        // Other fields omitted for brevity...
    }

    [DllImport("libc", EntryPoint = "stat")]
    public static extern int Stat(string path, out StatBuf buf);
}