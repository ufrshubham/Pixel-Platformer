using Godot;
using System;

public class SoundPlayer : Node
{
    public enum SoundEffect
    {
        Hurt,
        Jump
    }

    private AudioStreamPlayer _audioStreamPlayer;
    private Node _audioPlayers;
    private AudioStream _hurt;
    private AudioStream _jump;

    public override void _Ready()
    {
        _hurt = ResourceLoader.Load<AudioStream>("res://sounds/Hurt.wav");
        _jump = ResourceLoader.Load<AudioStream>("res://sounds/Jump.wav");

        _audioPlayers = GetNode<Node>("AudioPlayers");
        _audioStreamPlayer = GetNode<AudioStreamPlayer>("AudioPlayers/AudioStreamPlayer");
    }

    public void PlaySound(SoundEffect soundEffect)
    {
        AudioStream stream = GetAudioStreamFor(soundEffect);

        if (stream != null)
        {
            foreach (AudioStreamPlayer audioStreamPlayer in _audioPlayers.GetChildren())
            {
                if (!audioStreamPlayer.Playing)
                {
                    audioStreamPlayer.Stream = stream;
                    audioStreamPlayer.Play();
                    break;
                }
            }
        }
    }

    private AudioStream GetAudioStreamFor(SoundEffect soundEffect)
    {
        switch (soundEffect)
        {
            case SoundEffect.Hurt:
                return _hurt;
            case SoundEffect.Jump:
                return _jump;
            default:
                return null;
        }
    }
}
