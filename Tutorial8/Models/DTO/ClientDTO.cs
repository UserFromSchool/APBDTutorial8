using System.ComponentModel.DataAnnotations;

namespace Tutorial8.Models.DTO;

// Contains Client Table information
public class ClientDTO
{
    public required int Id { get; set; }
    [StringLength(120)]
    public required string FirstName { get; set; }
    [StringLength(120)]
    public required string LastName { get; set; }
    [StringLength(120)]
    public required string Email { get; set; }
    [StringLength(120)]
    public required string PhoneNumber { get; set; }
    [StringLength(120)]
    public required string Pesel { get; set; }

    public bool Validate()
    {
        if (FirstName.Length < 3) return false;
        if (LastName.Length < 3) return false;
        if (Email.Length < 4) return false;
        if (!Email.Contains('@')) return false;
        if (!PhoneNumber.Contains('+')) return false;
        if (PhoneNumber.Length != 12) return false;
        if (Pesel.Length != 11) return false;
        return true;
    }
}