using System;
using System.IO;
using MusicSharp.Enums;
using Gst;

namespace MusicSharp.SoundEngines;

/// <summary>
/// The audio player implementation for Linux based systems using GStreamer.
/// </summary>
public class LinuxPlayer : IPlayer
{
    public ePlayerStatus PlayerStatus { get; set; }
    public string LastFileOpened { get; set; }

    private readonly Element _playbin;
    private readonly Pipeline _pipeline;
    private double _currentVolume = 1.0; // Start at 100% volume
    private const double VolumeStep = 0.1; // 10% volume change per step

    public LinuxPlayer()
    {
        Application.Init();

        _playbin = ElementFactory.Make("playbin", "player");
        _pipeline = new Pipeline("pipeline");
        _pipeline.Add(_playbin);
    }

    /// <inheritdoc/>
    public void OpenFile(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("File not found", path);
        }

        var uri = new System.Uri(new FileInfo(path).FullName).ToString();
        PlayUri(uri);
    }

    private void PlayUri(string uri)
    {
        _pipeline.SetState(State.Null);
        _playbin["uri"] = uri;

        var stateChangeReturn = _pipeline.SetState(State.Playing);
        if (stateChangeReturn is StateChangeReturn.Failure)
        {
            _pipeline.SetState(State.Null);
            throw new InvalidOperationException("Failed to play the media");
        }

        LastFileOpened = uri;
        PlayerStatus = ePlayerStatus.Playing;
    }

    /// <inheritdoc/>
    public void OpenStream(string streamUrl)
    {
        PlayUri(streamUrl);
    }

    /// <inheritdoc/>
    public void PlayPause()
    {
        switch (_pipeline.CurrentState)
        {
            case State.Playing:
                _pipeline.SetState(State.Paused);
                break;

            case State.Paused:
            case State.Null:
            case State.VoidPending:
            case State.Ready:
                _pipeline.SetState(State.Playing);
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <inheritdoc/>
    public void Stop()
    {
        _pipeline.SetState(State.Null);
        _playbin["uri"] = string.Empty;
        PlayerStatus = ePlayerStatus.Stopped;
    }

    /// <inheritdoc/>
    public void IncreaseVolume()
    {
        _currentVolume = Math.Min(_currentVolume + VolumeStep, 1.0);
        _playbin["volume"] = _currentVolume;
    }

    /// <inheritdoc/>
    public void DecreaseVolume()
    {
        _currentVolume = Math.Max(_currentVolume - VolumeStep, 0.0);
        _playbin["volume"] = _currentVolume;
    }

    /// <inheritdoc/>
    public void PlayFromPlaylist(string path)
    {
        // Watching the WinPlayer implementation of this method I get that it is a placeholder for the future ?
        // I am forwarding the call to the OpenFile method to avoid putting a NotImplementedException and crashing the app.
        OpenFile(path);
    }

    /// <inheritdoc/>
    public TimeSpan CurrentTime()
    {
        if (_pipeline is null || PlayerStatus is ePlayerStatus.Stopped)
        {
            return TimeSpan.Zero;
        }

        // Query the current position in nanoseconds
        _pipeline.QueryPosition(Format.Time, out var position);

        // Convert from nanoseconds to TimeSpan
        // GStreamer uses nanoseconds (1 second = 1,000,000,000 nanoseconds)
        return TimeSpan.FromMilliseconds(position / 1_000_000.0);
    }

    /// <inheritdoc/>
    public TimeSpan TrackLength()
    {
        if (_pipeline is null || PlayerStatus is ePlayerStatus.Stopped)
        {
            return TimeSpan.Zero;
        }

        // Query the duration in nanoseconds
        _pipeline.QueryDuration(Format.Time, out var duration);

        // Convert from nanoseconds to TimeSpan
        // GStreamer uses nanoseconds (1 second = 1,000,000,000 nanoseconds)
        return TimeSpan.FromMilliseconds(duration / 1_000_000.0);
    }

    /// <inheritdoc/>
    public void SeekForward()
    {
        SeekTime(true);
    }

    /// <inheritdoc/>
    public void SeekBackwards()
    {
        SeekTime(false);
    }

    private void SeekTime(bool isForward)
    {
        if (_pipeline is null || PlayerStatus is ePlayerStatus.Stopped || IsStream())
        {
            return;
        }

        _pipeline.QueryPosition(Format.Time, out var position);
        _pipeline.QueryDuration(Format.Time, out var duration);

        long newPosition;

        if (isForward)
        {
            newPosition = position + (5 * 1_000_000_000L);
            newPosition = Math.Min(newPosition, duration);
        }
        else
        {
            newPosition = position - (5 * 1_000_000_000L);
            newPosition = Math.Max(newPosition, 0);
        }

        _pipeline.SeekSimple(
            Format.Time,
            SeekFlags.Flush | SeekFlags.KeyUnit,
            newPosition
        );
    }

    private bool IsStream()
    {
        return LastFileOpened.StartsWith("http") || LastFileOpened.StartsWith("https");
    }
}