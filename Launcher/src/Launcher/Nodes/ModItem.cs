using System.Text;
using Common.Api;
using KludgeBox.Events.Global;
using PatchApi.Events;

namespace Launcher.Nodes;

public partial class ModItem : Control
{
	[Export] public Label ModName;

	[Export] public Button ModToggle;

	[Export] public Button ModSelectionDetector;
	public Label DescriptionLabel;
	public ModInfo ModInfo { get; private set; }
	public bool IsEnabled => ModToggle.ButtonPressed;
	
	public override void _Ready()
	{
		ModName.Text ??= "НЕ ИНИЦИАЛИЗИРОВАН";
		ModSelectionDetector.Pressed += () =>
		{
			if(ModInfo is null)
				return;
			
			var sb = new StringBuilder();
			sb.AppendLine(ModInfo.ReadableName);
			sb.AppendLine();
			sb.AppendLine(ModInfo.Description);
			sb.AppendLine();
			try
			{
				sb.AppendLine(ModInfo.Metadata[ModInfo.MetaModDependencies]);
			}
			catch
			{
				Log.Warning($"Информация о моде {ModInfo.FileName} не содержит метаданных о зависимостях. Отсутствие зависимостей также явно не указано.");
			}

			var evt = new OptionalModItemSelectedEvent(this, sb);

			if (EventBus.PublishIsCancelled(evt))
			{
				return;
			}

			DescriptionLabel.Text = evt.DescriptionBuilder.ToString();
		};

		ModToggle.Toggled += OnModToggled;
	}


	public void Setup(ModInfo modInfo, bool isEnabled, Label descriptionLabel)
	{
		var evt = new OptionalModSetupEvent(this, modInfo, isEnabled, descriptionLabel);

		if (EventBus.PublishIsCancelled(evt))
		{
			return;
		}
		
		modInfo = evt.ModInfo;
		isEnabled = evt.Enabled;
		descriptionLabel = evt.DescriptionLabel;
		
		ModInfo = modInfo;
		ModName.Text = ModInfo.ReadableName;
		ModToggle.ButtonPressed = isEnabled;
		DescriptionLabel  = descriptionLabel;
	}

	private void OnModToggled(bool isEnabled)
	{
		var evt = new OptionalModToggleEvent(this, isEnabled);
		if (EventBus.PublishIsCancelled(evt))
		{
			ModToggle.ButtonPressed = !isEnabled;
			return;
		}
		
		ModToggle.ButtonPressed = evt.Enabled;
	}
}