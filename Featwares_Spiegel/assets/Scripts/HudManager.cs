using Godot;
using System;

public partial class HudManager : Control
{
	[Export] public ProgressBar stamina; 
	
	public void Write(int number)
	{
		//stamina.Text = number.ToString();
		stamina.Value = number;
	}
}
