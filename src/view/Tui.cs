// <copyright file="Tui.cs" company="Mark-James McDougall">
// Licensed under the GNU GPL v3 License. See LICENSE in the project root for license information.
// </copyright>

namespace MusicSharp
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Terminal.Gui;

    /// <summary>
    /// The Gui class houses the CLI elements of MusicSharp.
    /// </summary>
    public class Tui
    {
        private static List<string> playlistTracks;
        private static ListView playlistView;
        private static FrameView playlistPane;
        private static FrameView playbackControls;
        private static FrameView nowPlaying;
        private static StatusBar statusBar;

        private static Label trackName;

        /// <summary>
        /// Create a new instance of the audio player engine.
        /// </summary>
        private readonly IPlayer player;

        private List<string> playlist = new List<string>();
        private PlaylistLoader playlistLoader = new PlaylistLoader();

        /// <summary>
        /// Initializes a new instance of the <see cref="Tui"/> class.
        /// </summary>
        /// <param name="player">The player to be injected.</param>
        public Tui(IPlayer player)
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

                new MenuItem("Open S_tream", "Open a music stream", () => this.OpenStream()),

                new MenuItem("Open Pla_ylist", "Load a playlist", () => this.LoadPlaylist()),

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

            statusBar = new StatusBar(new StatusItem[]
            {
                new StatusItem(Key.F1, "~F1~ Open file", () => this.OpenFile()),
                new StatusItem(Key.F2, "~F2~ Open stream", () => this.OpenStream()),
                new StatusItem(Key.F3, "~F3~ Load playlist", () => this.LoadPlaylist()),
                new StatusItem(Key.F4, "~F4~ Quit", () => Application.RequestStop()),
                new StatusItem(Key.Space, "~Space~ Play/Pause", () => this.PlayPause()),
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

            var playPauseButton = new Button(1, 1, "Play/Pause");
            playPauseButton.Clicked += () =>
            {
                this.PlayPause();
            };

            var stopButton = new Button(16, 1, "Stop");
            stopButton.Clicked += () =>
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

            playbackControls.Add(playPauseButton, stopButton, increaseVolumeButton, decreaseVolumeButton);

            // Create the left-hand playlists view.
            playlistPane = new FrameView("Playlist Tracks")
            {
                X = 0,
                Y = 1, // for menu
                Width = Dim.Fill(),
                Height = 23,
                CanFocus = false,
            };

            // The list of tracks in the playlist.
            playlistTracks = new List<string>();

            playlistView = new ListView(playlistTracks)
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = 23,
                AllowsMarking = false,
                CanFocus = true,
            };

            // Play the selection when a playlist path is clicked.
            playlistView.OpenSelectedItem += (a) =>
            {
                this.player.LastFileOpened = a.Value.ToString();
                this.player.PlayFromPlaylist(this.player.LastFileOpened);
                this.NowPlaying(this.player.LastFileOpened);
            };

            playlistPane.Add(playlistView);

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
            top.Add(menu, playlistPane, playbackControls, nowPlaying, statusBar);

            Application.Run();
        }

        private void PlayPause()
        {
            try
            {
                this.player.PlayPause();
            }
            catch (Exception)
            {
                MessageBox.Query("Warning", "Select a file or stream first.", "Close");
            }
        }

        // Display a file open dialog and return the path of the user selected file.
        private void OpenFile()
        {
            var d = new OpenDialog("Open", "Open an audio file") { AllowsMultipleSelection = false };

            d.DirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            // This will filter the dialog on basis of the allowed file types in the array.
            d.AllowedFileTypes = new string[] { ".mp3", ".wav", ".flac" };
            Application.Run(d);

            if (!d.Canceled)
            {
                this.player.LastFileOpened = d.FilePath.ToString();
                this.player.OpenFile(this.player.LastFileOpened);
                this.NowPlaying(this.player.LastFileOpened);
            }
        }

        // Open and play an audio stream.
        private void OpenStream()
        {
            var d = new Dialog("Open Stream", 50, 15);

            var editLabel = new Label("Enter the url of the audio stream to load:\n(.mp3 only)")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
            };

            var streamURL = new TextField(string.Empty)
            {
                X = 3,
                Y = 4,
                Width = 42,
            };

            var loadStream = new Button(12, 7, "Load Stream");
            loadStream.Clicked += () =>
            {
                this.player.OpenStream(streamURL.Text.ToString());
                Application.RequestStop();
            };

            var cancelStream = new Button(29, 7, "Cancel");
            cancelStream.Clicked += () =>
            {
                Application.RequestStop();
            };

            d.AddButton(loadStream);
            d.AddButton(cancelStream);
            d.Add(editLabel, streamURL);
            Application.Run(d);
        }

        // Load a playlist file. Currently, only M3U is supported.
        private void LoadPlaylist()
        {
            var d = new OpenDialog("Open", "Open a playlist") { AllowsMultipleSelection = false };

            // This will filter the dialog on basis of the allowed file types in the array.
            d.AllowedFileTypes = new string[] { ".m3u" };
            Application.Run(d);

            if (!d.Canceled)
            {
                this.playlist = this.playlistLoader.LoadPlaylist(d.FilePath.ToString());

                if (this.playlist == null)
                {
                    Application.RequestStop();
                }
                else
                {
                    foreach (string track in this.playlist)
                    {
                        playlistTracks.Add(track);
                    }

                    Application.Run();
                }
            }
        }

        private void NowPlaying(string track)
        {
            trackName = new Label(track)
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
            };

            nowPlaying.Add(trackName);
        }
    }
}