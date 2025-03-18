using System.IO;
using System.Threading.Tasks;

namespace MusicSharp.AudioPlayer;

public interface IStreamConverter
{
    public Stream ConvertFileToStream(string path);
    
    public Task<Stream> ConvertUrlToStream(string url);
}