// <copyright file="Program.cs" company="Mark-James McDougall">
// Licensed under the GNU GPL v3 License. See LICENSE in the project root for license information.
// </copyright>

using System.Net.Http;
using MusicSharp.UI;
using MusicSharp.AudioPlayer;
using SoundFlow.Backends.MiniAudio;
using SoundFlow.Enums;
using Terminal.Gui;

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
        
       // ConfigurationManager.RuntimeConfig = """{ "Theme": "Dark" }""";
        Application.Init();
        using var ui = new Tui(player);
        Application.Run(ui);
        Application.Shutdown ();
    }
}