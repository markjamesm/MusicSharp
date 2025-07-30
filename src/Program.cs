using MusicSharp.UI;
using MusicSharp.AudioPlayer;
using SoundFlow.Backends.MiniAudio;
using Terminal.Gui.App;

namespace MusicSharp;

public static class Program
{
    public static void Main()
    {
        using var audioEngine = new MiniAudioEngine();
        using IPlayer player = new SoundFlowPlayer(audioEngine);
        using var ui = new Tui(player);
        
        Application.Init();
        Application.Run(ui);
        Application.Shutdown();
    }
}