using System;
using ATL;
using Microsoft.EntityFrameworkCore;

namespace MusicSharp.Library;

public class LibraryContext : DbContext
{
    public DbSet<Track> Tracks { get; set; }
    public string DbPath { get; }

    public LibraryContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "musicsharp-library.db");
    }
    
    // The following configures EF to create a Sqlite database file in the
    // platform-specific local folder.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}