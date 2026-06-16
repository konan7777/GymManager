namespace GymManager.Api.Entities;

public class Member
{
    public long Id { get; set; }

    public long GymId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string? Email { get; set; }

    public DateOnly? BirthDate { get; set; }

    public short Gender { get; set; }

    public short MemberType { get; set; }

    public string? Notes { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Gym Gym { get; set; } = null!;

    public ICollection<MembershipPayment> MembershipPayments { get; set; } = new List<MembershipPayment>();
}