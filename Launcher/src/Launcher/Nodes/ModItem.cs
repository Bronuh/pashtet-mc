using System.Linq;
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
				sb.AppendLine($"Зависит от: {ModInfo.Metadata[ModInfo.MetaModDependencies]}");
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

	public void SetEnabled(bool enabled)
	{
		ModToggle.ButtonPressed = enabled;
	}

	public void Enable() => SetEnabled(true);
	public void Disable() => SetEnabled(false);

	public bool HasDependency(ModInfo supposedDependency)
	{
		var dependencyNames = (ModInfo.Metadata[ModInfo.MetaModDependencies] ?? "")
			.Split(",")
			.Select(x => x.Trim())
			.ToList();

		return dependencyNames.Contains(supposedDependency.ReadableName) || dependencyNames.Contains(supposedDependency.FileName);
	}
	
	public bool HasDependency(ModItem supposedDependency)
	{
		var dependencyNames = (ModInfo.Metadata[ModInfo.MetaModDependencies] ?? "")
			.Split(",")
			.Select(x => x.Trim())
			.ToList();

		return dependencyNames.Contains(supposedDependency.ModInfo.ReadableName) || dependencyNames.Contains(supposedDependency.ModInfo.FileName);
	}

	public IEnumerable<ModItem> GetDependencies(IEnumerable<ModItem> modsList)
	{
		// ReSharper disable once ArrangeThisQualifier
		// ReSharper disable once ConvertClosureToMethodGroup
		// make it explicit
		var dependencies = modsList.Where(mod => this.HasDependency(mod)); 
		return dependencies;
	}

	public IEnumerable<ModItem> GetDependents(IEnumerable<ModItem> modsList)
	{
		var dependents = modsList.Where(mod => mod.HasDependency(this));
		return dependents;
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