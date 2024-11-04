#region

using KludgeBox.Events.EventTypes;

#endregion

namespace PatchApi.Events;


/// <summary>
/// Fired just before <see cref="LauncherPatch.Run"/> call.
/// </summary>
/// <param name="patch">Patch that will be run</param>
public class PatchAboutToRunEnvent(LauncherPatch patch) : CancellableEvent
{
    public LauncherPatch Patch { get; } = patch;
}

/// <summary>
/// Fired when patch execution throws an unhandled exception
/// </summary>
/// <param name="Patch">errored patch</param>
/// <param name="Exception">thrown exception</param>
public record struct PatchRunErroredEvent(LauncherPatch Patch, Exception Exception) : IEvent;