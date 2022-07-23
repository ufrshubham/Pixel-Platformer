using Godot;

// For Matf.MoveToward: https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/c_sharp_differences.html#tips
using static Godot.Mathf;

public class Player : KinematicBody2D
{
    [Export] private PlayerMovementData playerMovementData;

    private Vector2 _velocity = Vector2.Zero;
    private AnimatedSprite _animatedSprite;

    public override void _Ready()
    {
        _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        _animatedSprite.Frames = ResourceLoader.Load<SpriteFrames>("res://PlayerBlueSkin.tres");
    }

    public override void _PhysicsProcess(float delta)
    {
        ApplyGravity();

        Vector2 input = Vector2.Zero;
        input.x = Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left");

        if (input.x == 0)
        {
            ApplyFriction();
            _animatedSprite.Animation = "Idle";
        }
        else
        {
            ApplyAcceleration(input.x);
            _animatedSprite.Animation = "Run";

            _animatedSprite.FlipH = input.x > 0;
        }

        if (IsOnFloor())
        {
            if (Input.IsActionPressed("ui_up"))
            {
                _velocity.y = playerMovementData.jumpForce;
            }
        }
        else
        {
            _animatedSprite.Animation = "Jump";
            if (Input.IsActionJustReleased("ui_up") && _velocity.y < playerMovementData.jumpReleaseForce)

            {
                _velocity.y = playerMovementData.jumpReleaseForce;
            }

            if (_velocity.y > 0)
            {
                _velocity.y += playerMovementData.additionalFallGravity;
            }
        }

        bool wasInAir = !IsOnFloor();
        _velocity = MoveAndSlide(_velocity, Vector2.Up);

        bool justLanded = IsOnFloor() && wasInAir;

        if (justLanded)
        {
            _animatedSprite.Animation = "Run";
            _animatedSprite.Frame = 1;
        }
    }

    private void ApplyGravity()
    {
        _velocity.y += playerMovementData.gravity;
        _velocity.y = Min(_velocity.y, playerMovementData.maxGravity);
    }

    private void ApplyFriction()
    {
        _velocity.x = MoveToward(_velocity.x, 0, playerMovementData.friction);
    }

    private void ApplyAcceleration(float amount)
    {
        _velocity.x = MoveToward(_velocity.x, playerMovementData.maxSpeed * amount, playerMovementData.acceleration);
    }
}
