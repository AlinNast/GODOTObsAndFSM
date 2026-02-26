using Godot;
using System;

public partial class Enemy : CharacterBody3D
{
    [Export] public Area3D DamageArea;
    [Export] public Area3D HeadArea;

    [Export] public Node3D Blades;

    [Export] public Player TargetPlayer; 

    public const float Speed = 3.0f;
    private float rotationSpeed = 3.0f;

    public enum EnemyState
    {
        Idle,
        Chase,
        Waiting,
        Dead
    }

    private EnemyState _currentState = EnemyState.Idle;

    public override void _Ready()
    {
        DamageArea.BodyEntered += OnDamageAreaBodyEntered;
        HeadArea.BodyEntered += OnHeadAreaBodyEntered;
        ChangeState(EnemyState.Idle);
    }

    public override void _PhysicsProcess(double delta)
	{
        // Switch between states
        switch(_currentState)
        {
            case EnemyState.Idle:
                HandleIdleState();
                break;
            case EnemyState.Chase:
                HandleChaseState();
                break;
            case EnemyState.Waiting:
                HandleWaitingState();
                break;
            case EnemyState.Dead:
                HandleDeadState();
                break;
        }
        if (TargetPlayer == null) return;

        RotateY(rotationSpeed * (float)delta);

        MoveAndSlide();
    }

    private void OnDamageAreaBodyEntered(Node3D body)
    {
        if (body is Player player)
        {
            player.TakeDamage(10); // Example damage value
            ChangeState(EnemyState.Waiting); // Transition to waiting state after attacking
        }
    }

    private void OnHeadAreaBodyEntered(Node3D body)
    {
        if (body is Player player)
        {
            GD.Print("Enemy head hit!"); // For testing purposes, you can replace this with actual logic (e.g., take damage, play sound, etc.)
            player.Velocity = new Vector3(player.Velocity.X, 10.0f, player.Velocity.Z);
            ChangeState(EnemyState.Dead); 
        }
    }

    private void HandleIdleState()
    {
        if (TargetPlayer == null) return;
        float distanceToPlayer = GlobalPosition.DistanceTo(TargetPlayer.GlobalPosition);
        if (distanceToPlayer < 10.0f) 
        {
            ChangeState(EnemyState.Chase);
        }
        Velocity = Vector3.Zero; // Stay still in idle state
    }

    private void HandleChaseState()
    {
        if (TargetPlayer == null) return;
        float distanceToPlayer = GlobalPosition.DistanceTo(TargetPlayer.GlobalPosition);
        if (distanceToPlayer > 15.0f) 
        {
            ChangeState(EnemyState.Idle);
        }
        // Calculate the vector pointing from Enemy to Player
        Vector3 playerPos = TargetPlayer.GlobalPosition;
        Vector3 currentPos = GlobalPosition;

        // zero out the Y axis 
        playerPos.Y = currentPos.Y;

        float distance = currentPos.DistanceTo(playerPos);

        // Calculate direction and velocity
        Vector3 direction = (playerPos - currentPos).Normalized();
        Velocity = direction * Speed;
    }

    private void HandleWaitingState()
    {
        Velocity = Vector3.Zero;
    }

    private void HandleDeadState()
    {
        Velocity = Vector3.Zero;

    }

    public void ChangeState(EnemyState newState)
    {
        // Don't allow transitions if already dead
        if (_currentState == EnemyState.Dead) return;

        _currentState = newState;

        if (_currentState == EnemyState.Dead)
        {
            Scale = new Vector3(1.0f, 0.5f, 1.0f);
            Blades.Visible = true;
            rotationSpeed = 0.0f; // Stop rotating when dead
            DamageArea.BodyEntered -= OnDamageAreaBodyEntered;
            HeadArea.BodyEntered -= OnHeadAreaBodyEntered;
            return; // No further logic needed when entering Dead state
        }

        // Logic that happens "On Enter" a state
        if (_currentState == EnemyState.Waiting)
        {
            rotationSpeed = 5.0f;
            StartWaitingTimer();
        }

        if(_currentState == EnemyState.Idle)
        {
            Blades.Visible = false;
            rotationSpeed = 3.0f;
        }

        if(_currentState == EnemyState.Chase)
        {
            Blades.Visible = true;
            rotationSpeed = 7.0f;
        }
    }
    private async void StartWaitingTimer()
    {
        // Start a one-shot timer for 3 seconds
        await ToSignal(GetTree().CreateTimer(3.0), SceneTreeTimer.SignalName.Timeout);

        // After 3 seconds, if we are still in Waiting, go back to Idle
        if (_currentState == EnemyState.Waiting)
        {
            ChangeState(EnemyState.Idle);
        }
    }
}
