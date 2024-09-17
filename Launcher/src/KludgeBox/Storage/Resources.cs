#region

using KludgeBox.VFS;

#endregion

namespace KludgeBox.Storage;

[Flags]
public enum LoadFlags
{
    Cache = 1,
    ForceReload = 1 << 1
}

public class Resources
{
    private static Dictionary<string, Resource> _resources = new Dictionary<string, Resource>();
    private static Dictionary<string, string> _textResources = new Dictionary<string, string>();
    private static Dictionary<string, FsFile> _fileHandles = new Dictionary<string, FsFile>();
    
    public static Resource Load(string path, LoadFlags flags = LoadFlags.Cache)
    {
        path = path.NormalizePath();
        var forceReload = flags.HasFlag(LoadFlags.ForceReload);

        if (_resources.TryGetValue(path, out var cachedResource) && !forceReload)
        {
            return cachedResource;
        }
        
        Resource resource;
        
        if (path.EndsWith(".png"))
        {
            var data = Storage.GetFile(path).ReadAllBytes();
            var image = new Image();
            var error = image.LoadPngFromBuffer(data);
            if (error is not Error.Ok)
                throw new Exception($"Unable to load image from path '{path}': {error}");

            resource = ImageTexture.CreateFromImage(image);
        }
        else if (path.EndsWith(".svg")) 
        {
            var data = Storage.GetFile(path).ReadAllBytes();
            var image = new Image();
            var error = image.LoadSvgFromBuffer(data);
            if (error is not Error.Ok)
                throw new Exception($"Unable to load image from path '{path}': {error}");

            resource = ImageTexture.CreateFromImage(image);
        }
        else if (path.EndsWith(".ogg"))
        {
            var data = Storage.GetFile(path).ReadAllBytes();
            resource = AudioStreamOggVorbis.LoadFromBuffer(data);
        }
        else
        {
            throw new Exception($"Unknown or unsupported resource file extension: {path}");
        }

        if (flags.HasFlag(LoadFlags.Cache))
        {
            _resources[path] = resource; 
        }

        return resource;
    }

    public static string LoadAsText(string path, LoadFlags flags = LoadFlags.Cache)
    {
        path = path.NormalizePath();
        var forceReload = flags.HasFlag(LoadFlags.ForceReload);
        if (_textResources.TryGetValue(path, out var cachedText) && !forceReload)
        {
            return cachedText;
        }
        
        var text = Storage.GetFile(path).ReadAllText();
        if (flags.HasFlag(LoadFlags.Cache))
        {
            _textResources[path] = text;
        }
        
        return text;
    }

    /// <summary>
    /// Returns readonly FsFile handle for the given path if available.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="flags"></param>
    /// <returns></returns>
    public static FsFile LoadAsFileHandle(string path, LoadFlags flags = LoadFlags.Cache)
    {
        path = path.NormalizePath();
        var forceReload = flags.HasFlag(LoadFlags.ForceReload);

        if (_fileHandles.TryGetValue(path, out var cachedFileHandle) && !forceReload)
        {
            return cachedFileHandle;
        }
        
        var file = Storage.GetFile(path);
        if (flags.HasFlag(LoadFlags.Cache))
        {
            _fileHandles[path] = file;
        }
        
        return file;
    }
    
    public static void RemoveFromCache(string path)
    {
        path = path.NormalizePath();
        _resources.Remove(path);
        _textResources.Remove(path);
        _fileHandles.Remove(path);
    }
    
    public static void ClearCache()
    {
        _resources.Clear();
        _textResources.Clear();
        _fileHandles.Clear();
    }
}