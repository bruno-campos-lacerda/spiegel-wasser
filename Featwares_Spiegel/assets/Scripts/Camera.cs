using Godot;
using System;

public partial class Camera : Camera2D
{
	private CharacterBody2D player;
	private Node2D follow;
	private Movement movement;
	
	public int leftLimit;
	public int topLimit;
	public int rightLimit;
	public int bottomLimit;
	
	private Vector2 actualPosition;
	private float horizontalSpeed;
	private float verticalSpeed;
	private bool isTransiting;
	[Export] public float transitionSpeed;
	
	private float deltaTime;
	
	public override void _Ready()
	{
		player = GetNode<CharacterBody2D>($"../player");
		follow = player.GetNode<Node2D>($"follow");
		movement = player.GetNode<Movement>($"../player");
	}
	
	public override void _Process(double delta)
	{
		deltaTime = (float)delta;
		actualPosition = Position;
		
		if(isTransiting)
		{
			Transition();
		}
		else
		{
			actualPosition.X += Direction(actualPosition.X, followPosition().X, horizontalSpeed);
			actualPosition.Y += Direction(actualPosition.Y, followPosition().Y, verticalSpeed);
			BorderLimit();
		}
		
		Position = actualPosition;
	}

	private Vector2 followPosition()
	{
		return follow.Position;
	}
	
	private float Direction(float position, float newPosition, float speed)
	{
		return (newPosition - position) * speed * deltaTime;
	}
	
	public void SetBorderLimit(int newLeft, int newTop, int newRight, int newBottom)
	{
		leftLimit = newLeft;
		topLimit = newTop;
		rightLimit = newRight;
		bottomLimit = newBottom;
	}
	
	public void SetCameraSpeed(float newHorizontalSpeed, float newVerticalSpeed)
	{
		horizontalSpeed = newHorizontalSpeed;
		verticalSpeed = newVerticalSpeed;
	}
	
	public void TransitionOnOff(bool transition)
	{
		isTransiting = transition;
	}
	
	private void BorderLimit()
	{
		actualPosition.X = actualPosition.X < leftLimit ? leftLimit : actualPosition.X;
		actualPosition.Y = actualPosition.Y < topLimit ? topLimit : actualPosition.Y;
		actualPosition.X = actualPosition.X > rightLimit ? rightLimit : actualPosition.X;
		actualPosition.Y = actualPosition.Y > bottomLimit ? bottomLimit : actualPosition.Y;
	}
	
	private void Transition()
	{
		if(actualPosition.X < leftLimit)
		{
			actualPosition.X = Mathf.MoveToward(actualPosition.X, leftLimit, deltaTime * transitionSpeed);
		}
		if(actualPosition.Y < topLimit)
		{
			actualPosition.Y = Mathf.MoveToward(actualPosition.Y, topLimit, deltaTime * transitionSpeed);
		}
		if(actualPosition.X > rightLimit)
		{
			actualPosition.X = Mathf.MoveToward(actualPosition.X, rightLimit, deltaTime * transitionSpeed);
		}
		if(actualPosition.Y > bottomLimit)
		{
			actualPosition.Y = Mathf.MoveToward(actualPosition.Y, bottomLimit, deltaTime * transitionSpeed);
		}
		
		if(actualPosition.X >= leftLimit &&
			actualPosition.Y >= topLimit &&
			actualPosition.X <= rightLimit &&
			actualPosition.Y <= bottomLimit)
		{
			isTransiting = false;
			movement.SetTransition(false);
		}
		else
		{
			movement.SetTransition(true);
		}
	}
}
