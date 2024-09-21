using Godot;
using System;
using Launcher;

public partial class ServerStatusPanel : PanelContainer
{
	[Export] HBoxContainer StatusContainer;
	[Export] HBoxContainer VersionContainer;
	[Export] HBoxContainer PingContainer;
	[Export] HBoxContainer PlayersCountContainer;

	[Export] Panel DividerPanel;
	[Export] VBoxContainer SideContainer;
	[Export] VBoxContainer PlayersContainer;

	[Export] Label StatusLabel;
	[Export] Label VersionLabel;
	[Export] Label PingLabel;
	[Export] Label PlayersCountLabel;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		PlayersCountContainer.MouseEntered += () =>
		{
			// Show empty list if received players list is empty
			var players = ServerInfo.PlayerList ?? Array.Empty<string>();
				
			DividerPanel.Visible = true;
			SideContainer.Visible = true;

			foreach (var playerName in players)
			{
				var label = new Label();
				label.Text = playerName;
				PlayersContainer.AddChild(label);
			}
		};

		PlayersCountContainer.MouseExited += () =>
		{
			DividerPanel.Visible = false;
			SideContainer.Visible = false;

			foreach (var child in PlayersContainer.GetChildren())
			{
				child.QueueFree();
			}
		};
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!ServerInfo.IsReady)
		{
			Hide();
			StatusLabel.Text = "Проверяется...";
			StatusLabel.Modulate = Colors.Yellow;
		}
		else
		{
			if (!ServerInfo.IsOnline)
			{
				Hide();
				StatusLabel.Text = "ОФФЛАЙН";
				StatusLabel.Modulate = Colors.Red;
			}
			else
			{
				Show();
				StatusLabel.Text = "ОНЛАЙН";
				StatusLabel.Modulate = Colors.Green;
				VersionLabel.Text = ServerInfo.Version;
				PingLabel.Text = $"{ServerInfo.Latency} ms";
				PlayersCountLabel.Text = $"{ServerInfo.CurrentPlayers}/{ServerInfo.MaximumPlayers}";
			}
		}
	}


	private void Hide()
	{
		VersionContainer.Visible = false;
		PingContainer.Visible = false;
		PlayersCountLabel.Visible = false;
		
		DividerPanel.Visible = false;
		SideContainer.Visible = false;
	}

	private void Show()
	{
		VersionContainer.Visible = true;
		PingContainer.Visible = true;
		PlayersCountLabel.Visible = true;
	}
}
