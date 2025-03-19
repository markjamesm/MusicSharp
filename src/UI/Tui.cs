using System;
using System.Collections.Generic;
using System.IO;
using MusicSharp.AudioPlayer;
using Terminal.Gui;
using MenuBarItem = Terminal.Gui.MenuBarItem;

namespace MusicSharp.UI;

public class Tui : Toplevel
{
    private readonly IPlayer _player;
    private readonly IStreamConverter _streamConverter;
    
    private readonly StatusBar? _statusBar;

    public Tui(IPlayer player, IStreamConverter streamConverter)
    {
        _player = player;
        _streamConverter = streamConverter;

        HighlightStyle = HighlightStyle.Hover;

        // Title = $"Example App ({Application.QuitKey} to quit)";
        
        var menuBar = new MenuBar()
        {
            HighlightStyle = HighlightStyle.Hover,
            MenusBorderStyle = LineStyle.Rounded,
            Menus =
            [
                new(
                    "_File",
                    new MenuItem[]
                    {
                        new(
                            "_Open file",
                            "Open a local audio file",
                            OpenFile // Open a file dialog
                        ),
                        new(
                            "_Quit",
                            "Quit UI Catalog",
                            RequestStop
                        )
                    }
                )
            ]
        };
        
        var libraryWindow = new View()
        {
            Title = "Library",
            Y = Pos.Bottom(menuBar),
            Width = Dim.Fill(),
            Height = Dim.Auto(),
            CanFocus = true,
            BorderStyle = LineStyle.Rounded
        };
        
        // Create the audio progress bar frame.
        var playbackControls = new FrameView()
        {
            Title = "Playback",
            X = 0,
            Y = Pos.Bottom(libraryWindow),
            Width = Dim.Auto(),
            Height = Dim.Auto(),
            CanFocus = true,
            BorderStyle = LineStyle.Rounded
        };

        var playPauseButton = new Button { X = 0, Y = 0, IsDefault = true, Text = "Play/Pause" };
        var stopButton = new Button { X = 0, Y = Pos.Bottom(playPauseButton), IsDefault = false, Text = "Stop" };
        var volumeIncreaseButton = new Button { X = Pos.Right(playPauseButton), Y = 0, IsDefault = false, Text = "Volume +" };
        var volumeDecreaseButton = new Button { X = Pos.Right(playPauseButton), Y = Pos.Bottom(volumeIncreaseButton), IsDefault = false, Text = "Volume -" };
        var seekForwardButton = new Button { X = Pos.Right(volumeIncreaseButton), Y = 0, IsDefault = false, Text = "Seek 5s" };
        var seekBackwardButton = new Button { X = Pos.Right(volumeDecreaseButton), Y = Pos.Bottom(seekForwardButton), IsDefault = false, Text = "Seek -5s" };
        
        playbackControls.Add (playPauseButton, stopButton, volumeIncreaseButton, volumeDecreaseButton, seekForwardButton, seekBackwardButton);

        var progressBar = new ProgressBar()
        {
            X = Pos.Right(playbackControls),
            Y = Pos.Bottom(libraryWindow),
            Width = Dim.Auto(),
            Height = Dim.Auto(),
            CanFocus = false,
            BorderStyle = LineStyle.Rounded
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
            var stream = _streamConverter.ConvertFileToStream(d.FilePaths[0].ToString());
            _player.Play(stream);   
        }
    }
}