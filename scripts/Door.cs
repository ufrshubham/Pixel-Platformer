using Godot;
using System;

public class Door : Area2D
{
    [Export(PropertyHint.File, "*.tscn")]
    private String _targetLevelPath;

    private Transitions _transitions;

    public override void _Ready()
    {
        _transitions = GetNode<Transitions>("/root/Transitions");
    }

    private async void OnBodyEntered(Node body)
    {
        if (!(body is Player)) return;
        if (_targetLevelPath == null) return;

        _transitions.PlayExitTransition();
        GetTree().Paused = true;

        await ToSignal(_transitions, "TransitionCompleted");

        _transitions.PlayEnterTransition();
        GetTree().Paused = false;

        GetTree().ChangeScene(_targetLevelPath);
    }
}
