using MusicSharp.SoundEngines;
using MusicSharp.Enums;

namespace MusicSharpTests;

[TestFixture]
public class LinuxPlayerTests
{
    private LinuxPlayer _player;
    private const string ValidFilePath = "test.mp3";
    private const string ValidStreamUrl = "https://example.com/stream";

    [SetUp]
    public void Setup()
    {
        // Create test file
        File.WriteAllText(ValidFilePath, "dummy content");
        _player = new LinuxPlayer();
    }

    [TearDown]
    public void TearDown()
    {
        if (File.Exists(ValidFilePath))
            File.Delete(ValidFilePath);

        // Ensure player is stopped after each test
        _player.Stop();
    }

    [Test]
    public void OpenFile_WithInvalidPath_ThrowsFileNotFoundException()
    {
        Assert.Throws<FileNotFoundException>(() => _player.OpenFile("nonexistent.mp3"));
    }

    [Test]
    public void OpenFile_WithValidPath_UpdatesPlayerStatus()
    {
        // Act
        _player.OpenFile(ValidFilePath);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(_player.PlayerStatus, Is.EqualTo(ePlayerStatus.Playing));
            Assert.That(_player.LastFileOpened, Does.Contain(ValidFilePath));
        });
    }

    [Test]
    public void OpenStream_UpdatesLastFileOpened()
    {
        // Act
        _player.OpenStream(ValidStreamUrl);

        // Assert
        Assert.That(_player.LastFileOpened, Is.EqualTo(ValidStreamUrl));
    }

    [Test]
    public void Stop_SetsCorrectPlayerStatus()
    {
        // Arrange
        _player.OpenFile(ValidFilePath);

        // Act
        _player.Stop();

        // Assert
        Assert.That(_player.PlayerStatus, Is.EqualTo(ePlayerStatus.Stopped));
    }

    [Test]
    public void CurrentTime_WhenStopped_ReturnsZero()
    {
        // Act
        var result = _player.CurrentTime();

        // Assert
        Assert.That(result, Is.EqualTo(TimeSpan.Zero));
    }

    [Test]
    public void TrackLength_WhenStopped_ReturnsZero()
    {
        // Act
        var result = _player.TrackLength();

        // Assert
        Assert.That(result, Is.EqualTo(TimeSpan.Zero));
    }

    [Test]
    public void PlayFromPlaylist_WithValidPath_UpdatesPlayerStatus()
    {
        // Act
        _player.PlayFromPlaylist(ValidFilePath);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(_player.PlayerStatus, Is.EqualTo(ePlayerStatus.Playing));
            Assert.That(_player.LastFileOpened, Does.Contain(ValidFilePath));
        });
    }
}