using MusicSharp.SoundEngines;
using SoundFlow.Backends.MiniAudio;
using SoundFlow.Enums;

namespace MusicSharpTests;

public class SoundEngineTests
{
    [Test]
    public void Play_NullFile()
    {
        // arrange
        var isFileValid = File.Exists("thisisafail.exe");

        // act and assert
        Assert.That(isFileValid, Is.False);
    }
    
    [Test]
    public void PlayFromPlaylist_NullFile()
    {
        // arrange
        using var soundEngine = new MiniAudioEngine(44100, Capability.Playback);
        using var player = new SoundEngine(soundEngine);

        // act and assert
        Assert.Throws<NullReferenceException>(() => player.Play(null));
    }
}