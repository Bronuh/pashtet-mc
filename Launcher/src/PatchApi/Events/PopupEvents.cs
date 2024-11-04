#region

using KludgeBox.Events.EventTypes;
using Launcher.Nodes;
using Popup = Launcher.Nodes.Popup;

#endregion

namespace PatchApi.Events;

public class PopupRequestEnqueueEvent(Popup Popup, PopupRequest Request) : CancellableEvent;
public class PopupTryingToShowEvent(Popup Popup) : CancellableEvent;
public class PopupShowEvent(Popup Popup, PopupRequest Request) : CancellableEvent;
public class PopupClearingEvent(Popup Popup) : CancellableEvent;