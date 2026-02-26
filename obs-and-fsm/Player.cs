using Godot;
using JourneyCameraPrototype;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

public partial class Player : CharacterBody3D, ICameraController
{
    // Player movement parameters
    public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;
    public float Acceleration = 15.0f;
    public float Friction = 20.0f;

    // Player health parameters
    private float _health = 100.0f;
    public float MaxHealth = 100.0f;

    // This allows you to link your CameraController in the editor
    [Export] public Node3D CameraPivot;

    // This event can be used to update the UI when the player's health changes
    [Signal] public delegate void HealthChangedEventHandler(float current, float max);

    public void ChangeToCinematic()
    {
        throw new NotImplementedException();
    }

    public void ChangeToPlayer()
    {
        throw new NotImplementedException();
    }

    public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

        // Handle Jump.
        if (Input.IsActionJustPressed("Jump") && IsOnFloor())
        {
            velocity.Y = JumpVelocity;
            TakeDamage(10); // Example: Take damage when jumping (for testing purposes)
        }

        // Get Joystick Input (-1.0 to 1.0)
        float inputX = Input.GetJoyAxis(0, JoyAxis.LeftX);
        float inputY = Input.GetJoyAxis(0, JoyAxis.LeftY);
        Vector2 input = new Vector2(inputX, inputY);

        // Apply Deadzone (Prevents "stick drift")
        if (input.Length() < 0.2f)
        {
            input = Vector2.Zero;
        }
        // Convert Input to World Direction
        Vector3 direction = Vector3.Zero;

        if (CameraPivot != null)
        {
            Basis camBasis = CameraPivot.GlobalTransform.Basis;
            // Calculate movement relative to Camera rotation
            Vector3 forward = -camBasis.Z;
            Vector3 right = camBasis.X;

            // Flatten vectors so the player doesn't move into the ground
            forward.Y = 0;
            right.Y = 0;
            forward = forward.Normalized();
            right = right.Normalized();

            direction = (forward * -input.Y) + (right * input.X);
        }
        else
        {
            // Fallback: Move along World Axes if no camera is linked
            direction = new Vector3(input.X, 0, input.Y);
        }

        // Apply Acceleration and Friction
        if (direction != Vector3.Zero)
        {
            velocity.X = Mathf.MoveToward(velocity.X, direction.X * Speed, Acceleration * (float)delta);
            velocity.Z = Mathf.MoveToward(velocity.Z, direction.Z * Speed, Acceleration * (float)delta);
        }
        else
        {
            velocity.X = Mathf.MoveToward(velocity.X, 0, Friction * (float)delta);
            velocity.Z = Mathf.MoveToward(velocity.Z, 0, Friction * (float)delta);
        }


        Velocity = velocity;
		MoveAndSlide();
	}

    public void TakeDamage(float amount)
    {
        // Subtract damage and clamp so it doesn't go below 0
        _health = Mathf.Clamp(_health - amount, 0, MaxHealth);

        // 4. Emit the signal (The "Shout")
        // This sends the current numbers to anyone listening (the UI Manager)
        EmitSignal(SignalName.HealthChanged, _health, MaxHealth);

    }
}
