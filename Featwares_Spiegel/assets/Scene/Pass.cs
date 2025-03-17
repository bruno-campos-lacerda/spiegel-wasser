using Godot;
using System;

public partial class Pass : Area2D
{
	[Export] public string gameScenePath; 
	private CharacterBody2D player;
	
	public override void _Ready()
	{
		player = GetNode<CharacterBody2D>($"../player");
	}
	
	private void _on_body_entered(Node2D body)
	{
		if(body == player)
		{
			GetTree().ChangeSceneToFile(gameScenePath);
		}
	}
}
