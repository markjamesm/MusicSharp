using System.Net.Http;
using MusicSharp.UI;
using MusicSharp.AudioPlayer;
using SoundFlow.Backends.MiniAudio;
using SoundFlow.Enums;
using Terminal.Gui.App;

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
        var soundEngine = new MiniAudioEngine(44100, Capability.Playback);
        using var httpClient = new HttpClient();
        IStreamConverter streamConverter = new SoundFlowPlayerStreamConverter(httpClient);
        using IPlayer player = new SoundFlowPlayer(soundEngine, streamConverter);
        
        Application.Init();
        using var ui = new Tui(player);
        Application.Run(ui);
        Application.Shutdown ();
    }
}