﻿// <copyright file="WinPlayer.cs" company="Mark-James McDougall">
// Licensed under the GNU GPL v3 License. See LICENSE in the project root for license information.
// </copyright>

namespace MusicSharp
{
    using NAudio.Wave;

    /// <summary>
    /// The audio player implementation for Windows using NAudio.
    /// </summary>
    public class WinPlayer : IPlayer
    {
        private readonly WaveOutEvent outputDevice;
        private AudioFileReader audioFileReader;

        /// <summary>
        /// Initializes a new instance of the <see cref="WinPlayer"/> class.
        /// </summary>
        public WinPlayer()
        {
            this.outputDevice = new WaveOutEvent();
            this.outputDevice.PlaybackStopped += this.OnPlaybackStopped;
        }

        /// <inheritdoc/>
        public string LastFileOpened { get; set; }

        /// <inheritdoc/>
        public void Stop()
        {
            this.outputDevice.Stop();
        }

        /// <inheritdoc/>
        public void Play(string path)
        {
            if (this.outputDevice.PlaybackState == PlaybackState.Stopped)
            {
                this.audioFileReader = new AudioFileReader(path);
                this.outputDevice.Init(this.audioFileReader);
            }

            this.outputDevice.Play();
        }

        /// <inheritdoc/>
        public void Pause()
        {
            this.outputDevice.Pause();
        }

        /// <summary>
        /// Dispose of our device once playback is stopped.
        /// </summary>
        /// <param name="sender">The object sender.</param>
        /// <param name="args">The StoppedEventArgs.</param>
        public void OnPlaybackStopped(object sender, StoppedEventArgs args)
        {
            this.audioFileReader.Dispose();
            this.outputDevice.Dispose();
        }
    }
}
