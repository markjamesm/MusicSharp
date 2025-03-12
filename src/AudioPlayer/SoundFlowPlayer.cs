using System;
using System.IO;
using MusicSharp.Enums;
using SoundFlow.Backends.MiniAudio;
using SoundFlow.Components;
using SoundFlow.Providers;

namespace MusicSharp.AudioPlayer;

// Cross-platform sound engine that works for all devices which
//  the .NET platform runs on.
public sealed class SoundFlowPlayer : IPlayer, IDisposable
{
    private readonly MiniAudioEngine _soundEngine;
    private SoundPlayer _player;

    public EPlayerStatus PlayerStatus { get; set; }
    public string LastFileOpened { get; set; }


    public SoundFlowPlayer(MiniAudioEngine soundEngine)
    {
        _soundEngine = soundEngine;
    }

    public void Play(object path, EFileType fileType)
    {
        if (_player != null)
        {
            _player.Stop();
        }

        switch (fileType)
        {
            case EFileType.File:
                _player = new SoundPlayer(new StreamDataProvider(File.OpenRead((string)path)));
                break;
            
            case EFileType.Stream:
                _player = new SoundPlayer(new StreamDataProvider((Stream)path));
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }

        // Add the player to the master mixer. This connects the player's output to the audio engine's output.
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
        if (_player.Volume < 2.0f)
        {
            _player.Volume += .1f;
        }
    }

    public void DecreaseVolume()
    {
        // Ensure that the volume isn't negative
        // otherwise the player will crash
        if (_player.Volume > .1f)
        {
            _player.Volume -= .1f;
        }

        if (_player.Volume <= .1f)
        {
            _player.Volume = 0f;
        }
    }
    
    public void SeekForward()
    {
        if (_player.Time < _player.Duration - 5f)
        {
            _player.Seek(_player.Time + 5f);
        }
    }

    public void SeekBackwards()
    {
        if (_player.Time > 5f)
        {
            _player.Seek(_player.Time - 5f);
        }
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