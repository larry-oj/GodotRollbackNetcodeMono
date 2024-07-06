using Godot;
using GodotRollbackNetcode;

namespace GodotRollbackPredictionMono;

public partial class Main : Node2D
{
	private Control _connectionPanel;
	private LineEdit _hostField;
	private LineEdit _portField;
	private Label _messageLabel;
	private Label _syncLabel;
	
	public override void _Ready()
	{
		_connectionPanel = GetNode<Control>("%ConnectionPanel");
		_hostField = GetNode<LineEdit>("%HostField");
		_portField = GetNode<LineEdit>("%PortField");
		_messageLabel = GetNode<Label>("%MessageLabel");
		_syncLabel = GetNode<Label>("%SyncLabel");
		
		Multiplayer.PeerConnected += OnPeerConnected;
		Multiplayer.PeerDisconnected += OnPeerDisconnected;
		Multiplayer.ServerDisconnected += OnServerDisconnected;
		SyncManager.Global.SyncStarted += OnSyncStarted;
		SyncManager.Global.SyncLost += OnSyncLost;
		SyncManager.Global.SyncRegained += OnSyncRegained;
		SyncManager.Global.SyncStopped += OnSyncStopped;
		SyncManager.Global.SyncError += OnSyncError;
	}

	private void OnServerButtonPressed()
	{
		var peer = new ENetMultiplayerPeer();
		peer.CreateServer(int.Parse(_portField.Text), 1);
		Multiplayer.MultiplayerPeer = peer;
		_connectionPanel.Hide();
		_messageLabel.Text = "Listening...";
	}
	
	private void OnClientButtonPressed()
	{
		var peer = new ENetMultiplayerPeer();
		peer.CreateClient(_hostField.Text, int.Parse(_portField.Text));
		Multiplayer.MultiplayerPeer = peer;
		_connectionPanel.Hide();
		_messageLabel.Text = "Connecting...";
	}

	private async void OnPeerConnected(long id)
	{
		_messageLabel.Text = "Connected!";
		SyncManager.Global.AddPeer((int)id);
		
		GetNode("%PlayerServer").SetMultiplayerAuthority(1);
		if (Multiplayer.IsServer())
			GetNode("%PlayerClient").SetMultiplayerAuthority((int)id);
		else
			GetNode("%PlayerClient").SetMultiplayerAuthority(Multiplayer.GetUniqueId());
		
		if (Multiplayer.IsServer())
		{
			_messageLabel.Text = "Starting...";
			await ToSignal(GetTree().CreateTimer(2.0f), Timer.SignalName.Timeout);
			SyncManager.Global.Start();
		}
	}
	
	private void OnPeerDisconnected(long id)
	{
		_messageLabel.Text = "Disconnected!";
		SyncManager.Global.RemovePeer((int)id);
	}

	private void OnServerDisconnected()
	{
		OnPeerDisconnected(1);
	}

	private void OnSyncStarted()
	{
		_messageLabel.Text = "Sync started!";
	}

	private void OnSyncLost()
	{
		_syncLabel.Show();
	}
	
	private void OnSyncRegained()
	{
		_syncLabel.Hide();
	}
	
	private void OnSyncStopped()
	{
		_messageLabel.Text = "Sync stopped!";
	}
	
	private void OnSyncError(string message)
	{
		_messageLabel.Text = "Fatal sync error: " + message;
		_syncLabel.Hide();

		var peer = Multiplayer.MultiplayerPeer;
		if (peer != null)
		{
			peer.Close();
		}
		SyncManager.Global.ClearPeers();
	}
	
	private void OnResetButtonPressed()
	{
		SyncManager.Global.Stop();
		SyncManager.Global.ClearPeers();
		var peer = Multiplayer.MultiplayerPeer;
		if (peer != null)
		{
			peer.Close();
		}

		GetTree().ReloadCurrentScene();
	}
}