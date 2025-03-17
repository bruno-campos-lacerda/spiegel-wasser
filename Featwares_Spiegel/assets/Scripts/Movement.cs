using Godot;
using System;

public partial class Movement : CharacterBody2D
{
	private Vector2 direction;
	private Vector2 dashDirection;
	private Vector2 velocity;
	private Vector2 scale;
	[Export] public Vector2 initialPoint;
	
	[Export] public float horizontalSpeed { get; set; } = 150.0f;
	[Export] public float verticalSpeed { get; set; } = 150.0f;
	[Export] public float verticalForce { get; set; } = -300.0f;
	[Export] public float dashForce { get; set; } = 400.0f;
	[Export] public int stamina;
	[Export] public float gravityForce {get; set; } = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	
	private float timeAwait = 1.0f;
	private float deltaTime;
	private float dashTime;
	private float wallJumpTime; 
	private float airFloat;
	private float coyotTime;
	private float wallCoyotTime;
	private bool canDash;
	private bool isDead;
	private bool isOnFloor;
	private bool isOnWall;
	private bool isOnAir;
	private bool isOnScaleSurface;
	private bool canClimb;
	private bool canClimbJump;
	private bool isDashing;
	private bool isTransiting;
	private bool isCharged;
	private bool longJumped;
	private bool longWallJumped;
	private bool superJumped;
	private bool spining;
	
	[Export] public Node2D cameraTarget;
	[Export] public Target setTargetPosition;
	private AnimatedSprite2D sprite;
	private AudioStreamPlayer2D jumpSound;
	private AudioStreamPlayer2D dashSound;
	private AudioStreamPlayer2D deathSound;
	
	private Control staminaLabel;
	private HudManager hudManager;
	
	public override void _Ready()
	{
		stamina = 100;
		airFloat = 1.5f;
		isDead = false;
		spining = false;
		staminaLabel = GetNode<Control>($"../Canvas/Interface");
		hudManager = staminaLabel.GetNode<HudManager>($"../Interface");
		sprite = GetNode<AnimatedSprite2D>($"AnimatedSprite2D");
		jumpSound = GetNode<AudioStreamPlayer2D>($"jump");
		dashSound = GetNode<AudioStreamPlayer2D>($"dash");
		deathSound = GetNode<AudioStreamPlayer2D>($"death");
		scale = sprite.Scale;
	}
	
	public override void _Process(double delta)
	{
		deltaTime = (float)delta;
		isCharged = Input.IsActionPressed("powerType");
		
		direction = Input.GetVector("leftInput", "rightInput", "upInput", "downInput");
		float turnDirection = Input.GetAxis("leftInput", "rightInput");
		sprite.Scale = direction.X != 0.0f ? new Vector2(scale.X * turnDirection, scale.Y) : sprite.Scale;
		velocity = Velocity;
		
		if(Input.IsActionJustPressed("dashInput") && direction != Vector2.Zero && !isDashing && canDash)
		{
			canDash = false;
			canClimb = false;
			velocity = Vector2.Zero;
			dashDirection = direction.Normalized();
			dashTime = 0.2f;
		}
		isDashing = dashTime > 0.0f;
		isOnFloor = IsOnFloor();
		isOnAir = !IsOnFloor();
		isOnWall = IsOnWall() && (direction.X != 0.0f || Input.IsActionPressed("powerType") || Input.IsActionPressed("upInput")) && stamina > 0.0f;
		canClimbJump = true;
		if(isOnScaleSurface)
		{
			if(Input.IsActionJustPressed("jumpInput") || Input.IsActionJustPressed("upInput"))
			{
				if(canClimb == false && stamina > 0.0f && !isOnFloor)
				{
					canClimb = true;
					canClimbJump = false;
				}
			}
		}
		stamina = stamina < 0 ? 0 : stamina;
		hudManager.Write(stamina);
		
		setTargetPosition.SetActualPositionZero(false);
		coyotTime = !isOnFloor && coyotTime > 0.0f ? coyotTime - deltaTime : coyotTime;
		wallCoyotTime = !isOnWall && wallCoyotTime > 0.0f ? wallCoyotTime - deltaTime : wallCoyotTime;
		StyleManager(isDead, isDashing, isOnFloor, canClimb, isOnWall, isOnAir);
		
		if(!isTransiting)
		{
			Velocity = velocity;
		}
		else
		{
			Velocity = Vector2.Zero;
		}
		MoveAndSlide();
	}
	
	private void StyleManager(bool dead, bool dashing, bool ground, bool scaleSurface, bool wall, bool air)
	{
		if(!isTransiting)
		{
			if(dead)
			{
				Dead();
			}
			else if(dashing)
			{
				DashStyle();
			}
			else if(ground)
			{
				GroundStyle();
			}
			else if(scaleSurface)
			{
				ClimbingStyle();
			}
			else if(wall)
			{
				WallStyle();
			}
			else if (air)
			{
				AirStyle();
			}
		}
	}
	
	private void Dead()
	{
		if(!deathSound.Playing)
		{
			deathSound.Play();
		}
		sprite.Animation = "Death";
		velocity = Vector2.Zero;
		if(!sprite.IsPlaying())
		{
			sprite.Animation = "Idle";
			Position = initialPoint;
			stamina = 100;
			isDead = false;
			longJumped = false;
			superJumped = false;
			longWallJumped = false;
			dashTime = 0.0f;
		}
	}
	private void DashStyle()
	{
		if(!dashSound.Playing)
		{
			dashSound.Play();
		}
		if(direction.Y < 0.0f)
		{
			sprite.Animation = "UpDash";
		}
		else if(direction.Y > 0.0f)
		{
			sprite.Animation = "DownDash";
		}
		else
		{
			sprite.Animation = "HorizontalDash";
		}
		dashTime -= deltaTime;
		longJumped = false;
		superJumped = false;
		longWallJumped = false;
		airFloat = 1.5f;
		if(dashTime > 0.0f)
		{
			velocity = dashDirection * dashForce;
		}
		else
		{
			velocity = Vector2.Zero;
		}
	}
	private void GroundStyle()
	{
		coyotTime = 0.15f;
		airFloat = 1.5f;
		stamina = stamina < 100 && !isCharged ? stamina + StaminaManager(200.0f) : stamina;
		canDash = true;
		longJumped = false;
		superJumped = false;
		longWallJumped = false;
		if(velocity.X != 0.0f)
		{
			sprite.Play("Run");
		}
		else
		{
			float looking = Input.GetAxis("upInput", "downInput");
			if(isCharged || looking == 1.0f)
			{
				sprite.Animation = "CrounchStart";
			}
			else if(looking == -1.0f)
			{
				sprite.Animation = "LookingUp";
			}
			else
			{
				sprite.Play("Idle");
			}
		}
		HorizontalInput();
		JumpManager();
	}
	private void WallStyle()
	{
		wallCoyotTime = 0.15f;
		if(isCharged)
		{
			float wallRunInput = Input.GetAxis("upInput", "downInput");
			velocity.Y = (verticalSpeed / 2.0f) * wallRunInput;
			if(wallRunInput < 0.0f)
			{
				sprite.Play("Climbing");
				stamina -= StaminaManager(72.0f);
			}
		}
		else
		{
			velocity.Y += (gravityForce * deltaTime) * 3.0f;
			velocity.Y = velocity.Y >= (gravityForce / 20.0f) ? (gravityForce / 20.0f) : velocity.Y;
			stamina -= StaminaManager(7.0f);
		}
		longJumped = false;
		superJumped = false;
		longWallJumped = false;
		airFloat = 1.5f;
		if(velocity.Y >= 0.0f)
		{
			sprite.Play("ClimbingPause");
		}
		HorizontalInput();
		JumpManager();
	}
	private void ClimbingStyle()
	{
		direction = direction.Normalized();
		if(isCharged)
		{
			velocity = direction * horizontalSpeed;
		}
		else
		{
			velocity = direction * horizontalSpeed / 2.0f;
		}
		longJumped = false;
		superJumped = false;
		longWallJumped = false;
		airFloat = 1.5f;
		if(velocity != Vector2.Zero)
		{
			sprite.Play("ClimbingWall");
			if(isCharged)
			{
				stamina -= StaminaManager(25.0f);
			}
			else
			{
				stamina -= StaminaManager(15.0f);
			}
		}
		else
		{
			sprite.Play("ClimbingWallPause");
			stamina = stamina < 100 ? stamina + StaminaManager(2.5f) : stamina;
		}
		if(Input.IsActionJustPressed("jumpInput") && canClimbJump)
		{
			canClimb = false;
			velocity.Y = verticalForce;
		}
		else if(stamina <= 0)
		{
			canClimb = false;
		}
	}
	private void AirStyle()
	{
		
		velocity.Y = airFloat >= 0.25f ? velocity.Y + gravityForce * deltaTime : velocity.Y;
		velocity.Y = velocity.Y >= 300.0f ? 300.0f : velocity.Y;
		if(Input.IsActionJustPressed("jumpInput") && !(velocity.Y < 0.0f && superJumped))
		{
			if(airFloat >= 0.5f && stamina > 0 && coyotTime <= 0.0f && wallCoyotTime <= 0.0f)
			{
				jumpSound.Play();
				stamina -= 5;
				airFloat = 0.0f;
				velocity.Y = 0.0f;
				sprite.Animation = "Spin";
				spining = true;
			}
		}
		airFloat = airFloat <= 0.75f ? airFloat + deltaTime : airFloat;
		if(velocity.Y <= 0.0f && !spining)
		{
			sprite.Animation = "Jumping";
		}
		else if(velocity.Y > 0.0f)
		{
			sprite.Play("Falling");
			spining = false;
		}
		JumpManager();
		HorizontalInput();
	}
	
	private void HorizontalInput()
	{
		if(direction.X != 0.0f)
		{
			float speed = horizontalSpeed;
			speed = isCharged && stamina > 0 && isOnFloor ? speed * 1.5f : speed;
			speed = longJumped ? speed * 2.1f : speed;
			speed = longWallJumped ? speed * 1.8f : speed;
			speed = superJumped ? speed * 0.5f : speed;

			stamina = isCharged ? stamina - StaminaManager(3.5f) : stamina;

			if(wallJumpTime <= 0.0f)
			{
				velocity.X += (direction.X * speed) * deltaTime * 7.5f;
			}
			else
			{
				wallJumpTime -= deltaTime;
				velocity.X -= (direction.X * speed) * deltaTime * 10.0f;
			}
			velocity.X = velocity.X >= speed ? speed : velocity.X;
			velocity.X = velocity.X <= -speed ? -speed : velocity.X;
			setTargetPosition.SetActualPositionZero(true);
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, horizontalSpeed / 2.0f);
			if(isOnFloor)
			{
				setTargetPosition.SetActualPosition();
			}
			else
			{
				setTargetPosition.SetActualPositionZero(true);
			}
		}
	}
	private void JumpManager()
	{
		if(Input.IsActionJustPressed("jumpInput"))
		{
			if(isOnFloor || coyotTime > 0.0f)
			{
				jumpSound.Play();
				if(isCharged && stamina > 0)
				{
					if(velocity.X != 0.0f)
					{
						velocity.Y = verticalForce * 0.65f;
						longJumped = true;
					}
					else
					{
						velocity.Y = verticalForce * 1.5f;
						superJumped = true;
					}
					stamina -= 8;
				}
				else
				{
					velocity.Y = verticalForce;
				}
				coyotTime = 0.0f;
			}
			else if(isOnWall || (wallCoyotTime > 0.0f && wallCoyotTime < 0.15f))
			{
				jumpSound.Play();
				float jumpDirection = direction.X;
				jumpDirection = isOnWall ? jumpDirection * -1.0f : jumpDirection;
				wallJumpTime = 0.05f;
				velocity.X = jumpDirection * horizontalSpeed;
				if(jumpDirection != 0.0f)
				{
					if(isCharged)
					{
						longWallJumped = true;
						velocity.Y = verticalForce * 0.65f;
						stamina -= 20;
					}
					else
					{
						velocity.Y = verticalForce;
						stamina -= 12;
					}
				}
			}
		}
	}
	
	private int StaminaManager(float val)
	{
		timeAwait -= deltaTime * val;
		if(timeAwait <= 0.0f)
		{
			timeAwait = 1.0f;
			return 1;
		}
		return 0;
	}
	
	public void SetScaleStyleTrue()
	{
		isOnScaleSurface = true;
	}
	public void SetScaleStyleFalse()
	{
		isOnScaleSurface = false;
		canClimb = false;
	}
	public void SetCanDashTrue()
	{
		canDash = true;
	}
	public void SetTransition(bool transition)
	{
		isTransiting = transition;
	}
	public void SetCheckpoint(Vector2 newInitialPoint)
	{
		initialPoint = newInitialPoint;
	}
	public void Restart()
	{
		isDead = true;
	}
}
