// <copyright file="Gui.cs" company="Mark-James McDougall">
// Licensed under the GNU GPL v3 License. See LICENSE in the project root for license information.
// </copyright>

namespace MusicSharp
{
    using System;
    using NAudio.Wave;
    using NAudio.Wave.SampleProviders;
    using Terminal.Gui;

    /// <summary>
    /// The Gui class houses the CLI elements of MusicSharp.
    /// </summary>
    public class Player
    {
        // Create an audio output device.
        private WaveOutEvent outputDevice;
        private AudioFileReader audioFile;

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="player">The audio player to implement.</param>
        public Player()
        {
        }

        /// <summary>
        /// The Start method builds the user interface.
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

            var stopBtn = new Button(3, 5, "Stop");
            stopBtn.Clicked += () =>
            {
                this.OnButtonStopClick();
            };

            win.Add(stopBtn);

            // Create the menubar.
            var menu = new MenuBar(new MenuBarItem[]
            {
            new MenuBarItem("_File", new MenuItem[]
            {
                new MenuItem("_Open", "Open a music file", () => this.OpenFile()),

                new MenuItem("_Load Stream", "Load a stream", () => this.OpenFile()),

                new MenuItem("_Quit", "Exit MusicSharp", () => Application.RequestStop()),
            }),

            new MenuBarItem("_Help", new MenuItem[]
            {
                new MenuItem("_About", string.Empty, () =>
                {
                    MessageBox.Query("Music Sharp 0.4.0", "\nMusic Sharp is a lightweight CLI\n music player written in C#.\n\nDeveloped by Mark-James McDougall\nand licensed under the GPL v3.\n ", "Close");
                }),
            }),
            });

            // Add the layout elements and run the app.
            top.Add(menu, win);

            Application.Run();
        }

        private void OnButtonStopClick()
        {
            this.outputDevice?.Stop();
        }

        // Method to display a file open dialog and return the path of the user selected file.
        private void OpenFile()
        {
            var d = new OpenDialog("Open", "Open an audio file") { AllowsMultipleSelection = false };
            Application.Run(d);

            if (!d.Canceled)
            {
                if (this.outputDevice == null)
                {
                    this.outputDevice = new WaveOutEvent();
                    this.outputDevice.PlaybackStopped += this.OnPlaybackStopped;
                }

                if (this.audioFile == null)
                {
                    this.audioFile = new AudioFileReader(d.FilePath.ToString());
                    this.outputDevice.Init(this.audioFile);
                }

                this.outputDevice.Play();
            }
        }

        private void OnPlaybackStopped(object sender, StoppedEventArgs args)
        {
            this.outputDevice.Dispose();
            this.outputDevice = null;
            this.audioFile.Dispose();
            this.audioFile = null;
        }
    }
}