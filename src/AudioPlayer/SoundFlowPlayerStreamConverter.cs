using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace MusicSharp.AudioPlayer;

public class SoundFlowPlayerStreamConverter : IStreamConverter
{
    private readonly HttpClient _httpClient;

    public SoundFlowPlayerStreamConverter(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public Stream ConvertFileToStream(string path)
    {
        return File.OpenRead(path);
    }

    public async Task<Stream> ConvertUrlToStream(string url)
    {
        var stream = await _httpClient.GetStreamAsync(url);
        return stream;
    }
}