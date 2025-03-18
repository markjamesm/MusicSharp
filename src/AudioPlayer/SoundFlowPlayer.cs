using System;
using System.IO;
using MusicSharp.Enums;
using SoundFlow.Backends.MiniAudio;
using SoundFlow.Components;
using SoundFlow.Providers;

namespace MusicSharp.AudioPlayer;

// Cross-platform sound engine that works for all devices which
//  the .NET platform runs on.
public sealed class SoundFlowPlayer : IPlayer
{
    private readonly MiniAudioEngine _soundEngine;
    private SoundPlayer _player;

    public EPlayerStatus PlayerStatus { get; set; }
    public string LastFileOpened { get; set; }


    public SoundFlowPlayer(MiniAudioEngine soundEngine)
    {
        _soundEngine = soundEngine;
    }

    public void Play(Stream stream)
    {
        if (_player != null)
        {
            _player.Stop();
        }

        _player = new SoundPlayer(new StreamDataProvider(stream));
        Mixer.Master.AddComponent(_player);
        _player.Play();
        
        PlayerStatus = EPlayerStatus.Playing;
    }

    public void PlayPause()
    {
        switch (PlayerStatus)
        {
            case EPlayerStatus.Playing:
                _player.Pause();
                PlayerStatus = EPlayerStatus.Paused;
                break;
            case EPlayerStatus.Paused:
            case EPlayerStatus.Stopped:
                _player.Play();
                PlayerStatus = EPlayerStatus.Playing;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void Stop()
    {
        if (PlayerStatus != EPlayerStatus.Stopped)
        {
            _player.Stop();
            PlayerStatus = EPlayerStatus.Stopped;
        }
    }

    public void IncreaseVolume()
    {
        // Need to verify what SoundFlow's max volume level is
        // For now this should be enough based on testing
        _player.Volume = Math.Clamp(_player.Volume + .1f, 0f, 2f);
    }

    public void DecreaseVolume()
    {
        _player.Volume = Math.Clamp(_player.Volume - .1f, 0f, 2f);
    }
    
    public void SeekForward()
    {
        _player.Seek(Math.Clamp(_player.Time + 5f, 0f, _player.Duration - 0.1f));
    }

    public void SeekBackwards()
    {
        _player.Seek(Math.Clamp(_player.Time - 5f, 0f, _player.Duration));
    }
    
    public float CurrentTime()
    {
        return _player.Time;
    }

    public float TrackLength()
    {
        return _player.Duration;
    }

    public void Dispose()
    {
        _soundEngine.Dispose();
    }
}