using Godot;
using System;
using System.Threading.Tasks;

public class Door : Area2D
{
    [Export(PropertyHint.File, "*.tscn")]
    private String _targetLevelPath;

    private Transitions _transitions;
    private Player _player;

    public override void _Ready()
    {
        _transitions = GetNode<Transitions>("/root/Transitions");
    }

    public override void _Process(float delta)
    {
        if (_player == null) return;
        if (!_player.IsOnFloor()) return;

        if (Input.IsActionJustPressed("ui_up"))
        {
            if (_targetLevelPath == null) return;
            Task task = GoToNextRoom();
        }
    }

    private void OnBodyEntered(Node body)
    {
        if (!(body is Player)) return;

        _player = body as Player;
        _player.OnDoor = true;
        if (_targetLevelPath == null) return;
    }

    private void OnDoorBodyExited(Node body)
    {
        if (!(body is Player)) return;
        _player.OnDoor = false;
        _player = null;
    }

    private async Task GoToNextRoom()
    {
        _transitions.PlayExitTransition();
        GetTree().Paused = true;

        await ToSignal(_transitions, "TransitionCompleted");

        _transitions.PlayEnterTransition();
        GetTree().Paused = false;

        GetTree().ChangeScene(_targetLevelPath);
    }
}
