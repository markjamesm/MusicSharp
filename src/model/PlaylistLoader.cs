// <copyright file="PlaylistLoader.cs" company="Mark-James McDougall">
// Licensed under the GNU GPL v3 License. See LICENSE in the project root for license information.
// </copyright>
namespace MusicSharp
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using PlaylistsNET.Content;
    using PlaylistsNET.Models;

    /// <summary>
    /// The PlaylistLoader class loads a playlist of a given type.
    /// </summary>
    public class PlaylistLoader
    {
        /// <summary>
        /// This will be used in the future to allow for playlist types beyond M3U.
        /// </summary>
        // public virtual void LoadPlaylist() { }

        /// <returns>Returns a list of playlist information.</returns>
        public List<string> LoadPlaylist(string userPlaylist)
        {
            M3uContent content = new M3uContent();

            // convert string to stream
            byte[] byteArray = Encoding.UTF8.GetBytes(userPlaylist);
            MemoryStream stream = new MemoryStream(byteArray);

            // M3uContent playlist = userPlaylist;

            var playlist = content.GetFromStream(stream);

            return playlist.GetTracksPaths();
        }

    }
}
