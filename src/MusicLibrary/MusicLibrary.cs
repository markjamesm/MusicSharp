using System.Collections.Generic;
using MusicSharp.Models.Repository;

namespace MusicSharp.MusicLibrary;

public static class MusicLibrary
{
    public static void AddMusicToLibrary(IReadOnlyList<string> musicFilePaths)
    {
        using var db = new LibraryContext();

        foreach (var filePath in musicFilePaths)
        {
            var track = Mappers.Mappers.GetTrackData(filePath);
            
        }
    }
}