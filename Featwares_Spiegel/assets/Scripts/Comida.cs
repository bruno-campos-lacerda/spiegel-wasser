using Godot;
using System;

public partial class Comida : Area2D
{
	private CharacterBody2D player;
	private Movement movement;
	
	public override void _Ready()
	{
		player = GetNode<CharacterBody2D>($"../player");
		movement = player.GetNode<Movement>($"../player");
	}
	
	private void _on_body_entered(Node2D body)
	{
		if(body == player)
		{
			movement.SetCanDashTrue();
		}
	}
}
