using Godot;
using System;

public partial class UiManager : Node3D
{

    [Export] public ProgressBar HealthBar;
    [Export] public Player PlayerNode;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        PlayerNode.HealthChanged += UpdateHealthBar;
        UpdateHealthBar(100, 100);
    }

    private void UpdateHealthBar(float current, float max)
    {
        HealthBar.MaxValue = max;
        HealthBar.Value = current;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
