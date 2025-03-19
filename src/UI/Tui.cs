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
            Y = Pos.Bottom(menuBar),
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            BorderStyle = LineStyle.Rounded
        };
        
        // Add the views to the Window
        Add(menuBar);
        Add(libraryWindow);
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