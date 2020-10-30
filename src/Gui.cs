// <copyright file="Gui.cs" company="Mark-James McDougall">
// Licensed under the GNU GPL v3 License. See LICENSE in the project root for license information.
// </copyright>

namespace MusicSharp
{
    using NAudio.Wave;
    using Terminal.Gui;

    /// <summary>
    /// The Gui class houses the CLI elements of MusicSharp.
    /// </summary>
    public class Gui
    {
        /// <summary>
        /// Create a new instance of the audio player engine.
        /// </summary>
        private readonly IPlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="Gui"/> class.
        /// </summary>
        /// <param name="player">The player to be injected.</param>
        public Gui(IPlayer player)
        {
            this.player = player;
        }

        /// <summary>
        /// Start the UI.
        /// </summary>
        public void Start()
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

            // Add components to our window
            var stopBtn = new Button(24, 22, "Stop");
            stopBtn.Clicked += () =>
            {
                this.player.Stop();
            };

            var playBtn = new Button(3, 22, "Play");
            playBtn.Clicked += () =>
            {
                if (this.player.LastFileOpened != null && this.player.OutputDevice != null)
                {
                    try
                    {
                        this.player.OutputDevice.Play();
                    }
                    catch (System.NullReferenceException)
                    {
                    }
                }
                else
                {
                    this.OpenFile();
                }
            };

            var pauseBtn = new Button(13, 22, "Pause");
            pauseBtn.Clicked += () =>
            {
                this.player.Pause();
            };

            var nowPlaying = new Label("Test")
            {
                X = 1,
                Y = 1,
                Width = 20,
                Height = 4,
            };

            win.Add(playBtn, stopBtn, pauseBtn, nowPlaying);

            // Create the menubar.
            var menu = new MenuBar(new MenuBarItem[]
            {
            new MenuBarItem("_File", new MenuItem[]
            {
                new MenuItem("_Open", "Open a music file", () => this.OpenFile()),

                new MenuItem("_Load Stream", "Load a stream", () => this.OpenFile()), // Replace this with an OpenStream() method.

                new MenuItem("_Quit", "Exit MusicSharp", () => Application.RequestStop()),
            }),

            new MenuBarItem("_Help", new MenuItem[]
            {
                new MenuItem("_About MusicSharp", string.Empty, () =>
                {
                    MessageBox.Query("Music Sharp 0.4.1", "\nMusic Sharp is a lightweight CLI\n music player written in C#.\n\nDeveloped by Mark-James McDougall\nand licensed under the GPL v3.\n ", "Close");
                }),
            }),
            });

            // Add the layout elements and run the app.
            top.Add(menu, win);

            Application.Run();
        }

        // Display a file open dialog and return the path of the user selected file.
        private void OpenFile()
        {
            var d = new OpenDialog("Open", "Open an audio file") { AllowsMultipleSelection = false };
            Application.Run(d);

            if (!d.Canceled)
            {
                this.player.LastFileOpened = d.FilePath.ToString();
                this.player.Play(d.FilePath.ToString());
            }
        }
    }
}