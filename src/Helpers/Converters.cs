using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace MusicSharp.Helpers;

public class Converters
{
    private readonly HttpClient _httpClient;

    public Converters(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public static Stream ConvertFileToStream(string path)
    {
        return File.OpenRead(path);
    }

    public async Task<Stream> ConvertUrlToStream(string url)
    {
        var stream = await _httpClient.GetStreamAsync(url);
        return stream;
    }
}