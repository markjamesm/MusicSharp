﻿// <copyright file="Program.cs" company="Mark-James McDougall">
// Licensed under the GNU GPL v3 License. See LICENSE in the project root for license information.
// </copyright>

using System.Net.Http;
using MusicSharp.SoundEngines;
using MusicSharp.Interface;
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
        using var httpClient = new HttpClient();
        var soundEngine = new MiniAudioEngine(44100, Capability.Playback);
        using var player = new SoundEngine(soundEngine);
        var gui = new Tui(player, httpClient);

        gui.Start();
    }
}