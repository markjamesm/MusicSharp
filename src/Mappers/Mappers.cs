using ATL;

namespace MusicSharp.Mappers;

public static class Mappers
{
    public static Track GetTrackData(string filePath)
    {
        var track = new Track(filePath);
        return track;
    }
    
    public static string GetTrackAndArtistName(string filePath)
    {
        var trackData = new Track(filePath);

        if (trackData.Title == string.Empty)
        {
            return "Unknown - Unknown";
        }
        
        return trackData.Title + " - " + trackData.Artist;
    }
}