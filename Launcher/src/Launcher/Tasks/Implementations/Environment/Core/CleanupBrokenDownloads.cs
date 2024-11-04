using System.IO;
using System.Threading.Tasks;
using Common.Api;
using Common.IO.Checksum;
using HashedFiles;
using Launcher.Nodes;

namespace Launcher.Tasks.Implementations.Environment.Core;

public class CleanupBrokenDownloads : LauncherTask
{
    public override string Name { get; } = "Очистка невалидных закачек";
    public IChecksumProvider ChecksumProvider { get; set; }
    protected override async Task Start()
    {
        ChecksumProvider ??= Main.ChecksumProvider;
        var minecraftZipPath = Paths.MinecraftZipPath.AsAbsolute();
        var jreZipPath = Paths.JreZipPath.AsAbsolute();

        var localMinecraftZip = new LocalFile(minecraftZipPath, ChecksumProvider);
        var localJreZip = new LocalFile(jreZipPath, ChecksumProvider);
        
        if (localMinecraftZip.Exists())
        {
            var remoteMinecraftZip = await Main.ApiProvider.GetMinecraftInfoAsync();
            if (remoteMinecraftZip.Checksum != localMinecraftZip.GetPrecalculatedChecksum())
            {
                Log.Warning($"Контрольная сумма {localMinecraftZip.FileName} не соответствует оригинальной:\n{localMinecraftZip.GetPrecalculatedChecksum()} != {remoteMinecraftZip.Checksum}");
                File.Delete(localMinecraftZip.FilePath);
            }
            else
            {
                Log.Info($"{localMinecraftZip.FileName} существует и соответствует оригинальному");
            }
        }
        
        if (localJreZip.Exists())
        {
            var remoteJarZip = await Main.ApiProvider.GetJavaInfoAsync();
            if (remoteJarZip.Checksum != localJreZip.GetPrecalculatedChecksum())
            {
                Log.Warning($"Контрольная сумма {localJreZip.FileName} не соответствует оригинальной:\n{localJreZip.GetPrecalculatedChecksum()} != {remoteJarZip.Checksum}");
                File.Delete(localJreZip.FilePath);
            }
            else
            {
                Log.Info($"{localJreZip.FileName} существует и соответствует оригинальному");
            }
        }
    }

    public override IEnumerable<LauncherTask> OnTaskFinished()
    {
        return null;
    }
}