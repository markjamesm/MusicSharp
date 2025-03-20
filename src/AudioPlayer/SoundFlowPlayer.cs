using System;
using MusicSharp.Enums;
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

    public EPlayerStatus PlayerState => GetPlayerStateMapper();

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
        
        if (_player == null)
        {
            _player = new SoundPlayer(new StreamDataProvider(stream));
        }
        
        _player = new SoundPlayer(new StreamDataProvider(stream));
        
        Mixer.Master.AddComponent(_player);
        _player.Play();
    }

    public void PlayPause()
    {
        switch (PlayerState)
        {
            case EPlayerStatus.Playing:
                _player?.Pause();
                break;
            case EPlayerStatus.Paused:
            case EPlayerStatus.Stopped:
                _player?.Play();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void Stop()
    {
        if (PlayerState != EPlayerStatus.Stopped)
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

    private EPlayerStatus GetPlayerStateMapper()
    {
        return _player?.State switch
        {
            PlaybackState.Playing => EPlayerStatus.Playing,
            PlaybackState.Paused => EPlayerStatus.Paused,
            PlaybackState.Stopped => EPlayerStatus.Stopped,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}