using Godot;
using System;

public class StompEnemy : Node2D
{
    enum State
    {
        HOVER,
        FALL,
        LAND,
        RISE
    }

    private State _state = State.HOVER;
    private Vector2 _startPosition;
    private Timer _timer;
    private RayCast2D _raycast;
    private AnimatedSprite _animatedSprite;
    private Particles2D _particles;

    public override void _Ready()
    {
        _startPosition = GlobalPosition;
        _timer = GetNode<Timer>("Timer");
        _raycast = GetNode<RayCast2D>("RayCast2D");
        _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        _particles = GetNode<Particles2D>("Particles2D");
        _particles.OneShot = true;
    }

    public override void _PhysicsProcess(float delta)
    {
        switch (_state)
        {
            case State.HOVER:
                HoverState();
                break;
            case State.FALL:
                FallState(delta);
                break;
            case State.LAND:
                LandState();
                break;
            case State.RISE:
                RiseState(delta);
                break;
        }
    }

    private void HoverState()
    {
        _state = State.FALL;
    }

    private void FallState(float delta)
    {
        _animatedSprite.Play("Falling");
        Position += new Vector2(0, 60) * delta;

        if (_raycast.IsColliding())
        {
            Vector2 collisionPoint = _raycast.GetCollisionPoint();
            Position = new Vector2(Position.x, collisionPoint.y);
            _state = State.LAND;
            _timer.Start(1);
            _particles.Emitting = true;
        }
    }

    private void LandState()
    {
        if (_timer.TimeLeft == 0)
        {
            _state = State.RISE;
        }
    }

    private void RiseState(float delta)
    {
        _animatedSprite.Play("Rising");
        Position = new Vector2(Position.x, Mathf.MoveToward(Position.y, _startPosition.y, 20 * delta));
        if (Position.y == _startPosition.y)
        {
            _state = State.HOVER;
        }
    }
}
