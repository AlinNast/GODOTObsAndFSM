using Godot;
using System;

public partial class VfxManager : Node3D
{
    [Export] public PackedScene SplatScene; // Drag your Splat scene here
    [Export] public Player PlayerNode;
    
	// Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        if (PlayerNode != null)
        {
            PlayerNode.HealthChanged += OnPlayerHealthChanged;
        }
    }

    private void OnPlayerHealthChanged(float currentHealth, float maxHealth)
    {
        SpawnSplat();
    }

    private void SpawnSplat()
    {
        // 1. Create an instance of the splat scene
        var splat = SplatScene.Instantiate<Sprite3D>();

        // 2. Add it to the world (not as a child of the player, 
        // so it doesn't move with the player after spawning)
        AddChild(splat);

        // 3. Position it on the player's head
        // We use GlobalPosition so it stays in the world spot where they got hit
        splat.GlobalPosition = PlayerNode.GlobalPosition + new Vector3(0, 2.0f, 0);

        // 4. Optional: Randomize rotation slightly for variety
        splat.RotationDegrees = new Vector3(0, 0, (float)GD.RandRange(0, 360));
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
