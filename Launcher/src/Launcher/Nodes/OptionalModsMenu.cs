using System.Linq;
using KludgeBox.Events.Global;
using PatchApi.Events;

namespace Launcher.Nodes;

public partial class OptionalModsMenu : Control
{
	[Export] public Button OkButton;
	[Export] public Button CancelButton;
	[Export] public VBoxContainer ModsListContainer;
	[Export] public Label ModDescriptionLabel;
	[Export] public PackedScene ModItemScene;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		OkButton.Pressed += OnOkButtonPressed;
		CancelButton.Pressed += OnCancelButtonPressed;
		
		HideMenu();
	}

	public void HideMenu()
	{
		var evt = new OptionalModsMenuHidingEvent(this);
		if (EventBus.PublishIsCancelled(evt))
		{
			return;
		}
		
		Visible = false;
		foreach (var child in ModsListContainer.GetChildren())
		{
			child.QueueFree();
		}
	}

	public async void ShowMenu()
	{
		var evt = new OptionalModsMenuShowingEvent(this);
		if (EventBus.PublishIsCancelled(evt))
		{
			return;
		}
		Visible = true;

		var modsInfo = await Main.ApiProvider.GetOptionalModsInfoAsync();
		var enabledMods = Main.Settings.GetCustomObject<List<string>>(CustomSettingsKeys.EnabledModsList, new ());
		
		var buildEvt = new OptionalModsListBuildingEvent(modsInfo, enabledMods);
		EventBus.Publish(buildEvt);
		modsInfo = buildEvt.Mods;
		enabledMods = buildEvt.EnabledModsNames;
		
		foreach (var modInfo in modsInfo)
		{
			var modItem = ModItemScene.Instantiate() as ModItem;
			if (modItem is null)
			{
				Log.Warning($"[OptionalModsMenu] Не удается создать экземпляр ModItem: {modInfo.ReadableName}");
				continue;
			}
			
			modItem.Setup(modInfo, enabledMods.Any(m => m == modInfo.FileName), ModDescriptionLabel);
			ModsListContainer.AddChild(modItem);
		}
	}

	private void OnCancelButtonPressed()
	{
		var evt = new OptionalMenuCancelButtonPressedEvent(this, CancelButton);
		if (EventBus.PublishIsCancelled(evt))
		{
			return;
		}
		
		HideMenu();
	}

	private void OnOkButtonPressed()
	{
		try
		{
			var evt = new OptionalMenuOkButtonPressedEvent(this, OkButton);
			if (EventBus.PublishIsCancelled(evt))
			{
				return;
			}
			
			var enabledMods = ModsListContainer.GetChildren()
				.OfType<ModItem>()
				.Where(mod => mod.IsEnabled)
				.Select(mod => mod.ModInfo.FileName)
				.ToList();
			
			Main.Settings.SetCustomObject<List<string>>(CustomSettingsKeys.EnabledModsList, enabledMods);
			SettingsUtils.SaveSettings(Main.Settings);
		}
		catch (Exception ex)
		{
			Log.Error($"Ошибка при закрытии окна доп. модов (ок): {ex.Message}");
		}
		HideMenu();
	}
}