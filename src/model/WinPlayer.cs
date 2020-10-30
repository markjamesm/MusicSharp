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
        private WaveOutEvent outputDevice;
        private AudioFileReader audioFile;

        /// <inheritdoc/>
        public string LastFileOpened { get; set; }

        /// <inheritdoc/>
        public WaveOutEvent OutputDevice
        {
            get
            {
                return this.outputDevice;
            }

            set
            {
                this.outputDevice = this.OutputDevice;
            }
        }

        /// <inheritdoc/>
        public AudioFileReader AudioFile
        {
            get
            {
                return this.audioFile;
            }

            set
            {
                this.audioFile = this.AudioFile;
            }
        }

        /// <inheritdoc/>
        public void Stop()
        {
            if (this.outputDevice != null)
            {
                try
                {
                    this.outputDevice?.Stop();
                    this.outputDevice.PlaybackStopped += this.OnPlaybackStopped;
                }
                catch (System.NullReferenceException)
                {
                }
            }
        }

        /// <inheritdoc/>
        public void Play(string path)
        {
            if (this.outputDevice == null)
            {
                this.outputDevice = new WaveOutEvent();
                this.outputDevice.PlaybackStopped += this.OnPlaybackStopped;
            }

            if (this.audioFile == null)
            {
                try
                {
                    this.audioFile = new AudioFileReader(this.LastFileOpened);
                    this.outputDevice.Init(this.audioFile);
                    this.outputDevice.Play();
                }
                catch (System.Runtime.InteropServices.COMException)
                {
                }
            }

            if (this.audioFile != null)
            {
                try
                {
                    this.outputDevice.Play();
                }
                catch (System.Runtime.InteropServices.COMException)
                {
                }
            }
        }

        /// <inheritdoc/>
        public void Pause()
        {
            try
            {
                this.outputDevice?.Pause();
            }
            catch (System.NullReferenceException)
            {
            }
        }

        /// <inheritdoc/>
        // Dispose of our device and AudioFile once playback is stopped.
        // These will be changed in the future as we might want to allow
        // users to carry on playback from where they left off.
        public void OnPlaybackStopped(object sender, StoppedEventArgs args)
        {
            this.outputDevice.Dispose();
            this.outputDevice = null;

            // this.AudioFile.Dispose();

            // By resetting the AudioFile position to 0, playback can start again.
            // this.AudioFile.Position = 0;
            //   this.AudioFile = null;
        }
    }
}
