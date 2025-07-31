using ATL;

namespace MusicSharp.Data;

public class AudioFile(string filepath)
{
    public Track TrackInfo { get; } = new(filepath);
    public string Path { get; } = filepath;
}