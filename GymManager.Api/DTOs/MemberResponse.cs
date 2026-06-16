namespace GymManager.Api.DTOs.Members;

public class MemberResponse
{
    public long Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string? Email { get; set; }

    public DateOnly? BirthDate { get; set; }

    public short Gender { get; set; }

    public short MemberType { get; set; }

    public string? Notes { get; set; }

    public bool IsActive { get; set; }

    public DateOnly? MembershipEndDate { get; set; }

    public bool HasActiveMembership { get; set; }

    public DateTime CreatedAt { get; set; }
}