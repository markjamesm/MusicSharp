using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicSharp.Models;

public class Artist
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required, MaxLength(100)] 
    public string Name { get; set; }
    
    public ICollection<Album> Albums { get; set; } =  new List<Album>();
}