// <copyright file="WinPlayer.cs" company="Mark-James McDougall">
// Licensed under the GNU GPL v3 License. See LICENSE in the project root for license information.
// </copyright>
/*
using MusicSharp.Enums;
using System;
using System.IO;
using NAudio.Wave;

namespace MusicSharp.SoundEngines;

/// <summary>
/// The audio player implementation for Windows using NAudio.
/// </summary>
public class WinPlayer : ISoundEngine
{
    
    private readonly WaveOutEvent _outputDevice;
    private AudioFileReader _audioFileReader;

    /// <summary>
    /// Initializes a new instance of the <see cref="WinPlayer"/> class.
    /// </summary>
    public WinPlayer()
    {
        _outputDevice = new WaveOutEvent();
        _outputDevice.PlaybackStopped += OnPlaybackStopped;
    }

    /// <inheritdoc/>
    public ePlayerStatus PlayerStatus { get; set; } = ePlayerStatus.Stopped;

    /// <inheritdoc/>
    public string LastFileOpened { get; set; }

    /// <inheritdoc/>
    public void Stop()
    {
        _outputDevice.Stop();
        PlayerStatus = ePlayerStatus.Stopped;
    }

    /// <summary>
    /// Opens an audio file and then plays it.
    /// </summary>
    /// <param name="path">The filepath.</param>
    public void OpenFile(string path)
    {
        var isFileValid = File.Exists(path);

        if (isFileValid)
        {
            try
            {
                _audioFileReader = new AudioFileReader(path);
                _outputDevice.Init(_audioFileReader);
                _outputDevice.Play();
                PlayerStatus = ePlayerStatus.Playing;   
            }
            catch (FileNotFoundException)
            {
            }   
        }

        // Space for error message, should one be wanted/needed.
    }

    /// <summary>
    /// Method to play and pause audio playback depending on PlaybackState.
    /// </summary>
    public void PlayPause()
    {
        if (_outputDevice.PlaybackState == PlaybackState.Stopped)
        {
            _outputDevice.Play();
            PlayerStatus = ePlayerStatus.Playing;
            return;
        }
        if (_outputDevice.PlaybackState == PlaybackState.Paused)
        {
            _outputDevice.Play();
            PlayerStatus = ePlayerStatus.Playing;
            return;
        }
        if (_outputDevice.PlaybackState == PlaybackState.Playing)
        {
            _outputDevice.Pause();
            PlayerStatus = ePlayerStatus.Paused;
        }
    }

    /// <inheritdoc/>
    public void PlayFromPlaylist(string path)
    {
        if (_outputDevice != null)
        {
            _outputDevice.Dispose();

            try
            {
                _audioFileReader = new AudioFileReader(path);
                _outputDevice.Init(_audioFileReader);
                _outputDevice.Play();
                PlayerStatus = ePlayerStatus.Playing;
            }
            catch (FileNotFoundException)
            {
            }
        }
    }

    /// <summary>
    /// Dispose of our device once playback is stopped.
    /// </summary>
    /// <param name="sender">The object sender.</param>
    /// <param name="args">The StoppedEventArgs.</param>
    public void OnPlaybackStopped(object sender, StoppedEventArgs args)
    {
        if (_audioFileReader is not null)
        {
            _audioFileReader.Dispose();
        }

        _outputDevice.Dispose();
    }

    /// <summary>
    /// Method to increase audio playback volume.
    /// </summary>
    public void IncreaseVolume()
    {
        // Use this construct to prevent edge cases going over 1.0f
        // This is caused by using floats in WaveOutEvent
        if (_outputDevice.Volume > 0.9f)
        {
            _outputDevice.Volume = 1.0f;
            return;
        }

        _outputDevice.Volume += 0.1f;
    }

    /// <summary>
    /// Method to decrease audio playback volume.
    /// </summary>
    public void DecreaseVolume()
    {
        // Use this construct to prevent edge cases going under 0.0f
        // This is caused by using floats in WaveOutEvent
        if (_outputDevice.Volume < 0.1f)
        {
            _outputDevice.Volume = 0.0f;
            return;
        }

        _outputDevice.Volume -= 0.1f;
    }

    /// <summary>
    /// Method to open an audio stream.
    /// </summary>
    /// <param name="streamUrl">The URL of the stream.</param>
    public void OpenStream(string streamUrl)
    {
        try
        {
            using var mf = new MediaFoundationReader(streamUrl);

            _outputDevice.Init(mf);
            _outputDevice.Play();
        }
        catch (NullReferenceException)
        {
        }
    }

    /// <inheritdoc/>
    public TimeSpan CurrentTime()
    {
        var zeroTime = new TimeSpan(0);

        if (_outputDevice.PlaybackState != PlaybackState.Stopped)
        {
            return _audioFileReader.CurrentTime;
        }
        else
        {
            return zeroTime;
        }
    }

    /// <inheritdoc/>
    public TimeSpan TrackLength()
    {
        return _audioFileReader.TotalTime;
    }

    /// <inheritdoc/>
    public void SeekForward()
    {
        if (_audioFileReader != null && _audioFileReader.CurrentTime <= _audioFileReader.TotalTime)
        {
            _audioFileReader.CurrentTime = _audioFileReader.CurrentTime.Add(TimeSpan.FromSeconds(5));
        }
    }

    /// <inheritdoc/>
    public void SeekBackwards()
    {
        if (_audioFileReader != null && _audioFileReader.CurrentTime >= TimeSpan.FromSeconds(5))
        {
            _audioFileReader.CurrentTime = _audioFileReader.CurrentTime.Subtract(TimeSpan.FromSeconds(5));
        }
    }
}
*/