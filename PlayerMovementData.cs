using Godot;

public class PlayerMovementData : Resource
{
    [Export] public int jumpForce = -160;
    [Export] public int jumpReleaseForce = -40;
    [Export] public int maxSpeed = 75;
    [Export] public int acceleration = 10;
    [Export] public int friction = 10;
    [Export] public int gravity = 5;
    [Export] public int additionalFallGravity = 2;
    [Export] public int maxGravity = 300;
    [Export] public int climbSpeed = 50;
    [Export] public int doubleJumpCount = 1;
}
