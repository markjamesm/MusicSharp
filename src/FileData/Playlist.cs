using System.Collections.Generic;
using System.Linq;
using ATL.Playlist;

namespace MusicSharp.FileData;

public static class Playlist
{
    /// <summary>
    /// Load an M3U playlist.
    /// </summary>
    /// <returns>Returns a list of playlist information.</returns>
    /// <param name="playlist">The user specified playlist path.</param>
    public static List<string> LoadPlaylist(string playlist)
    {
        var theReader = PlaylistIOFactory.GetInstance().GetPlaylistIO(playlist);

        // Fix space formatting as SoundFlow doesn't support encoded spaces
        return theReader.FilePaths.Select(s => s.Replace("%20", " ")).ToList();
    }

    public static void SavePlaylistToFile(string playlistFilePath, List<string> playlistFiles)
    {
        var pls = PlaylistIOFactory.GetInstance().GetPlaylistIO(playlistFilePath);
        pls.FilePaths = playlistFiles.ToList();
        pls.Save();
    }
}