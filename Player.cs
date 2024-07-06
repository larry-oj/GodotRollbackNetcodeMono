using Godot;
using Godot.Collections;
using GodotRollbackNetcode;

namespace GodotRollbackPredictionMono;

public partial class Player : Node2D, IGetLocalInput, INetworkProcess, INetworkSerializable
{
	[Export(hintString: "Speed in pixels per tick")] private float _speed = 5;
	
	public Dictionary _GetLocalInput()
	{
		var inputVector = Input.GetVector("player_left", "player_right", "player_up", "player_down");

		return new Dictionary
		{
			{"input_vector", inputVector}
		};
	}
	
	public void _NetworkProcess(Dictionary input)
	{
		var success = input.TryGetValue("input_vector", out var inputVector);
		if (success)	
			Position += (Vector2)inputVector * _speed;
	}
	
	public Dictionary _SaveState()
	{
		return new Dictionary
		{
			{"position", Position}
		};
	}
	
	public void _LoadState(Dictionary state)
	{
		var success = state.TryGetValue("position", out var position);
		if (success)
			Position = (Vector2)position;
	}
}	