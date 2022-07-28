using Godot;

// For Matf.MoveToward: https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/c_sharp_differences.html#tips
using static Godot.Mathf;

public class Player : KinematicBody2D
{
    enum State
    {
        MOVE, CLIMB
    }

    [Export] private PlayerMovementData playerMovementData;

    private State _state = State.MOVE;
    private Vector2 _velocity = Vector2.Zero;
    private int _doubleJump = 1;

    private AnimatedSprite _animatedSprite;
    private RayCast2D _ladderCheck;

    public override void _Ready()
    {
        _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        // _animatedSprite.Frames = ResourceLoader.Load<SpriteFrames>("res://PlayerBlueSkin.tres");
        _ladderCheck = GetNode<RayCast2D>("LadderCheck");
    }

    public override void _PhysicsProcess(float delta)
    {
        Vector2 input = Vector2.Zero;
        input.x = Input.GetAxis("ui_left", "ui_right");
        input.y = Input.GetAxis("ui_up", "ui_down");

        switch (_state)
        {
            case State.MOVE:
                MoveState(input);
                break;
            case State.CLIMB:
                ClimbState(input);
                break;
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

    private bool IsOnLadder()
    {
        if (!_ladderCheck.IsColliding())
        {
            return false;
        }

        var collider = _ladderCheck.GetCollider();
        if (!(collider is Ladder))
        {
            return false;
        }

        return true;
    }

    void MoveState(Vector2 input)
    {
        if (IsOnLadder() && Input.IsActionPressed("ui_up"))
        {
            _state = State.CLIMB;
        }

        ApplyGravity();

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
            _doubleJump = playerMovementData.doubleJumpCount;

            if (Input.IsActionJustPressed("ui_up"))
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

            if (Input.IsActionJustPressed("ui_up") && _doubleJump > 0)
            {
                _velocity.y = playerMovementData.jumpForce;
                _doubleJump -= 1;
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

    void ClimbState(Vector2 input)
    {
        if (!IsOnLadder())
        {
            _state = State.MOVE;
        }

        if (input.Length() != 0)
        {
            _animatedSprite.Animation = "Run";
        }
        else
        {
            _animatedSprite.Animation = "Idle";
        }

        _velocity = input * playerMovementData.climbSpeed;
        _velocity = MoveAndSlide(_velocity, Vector2.Up);
    }
}
