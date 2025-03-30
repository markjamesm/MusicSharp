using System;
using Microsoft.EntityFrameworkCore;

namespace MusicSharp.Models.Repository;

public class LibraryContext : DbContext
{
    public DbSet<Artist> Artists { get; set; }
    public DbSet<Album> Albums { get; set; }
    public DbSet<Track> Tracks { get; set; }
    public string DbPath { get; }

    public LibraryContext()
    {
        const Environment.SpecialFolder folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "musicsharp-library.db");
    }
    
    // Create the db in the OS-specific app data directory.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}