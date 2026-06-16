namespace GymManager.Api.Entities;

public class MembershipPayment
{
    public long Id { get; set; }

    public long MemberId { get; set; }

    public long CreatedByUserId { get; set; }

    public DateOnly PaymentDate { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public decimal Amount { get; set; }

    public string? Note { get; set; }

    public bool IsDeleted { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Member Member { get; set; } = null!;

    public User CreatedByUser { get; set; } = null!;
}