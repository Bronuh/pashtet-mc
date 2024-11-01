#region

using System.ComponentModel;
using System.Runtime.InteropServices;

#endregion

namespace Common.FileSystem.Deploy;

public class WindowsHardLinkDeployer : IFileDeployer
{
    public IHardLinkDeploymentChecker Checker { get; set;  } = new WindowsHardLinkDeploymentChecker();
    
    public bool IsDeployable(string from, string to)
    {
        return Checker.IsHardLinkDeploymentAvailable(from, to);
    }

    public void DeployFile(string from, string to)
    {
        if (!CreateHardLink(to, from, IntPtr.Zero))
        {
            int error = Marshal.GetLastWin32Error();
            throw new Win32Exception(error);
        }
    }
    
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    static extern bool CreateHardLink(string lpFileName, string lpExistingFileName, IntPtr lpSecurityAttributes);
}