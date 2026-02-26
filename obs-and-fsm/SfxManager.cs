using Godot;
using System;

public partial class SfxManager : Node3D
{
    [Export] public Player PlayerNode;
    [Export] public AudioStreamPlayer HitSoundPlayer;

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
        PlayHitSound();
    }

    private void PlayHitSound()
    {
        HitSoundPlayer.Play();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
