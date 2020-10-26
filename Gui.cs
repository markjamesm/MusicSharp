// <copyright file="Gui.cs" company="Mark-James McDougall">
// Licensed under the GNU GPL v3 License. See LICENSE in the project root for license information.
// </copyright>

namespace MusicSharp
{
    using Terminal.Gui;

    /// <summary>
    /// The Gui class houses the CLI elements of MusicSharp.
    /// </summary>
    public static class Gui
    {
        /// <summary>
        /// The Start method builds the user interface.
        /// </summary>
        public static void Start()
        {
            // Creates a instance of MainLoop to process input events, handle timers and other sources of data.
            Application.Init();

            var top = Application.Top;
            var tframe = top.Frame;

            // Create the top-level window.
            var win = new Window("MusicSharp")
            {
                X = 0,
                Y = 1, // Leave one row for the toplevel menu

                // By using Dim.Fill(), it will automatically resize without manual intervention
                Width = Dim.Fill(),

                // Subtract one row for the statusbar
                Height = Dim.Fill() - 1,
            };

            // Create the menubar.
            var menu = new MenuBar(new MenuBarItem[]
            {
            new MenuBarItem("_File", new MenuItem[]
            {
                new MenuItem("_Open", "Open a music file", () => Player.PlayAudioFile()),

                new MenuItem("_Open Stream", "Open a stream", () => OpenStream()),

                new MenuItem("_Quit", "Exit MusicSharp", () => Application.RequestStop()),
            }),

            new MenuBarItem("_Help", new MenuItem[]
            {
                new MenuItem("_About", string.Empty, () =>
                {
                    MessageBox.Query("Music Sharp 0.2.0", "\nMusic Sharp is a lightweight CLI\n music player written in C#.\n\nDeveloped by Mark-James McDougall\nand licensed under the GPL v3.\n ", "Close");
                }),
            }),
            });

            // Add the layout elements and run the app.
            top.Add(win, menu);
            Application.Run();
        }

        // Component Methods.
        // Placeholder method until the functionality is implemented.
        private static void OpenStream()
        {
            // Create vars for the button and add them to the Dialog.
            var ok = new Button("Ok", is_default: true);
            var cancel = new Button("Cancel");

            ok.Clicked += () => { Application.RequestStop(); };
            cancel.Clicked += () => { Application.RequestStop(); };

            var d = new Dialog(
                "Feature not yet implemented", ok, cancel);

            Application.Run(d);
        }
    }
}