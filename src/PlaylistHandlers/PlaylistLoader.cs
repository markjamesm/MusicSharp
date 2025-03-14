// <copyright file="PlaylistLoader.cs" company="Mark-James McDougall">
// Licensed under the GNU GPL v3 License. See LICENSE in the project root for license information.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using ATL.Playlist;

namespace MusicSharp.PlaylistHandlers;

/// <summary>
/// The PlaylistLoader class loads a playlist of a given type.
/// </summary>
public static class PlaylistLoader
{
    // This will be used in the future to allow for playlist types beyond M3U.
    // public virtual void LoadPlaylist() { }

    /// <summary>
    /// Load an M3U playlist.
    /// </summary>
    /// <returns>Returns a list of playlist information.</returns>
    /// <param name="userPlaylist">The user specified playlist path.</param>
    public static List<string> LoadPlaylist(string userPlaylist)
    {
        var theReader = PlaylistIOFactory.GetInstance().GetPlaylistIO(userPlaylist);

        // Fix space formatting as SoundFlow doesn't support encoded spaces
        return theReader.FilePaths.Select(s => s.Replace("%20", " ")).ToList();
    }
}