using System;
using SoundFlow.Enums;

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
    /// Returns the total length of the audio file.
    /// </summary>
    float TrackLength { get; }
    
    /// <summary>
    /// Returns the current time played.
    /// </summary>
    float CurrentTime { get; }
    
    /// <summary>
    /// Indicates whether an audio stream is loaded.
    /// </summary>
    bool IsStreamLoaded { get; }

    /// <summary>
    /// Method to play audio.
    /// </summary>
    /// <param name="path">The path to the file or stream.</param>
    void Play(string path);
    
    /// <summary>
    /// Method to pause audio playback.
    /// </summary>
    void PlayPause();
    
    /// <summary>
    /// Method to stop audio playback.
    /// </summary>
    void Stop();

    /// <summary>
    /// Method to increase audio playback volume.
    /// </summary>
    void IncreaseVolume();

    /// <summary>
    /// Method to decrease audio playback volume.
    /// </summary>
    void DecreaseVolume();

    /// <summary>
    /// Skip ahead in the audio file 5s.
    /// </summary>
    public void SeekForward();

    /// <summary>
    /// Skip back in the audio file 5s.
    /// </summary>
    public void SeekBackward();
}