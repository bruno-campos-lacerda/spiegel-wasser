using Godot;
using System;

public partial class Target : Node2D
{
	private CharacterBody2D playerPosition;
	private Vector2 actualPosition;
	private Vector2 positionZero;
	private float directionFromInitialPoint;
	[Export] public float distanceFromInitialPoint;
	[Export] public float delayToMakeDistance = 1.0f;
	private float currentDelayToMakeDistance;
	private float deltaTime;

	public override void _Ready()
	{
		playerPosition = GetNode<CharacterBody2D>($"..");
		positionZero = playerPosition.Position;
		currentDelayToMakeDistance = delayToMakeDistance;
		Position = positionZero;
	}

	public override void _Process(double delta)
	{
		deltaTime = (float)delta;
		positionZero = playerPosition.Position;
		directionFromInitialPoint = Input.GetAxis("upInput", "downInput");
	}

	public void SetActualPositionZero(bool canResetDelay)
	{
		if(canResetDelay)
		{
			currentDelayToMakeDistance = delayToMakeDistance;
		}
		actualPosition = positionZero;
		Position = actualPosition;
	}

	public void SetActualPosition()
	{
		currentDelayToMakeDistance -= deltaTime;
		if(currentDelayToMakeDistance <= 0.0f)
		{
			actualPosition = new Vector2(positionZero.X, positionZero.Y + (distanceFromInitialPoint * directionFromInitialPoint));
			Position = actualPosition;
		}
		else
		{
			actualPosition = positionZero;
			Position = actualPosition;
		}
	}
}
