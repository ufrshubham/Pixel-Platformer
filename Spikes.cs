using Godot;
using System;

public class Spikes : Area2D
{
    public void OnBodyEntered(Node body)
    {
        if (body is Player)
        {
            GetTree().ReloadCurrentScene();
        }
    }
}
