﻿using Common.Api;

namespace BronuhMcBackend.Models.Api;

public class DefaultApiProvider : IApiProvider
{
    
    public const string CoreDirName = "core";
    public const string JreDirName = "jre";
    public const string JreFileName = "jdk-21.zip";
    public const string MinecraftDirName = "minecraft";
    public const string MinecraftFileName = "minecraft-min.zip";
    
    public const string ModsDirName = "mods";
    public const string OptionalModsDirName = "optional-mods";
    public const string ConfigDirName = "config";
    
    public const string LauncherDirName = "launcher";

    
    public const string StorageDirPath = "/storage";
    public const string McServerDirPath = "/mcserver";
    
    public string CoreDirPath => Path.Combine(StorageDirPath, CoreDirName);
    public string JreDirPath => Path.Combine(CoreDirPath, JreDirName);
    public string JreFilePath => Path.Combine(JreDirPath, JreFileName);
    public string MinecraftDirPath => Path.Combine(CoreDirPath, MinecraftDirName);
    public string MinecraftFilePath => Path.Combine(MinecraftDirPath, MinecraftFileName);
    
    public string ModsDirPath => Path.Combine(McServerDirPath, ModsDirName);
    public string OptionalModsDirPath => Path.Combine(StorageDirPath, OptionalModsDirName);
    public string ConfigDirPath => Path.Combine(McServerDirPath, ConfigDirName);
    

    public string RootPath { get; private set; } = "";
    
    private ILogger _logger;

    public DefaultApiProvider(ILogger<DefaultApiProvider> logger)
    {
        _logger = logger;
    }


    public void Initialize()
    {
        Directory.CreateDirectory(JreDirPath);
        Directory.CreateDirectory(MinecraftDirPath);
        Directory.CreateDirectory(OptionalModsDirPath);
    }

    public LocalFile GetJavaFile()
    {
        return new LocalFile(JreFilePath);
    }

    public LocalFile GetMinecraftFile()
    {
       return new LocalFile(MinecraftFilePath);
    }


    public IEnumerable<LocalFile> GetRequiredModsList()
    {
        var modsPaths = Directory.GetFiles(ModsDirPath);
        var localFiles = modsPaths.Select(modPath => new LocalFile(modPath)).ToList();
        
        return localFiles;
    }

    public IEnumerable<LocalFile> GetOptionalModsList()
    {
        var modsPaths = Directory.GetFiles(OptionalModsDirPath);
        var localFiles = modsPaths.Select(modPath => new LocalFile(modPath)).ToList();
        
        return localFiles;
    }

    public LocalFile GetRequiredModFile(string modName)
    {
        return new LocalFile(Path.Combine(ModsDirPath, modName));
    }
    
    
    public LocalFile GetOptionalModFile(string modName)
    {
        return new LocalFile(Path.Combine(OptionalModsDirPath, modName));
    }
}