// <copyright file="IPlayer.cs" company="Mark-James McDougall">
// Licensed under the GNU GPL v3 License. See LICENSE in the project root for license information.
// </copyright>

namespace MusicSharp
{
    /// <summary>
    /// Interface for the audio playing capabilities.
    /// </summary>
    public interface IPlayer
    {
        /// <summary>
        /// Method to play audio.
        /// </summary>
        /// <param name="filepath">The Path to the audio file to play.</param>
        void PlayAudioFile(string filepath);

        /// <summary>
        /// Method to stop playing audio.
        /// </summary>
        void Stop();
    }
}