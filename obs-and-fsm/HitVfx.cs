using Godot;
using System;

public partial class HitVfx : Sprite3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        // deletes itself after a moment
        GetTree().CreateTimer(0.5).Timeout += () => QueueFree();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
