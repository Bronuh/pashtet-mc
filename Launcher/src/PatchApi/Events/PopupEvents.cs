#region

using KludgeBox.Events.EventTypes;
using Launcher.Nodes;
using Popup = Launcher.Nodes.Popup;

#endregion

namespace PatchApi.Events;

public class PopupRequestEnqueueEvent(Popup popup, PopupRequest request) : CancellableEvent
{
    public Popup Popup { get; } = popup;
    public PopupRequest Request { get; } = request;
}

public class PopupTryingToShowEvent(Popup popup) : CancellableEvent
{
    public Popup Popup { get; } = popup;
}

public class PopupShowEvent(Popup popup, PopupRequest request) : CancellableEvent
{
    public Popup Popup { get; } = popup;
    public PopupRequest Request { get; } = request;
}

public class PopupClearingEvent(Popup popup) : CancellableEvent
{
    public Popup Popup { get; } = popup;
}