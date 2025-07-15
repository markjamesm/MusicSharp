using System.Net.Http;
using MusicSharp.UI;
using MusicSharp.AudioPlayer;
using SoundFlow.Backends.MiniAudio;
using SoundFlow.Enums;
using Terminal.Gui.App;
using Terminal.Gui.Configuration;

namespace MusicSharp;

/// <summary>
/// Entry Point class.
/// </summary>
public static class Program
{
    /// <summary>
    /// Entry point.
    /// </summary>
    public static void Main()
    {
        using var httpClient = new HttpClient();
        IStreamConverter streamConverter = new SoundFlowPlayerStreamConverter(httpClient);
        var soundEngine = new MiniAudioEngine(44100, Capability.Playback);
        using IPlayer player = new SoundFlowPlayer(soundEngine, streamConverter);
        using var ui = new Tui(player);
        
        Application.Init();
        Application.Run(ui);
        Application.Shutdown();
    }
}