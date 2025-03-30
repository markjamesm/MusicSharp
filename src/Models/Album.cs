using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicSharp.Models;

public class Album
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [MaxLength(500)] 
    public string Name { get; set; }
    
    public int ArtistId { get; set; }
    public Artist Artist { get; set; } = null!;
    
    public ICollection<Track> Tracks { get; set; } = new List<Track>();

    [MaxLength(500)] 
    public string Genre { get; set; } = "";
    
    public DateOnly ReleaseDate { get; set; }
}