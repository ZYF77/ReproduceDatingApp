using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API.Entities;

public class Member
{
    public string Id { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime LastActive { get; set; } = DateTime.UtcNow;
    public required string DisplayName { get; set; }
    public required string Gender { get; set; }
    public string? Description { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }

    // Navigation property
    [JsonIgnore]
    [ForeignKey(nameof(Id))]
    public AppUser User { get; set; } = null!;

}
