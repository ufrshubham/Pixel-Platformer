using Godot;
using System;

// For Matf.MoveToward: https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/c_sharp_differences.html#tips
using static Godot.Mathf;

public class Player : KinematicBody2D
{
    [Export] private int _jumpForce = -130;
    [Export] private int _jumpReleaseForce = -70;
    [Export] private int _maxSpeed = 50;
    [Export] private int _acceleration = 10;
    [Export] private int _friction = 10;
    [Export] private int _gravity = 4;
    [Export] private int _additionalFallGravity = 4;
    [Export] private int _maxGravity = 300;

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
                _velocity.y = _jumpForce;
            }
        }
        else
        {
            _animatedSprite.Animation = "Jump";
            if (Input.IsActionJustReleased("ui_up") && _velocity.y < _jumpReleaseForce)

            {
                _velocity.y = _jumpReleaseForce;
            }

            if (_velocity.y > 0)
            {
                _velocity.y += _additionalFallGravity;
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
        _velocity.y += _gravity;
        _velocity.y = Min(_velocity.y, _maxGravity);
    }

    private void ApplyFriction()
    {
        _velocity.x = MoveToward(_velocity.x, 0, _friction);
    }

    private void ApplyAcceleration(float amount)
    {
        _velocity.x = MoveToward(_velocity.x, _maxSpeed * amount, _acceleration);
    }
}
