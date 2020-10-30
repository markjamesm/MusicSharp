// <copyright file="Player.cs" company="Mark-James McDougall">
// Licensed under the GNU GPL v3 License. See LICENSE in the project root for license information.
// </copyright>

namespace MusicSharp
{
    using NAudio.Wave;
    using Terminal.Gui;

    /// <summary>
    /// The Gui class houses the CLI elements of MusicSharp.
    /// </summary>
    public class Player
    {
        // Create an audio output device.
        private WaveOutEvent outputDevice;
        private AudioFileReader audioFile;
        private string lastFileOpened = "Testing";

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        public Player()
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
                this.Stop();
            };

            var playBtn = new Button(3, 22, "Play");
            playBtn.Clicked += () =>
            {
                if (this.lastFileOpened != null && this.outputDevice != null)
                {
                    try
                    {
                        this.outputDevice.Play();
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
                this.Pause();
            };

            var nowPlaying = new Label(this.lastFileOpened)
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
                this.lastFileOpened = d.FilePath.ToString();
                this.Play(d.FilePath.ToString());
            }
        }

        // Method to stop audio playback
        private void Stop()
        {
            if (this.outputDevice != null)
            {
                try
                {
                    this.outputDevice?.Stop();
                    this.outputDevice.PlaybackStopped += this.OnPlaybackStopped;
                }
                catch (System.NullReferenceException)
                {
                }
            }
        }

        // Start playing audio
        private void Play(string path)
        {
            if (this.outputDevice == null)
            {
                this.outputDevice = new WaveOutEvent();
                this.outputDevice.PlaybackStopped += this.OnPlaybackStopped;
            }

            if (this.audioFile == null)
            {
                try
                {
                    this.audioFile = new AudioFileReader(path);
                    this.outputDevice.Init(this.audioFile);
                    this.outputDevice.Play();
                }
                catch (System.Runtime.InteropServices.COMException)
                {
                }
            }

            if (this.audioFile != null)
            {
                try
                {
                    this.outputDevice.Play();
                }
                catch (System.Runtime.InteropServices.COMException)
                {
                }
            }
        }

        // Pause our audio player
        private void Pause()
        {
            try
            {
                this.outputDevice?.Pause();
            }
            catch (System.NullReferenceException)
            {
            }
        }

        // Dispose of our device and audioFile once playback is stopped.
        // These will be changed in the future as we might want to allow
        // users to carry on playback from where they left off.
        private void OnPlaybackStopped(object sender, StoppedEventArgs args)
        {
            this.outputDevice.Dispose();
            this.outputDevice = null;

            // this.audioFile.Dispose();

            // By resetting the audioFIle position to 0, playback can start again.
            // this.audioFile.Position = 0;
            //   this.audioFile = null;
        }
    }
}