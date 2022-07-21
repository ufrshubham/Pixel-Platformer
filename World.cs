using Godot;
using System;

public class World : Node2D
{
    public override void _Ready()
    {
        VisualServer.SetDefaultClearColor(Colors.LightBlue);
    }
}
