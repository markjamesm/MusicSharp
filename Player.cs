// <copyright file="Player.cs" company="Mark-James McDougall">
// Licensed under the GNU GPL v3 License. See LICENSE in the project root for license information.
// </copyright>

namespace MusicSharp
{
    using Terminal.Gui;

    /// <summary>
    /// Class which houses the CLI methods.
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Method to create and start the CLI.
        /// </summary>
        public static void Start()
        {
            Application.Init();

            // Create the menubar.
            var menu = new MenuBar(new MenuBarItem[]
            {
            new MenuBarItem("_File", new MenuItem[]
            {
                new MenuItem("_Open", string.Empty, () =>
                {
                    // Insert code to open music files here.
                }),

                new MenuItem("_Open Stream", string.Empty, () =>
                {
                    // Insert code to open music streams here.
                }),

                new MenuItem("_Quit", string.Empty, () =>
                {
                    Application.RequestStop();
                }),
            }),

            new MenuBarItem("_Help", new MenuItem[]
            {
                new MenuItem("_About", string.Empty, () =>
                {
                    MessageBox.Query("Music Sharp 0.2.0", "\nMusic Sharp is a lightweight CLI\n music player written in C#.\n\nCreated by Mark-James McDougall and licensed\nunder the GPL v3.", "Close");
                }),
            }),
            });

            // Creates the top-level window to show.
            var win = new Window("MusicSharp")
            {
                X = 0,
                Y = 1, // Leave one row for the toplevel menu

                // By using Dim.Fill(), it will automatically resize without manual intervention
                Width = Dim.Fill(),

                // Subtract one row for the statusbar
                Height = Dim.Fill() - 1,
            };

            Application.Top.Add(win, menu);
            Application.Run();
        }
    }
}