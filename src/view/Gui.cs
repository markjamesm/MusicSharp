// <copyright file="Gui.cs" company="Mark-James McDougall">
// Licensed under the GNU GPL v3 License. See LICENSE in the project root for license information.
// </copyright>

namespace MusicSharp
{
    using System.Collections.Generic;
    using Terminal.Gui;

    /// <summary>
    /// The Gui class houses the CLI elements of MusicSharp.
    /// </summary>
    public class Gui
    {
        private static List<string> categories;
        private static ListView categoryListView;
        private static FrameView leftPane;
        private static FrameView rightPane;
        private static FrameView playbackControls;
        private static FrameView nowPlaying;
        private static ListView scenarioListView;

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
        ///  Gets and sets the current audio file play progress.
        /// </summary>
        internal ProgressBar AudioProgressBar { get; private set; }

        /// <summary>
        /// Start the UI.
        /// </summary>
        public void Start()
        {
            // Creates a instance of MainLoop to process input events, handle timers and other sources of data.
            Application.Init();

            var top = Application.Top;
            var tframe = top.Frame;

            // Create the menubar.
            var menu = new MenuBar(new MenuBarItem[]
            {
            new MenuBarItem("_File", new MenuItem[]
            {
                new MenuItem("_Open", "Open a music file", () => this.OpenFile()),

                new MenuItem("_Quit", "Exit MusicSharp", () => Application.RequestStop()),
            }),

            new MenuBarItem("_Help", new MenuItem[]
            {
                new MenuItem("_About MusicSharp", string.Empty, () =>
                {
                    MessageBox.Query("Music Sharp 0.6.6", "\nMusic Sharp is a lightweight CLI\n music player written in C#.\n\nDeveloped by Mark-James McDougall\nand licensed under the GPL v3.\n ", "Close");
                }),
            }),
            });

            // Create the playback controls frame.
            playbackControls = new FrameView("Playback")
            {
                X = 0,
                Y = 24,
                Width = 70,
                Height = 5,
                CanFocus = true,
            };

            var playBtn = new Button(1, 1, "Play");
            playBtn.Clicked += () =>
            {
                if (this.player.LastFileOpened == null)
                {
                    this.OpenFile();
                    return;
                }

                this.player.Play(this.player.LastFileOpened);
            };

            var pauseBtn = new Button(10, 1, "Pause");
            pauseBtn.Clicked += () =>
            {
                this.player.Pause();
            };

            var stopBtn = new Button(20, 1, "Stop");
            stopBtn.Clicked += () =>
            {
                this.player.Stop();
            };

            var increaseVolumeButton = new Button(55, 0, "+ Volume");
            increaseVolumeButton.Clicked += () =>
            {
                this.player.IncreaseVolume();
            };

            var decreaseVolumeButton = new Button(55, 2, "- Volume");
            decreaseVolumeButton.Clicked += () =>
            {
                this.player.DecreaseVolume();
            };

            playbackControls.Add(playBtn, pauseBtn, stopBtn, increaseVolumeButton, decreaseVolumeButton);

            // Create the left-hand playlists view.
            leftPane = new FrameView("Playlists")
            {
                X = 0,
                Y = 1, // for menu
                Width = 25,
                Height = 23,
                CanFocus = false,
            };

            categories = new List<string>();
            categories.Add("Rockin' Tunes");
            categoryListView = new ListView(categories)
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = 23,
                AllowsMarking = false,
                CanFocus = true,
            };

            categoryListView.OpenSelectedItem += (a) =>
            {
                rightPane.SetFocus();
            };

            leftPane.Add(categoryListView);

            rightPane = new FrameView("Media")
            {
                X = 25,
                Y = 1, // for menu
                Width = Dim.Fill(),
                Height = 23,
                CanFocus = true,
            };

            scenarioListView = new ListView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = 23,
                AllowsMarking = false,
                CanFocus = true,
            };

            // Create the audio progress bar frame.
            nowPlaying = new FrameView("Now Playing")
            {
                X = 70,
                Y = 24,
                Width = Dim.Fill(),
                Height = 5,
                CanFocus = false,
            };

            this.AudioProgressBar = new ProgressBar()
            {
                X = 1,
                Y = 2,
                Width = Dim.Fill() - 1,
                Height = 1,
                Fraction = 0.4F,
                ColorScheme = Colors.Error,
            };

            nowPlaying.Add(this.AudioProgressBar);

            // Add the layout elements and run the app.
            top.Add(menu, leftPane, rightPane, playbackControls, nowPlaying);

            Application.Run();
        }

        // Display a file open dialog and return the path of the user selected file.
        private void OpenFile()
        {
            var d = new OpenDialog("Open", "Open an audio file") { AllowsMultipleSelection = false };

            // This will filter the dialog on basis of the allowed file types in the array.
            d.AllowedFileTypes = new string[] { ".mp3", ".wav", ".flac" };
            Application.Run(d);

            if (!d.Canceled)
            {
                this.player.LastFileOpened = d.FilePath.ToString();
                this.player.Play(this.player.LastFileOpened);
            }
        }
    }
}
