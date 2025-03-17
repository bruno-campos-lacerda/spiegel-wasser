using Godot;
using System;

public partial class player : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;

	[Export]public float gravity {get; set; } = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y += gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("upInput") && IsOnFloor())
			velocity.Y = JumpVelocity;

		Vector2 direction = Input.GetVector("leftInput", "rightInput", "upInput", "downInput");
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
	
	private void _on_area_2d_body_entered(Node2D body)
	{
		GD.Print("Entrou");
	}
	private void _on_area_2d_body_exited(Node2D body)
	{
		GD.Print("Saiu");
	}
}
