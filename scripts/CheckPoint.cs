using Godot;

public class CheckPoint : Area2D
{
    private AnimatedSprite _animatedSprite;
    private Events _events;

    public override void _Ready()
    {
        _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        _events = GetNode<Events>("/root/Events");
    }

    private void OnBodyEntered(Node body)
    {
        if (body is Player)
        {
            if (_animatedSprite.Animation != "Checked")
            {
                _animatedSprite.Play("Checked");
                _events.EmitSignal("HitCheckPoint", Position);
            }
        }
    }
}
