// <copyright file="Program.cs" company="Mark-James McDougall">
// Licensed under the GNU GPL v3 License. See LICENSE in the project root for license information.
// </copyright>

using System.Net.Http;
using MusicSharp.UI;
using MusicSharp.AudioPlayer;
using SoundFlow.Backends.MiniAudio;
using SoundFlow.Enums;

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
        using IPlayer player = new SoundFlowPlayer(soundEngine);
        using var httpClient = new HttpClient();
        IStreamConverter streamConverter = new SoundFlowPlayerStreamConverter(httpClient);
        
        var ui = new Tui(player, streamConverter);
        ui.Start();
    }
}