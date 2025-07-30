using System;
using System.IO;
using System.Linq;
using MusicSharp.Enums;
using SoundFlow.Abstracts.Devices;
using SoundFlow.Backends.MiniAudio;
using SoundFlow.Components;
using SoundFlow.Enums;
using SoundFlow.Interfaces;
using SoundFlow.Providers;
using SoundFlow.Structs;

namespace MusicSharp.AudioPlayer;

public sealed class SoundFlowPlayer : IPlayer
{
    private readonly MiniAudioEngine _audioEngine;
    private readonly AudioPlaybackDevice _audioPlaybackDevice;
    private readonly AudioFormat _audioFormat;
    private ISoundDataProvider? _streamDataProvider;
    private SoundPlayer? _player;
    
    // If we don't know the state of the player, default to stopped?
    public PlaybackState State => _player?.State ?? PlaybackState.Stopped;
    public float TrackLength => _player?.Duration ?? 0;
    public float CurrentTime => _player?.Time ?? 0;
    
    public SoundFlowPlayer(MiniAudioEngine audioEngine)
    {
        _audioEngine = audioEngine;
        
        var defaultPlaybackDevice = _audioEngine.PlaybackDevices.FirstOrDefault(d => d.IsDefault);
        
        // Handle case no default playback device is found.
        if (defaultPlaybackDevice.Id == IntPtr.Zero)
        {
        }
        
        _audioFormat = new AudioFormat
        {
            Format = SampleFormat.F32,
            SampleRate = 48000,
            Channels = 2
        };
        
        _audioPlaybackDevice = _audioEngine.InitializePlaybackDevice(defaultPlaybackDevice, _audioFormat);
    }
    
    public void Play(string filepath, EFileType fileType)
    {
        if (_player != null && _player.State == PlaybackState.Playing)
        {
            _player.Stop();
            _audioPlaybackDevice.Stop();
        }

        if (fileType == EFileType.File)
        {
            _streamDataProvider = new StreamDataProvider(_audioEngine, _audioFormat, File.OpenRead(filepath));
        }

        if (fileType == EFileType.WebStream)
        {
            _streamDataProvider = new NetworkDataProvider(_audioEngine, _audioFormat, filepath);
        }

        if (_streamDataProvider != null)
        {
            _player = new SoundPlayer(_audioEngine, _audioFormat, _streamDataProvider);
            _audioPlaybackDevice.MasterMixer.AddComponent(_player);
            _audioPlaybackDevice.Start();
            _player.Play();
        }
    }

    public void PlayPause()
    {
        if (_player != null)
        {
            switch (_player.State)
            {
                case PlaybackState.Playing:
                    _player.Pause();
                    break;
                case PlaybackState.Paused:
                case PlaybackState.Stopped:
                    _player.Play();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public void Stop()
    {
        if (_player != null && _player.State != PlaybackState.Stopped)
        {
            _player?.Stop();
            _audioPlaybackDevice.Stop();
        }
    }

    public void IncreaseVolume()
    {
        if (_player !=null)
        {
            // Verify what SoundFlow's max volume level is
            // For now this should be enough based on testing
            _player.Volume = Math.Clamp(_player.Volume + .1f, 0f, 2f);
        }
    }

    public void DecreaseVolume()
    {
        if (_player != null)
        {
            _player.Volume = Math.Clamp(_player.Volume - .1f, 0f, 2f);
        }
    }
    
    public void SeekForward()
    {
        if (_streamDataProvider is StreamDataProvider && _player != null)
        {
            if (_player.State is PlaybackState.Playing or PlaybackState.Paused)
            {
                _player.Seek(Math.Clamp(_player.Time + 5f, 0f, _player.Duration - 0.1f));
            }
        }
    }

    public void SeekBackward()
    {
        if (_streamDataProvider is StreamDataProvider && _player != null)
        {
            if (_player.State is PlaybackState.Playing or PlaybackState.Paused)
            {
                _player.Seek(Math.Clamp(_player.Time - 5f, 0f, _player.Duration));
            }
        }
    }

    public void Dispose()
    {
        _audioPlaybackDevice.Dispose();
        _streamDataProvider?.Dispose();
        _player?.Dispose();
    }
}