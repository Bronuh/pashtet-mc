using System.Text;
using Common.Api;
using KludgeBox.Events.EventTypes;
using Launcher.Nodes;

namespace PatchApi.Events;

public class OptionalModItemSelectedEvent : CancellableEvent
{
    public ModItem ModItem { get; }
    public StringBuilder DescriptionBuilder { get; set; }

    public OptionalModItemSelectedEvent(ModItem modItem, StringBuilder descriptionBuilder)
    {
        ModItem = modItem;
        DescriptionBuilder = descriptionBuilder;
    }
}

public class OptionalModSetupEvent : CancellableEvent
{
    public ModItem ModItem { get; }
    public ModInfo ModInfo { get; set; }
    public bool Enabled { get; set; }
    public Label DescriptionLabel { get; set; }

    public OptionalModSetupEvent(ModItem modItem, ModInfo modInfo, bool enabled, Label descriptionLabel)
    {
        ModItem = modItem;
        ModInfo = modInfo;
        Enabled = enabled;
        DescriptionLabel = descriptionLabel;
    }
}

public class OptionalModToggleEvent : CancellableEvent
{
    public ModItem ModItem { get; }
    public bool Enabled { get; set; }

    public OptionalModToggleEvent(ModItem modItem, bool enabled)
    {
        ModItem = modItem;
        Enabled = enabled;
    }
}


public class OptionalModsMenuHidingEvent : CancellableEvent
{
    public OptionalModsMenu Menu { get; }

    public OptionalModsMenuHidingEvent(OptionalModsMenu menu)
    {
        Menu = menu;
    }
}

public class OptionalModsMenuShowingEvent : CancellableEvent
{
    public OptionalModsMenu Menu { get; }

    public OptionalModsMenuShowingEvent(OptionalModsMenu menu)
    {
        Menu = menu;
    }
}

public class OptionalMenuOkButtonPressedEvent : CancellableEvent
{
    public OptionalModsMenu Menu { get; }
    public Button OkButton { get; }

    public OptionalMenuOkButtonPressedEvent(OptionalModsMenu menu, Button okButton)
    {
        Menu = menu;
        OkButton = okButton;
    }
}

public class OptionalMenuCancelButtonPressedEvent : CancellableEvent
{
    public OptionalModsMenu Menu { get; }
    public Button CancelButton { get; }

    public OptionalMenuCancelButtonPressedEvent(OptionalModsMenu menu, Button cancelButton)
    {
        Menu = menu;
        CancelButton = cancelButton;
    }
}