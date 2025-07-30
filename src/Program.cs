using MusicSharp.UI;
using MusicSharp.AudioPlayer;
using SoundFlow.Backends.MiniAudio;
using Terminal.Gui.App;
using System.Diagnostics.CodeAnalysis;

namespace MusicSharp;

public static class Program
{
    [UnconditionalSuppressMessage("AssemblyLoadTrimming", 
                                  "IL2104:Assembly 'SoundFlow' produced trim warnings.", 
                                  Justification = """
                                                  Can safely ignore as this is the only warning and the 
                                                  method is only ever called with primitive types
                                                  https://github.com/LSXPrime/SoundFlow/blob/master/Src/Utils/Extensions.cs#L49
                                                  """)]
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