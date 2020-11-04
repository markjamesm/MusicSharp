// <copyright file="WinPlayer.cs" company="Mark-James McDougall">
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

        /// <inheritdoc/>
        public void PlayStream(string streamURL)
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
    }
}