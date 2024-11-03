using KludgeBox.Events.EventTypes;

namespace PatchApi.Events;


/// <summary>
/// Fired just before <see cref="LauncherPatch.Run"/> call.
/// </summary>
/// <param name="Patch">Patch that will be run</param>
public class PatchAboutToRunEnvent(LauncherPatch Patch) : CancellableEvent;

/// <summary>
/// Fired when patch execution throws an unhandled exception
/// </summary>
/// <param name="Patch">errored patch</param>
/// <param name="Exception">thrown exception</param>
public record struct PatchRunErroredEvent(LauncherPatch Patch, Exception Exception) : IEvent;