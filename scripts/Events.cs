using Godot;

public class Events : Node
{
    [Signal] delegate void PlayerDied();
    [Signal] delegate void HitCheckPoint(Vector2 checkPointPosition);
}
