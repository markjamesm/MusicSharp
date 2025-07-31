using System.Collections.Generic;
using System.Linq;
using ATL.Playlist;
using MusicSharp.Data;

namespace MusicSharp.Helpers;

public static class PlaylistHelpers
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

    public static void SavePlaylistToFile(string playlistFilePath, List<AudioFile> filesForPlaylist)
    {
        var pls = PlaylistIOFactory.GetInstance().GetPlaylistIO(playlistFilePath);
        
        foreach (var file in filesForPlaylist)
        {
            pls.FilePaths.Add(file.Path);    
        }
        
        pls.Save();
    }
}