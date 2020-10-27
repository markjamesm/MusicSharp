// <copyright file="Program.cs" company="Mark-James McDougall">
// Licensed under the GNU GPL v3 License. See LICENSE in the project root for license information.
// </copyright>

using MusicSharp;

/// <summary>
/// Entry Point class.
/// </summary>
public class Program
{
    /// <summary>
    /// Entry point.
    /// </summary>
    public static void Main()
    {
        // Create a new instance of Player class.
        IPlayer player = new WinPlayer();

        // Start MusicSharp.
        Gui gui = new Gui(player);
        gui.Start();
    }
}