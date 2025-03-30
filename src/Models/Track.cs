using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicSharp.Models;

public class Track
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required, MaxLength(500)]
    public string Title { get; set; }
    
    public int AlbumId { get; set; }
    public Album Album { get; set; } = null!;

    [Required]
    public TimeOnly Duration { get; set; }
    
    [Required, MaxLength(1200)] 
    public string FilePath { get; set; }
    
    [Required]
    public DateTime DateAdded { get; set; } = DateTime.Now;

    public int PlayCount { get; set; } = 0;

    [MaxLength(500)] public string Codec { get; set; }
}