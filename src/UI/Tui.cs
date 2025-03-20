using MusicSharp.AudioPlayer;
using Terminal.Gui;

namespace MusicSharp.UI;

public class Tui : Window
{
    private readonly IPlayer _player;

    public Tui(IPlayer player)
    {
        _player = player;
        
        var menuBar = new MenuBar()
        {
            MenusBorderStyle = LineStyle.Rounded,
            Menus =
            [
                new MenuBarItem(
                    "_File",
                    new MenuItem[]
                    {
                        new(
                            "_Open file",
                            "Open a local audio file",
                            OpenFile
                        ),
                        new(
                            "_Quit",
                            "Quit MusicSharp",
                            RequestStop
                        )
                    }
                )
            ]
        };
        
        var libraryWindow = new Window()
        {
            Title = "Library",
            Y = Pos.Bottom(menuBar),
            Width = Dim.Fill(),
            Height = Dim.Auto(),
            CanFocus = true,
            BorderStyle = LineStyle.Rounded
        };
        
        // Create the audio progress bar frame.
        var playbackControls = new Window()
        {
            Title = "Playback",
            X = 0,
            Y = Pos.Bottom(libraryWindow),
            Width = Dim.Auto(),
            Height = Dim.Auto(),
            CanFocus = true,
            BorderStyle = LineStyle.Rounded
        };

        var playPauseButton = new Button { X = 0, Y = 0, IsDefault = false, Text = "Play/Pause" };
        var stopButton = new Button { X = 0, Y = Pos.Bottom(playPauseButton), IsDefault = false, Text = "Stop" };
        var volumeIncreaseButton = new Button { X = Pos.Right(playPauseButton), Y = 0, IsDefault = false, Text = "Volume +" };
        var volumeDecreaseButton = new Button { X = Pos.Right(playPauseButton), Y = Pos.Bottom(volumeIncreaseButton), IsDefault = false, Text = "Volume -" };
        var seekForwardButton = new Button { X = Pos.Right(volumeIncreaseButton), Y = 0, IsDefault = false, Text = "Seek 5s" };
        var seekBackwardButton = new Button { X = Pos.Right(volumeDecreaseButton), Y = Pos.Bottom(seekForwardButton), IsDefault = false, Text = "Seek -5s" };
        
        playPauseButton.Accepting += (s, args) => _player.PlayPause();
        stopButton.Accepting += (s, args) => _player.Stop();
        volumeIncreaseButton.Accepting += (s, args) => _player.IncreaseVolume(); 
        volumeDecreaseButton.Accepting += (s, args) => _player.DecreaseVolume();
        seekForwardButton.Accepting += (s, args) => _player.SeekForward();
        seekBackwardButton.Accepting += (s, a) => _player.SeekBackward();
        
        playbackControls.Add (playPauseButton, stopButton, volumeIncreaseButton, volumeDecreaseButton, seekForwardButton, seekBackwardButton);

        var progressBar = new ProgressBar()
        {
            X = Pos.Right(playbackControls),
            Y = Pos.Bottom(libraryWindow),
            Width = Dim.Auto(),
            Height = Dim.Auto(),
            CanFocus = false,
            BorderStyle = LineStyle.Rounded,
        };
        
        // Add the views to the Window
        Add(menuBar, libraryWindow, playbackControls, progressBar);
        //Add(libraryWindow);
    }
    
    // Action Methods
    private void OpenFile()
    {
        var d = new OpenDialog()
        {
            AllowsMultipleSelection = false,
            Title = "Open an audio file",
            AllowedTypes = [new AllowedType("Allowed filetypes", ".mp3")]
        };
        
        Application.Run(d);

        if (!d.Canceled)
        {
            _player.Play(d.FilePaths[0]);
        }
    }

    // private void PlayHandler(Stream stream)
    // {
    //     switch (_player.PlayerStatus)
    //     {
    //         case EPlayerStatus.Playing:
    //             _player.Stop();
    //     }
    // }
}