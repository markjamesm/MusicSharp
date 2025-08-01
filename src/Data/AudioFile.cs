using ATL;
using MusicSharp.Enums;

namespace MusicSharp.Data;

public class AudioFile(string filepath, EFileType fileType)
{
    public Track TrackInfo { get; } = new(filepath);
    public string Path { get; } = filepath;
    public EFileType Type { get; } = fileType;
}