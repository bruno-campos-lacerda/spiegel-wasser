using Godot;
using System;

public partial class SetCamPos : Area2D
{
	[Export] public int leftLimit;
	[Export] public int topLimit;
	[Export] public int rightLimit;
	[Export] public int bottomLimit;
	
	[Export] public float horizontalSpeed;
	[Export] public float verticalSpeed;
	
	private CharacterBody2D player;
	private Camera2D camera;
	private Camera cameraScript;
	
	public override void _Ready()
	{
		player = GetNode<CharacterBody2D>($"../player");
		camera = GetNode<Camera2D>($"../playerCamera");
		cameraScript = camera.GetNode<Camera>($"../playerCamera");
	}
	
	private void _on_body_entered(Node2D body)
	{
		if(body == player)
		{
			cameraScript.SetBorderLimit(leftLimit, topLimit, rightLimit, bottomLimit);
			cameraScript.SetCameraSpeed(horizontalSpeed, verticalSpeed);
			cameraScript.TransitionOnOff(true);
		}
	}
}
