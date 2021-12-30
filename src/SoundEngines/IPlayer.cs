// <copyright file="IPlayer.cs" company="Mark-James McDougall">
// Licensed under the GNU GPL v3 License. See LICENSE in the project root for license information.
// </copyright>

using MusicSharp.Enums;

namespace MusicSharp.SoundEngines;

/// <summary>
/// Defines the methods an audio player class should implement.
/// </summary>
public interface IPlayer
{
    /// <summary>
    /// Gets or sets a value indicating whether the audio player is playing.
    /// </summary>
    ePlayerStatus PlayerStatus { get; set; }

    /// <summary>
    /// Gets or sets the last file opened by the player.
    /// </summary>
    string LastFileOpened { get; set; }

    /// <summary>
    /// Method to play audio.
    /// </summary>
    /// <param name="path">The filepath of the audio file to play.</param>
    void OpenFile(string path);

    /// <summary>
    /// Method to play an audio stream from a URL.
    /// </summary>
    /// <param name="streamUrl">The stream URL of the audio file to play.</param>
    void OpenStream(string streamUrl);

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
    /// Play an audio file contained in a playlist.
    /// </summary>
    /// <param name="path">The path to the audio file.</param>
    void PlayFromPlaylist(string path);

    /// <summary>
    /// Returns the current playtime of the audioFileReader instance.
    /// </summary>
    /// <returns>The current time played as TimeSpan.</returns>
    System.TimeSpan CurrentTime();

    /// <summary>
    /// Returns the total track length in timespan format.
    /// </summary>
    /// <returns>The length of the track in timespan format.</returns>
    System.TimeSpan TrackLength();

    /// <summary>
    /// Skip ahead in the audio file 5s.
    /// </summary>
    public void SeekForward();

    /// <summary>
    /// Skip back in the audio file 5s.
    /// </summary>
    public void SeekBackwards();
}