// <copyright file="WinPlayer.cs" company="Mark-James McDougall">
// Licensed under the GNU GPL v3 License. See LICENSE in the project root for license information.
// </copyright>

namespace MusicSharp
{
    using System.Threading;
    using System.Threading.Tasks;
    using NAudio.Wave;

    /// <summary>
    /// The Player class handles audio playback.
    /// </summary>
    public class WinPlayer : IPlayer
    {
        private static Mutex mut = new Mutex();

        private WaveOutEvent outputDevice = new WaveOutEvent();

        /// <summary>
        /// Method that implements audio playback from a file.
        /// </summary>
        /// <param name="filepath">The Path to the audio file to play.</param>
        public void PlayAudioFile(string filepath)
        {
            // Play the audio inside a new thread to prevent our GUI from blocking.
            Task.Run(() =>
            {
                // Block this section of the thread so only one audio file plays at once.
                mut.WaitOne();

                try
                {
                    // Load the audio file and select an output device.
                    using var audioFile = new AudioFileReader(filepath);
                    {
                        this.outputDevice.Init(audioFile);
                        this.outputDevice.Play();

                        // Sleep until playback is finished.
                        while (this.outputDevice.PlaybackState == PlaybackState.Playing)
                        {
                            Thread.Sleep(1000);
                        }
                    }
                }

                // Throw an exception if the file isn't found.
                catch (System.IO.FileNotFoundException e)
                {
                    System.Console.WriteLine("Error", e);
                }

                // Throw an exception if the file isn't playable.
                catch (System.Runtime.InteropServices.COMException e)
                {
                    System.Console.WriteLine("Error", e);
                }

                // Throw an exception if the user cancels the dialog without choosing a file.
                catch (System.ArgumentException e)
                {
                    System.Console.WriteLine("Error", e);
                }

                // Release the thread.
                mut.ReleaseMutex();
            });
        }

        /// <summary>
        /// Method to stop playing audio.
        /// </summary>
        public void Stop()
        {
            this.outputDevice.Stop();
        }
    }
}