namespace Launcher.Nodes;

public partial class Popup : Control
{
	[Export] public HBoxContainer ButtonsContainer { get; private set; }
	[Export] public Label TitleLabel { get; private set; }
	[Export] public Label DescriptionLabel { get; private set; }
	
	private Queue<PopupRequest> _popupRequests = new();
	public override void _Ready()
	{
		Clear();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public PopupBuilder BeginBuild()
	{
		var builder = new PopupBuilder(this);
		
		return builder;
	}

	public void QueuePopup(PopupRequest popup)
	{
		_popupRequests.Enqueue(popup);
		TryShowNextPopup();
	}

	private void ShowPopup(PopupRequest popup)
	{
		TitleLabel.Text = popup.Title;
		DescriptionLabel.Text = popup.Description;
		
		foreach (var buttonRequest in popup.Buttons)
		{
			var button = new Button();
			button.Text = buttonRequest.Title;
			button.Pressed += buttonRequest.Action;
			ButtonsContainer.AddChild(button);
		}
		
		Visible = true;
	}

	private void HidePopup()
	{
		Visible = false;
		Clear();
		TryShowNextPopup();
	}

	private void TryShowNextPopup()
	{
		if (_popupRequests.TryDequeue(out var popup))
		{
			ShowPopup(popup);
		}
	}
	
	private void Clear()
	{
		TitleLabel.Text = "";
		DescriptionLabel.Text = "";
		foreach (var button in ButtonsContainer.GetChildren())
		{
			button.QueueFree();
		}
	}
}