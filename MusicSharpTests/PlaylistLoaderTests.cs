using MusicSharp.Models;

namespace MusicSharpTests;

public class Tests
{
    [Test]
    public void Load_NullPlaylist()
    {
        // Act and assert
        Assert.Throws<NullReferenceException>(() => PlaylistLoader.LoadPlaylist(null));
    }
}