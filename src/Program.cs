using MusicSharp.UI;
using MusicSharp.AudioPlayer;
using SoundFlow.Backends.MiniAudio;
using Terminal.Gui.App;
using System.Diagnostics.CodeAnalysis;

namespace MusicSharp;

/// <summary>
/// Entry Point class.
/// </summary>
public static class Program
{
    /// <summary>
    /// Entry point.
    /// </summary>
    [UnconditionalSuppressMessage("AssemblyLoadTrimming", "IL2026:RequiresUnreferencedCode", Justification = "Application.Init doesn't actually require unreferenced code when given no arguments")]
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