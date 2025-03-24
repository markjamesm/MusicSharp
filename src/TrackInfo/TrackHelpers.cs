using ATL;

namespace MusicSharp.TrackInfo;

public static class TrackHelpers
{
    public static string GetTrackAndArtistName(string filePath)
    {
        var trackData = new Track(filePath);

        if (trackData.Title == null)
        {
            return "Unknown - Unknown";
        }
        
        return trackData.Title + " - " + trackData.Artist;
    }
}