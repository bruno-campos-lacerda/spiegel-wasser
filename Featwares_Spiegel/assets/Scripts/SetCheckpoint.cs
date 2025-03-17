using Godot;
using System;

public partial class SetCheckpoint : Area2D
{
	[Export] public Vector2 checkpoint;
	private CharacterBody2D player;
	private Movement script;
	
	public override void _Ready()
	{
		player = GetNode<CharacterBody2D>($"../player");
		script = player.GetNode<Movement>($"../player");
	}
	
	private void _on_body_entered(Node2D body)
	{
		if(body == player)
		{
			script.SetCheckpoint(checkpoint);
		}
	}
}
