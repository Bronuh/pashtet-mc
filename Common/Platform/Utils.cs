#region

using System.Runtime.InteropServices;

#endregion

namespace Common.Platform;

/// <summary>
/// Represents different types of platforms.
/// </summary>
public enum PlatformType
{
    /// <summary>
    /// Windows platform.
    /// </summary>
    Windows,

    /// <summary>
    /// Unix-like platform (Linux or macOS).
    /// </summary>
    Unix,

    /// <summary>
    /// Other platforms not specifically categorized.
    /// </summary>
    Other
}

public static class Utils
{
    /// <summary>
    /// Determines the current operating system platform.
    /// </summary>
    /// <returns>A <see cref="PlatformType"/> value representing the current platform.</returns>
    public static PlatformType GetPlatform()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return PlatformType.Windows;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return PlatformType.Unix;
        }

        return PlatformType.Other;
    }
}