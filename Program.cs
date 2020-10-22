// <copyright file="Program.cs" company="Mark-James McDougall">
// Licensed under the GNU GPL v3 License. See LICENSE in the project root for license information.
// </copyright>

using Terminal.Gui;

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
        Application.Init();

        // Create the menubar. This will be refactored into a separate file.
        var menu = new MenuBar(new MenuBarItem[]
        {
            new MenuBarItem("_File", new MenuItem[]
            {
                new MenuItem("_Quit", string.Empty, () =>
                {
                    Application.RequestStop();
                }),
            }),

            new MenuBarItem("Help", new MenuItem[]
            {
                new MenuItem("_About", string.Empty, () =>
                {
                    // Open a textbox here.
                }),
            }),
        });

        var win = new Window("MPlayer")
        {
            X = 0,
            Y = 1,
            Width = Dim.Fill(),
            // Subtract from the height to give room for a satusbar
            Height = Dim.Fill() - 2,
        };

        // Add both menu and win in a single call
        Application.Top.Add(menu, win);
        Application.Run();
    }
}