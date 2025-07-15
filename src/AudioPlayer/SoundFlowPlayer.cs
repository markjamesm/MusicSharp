using System;
using SoundFlow.Backends.MiniAudio;
using SoundFlow.Components;
using SoundFlow.Enums;
using SoundFlow.Providers;

namespace MusicSharp.AudioPlayer;

// Cross-platform sound engine that works for all devices that
//  the .NET platform runs on.
public sealed class SoundFlowPlayer : IPlayer
{
    private readonly MiniAudioEngine _soundEngine;
    private SoundPlayer? _player;
    private readonly IStreamConverter _streamConverter;

    // If we don't know the state of the player, default to stopped?
    public PlaybackState State => _player?.State ?? PlaybackState.Stopped;
    
    public string LastFileOpened { get; set; }
    public float TrackLength => _player?.Duration ?? 0;
    public float CurrentTime => _player?.Time ?? 0;
    public bool IsStreamLoaded => _player != null;


    public SoundFlowPlayer(MiniAudioEngine soundEngine, IStreamConverter streamConverter)
    {
        _soundEngine = soundEngine;
        _streamConverter = streamConverter;
    }
    
    public void Play(string path)
    {
        var stream = _streamConverter.ConvertFileToStream(path);
        
        // Test if this check is really necessary
        if (_player == null)
        {
            _player = new SoundPlayer(new StreamDataProvider(stream));
        }

        if (_player.State == PlaybackState.Playing )
        {
            _player.Stop();
        }
        
        _player = new SoundPlayer(new StreamDataProvider(stream));
        
        Mixer.Master.AddComponent(_player);
        _player.Play();
    }

    public void PlayPause()
    {
        switch (_player?.State)
        {
            case PlaybackState.Playing:
                _player?.Pause();
                break;
            case PlaybackState.Paused:
            case PlaybackState.Stopped:
                _player?.Play();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void Stop()
    {
        if (State != PlaybackState.Stopped)
        {
            _player?.Stop();
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

    public void SeekBackward()
    {
        _player.Seek(Math.Clamp(_player.Time - 5f, 0f, _player.Duration));
    }

    public void Dispose()
    {
        _soundEngine.Dispose();
    }
}