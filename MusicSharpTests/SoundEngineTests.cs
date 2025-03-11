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
}