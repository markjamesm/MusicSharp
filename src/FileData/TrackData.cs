using ATL;

namespace MusicSharp.FileData;

public static class TrackData
{
    public static string GetTrackData(string filePath)
    {
        var trackData = new Track(filePath);
        
        return $"{(string.IsNullOrWhiteSpace(trackData.Artist) ? "Unknown" : trackData.Artist)} - " +
               $"{(string.IsNullOrWhiteSpace(trackData.Title) ? "Unknown" : trackData.Title)} - " +
               $"{(string.IsNullOrWhiteSpace(trackData.Album) ? "Unknown": trackData.Album)}";
    }
}