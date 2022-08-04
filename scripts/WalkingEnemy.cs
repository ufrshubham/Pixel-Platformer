using Godot;
using System;

public class WalkingEnemy : KinematicBody2D
{
    private Vector2 _direction = Vector2.Right;
    private Vector2 _velocity = Vector2.Zero;
    private RayCast2D _ledgeCheck;
    private AnimatedSprite _animatedSprite;

    public override void _Ready()
    {
        _ledgeCheck = GetNode<RayCast2D>("LedgeCheck");
        _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
    }

    public override void _PhysicsProcess(float delta)
    {
        bool foundWall = IsOnWall();
        bool foundLedge = !_ledgeCheck.IsColliding();

        if (foundWall || foundLedge)
        {
            _direction *= -1;
            _ledgeCheck.Position = new Vector2(-_ledgeCheck.Position.x, _ledgeCheck.Position.y);
        }

        _animatedSprite.FlipH = _direction.x > 0;

        _velocity = _direction * 25;
        _velocity = MoveAndSlide(_velocity, Vector2.Up);
    }
}
