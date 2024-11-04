#region

using KludgeBox.Events.EventTypes;
using Launcher;
using Launcher.Tasks;

#endregion

namespace PatchApi.Events;

public record struct CreatingMainTasksEvent : IEvent;
public record struct RunningMainTasksOnTaskManagerEvent(LauncherTask[] task) : IEvent;

public class RunButtonAboutToUpdate(Button RunButton) : CancellableEvent;
public class RunButtonPressedEvent(Button RunButton) : CancellableEvent;

public class PlayerNameUpdatingEvent(LineEdit NameTextBox) : CancellableEvent;
public class PlayerPasswordUpdatingEvent(LineEdit PasswordTextBox) : CancellableEvent;
public class RamValueChangingEvent(ref double AmountGb) : CancellableEvent;
public class SettingsSavingEvent(Settings Settings) : CancellableEvent;