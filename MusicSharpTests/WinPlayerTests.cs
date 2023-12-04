using MusicSharp.SoundEngines;

namespace MusicSharpTests;

public class WinPlayerTests
{
    [Test]
    public void PlayFromPlaylist_NullFile()
    {
        // arrange
        var player = new WinPlayer();

        // act and assert
        Assert.Throws<NullReferenceException>(() => player.PlayFromPlaylist(null));
    }

    [Test]
    public void OpenStream_NullFile()
    {
        // arrange
        var player = new WinPlayer();

        // act and assert
        Assert.Throws<NullReferenceException>(() => player.OpenStream(null));
    }
}