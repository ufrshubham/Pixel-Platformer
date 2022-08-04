using Godot;

public class World : Node2D
{
    private Camera2D _camera2D;
    private Player _player;
    private Events _events;
    private PackedScene _playerScene;
    private Timer _timer;
    private Vector2 _playerSpawnLocation;

    public override void _Ready()
    {
        VisualServer.SetDefaultClearColor(Colors.LightBlue);

        _camera2D = GetNode<Camera2D>("Camera2D");
        _player = GetNode<Player>("Player");
        _timer = GetNode<Timer>("PlayerRespawnTimer");
        _events = GetNode<Events>("/root/Events");
        _playerScene = ResourceLoader.Load<PackedScene>("res://scenes/Player.tscn");

        _playerSpawnLocation = _player.Position;
        _player.ConnectCamera(_camera2D);
        _events.Connect("PlayerDied", this, "OnPlayerDied");
        _events.Connect("HitCheckPoint", this, "OnHitCheckPoint");
    }

    public void OnPlayerDied()
    {
        _timer.Start(1.0f);
    }

    private void OnPlayerRespawnTimerTimeout()
    {
        _player = _playerScene.Instance<Player>();
        _player.Position = _playerSpawnLocation;
        AddChild(_player);

        _player.ConnectCamera(_camera2D);
    }

    private void OnHitCheckPoint(Vector2 checkPointPosition)
    {
        _playerSpawnLocation = checkPointPosition;
    }
}
