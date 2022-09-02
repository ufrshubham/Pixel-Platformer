using Godot;
using System;

public class Transitions : CanvasLayer
{
    private AnimationPlayer _animationPlayer;
    [Signal] delegate void TransitionCompleted();

    public override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public void PlayEnterTransition()
    {
        _animationPlayer.Play("EnterLevel");
    }

    public void PlayExitTransition()
    {
        _animationPlayer.Play("ExitLevel");
    }

    public void OnAnimationPlayerAnimationFinished(String animationName)
    {
        EmitSignal("TransitionCompleted");
    }
}
