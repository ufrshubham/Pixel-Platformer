using Godot;

public class Hitbox : Area2D
{
    public void OnBodyEntered(Node body)
    {
        if (body is Player)
        {
            GetTree().ReloadCurrentScene();
        }
    }
}
