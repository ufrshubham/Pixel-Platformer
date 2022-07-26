using Godot;

// For Matf.MoveToward: https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/c_sharp_differences.html#tips
using static Godot.Mathf;

public class Player : KinematicBody2D
{
    enum State
    {
        MOVE,
        CLIMB
    }

    [Export] private PlayerMovementData playerMovementData;

    private State _state = State.MOVE;
    private Vector2 _velocity = Vector2.Zero;
    private int _doubleJump = 1;
    private bool _bufferedJump = false;
    private bool _coyoteJump = false;
    public bool OnDoor = false;

    private AnimatedSprite _animatedSprite;
    private RayCast2D _ladderCheck;
    private Timer _jumpBufferTimer;
    private Timer _coyoteJumpTimer;
    private RemoteTransform2D _remoteTransform2D;
    private SoundPlayer _soundPlayer;
    private Events _events;

    public override void _Ready()
    {
        _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        _ladderCheck = GetNode<RayCast2D>("LadderCheck");
        _jumpBufferTimer = GetNode<Timer>("JumpBufferTimer");
        _coyoteJumpTimer = GetNode<Timer>("CoyoteJumpTimer");
        _remoteTransform2D = GetNode<RemoteTransform2D>("RemoteTransform2D");

        _soundPlayer = GetNode<SoundPlayer>("/root/SoundPlayer");
        _events = GetNode<Events>("/root/Events");

        // _animatedSprite.Frames = ResourceLoader.Load<SpriteFrames>("res://PlayerBlueSkin.tres");
    }

    public override void _PhysicsProcess(float delta)
    {
        Vector2 input = Vector2.Zero;
        input.x = Input.GetAxis("ui_left", "ui_right");
        input.y = Input.GetAxis("ui_up", "ui_down");

        switch (_state)
        {
            case State.MOVE:
                MoveState(input, delta);
                break;
            case State.CLIMB:
                ClimbState(input);
                break;
        }
    }

    public void PlayerDie()
    {
        _soundPlayer.PlaySound(SoundPlayer.SoundEffect.Hurt);
        QueueFree();
        _events.EmitSignal("PlayerDied");
    }

    public void ConnectCamera(Camera2D camera2D)
    {
        NodePath cameraPath = camera2D.GetPath();
        _remoteTransform2D.RemotePath = cameraPath;
    }

    private void ApplyGravity(float delta)
    {
        _velocity.y += playerMovementData.gravity * delta;
        _velocity.y = Min(_velocity.y, playerMovementData.maxGravity);
    }

    private void ApplyFriction(float delta)
    {
        _velocity.x = MoveToward(_velocity.x, 0, playerMovementData.friction * delta);
    }

    private void ApplyAcceleration(float amount, float delta)
    {
        _velocity.x = MoveToward(_velocity.x, playerMovementData.maxSpeed * amount, playerMovementData.acceleration * delta);
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

    private void MoveState(Vector2 input, float delta)
    {
        if (IsOnLadder() && Input.IsActionJustPressed("ui_up"))
        {
            _state = State.CLIMB;
        }

        ApplyGravity(delta);

        if (HorizontalMove(input))
        {
            ApplyAcceleration(input.x, delta);
            _animatedSprite.Animation = "Run";

            _animatedSprite.FlipH = input.x > 0;
        }
        else
        {
            ApplyFriction(delta);
            _animatedSprite.Animation = "Idle";
        }

        if (IsOnFloor())
        {
            ResetDoubleJump();
        }
        else
        {
            _animatedSprite.Animation = "Jump";
        }

        if (!IsOnLadder())
        {
            if (CanJump())
            {
                InputJump();
            }
            else
            {
                InputJumpRelease();
                InputDoubleJump();
                BufferJump();
                FastFall(delta);
            }
        }

        bool wasInAir = !IsOnFloor();
        bool wasOnFloor = IsOnFloor();

        _velocity = MoveAndSlide(_velocity, Vector2.Up);

        bool justLanded = IsOnFloor() && wasInAir;
        bool justLeftGround = !IsOnFloor() && wasOnFloor;

        if (justLanded)
        {
            _animatedSprite.Animation = "Run";
            _animatedSprite.Frame = 1;
        }

        if (justLeftGround && _velocity.y >= 0)
        {
            _coyoteJump = true;
            _coyoteJumpTimer.Start();
        }
    }

    private void ClimbState(Vector2 input)
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

    private bool HorizontalMove(Vector2 input)
    {
        return input.x != 0;
    }

    private bool CanJump()
    {
        return IsOnFloor() || _coyoteJump;
    }

    private void ResetDoubleJump()
    {
        _doubleJump = playerMovementData.doubleJumpCount;
    }

    private void InputJump()
    {
        if (OnDoor) return;
        if (Input.IsActionJustPressed("ui_up") || _bufferedJump)
        {
            _soundPlayer.PlaySound(SoundPlayer.SoundEffect.Jump);
            _velocity.y = playerMovementData.jumpForce;
            _bufferedJump = false;
        }
    }

    private void InputJumpRelease()
    {
        if (Input.IsActionJustReleased("ui_up") && _velocity.y < playerMovementData.jumpReleaseForce)
        {
            _velocity.y = playerMovementData.jumpReleaseForce;
        }
    }

    private void InputDoubleJump()
    {
        if (Input.IsActionJustPressed("ui_up") && _doubleJump > 0)
        {
            _soundPlayer.PlaySound(SoundPlayer.SoundEffect.Jump);
            _velocity.y = playerMovementData.jumpForce;
            _doubleJump -= 1;
        }
    }

    private void BufferJump()
    {
        if (Input.IsActionJustPressed("ui_up"))
        {
            _bufferedJump = true;
            _jumpBufferTimer.Start();
        }
    }

    private void FastFall(float delta)
    {
        if (_velocity.y > 0)
        {
            _velocity.y += playerMovementData.additionalFallGravity * delta;
        }
    }

    private void OnBufferJumpTimerTimeout()
    {
        _bufferedJump = false;
    }

    private void OnCoyoteJumpTimerTimeout()
    {
        _coyoteJump = false;
    }
}
