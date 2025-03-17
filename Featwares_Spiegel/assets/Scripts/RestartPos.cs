using Godot;
using System;

public partial class RestartPos : Area2D
{
	private CharacterBody2D player;
	private Movement playerScript;
	
	public override void _Ready()
	{
		player = GetNode<CharacterBody2D>($"../player");
		playerScript = player.GetNode<Movement>($"../player");
	}
	
	private void _on_body_entered(Node2D body)
	{
		if(body == player)
		{
			playerScript.Restart();
		}
	}
}
