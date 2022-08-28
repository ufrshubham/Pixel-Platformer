using Godot;
using System;

public class Door : Area2D
{
    [Export(PropertyHint.File, "*.tscn")]
    String _targetLevelPath;

    private void OnBodyEntered(Node body)
    {
        if (!(body is Player)) return;
        if (_targetLevelPath == null) return;

        GetTree().ChangeScene(_targetLevelPath);
    }
}
