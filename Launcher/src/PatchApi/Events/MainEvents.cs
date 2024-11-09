#region

using KludgeBox.Events.EventTypes;
using Launcher;
using Launcher.Tasks;

#endregion

namespace PatchApi.Events;

public record struct CreatingMainTasksEvent : IEvent;
public record struct RunningMainTasksOnTaskManagerEvent(LauncherTask[] Tasks) : IEvent;

public class RunButtonAboutToUpdate(Button runButton) : CancellableEvent
{
    public Button RunButton { get; } = runButton;
}

public class RunButtonPressedEvent(Button runButton) : CancellableEvent
{
    public Button RunButton { get; } = runButton;
}

public class GameAboutToRunEvent(LauncherTask[] taskSet) : CancellableEvent
{
    public LauncherTask[] TaskSet { get; set; } = taskSet;
}

public class PlayerNameUpdatingEvent(LineEdit nameTextBox) : CancellableEvent
{
    public LineEdit NameTextBox { get; } = nameTextBox;
}

public class PlayerPasswordUpdatingEvent(LineEdit passwordTextBox) : CancellableEvent
{
    public LineEdit PasswordTextBox { get; } = passwordTextBox;
}

public class RamValueChangingEvent(double amountGb) : CancellableEvent
{
    public double AmountGb { get; } = amountGb;
}

public class SettingsSavingEvent(Settings settings) : CancellableEvent
{
    public Settings Settings { get; } = settings;
}