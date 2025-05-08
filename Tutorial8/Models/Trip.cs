namespace Tutorial8.Models;

// Contains Trip Table information
public class Trip
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required DateTime StartDate { get; set; }
    public required DateTime EndDate { get; set; }
    public required int MaxPeople { get; set; }
    public string? DestinationCountryName { get; set; }
    public int? PaymentDate { get; set; }
    public int? RegisteredAt { get; set; }
}