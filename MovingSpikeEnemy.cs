using Godot;

[Tool]
public class MovingSpikeEnemy : Path2D
{
    public enum AnimationType
    {
        Loop,
        Bounce
    }

    private AnimationPlayer animationPlayer;
    private AnimationType _type;

    [Export]
    public AnimationType animationType
    {
        get => _type;
        set
        {
            _type = value;

            // Play updated animation only when in editor mode. At runtime
            // animationType won't be modified once this node is ready and 
            // _Ready() will take care of running the correct animations then.
            if (Engine.EditorHint)
            {
                if (animationPlayer == null)
                {
                    animationPlayer = FindNode("AnimationPlayer") as AnimationPlayer;
                }
                PlayUpdatedAnimation();
            }
        }
    }

    private int _speed = 1;

    [Export]
    public int Speed
    {
        get => _speed;
        set
        {
            _speed = value;

            // Play updated animation only when in editor mode. At runtime
            // animationType won't be modified once this node is ready and 
            // _Ready() will take care of running the correct animations then.
            if (Engine.EditorHint)
            {
                if (animationPlayer == null)
                {
                    animationPlayer = FindNode("AnimationPlayer") as AnimationPlayer;
                }
                UpdateSpeed();
            }
        }
    }

    public override void _Ready()
    {
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        PlayUpdatedAnimation();
        UpdateSpeed();
    }

    void PlayUpdatedAnimation()
    {
        switch (animationType)
        {
            case AnimationType.Loop:
                animationPlayer.Play("MoveAlongPathLoop");
                break;
            case AnimationType.Bounce:
                animationPlayer.Play("MoveAlongPathBounce");
                break;
        }
    }

    private void UpdateSpeed()
    {
        animationPlayer.PlaybackSpeed = _speed;
    }
}
