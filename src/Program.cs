// <copyright file="Program.cs" company="Mark-James McDougall">
// Licensed under the GNU GPL v3 License. See LICENSE in the project root for license information.
// </copyright>

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
        var player = new WinPlayer();
        var gui = new Tui(player);

        gui.Start();
    }
}