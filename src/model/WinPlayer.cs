// <copyright file="WinPlayer.cs" company="Mark-James McDougall">
// Licensed under the GNU GPL v3 License. See LICENSE in the project root for license information.
// </copyright>

namespace MusicSharp
{
    using System;
    using System.IO;
    using NAudio.Wave;

    /// <summary>
    /// The audio player implementation for Windows using NAudio.
    /// </summary>
    public class WinPlayer : IPlayer
    {
        private readonly WaveOutEvent outputDevice;
        private AudioFileReader audioFileReader;

        /// <inheritdoc/>
        public PlayerStatus PlayerStatus { get; set; } = PlayerStatus.Stopped;

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
            this.PlayerStatus = PlayerStatus.Stopped;
        }

        /// <summary>
        /// Opens an audio file and then plays it.
        /// </summary>
        /// <param name="path">The filepath.</param>
        public void OpenFile(string path)
        {
            bool isFileValid = File.Exists(path);
            if (isFileValid)
            {
                this.audioFileReader = new AudioFileReader(path);
                this.outputDevice.Init(this.audioFileReader);
                this.outputDevice.Play();
                this.PlayerStatus = PlayerStatus.Playing;
            }
            else
            {
                // Space for error message, should one be wanted/needed
            }
        }

        /// <summary>
        /// Method to play and pause audio playback depending on PlaybackState.
        /// </summary>
        public void PlayPause()
        {
            if (this.outputDevice.PlaybackState == PlaybackState.Stopped)
            {
                this.outputDevice.Play();
                this.PlayerStatus = PlayerStatus.Playing;
                return;
            }
            else if (this.outputDevice.PlaybackState == PlaybackState.Paused)
            {
                this.outputDevice.Play();
                this.PlayerStatus = PlayerStatus.Playing;
                return;
            }
            else if (this.outputDevice.PlaybackState == PlaybackState.Playing)
            {
                this.outputDevice.Pause();
                this.PlayerStatus = PlayerStatus.Paused;
            }
        }

        /// <inheritdoc/>
        public void PlayFromPlaylist(string path)
        {
            if (this.outputDevice != null)
            {
                this.outputDevice.Dispose();

                try
                {
                    this.audioFileReader = new AudioFileReader(path);
                    this.outputDevice.Init(this.audioFileReader);
                    this.outputDevice.Play();
                    this.PlayerStatus = PlayerStatus.Playing;
                }
                catch (System.IO.FileNotFoundException)
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
            if (this.audioFileReader != null)
            {
                this.audioFileReader.Dispose();
            }

            this.outputDevice.Dispose();
        }

        /// <summary>
        /// Method to increase audio playback volume.
        /// </summary>
        public void IncreaseVolume()
        {
            // Use this construct to prevent edge cases going over 1.0f
            // This is caused by using floats in WaveOutEvent
            if (this.outputDevice.Volume > 0.9f)
            {
                this.outputDevice.Volume = 1.0f;
                return;
            }

            this.outputDevice.Volume += 0.1f;
        }

        /// <summary>
        /// Method to decrease audio playback volume.
        /// </summary>
        public void DecreaseVolume()
        {
            // Use this construct to prevent edge cases going under 0.0f
            // This is caused by using floats in WaveOutEvent
            if (this.outputDevice.Volume < 0.1f)
            {
                this.outputDevice.Volume = 0.0f;
                return;
            }

            this.outputDevice.Volume -= 0.1f;
        }

        /// <summary>
        /// Method to open an audio stream.
        /// </summary>
        /// <param name="streamURL">The URL of the stream.</param>
        public void OpenStream(string streamURL)
        {
            try
            {
                using (var mf = new MediaFoundationReader(streamURL))
                {
                    this.outputDevice.Init(mf);
                    this.outputDevice.Play();
                }
            }
            catch (System.ArgumentException)
            {
            }
            catch (System.IO.FileNotFoundException)
            {
            }
        }

        /// <inheritdoc/>
        public System.TimeSpan CurrentTime()
        {
            TimeSpan zeroTime = new TimeSpan(0);

            if (this.outputDevice.PlaybackState != PlaybackState.Stopped && this.outputDevice.PlaybackState != PlaybackState.Paused)
            {
                return this.audioFileReader.CurrentTime;
            }
            else
            {
                return zeroTime;
            }
        }

        /// <inheritdoc/>
        public System.TimeSpan TrackLength()
        {
            return this.audioFileReader.TotalTime;
        }

        /// <inheritdoc/>
        public void SeekForward()
        {
            if (this.audioFileReader != null && this.audioFileReader.CurrentTime <= this.audioFileReader.TotalTime)
            {
                this.audioFileReader.CurrentTime = this.audioFileReader.CurrentTime.Add(TimeSpan.FromSeconds(5));
            }
        }

        /// <inheritdoc/>
        public void SeekBackwards()
        {
            if (this.audioFileReader != null && this.audioFileReader.CurrentTime >= TimeSpan.FromSeconds(5))
            {
                this.audioFileReader.CurrentTime = this.audioFileReader.CurrentTime.Subtract(TimeSpan.FromSeconds(5));
            }
        }
    }
}