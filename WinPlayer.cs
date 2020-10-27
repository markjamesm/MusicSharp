// <copyright file="WinPlayer.cs" company="Mark-James McDougall">
// Licensed under the GNU GPL v3 License. See LICENSE in the project root for license information.
// </copyright>

namespace MusicSharp
{
    using System.Threading;
    using NAudio.Wave;

    /// <summary>
    /// The Player class handles audio playback.
    /// </summary>
    public class WinPlayer : IPlayer
    {
        /// <summary>
        /// Method that implements audio playback from a file.
        /// </summary>
        public void PlayAudioFile()
        {
            var file = @"C:\MusicSharp\example.mp3";

            // Load the audio file and select an output device.
            using var audioFile = new AudioFileReader(file);
            using var outputDevice = new WaveOutEvent();
            {
                outputDevice.Init(audioFile);
                outputDevice.Play();

                // Sleep until playback is finished.
                while (outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    Thread.Sleep(1000);
                }
            }
        }
    }
}