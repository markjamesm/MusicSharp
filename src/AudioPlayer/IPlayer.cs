using System;
using MusicSharp.Data;
using SoundFlow.Enums;
using SoundFlow.Structs;

namespace MusicSharp.AudioPlayer;

/// <summary>
/// Defines the methods an audio player class should implement.
/// </summary>
public interface IPlayer: IDisposable
{
    /// <summary>
    /// Gets a value indicating whether the audio player is playing.
    /// </summary>
    PlaybackState State { get; }

    /// <summary>
    /// Gets the list of audio playback devices.
    /// </summary>
    DeviceInfo[] PlaybackDevices { get; }
    
    /// <summary>
    /// Returns the total length of the audio file.
    /// </summary>
    float TrackLength { get; }
    
    /// <summary>
    /// Returns the current time played.
    /// </summary>
    float CurrentTime { get; }
    
    /// <summary>
    /// Returns the currently playing track.
    /// </summary>
    AudioFile? NowPlaying { get; set; }

    /// <summary>
    /// Changes the current audio playback device.
    /// </summary>
    void ChangePlaybackDevice(DeviceInfo device);
    
    /// <summary>
    /// Method to play and pause audio.
    /// </summary>
    /// <param name="audioFile">The AudioFile.</param>
    void PlayPause(AudioFile audioFile);
    
    /// <summary>
    /// Method to stop audio playback.
    /// </summary>
    void Stop();

    /// <summary>
    /// Change audio volume
    /// </summary>
    void ChangeVolume(float amount);

    /// <summary>
    /// Skip ahead in the audio file 5s.
    /// </summary>
    public void SeekForward();

    /// <summary>
    /// Skip back in the audio file 5s.
    /// </summary>
    public void SeekBackward();
}