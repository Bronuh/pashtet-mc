using KludgeBox.Events.Global;
using PatchApi.Events;

namespace Launcher.Nodes;

public partial class Popup : Control
{
	[Export] public HBoxContainer ButtonsContainer { get; private set; }
	[Export] public Label TitleLabel { get; private set; }
	[Export] public Label DescriptionLabel { get; private set; }
	
	public PopupRequest CurrentPopupRequest { get; private set; }
	
	private Queue<PopupRequest> _popupRequests = new();
	public override void _Ready()
	{
		Clear();
	}


	/// <summary>
	/// Returns PopupBuilder, attached to this Popup
	/// </summary>
	/// <returns></returns>
	public PopupBuilder BeginBuild()
	{
		var builder = new PopupBuilder(this);
		
		return builder;
	}

	/// <summary>
	/// Enqueues and shows new Popup
	/// </summary>
	/// <param name="popup"></param>
	public void QueuePopup(PopupRequest popup)
	{
		if (EventBus.PublishIsCancelled(new PopupRequestEnqueueEvent(this, popup)))
			return;
		
		_popupRequests.Enqueue(popup);
		TryShowNextPopup();
	}

	
	public void TryShowNextPopup()
	{
		if (EventBus.PublishIsCancelled(new PopupTryingToShowEvent(this)))
			return;
		
		if (_popupRequests.TryDequeue(out var popup))
		{
			ShowPopup(popup);
		}
	}

	private void ShowPopup(PopupRequest popup)
	{
		if (EventBus.PublishIsCancelled(new PopupShowEvent(this, popup)))
			return;
		
		TitleLabel.Text = popup.Title;
		DescriptionLabel.Text = popup.Description;
		
		foreach (var buttonRequest in popup.Buttons)
		{
			var button = new Button();
			button.Text = buttonRequest.Title;
			button.Pressed += buttonRequest.Action;
			ButtonsContainer.AddChild(button);
		}
		CurrentPopupRequest = popup;
		Visible = true;
	}

	private void HidePopup()
	{
		Visible = false;
		Clear();
		TryShowNextPopup();
	}
	
	private void Clear()
	{
		if (EventBus.PublishIsCancelled(new PopupClearingEvent(this)))
			return;
		
		CurrentPopupRequest = null;
		TitleLabel.Text = "";
		DescriptionLabel.Text = "";
		foreach (var button in ButtonsContainer.GetChildren())
		{
			button.QueueFree();
		}
	}
}