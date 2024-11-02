#region

using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

#endregion

namespace IO;

public static class HardLinkHelper
{
    public static void DeleteHardLink(string linkPath)
    {
        if (File.Exists(linkPath))
        {
            File.Delete(linkPath);
            Console.WriteLine($"Hard link '{linkPath}' has been deleted.");
        }
        else
        {
            Console.WriteLine($"File '{linkPath}' does not exist.");
        }
    }

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    static extern bool CreateHardLink(string lpFileName, string lpExistingFileName, IntPtr lpSecurityAttributes);

    public static void CreateHardLink(string from, string to)
    {
        if (!CreateHardLink(to, from, IntPtr.Zero))
        {
            int error = Marshal.GetLastWin32Error();
            throw new Win32Exception(error);
        }
    }
}