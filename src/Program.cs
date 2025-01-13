// <copyright file="Program.cs" company="Mark-James McDougall">
// Licensed under the GNU GPL v3 License. See LICENSE in the project root for license information.
// </copyright>

using System;
using System.Runtime.InteropServices;
using MusicSharp.SoundEngines;
using MusicSharp.View;

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
        IPlayer player;

        if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            player = new WinPlayer();
        }
        else if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            player = new LinuxPlayer();
        }
        else
        {
            throw new PlatformNotSupportedException();
        }

        var gui = new Tui(player);

        gui.Start();
    }
}